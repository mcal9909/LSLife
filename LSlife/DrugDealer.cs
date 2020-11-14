using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class DrugDealer
    {
        public Vehicle dealerCar = null;
        public Ped lieut = null;
        public Ped goon = null;
        Blip blip = null;
        enum DrugDealSates { SpawnStuff, ConfigStuff, WaitForPlayer, GetIntoPosition, GetReadyToDeal, Destroy, ReadyToDeal, Payed, PickedUp, Dismiss };
        DrugDealSates DealState = DrugDealSates.SpawnStuff;
        public bool EnterState = false;
        private bool Ready;
        DrugOrder PlayerOrder = null;
        DealerSpawnPostion dealerSpawn;
        Vector3 backPos = new Vector3(0, 0, 0);
        int time = 0;

        public bool Kill { get; private set; } = false;

        public DrugDealer(DrugOrder _order)
        {
            dealerSpawn = GenerateDealerSpawn();
            PlayerOrder = _order;
            if (dealerSpawn != null)
            {
                if(_order != null)
                {
                                        
                    DbugString("dealer spawn found,~n~heading " + dealerSpawn.Heading + ",~n~pos " + dealerSpawn.Position);
                    
                }
                else
                {
                    SendTextMsg("LsLife: Error Order is null");
                    Destroy();
                }    
                

            }
            else
            {
                SendTextMsg("We havent got anyone around there.");
                DealState = DrugDealSates.Destroy;
            }

        }

        public void OnTick()
        {
            switch (DealState)
            {
                case DrugDealSates.ReadyToDeal:
                    {
                        if (Ready)
                        {
                            if (Game.IsControlJustReleased(2, GTA.Control.Context))
                            {
                                if (PlayerOrder.PayBill())
                                {

                                    if (player.Money > 0) HandOverGoods(player.Character, lieut);

                                    DealState = DrugDealSates.Payed;
                                    EnterState = true;
                                    Ready = false;
                                    Function.Call(Hash._PLAY_AMBIENT_SPEECH1, lieut, "GENERIC_THANKS", "SPEECH_PARAMS_STANDARD", 0);
                                }
                            }
                        }
                        break;
                    }

                case DrugDealSates.Payed:
                    {

                        World.DrawMarker(MarkerType.UpsideDownCone, backPos, Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 0.5f), Color.Green);
                        if (Ready)
                        {
                            if (Game.IsControlJustReleased(2, GTA.Control.Context))
                            {
                                PlayerOrder.PickupDrugs();
                                if (PlayerOrder.pickedUp)
                                {
                                    NewOrder.ResetOrder();
                                    DealState = DrugDealSates.PickedUp;
                                    EnterState = true;
                                    Ready = false;
                                }
                            }
                        }
                        break;
                    }
            }

        }

        private static void UpdateZeeDealerMenu()
        {

        }

        public void OnSlowTick()
        {
            switch (DealState)
            {
                case DrugDealSates.SpawnStuff:
                    {
                        if(dealerCar == null)
                        {
                            dealerCar = World.CreateVehicle(VehicleHash.Dubsta, dealerSpawn.Position, dealerSpawn.Heading);
                            int time = Game.GameTime;
                            while (dealerCar == null && !dealerCar.Exists() || Game.GameTime < time + 1000)
                            {
                                Script.Wait(0);
                            }
                            if (dealerCar != null && dealerCar.Exists())
                            {                                
                                dealerCar.IsPersistent = true;
                                dealerCar.LockStatus = VehicleLockStatus.LockedForPlayer;                                
                            }
                            else
                            {
                                SendTextMsg("LsLife: ~r~Failed to spawn drug dealers Vehicle");
                                Destroy();
                            }
                        }
                        if (goon == null && dealerCar != null)
                        {
                            SpawnPed(VehicleSeat.Driver, false);
                            if (dealerCar.Driver != null) goon = dealerCar.Driver;
                        }
                        if(lieut == null && dealerCar != null)
                        {
                            SpawnPed(VehicleSeat.Passenger, true);
                            if (dealerCar.GetPedOnSeat(VehicleSeat.Passenger) != null) lieut = dealerCar.GetPedOnSeat(VehicleSeat.Passenger);
                        }
                        if(blip == null)
                        {
                            blip = World.CreateBlip(dealerSpawn.Position);
                            if (blip != null)
                            {
                                blip.Sprite = BlipSprite.SpecialCargo;
                                blip.Name = "Drug Pickup";
                                blip.ShowRoute = true;
                                DbugString("dealer car spawned");
                                string streetName = World.GetStreetName(dealerSpawn.Position);
                                SendTextMsg("Ok go meet my people near ~y~" + streetName + " ~s~, they will sort you out.");
                            }
                            else
                            {
                                SendTextMsg("LsLife: ~r~Failed to create blip");
                                Destroy();
                            }
                        }
                        if (lieut != null && lieut.Exists() && goon != null && goon.Exists())
                        {
                            EnterState = true;
                            DealState = DrugDealSates.ConfigStuff;
                        }
                        break;
                    }
                case DrugDealSates.ConfigStuff:
                    {
                        if (EnterState)
                        {
                            EnterState = false;
                            MakeGroup();                            
                        }
                        else
                        {
                            EnterState = true;
                            DealState = DrugDealSates.WaitForPlayer;
                        }
                        break;
                    }
                case DrugDealSates.WaitForPlayer:
                    {
                        if (EnterState)
                        {
                            EnterState = false;
                        }
                        else
                        if (playerPos.DistanceTo(lieut.Position) < 20 && CanSee(player.Character, lieut))
                        {
                            EnterState = true;
                            DealState = DrugDealSates.GetIntoPosition;
                        }
                        break;
                    }
                case DrugDealSates.GetIntoPosition:
                    {
                        if (EnterState)
                        {
                            EnterState = false;
                            backPos = Vector3.Add(dealerCar.Position, Vector3.Multiply(dealerCar.ForwardVector * -1, (dealerCar.Model.GetDimensions().Y / 2) + 0.5f));
                            dealerCar.OpenDoor(VehicleDoor.Trunk, false, false);
                            lieut.Task.LookAt(Vector3.Add(dealerCar.Position, Vector3.Multiply(dealerCar.ForwardVector * -1, (dealerCar.Model.GetDimensions().Y / 2) + 0.5f)));
                            lieut.Task.GoTo(Vector3.Add(dealerCar.Position, Vector3.Multiply(dealerCar.ForwardVector * -1, (dealerCar.Model.GetDimensions().Y / 2) + 0.5f)) + dealerCar.RightVector * 1);
                            goon.Task.GoTo(Vector3.Add(dealerCar.Position, Vector3.Multiply(dealerCar.ForwardVector * -1, (dealerCar.Model.GetDimensions().Y / 2) + 0.5f)) + dealerCar.RightVector * -2);
                        }
                        else
                        if (lieut.Position.DistanceTo(Vector3.Add(dealerCar.Position, Vector3.Multiply(dealerCar.ForwardVector * -1, (dealerCar.Model.GetDimensions().Y / 2) + 0.5f)) + dealerCar.RightVector * 1) < 1)
                        {
                            EnterState = true;
                            DealState = DrugDealSates.GetReadyToDeal;
                        }
                        break;
                    }
                case DrugDealSates.GetReadyToDeal:
                    {
                        if (EnterState)
                        {
                            EnterState = false;
                        }
                        else
                        {
                            if (dealerCar.IsDoorOpen(VehicleDoor.Trunk))
                            {
                                lieut.Task.LookAt(player.Character);
                                goon.Task.LookAt(player.Character);
                                lieut.Task.TurnTo(player.Character);
                                goon.Task.TurnTo(player.Character);
                                goon.Weapons.Select(goon.Weapons.BestWeapon);
                                EnterState = true;
                                DealState = DrugDealSates.ReadyToDeal;
                            }
                        }
                        break;
                    }
                case DrugDealSates.ReadyToDeal:
                    {
                        if (EnterState)
                        {
                            EnterState = false;
                        }
                        if (player.Character.IsStopped && player.Character.Position.DistanceTo(lieut.Position) < 3f)
                        {
                            PayBillPromt();
                            Ready = true;
                        }
                        else Ready = false;
                        break;
                    }
                case DrugDealSates.Payed:
                    {
                        if (EnterState)
                        {
                            EnterState = false;
                        }
                        if (player.Character.IsStopped && playerPos.DistanceTo(backPos) < 1.5F)
                        {
                            PickupPromt();
                            Ready = true;
                        }
                        else Ready = false;
                        break;
                    }
                case DrugDealSates.PickedUp:
                    {
                        if (EnterState)
                        {
                            Function.Call(Hash._PLAY_AMBIENT_SPEECH1, lieut, "GENERIC_BYE", "SPEECH_PARAMS_STANDARD", 0);
                            EnterState = false;
                        }
                        else
                        {
                            DealState = DrugDealSates.Dismiss;
                            EnterState = true;
                        }
                        break;
                    }


                case DrugDealSates.Dismiss:
                    {
                        if (EnterState)
                        {
                            time = Game.GameTime;
                            EnterState = false;
                            lieut.Task.EnterVehicle(dealerCar, VehicleSeat.RightFront);
                            goon.Task.EnterVehicle(dealerCar, VehicleSeat.Driver);
                        }
                        if (goon.IsInVehicle() && lieut.IsInVehicle() || Game.GameTime > time + 10000)
                        {
                            dealerCar.CloseDoor(VehicleDoor.Trunk, false);
                            EnterState = true;
                            DealState = DrugDealSates.Destroy;
                            if (PlayerOrder != null) PlayerOrder = null;
                        }
                        break;
                    }
                case DrugDealSates.Destroy:
                    {
                        if (EnterState)
                        {
                            EnterState = false;
                            if (lieut != null && lieut.Exists()) RemovePedFromWorld(lieut, true);
                            if (goon != null && lieut.Exists()) RemovePedFromWorld(goon, true);
                            if (dealerCar != null && dealerCar.Exists()) RemoveVehicle();
                            if (blip != null && blip.Exists()) RemoveBlip();
                        }
                        else
                        {
                            Kill = true;
                        }
                        break;
                    }
            }
        }

        private void PickupPromt()
        {
            if (!LsMenuPool.IsAnyMenuOpen() && Game.GameTime > displayHelpTimer + 6000)
            {
                displayHelpTimer = Game.GameTime;
                DisplayHelpText("Press ~INPUT_CONTEXT~ to pickup the Drugs");
            }
        }

        public void Dismiss()
        {
            EnterState = true;
            DealState = DrugDealSates.Dismiss;
        }

        void PayBillPromt()
        {
            if (!LsMenuPool.IsAnyMenuOpen() && Game.GameTime > displayHelpTimer + 6000)
            {
                displayHelpTimer = Game.GameTime;
                if (PlayerOrder.CanPayBill()) DisplayHelpText("Press ~INPUT_CONTEXT~ to do the deal");
                else DisplayHelpText("You do not have enough credit or cash to cover the price.");
            }
        }

        private void RemoveBlip()
        {
            blip.Alpha = 0;
            blip.Remove();
        }

        private void RemoveVehicle()
        {
            dealerCar.MarkAsNoLongerNeeded();
        }

        private void SpawnPed(VehicleSeat _seat, bool _leader)
        {
            PedHash pedHash = PedHash.ArmLieut01GMM;
            if (!_leader) pedHash = PedHash.ArmGoon02GMY;
            Model model = (Model)pedHash;
            model.Request();
            Script.Wait(100);
            if(model.IsInCdImage)
            {                
                Ped p = dealerCar.CreatePedOnSeat(_seat, model);
                time = Game.GameTime;
                while (p == null && !p.Exists() || Game.GameTime < time + 1000)
                {
                    Script.Wait(0);
                }
                if (p != null && p.Exists())
                {
                    p.IsPersistent = true;
                    p.RelationshipGroup = playersTeam;
                    SetRandomCombatStats(p);
                    p.FiringPattern = FiringPattern.BurstFireDriveby;
                    p.AlwaysKeepTask = true;
                    p.CanSwitchWeapons = true;
                    p.BlockPermanentEvents = true;
                    Function.Call(Hash.SET_PED_COMBAT_ABILITY, p, 100);

                    int wepRnd = rnd.Next(0, (int)(1 + areas[areaIndex].GangPresance * 0.5f));
                    switch (wepRnd)
                    {
                        case 5:
                            p.Weapons.Give(WeaponHash.AssaultRifle, 500, false, true);
                            p.Weapons.Give(WeaponHash.MicroSMG, 500, true, true);
                            break;
                        case 4:
                            p.Weapons.Give(WeaponHash.CompactRifle, 500, false, true);
                            p.Weapons.Give(WeaponHash.MiniSMG, 500, true, true);
                            break;

                        case 3:
                            p.Weapons.Give(WeaponHash.CompactRifle, 500, false, true);
                            p.Weapons.Give(WeaponHash.MachinePistol, 500, true, true);
                            break;
                        case 2:
                            p.Weapons.Give(WeaponHash.PumpShotgun, 500, false, true);
                            p.Weapons.Give(WeaponHash.MachinePistol, 500, true, true);
                            break;
                        case 1:
                            p.Weapons.Give(WeaponHash.SawnOffShotgun, 500, false, true);
                            p.Weapons.Give(WeaponHash.Pistol, 500, true, true);
                            break;
                        case 0:
                            p.Weapons.Give(WeaponHash.Pistol, 500, true, true);
                            break;
                    }
                }
                else SendTextMsg("LsLife: ~r~Failed to spawned DrugDealer Ped for meeting");
            }            
            else
            {
                SendTextMsg("LsLife: ~r~Failed to spawned DrugDealer Ped for meeting");
            }
            model.MarkAsNoLongerNeeded();
        }

        DealerSpawnPostion GenerateDealerSpawn()
        {
            List<DealerSpawnPostion> spawnPostionsTmp = spawnPostions.Where(s => s.Position.DistanceTo(playerPos) < 1800 && s.Position.DistanceTo(playerPos) > 400 - CurrentRepLvl(dealer1Rep)).ToList();
            if (spawnPostionsTmp.Count > 0)
            {
                int rndInt = rnd.Next(0, spawnPostionsTmp.Count);
                return spawnPostionsTmp[rndInt];
            }
            else return null;


            //Vector3 pos = GenerateSpawnPos(_pos.Around(350), Nodetype.Offroad, false);

            //OutputArgument outArgA = new OutputArgument();
            //OutputArgument outArgB = new OutputArgument();
            //int nodeNumber = 1;

            //if (Function.Call<bool>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID_WITH_HEADING, pos.X, pos.Y, pos.Z, nodeNumber,outArgA, outArgB, 1, 1077936128, 0))
            //{

            //    heading = outArgB.GetResult<float>(); //getting heading
            //    pos = outArgA.GetResult<Vector3>();
            //    OutputArgument BRPout = new OutputArgument();
            //    Vector3 BigRoadPos;
            //    Function.Call(Hash.GET_CLOSEST_VEHICLE_NODE, pos.X, pos.Y, pos.Z, BRPout, 0, 3.2f, 0);
            //    BigRoadPos = BRPout.GetResult<Vector3>();
            //    int nodetype = 1;
            //    bool foundSpot = false;
            //    int timeOut = Game.GameTime;
            //    while (!foundSpot || Game.GameTime < timeOut + 10000)
            //    {
            //        if(GetRoadFlags(pos).Contains(PathnodeFlags.SlowTraffic) && pos.DistanceTo(BigRoadPos) > 5 && pos.DistanceTo(playerPos) > 200)
            //        {
            //            foundSpot = true;
            //        }
            //        else                    
            //        if (Function.Call<bool>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID_WITH_HEADING, pos.X, pos.Y, pos.Z, nodeNumber, outArgA, outArgB, nodetype, 1077936128, 0))
            //        {
            //            heading = outArgB.GetResult<float>(); //getting heading
            //            pos = outArgA.GetResult<Vector3>();
            //            Function.Call(Hash.GET_CLOSEST_VEHICLE_NODE, pos.X, pos.Y, pos.Z, BRPout, 0, 3.2f, 0);
            //            BigRoadPos = BRPout.GetResult<Vector3>();
            //            //nodetype += 4;
            //            nodeNumber++;
            //        }
            //    }
            //    //pos = World.GetNextPositionOnStreet(pos, true);
            //    //pos.Z = World.GetGroundHeight(pos)+0.5f;
            //    //pos = World.GetSafeCoordForPed(pos, false);                
            //}
            //return pos;
        }

        void MakeGroup()
        {
            PedGroup peds = new PedGroup();
            if (lieut != null && lieut.Exists())
            {
                peds.Add(lieut, true);
                lieut.NeverLeavesGroup = true;
            }
            if(goon != null && goon.Exists())
            {
                peds.Add(goon, false);
                goon.NeverLeavesGroup = true;
            }            
        }

        public void Destroy()
        {
            EnterState = true;
            DealState = DrugDealSates.Destroy;
        }

    }
}
