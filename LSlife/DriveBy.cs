using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;
using System.Linq;
using static LSlife.Enums;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class DriveBy
    {
        public List<Ped> Peds = new List<Ped>();
        Vehicle Vehicle = null;
        VehicleHash vehicleHash;
        VehicleClass vehicleClass;
        int pedamount;
        public DriveBySates DriveBySate { get; private set; } = DriveBySates.VehDrive;
        bool enterState = false;
        public bool Dead = false;
        PedGroup Group = new PedGroup();
        Ped Leader;

        public Vector3 DefendPosition { get; private set; }
        Vector3 LastKnownPosition;
        Ped Target;
        int time;

        public DriveBy(VehicleClass _class, int _pedAmount, Ped _target)
        {
            DefendPosition = playerPos;
            LastKnownPosition = playerPos;
            vehicleClass = _class;
            pedamount = _pedAmount;
            Target = _target;
            SetVehicleClass();
            SpawnVehicle();
            SpawnPeds();
            if (Peds.Count > 0)
            {
                //SetTasks();
            }

        }

        public void OnTick()
        {
            CheckPeds();
            CheckVehicle();
            DoStuff();
        }

        private void DoStuff()
        {
            if (!Dead && Target.IsDead || Target.Position.DistanceTo(DefendPosition) > 500)
            {
                DriveBySate = DriveBySates.CleanDriveBy;
                enterState = true;
            }
            switch (DriveBySate)
            {
                case DriveBySates.VehDrive:
                    if (enterState)
                    {
                        enterState = false;
                        Leader.Task.DriveTo(Vehicle, LastKnownPosition, 10, 17, (int)DrivingStyle.AvoidTrafficExtremely);
                    }
                    bool seen = false;
                    foreach (Ped p in Peds)
                    {
                        if (CanSee(p, Target))
                        {
                            seen = true;
                            break;
                        }
                    }
                    if (seen)
                    {
                        LastKnownPosition = playerPos;
                        DriveBySate = DriveBySates.VehAttack;
                        enterState = true;
                        break;
                    }
                    else if (Leader.Position.DistanceTo(LastKnownPosition) < 10)
                    {
                        DriveBySate = DriveBySates.VehPatrol;
                        enterState = true;
                        break;
                    }

                    break;
                case DriveBySates.VehPatrol:
                    if (enterState)
                    {
                        enterState = false;
                        time = Game.GameTime;
                        Leader.Task.CruiseWithVehicle(Vehicle, 13f, (int)DrivingStyle.SometimesOvertakeTraffic);
                    }
                    if (CanSee(Leader, Target))
                    {
                        LastKnownPosition = playerPos;
                        DriveBySate = DriveBySates.VehAttack;
                        enterState = true;
                        break;
                    }
                    else if (Vehicle.Position.DistanceTo(LastKnownPosition) > 300)
                    {
                        DriveBySate = DriveBySates.VehDrive;
                        enterState = true;
                    }
                    else if (Game.GameTime > time + 120000)
                    {
                        DriveBySate = DriveBySates.CleanDriveBy;
                        enterState = true;
                    }
                    break;


                case DriveBySates.VehAttack:
                    if (enterState)
                    {
                        enterState = false;
                        Leader.Task.VehicleChase(Target);
                    }
                    if (!CanSee(Leader, Target))
                    {
                        LastKnownPosition = playerPos;
                        DriveBySate = DriveBySates.VehDrive;
                        enterState = true;
                    }
                    break;

                case DriveBySates.CleanDriveBy:
                    if (enterState)
                    {
                        //UI.Notify("DriveByCleaned");
                        enterState = false;
                        CleanDriveBy();
                    }
                    Dead = true;
                    break;

            }
        }

        private void CheckVehicle()
        {
            if (Vehicle != null && Vehicle.Exists())
            {
                if (Peds.Count == 0)
                {
                    RemoveVehicle();
                }
            }
        }

        private void RemoveVehicle()
        {
            //UI.Notify("VehicleREMOVED");
            Vehicle.CurrentBlip.Remove();
            Vehicle.MarkAsNoLongerNeeded();
            entitiesCleanUp.Add(Vehicle);
            Vehicle = null;
        }

        public void CleanDriveBy()
        {
            if (Peds.Count > 0)
            {
                foreach (Ped p in Peds.ToList())
                {
                    RemovePed(p);
                }
            }
            if (Vehicle != null)
            {
                RemoveVehicle();
            }
            Group.Dispose();
        }

        private void RemovePed(Ped p)
        {

            RemovePedFromWorld(p, true);
            Peds.Remove(p);
        }

        private void CheckPeds()
        {
            if (Peds.Count > 0)
            {
                foreach (Ped p in Peds.ToList())
                {
                    if (p != null && p.Exists())
                    {
                        if (!p.IsInVehicle() && !p.CurrentBlip.Exists())
                        {
                            AddBlip(p, BlipColor.Red, "Driveby Ped", false, false);
                        }
                        if (p.IsDead)
                        {
                            if (areas[areaIndex].GangPresance > 0) { areas[areaIndex].GangPresance -= 1; }
                            RemovePedFromWorld(p, true);
                            Peds.Remove(p);
                            //UI.Notify("DriveByPed removed");
                        }
                    }
                    else
                    {
                        Peds.Remove(p);
                    }
                }
            }
            if (Peds.Count == 0)
            {
                //UI.Notify("PedsDead");
                DriveBySate = DriveBySates.CleanDriveBy;
                enterState = true;
            }
            else if (Leader != Peds.First())
            {
                Leader = Peds.First();

                foreach (Ped p in Peds)
                {
                    if (p.CurrentPedGroup == Group) Group.Remove(p);
                    if (p == Leader) Group.Add(p, true);
                    else Group.Add(p, false);
                }
            }
        }

        private void SpawnVehicle()
        {
            Vehicle = World.CreateVehicle((Model)vehicleHash, GenerateSpawnPos(DefendPosition.Around(100), Nodetype.Road, false));

            int time = Game.GameTime;
            while (Vehicle == null || Game.GameTime > time + 1000)
            {
                Script.Wait(0);
            }
            if (Vehicle != null)
            {
                AddBlip(Vehicle, BlipColor.Red, "Driveby Vehicle", false, false);
                Vehicle.IsPersistent = true;
            }
            else if (DEBUG)
            {
                UI.Notify("~r~Spawning DriveBy Failed");
            }
        }

        private void SpawnPeds()
        {
            SpawnPed(VehicleSeat.Driver, true);
            if (pedamount > 0)
            {
                SpawnPed(VehicleSeat.RightFront, false);
                if (pedamount > 1)
                {
                    SpawnPed(VehicleSeat.RightRear, false);
                    if (pedamount > 2)
                    {
                        SpawnPed(VehicleSeat.LeftRear, false);
                    }
                }
            }
        }

        private void SpawnPed(VehicleSeat _seat, bool _leader)
        {
            Ped p = Vehicle.CreateRandomPedOnSeat(_seat);

            if (p != null && p.Exists())
            {
                p.IsPersistent = true;
                p.RelationshipGroup = rival_enemy;
                SetRandomCombatStats(p);
                p.FiringPattern = FiringPattern.BurstFireDriveby;
                p.AlwaysKeepTask = true;
                p.CanSwitchWeapons = true;
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
                if (_leader)
                {
                    Leader = p;
                    p.AlwaysKeepTask = true;
                    p.Task.DriveTo(Vehicle, LastKnownPosition, 10, 30, (int)DrivingStyle.AvoidTrafficExtremely);
                }
                else
                {
                    p.Task.VehicleShootAtPed(Target);
                }
                Peds.Add(p);
            }
        }

        private void SetVehicleClass()
        {
            var vehs = World.GetNearbyVehicles(Target, 200);
            vehicleHash = VehicleHash.Baller2;
            if (vehs.ToList().Count > 0)
            {
                foreach (Vehicle v in vehs)
                {
                    if (v.ClassType == vehicleClass)
                    {
                        vehicleHash = (VehicleHash)v.Model.Hash;
                        if (DEBUG) { UI.Notify("foundSUV"); }
                        break;
                    }
                }
            }
        }
    }
}
