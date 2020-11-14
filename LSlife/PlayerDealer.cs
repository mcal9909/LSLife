using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static LSlife.Enums;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class PlayerDealer
    {
        public int dArmour { get; set; } = 0;
        public List<XElement> dWeapons = new List<XElement>();
        public DealState dealState = DealState.GetNear;
        public string area { get; private set; }
        public int ID = 0;
        public Dictionary<string, int> Drugs = new Dictionary<string, int>();
        public Ped Ped { get; private set; }
        public Ped cPed { get; private set; }
        public Ped lPed { get; private set; }
        public int Money;
        public PdStates CurrentState = PdStates.NoState;
        public PdStates NewState = PdStates.Normal;
        HealPositionTypes HealPosType = HealPositionTypes.None;
        public PedHash pHash;
        public Vector3 Pos;
        Blip blip;
        private int time;
        UIText pedInfo;
        Vector3 Target;
        public Dictionary<int, Tuple<int, int>> pData;
        public bool Working = false;
        int health;

        public PlayerDealer(Ped ped)
        {
            if (ped != null)
            {
                Ped = ped;
                Ped.BlockPermanentEvents = true;
                Ped.IsPersistent = true;
                SetRandomCombatStats(Ped);
                //Ped.AlwaysKeepTask = true;
                SetAbilities();

                Ped.RelationshipGroup = playersTeam;
                SetImmuneToPlayer(Ped);
                dArmour = Ped.Armor;
                GetFreshPedsWeapons();
                Ped.Weapons.Select(Ped.Weapons.BestWeapon);
                Ped.CanSwitchWeapons = true;
                area = areas[areaIndex].Name;
                ID = DealerHandler.GetNewID();
                SetStartDrugs();
                pData = GetPedVariationData(Ped);
                SaveFreshDealer();
                NewState = PdStates.FollowPlayer;
                AddBlip(ped, BlipColor.Blue, "Dealer", true, true);
                SetBlip();

            }
        }
        public PlayerDealer(int id, PedHash hash, PdStates _newState, Vector3 pos, int weed, int crack, int cocaine, int money, int _armour, List<XElement> _weapons, Dictionary<int, Tuple<int, int>> _pData)
        {
            area = World.GetZoneNameLabel(pos);
            ID = id;
            pHash = hash;
            Pos = pos;
            Drugs.Add("Weed", weed);
            Drugs.Add("Crack", crack);
            Drugs.Add("Cocaine", cocaine);
            Money = money;
            dArmour = _armour;
            if (_weapons.Count > 0)
            {
                for (int i = 0; i < _weapons.Count; i++)
                {
                    dWeapons.Add(_weapons[i]);
                }
            }
            NewState = _newState;
            CreateLocationBlip();
            pData = _pData;
        }

        void Innit()
        {

        }

        void GetFreshPedsWeapons()
        {
            if (Ped != null) dWeapons = PedWeapons(Ped);

        }

        void SaveFreshDealer()
        {
            XDocument dealerDoc = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");

            Money = 0;
            Pos = Ped.Position;
            pHash = (PedHash)Ped.Model.Hash;

            var parentElement = new XElement("dealer");
            var id = new XElement("id");
            id.Value = ID.ToString();
            var pedHash = new XElement("hash");
            pedHash.Value = pHash.ToString();
            var posX = new XElement("posX");
            posX.Value = Ped.Position.X.ToString();
            var posY = new XElement("posY");
            posY.Value = Ped.Position.Y.ToString();
            var posZ = new XElement("posZ");
            posZ.Value = Ped.Position.Z.ToString();
            var weed = new XElement("weed");
            weed.Value = Drugs["Weed"].ToString();
            var crack = new XElement("crack");
            crack.Value = Drugs["Crack"].ToString();
            var coke = new XElement("cocaine");
            coke.Value = Drugs["Cocaine"].ToString();
            var money = new XElement("money");
            money.Value = Money.ToString();
            var arm = new XElement("armour");
            arm.Value = dArmour.ToString();

            var pedData = new XElement("pedData");
            bool savePedData = false;
            if (pData?.Count > 0)
            {
                savePedData = true;
                var headD = new XElement("headD", pData[0].Item1.ToString());
                var headT = new XElement("headT", pData[0].Item2.ToString());
                pedData.Add(headD);
                pedData.Add(headT);
                var beardD = new XElement("beardD", pData[1].Item1.ToString());
                var beardT = new XElement("beardT", pData[1].Item2.ToString());
                pedData.Add(beardD);
                pedData.Add(beardT);
                var hairD = new XElement("hairD", pData[2].Item1.ToString());
                var hairT = new XElement("hairT", pData[2].Item2.ToString());
                pedData.Add(hairD);
                pedData.Add(hairT);
                var torsoD = new XElement("torsoD", pData[3].Item1.ToString());
                var torsoT = new XElement("torsoT", pData[3].Item2.ToString());
                pedData.Add(torsoD);
                pedData.Add(torsoT);
                var legsD = new XElement("legsD", pData[4].Item1.ToString());
                var legsT = new XElement("legsT", pData[4].Item2.ToString());
                pedData.Add(legsD);
                pedData.Add(legsT);
                var handsD = new XElement("handsD", pData[5].Item1.ToString());
                var handsT = new XElement("handsT", pData[5].Item2.ToString());
                pedData.Add(handsD);
                pedData.Add(handsT);
                var footD = new XElement("footD", pData[6].Item1.ToString());
                var footT = new XElement("footT", pData[6].Item2.ToString());
                pedData.Add(footD);
                pedData.Add(footT);
                var acces1D = new XElement("acces1D", pData[8].Item1.ToString());
                var acces1T = new XElement("acces1T", pData[8].Item2.ToString());
                pedData.Add(acces1D);
                pedData.Add(acces1T);
                var acces2D = new XElement("acces2D", pData[9].Item1.ToString());
                var acces2T = new XElement("acces2T", pData[9].Item2.ToString());
                pedData.Add(acces2D);
                pedData.Add(acces2T);
                var decalD = new XElement("decalD", pData[10].Item1.ToString());
                var decalT = new XElement("decalT", pData[10].Item2.ToString());
                pedData.Add(decalD);
                pedData.Add(decalT);
                var torsoAuxD = new XElement("torsoAuxD", pData[11].Item1.ToString());
                var torsoAuxT = new XElement("torsoAuxT", pData[11].Item2.ToString());
                pedData.Add(torsoAuxD);
                pedData.Add(torsoAuxT);
            }
            parentElement.Add(id);
            parentElement.Add(pedHash);
            parentElement.Add(posX);
            parentElement.Add(posY);
            parentElement.Add(posZ);
            parentElement.Add(weed);
            parentElement.Add(crack);
            parentElement.Add(coke);
            parentElement.Add(money);
            parentElement.Add(arm);
            if (dWeapons?.Count > 0)
            {
                foreach (XElement ele in dWeapons)
                {
                    if (ele.Name == "weapon")
                    {
                        foreach (XElement e in ele.Elements())
                        {
                            if (e.Name == "HASH" && e.Value != "Unarmed")
                            {
                                parentElement.Add(e.Parent);
                            }
                        }
                    }
                }
            }
            if (savePedData) parentElement.Add(pedData);
            dealerDoc.Root.Add(parentElement);
            dealerDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
        }

        public void OnTick()
        {
            if (Ped != null)
            {
                //Function.Call(Hash._0x2E2F4240B3F24647, Ped.Handle, Game.Player.Character.CurrentPedGroup.Handle, false);
                //if(DEBUG)
                //{
                //    if (Ped.IsOnScreen)
                //    {
                //        //int ammunitionInWeapon = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Ped, (int)Ped.Weapons.Current.Hash);
                //        //string weaponAmmo = Ped.Weapons.Current.Ammo.ToString();
                //        Vector2 tPos = World3DToScreen2d(Ped.GetBoneCoord(Bone.FB_Brow_Centre_000));
                //        //Size size = Game.ScreenResolution;
                //        tPos.X = 1280 * tPos.X;
                //        tPos.Y = 720 * tPos.Y;
                //        pedInfo = new UIText(Ped.Health.ToString(), new Point((int)tPos.X, (int)tPos.Y), 0.4f, Color.WhiteSmoke, 0, true, true, true);
                //        pedInfo.Draw();
                //        //myUIText = new UIText(tPos.ToString(), new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                //        //myUIText.Draw();
                //    }
                //}
                switch (CurrentState)
                {
                    case PdStates.TalkToPlayer:
                    case PdStates.FollowPlayer:
                    case PdStates.FollowingPlayer:
                        PromtPlayerToTalk();
                        break;
                    default:
                        PromtToFollow();
                        break;
                }
            }
        }

        public void OnSlowTick()
        {
            if (Ped != null)
            {
                if (Ped.RelationshipGroup != playersTeam)
                {
                    UI.Notify("~r~Dealer was removed from companion group");
                    Ped.RelationshipGroup = playersTeam;
                }
                if (!isFollowing() && driveByHandler.AmountOfDriveBy() != 0 && !playerNear()) NewState = PdStates.Defend;
                if (Pos != Ped.Position) Pos = Ped.Position;
                if (area != World.GetZoneNameLabel(Ped.Position)) area = World.GetZoneNameLabel(Ped.Position);
                dArmour = Ped.Armor;
                if (CurrentState != PdStates.TalkToPlayer && CurrentState != PdStates.FollowPlayer && CurrentState != PdStates.FollowingPlayer && DealerHandler.currentDealer == this) DealerHandler.currentDealer = null;
                if (!Ped.IsAlive && NewState != PdStates.Dead) NewState = PdStates.Dead;


                if (NewState != CurrentState)
                {
                    if (cPed != null) RemovePedFromWorld(cPed, true);
                    if (DEBUG) UI.Notify("PDealer state = " + NewState);
                    switch (NewState)
                    {
                        case PdStates.Dead:
                            CurrentState = PdStates.Dead;
                            string onDeathSay = "One of your dealers dropped a ~g~bag.";
                            pickupHandler.AddBag(new DroppedBag(onDeathSay, Drugs["Weed"], Drugs["Crack"], Drugs["Cocaine"], Money, dWeapons.ToList(), Pos));
                            RemoveBlip();
                            Ped.IsPersistent = false;
                            Ped.MarkAsNoLongerNeeded();
                            break;

                        case PdStates.Defend:
                            CurrentState = PdStates.Defend;
                            Ped.BlockPermanentEvents = false;
                            break;


                        case PdStates.WalkToPath:
                            Target = World.GetNextPositionOnSidewalk(Ped.Position);
                            Ped.Task.GoTo(Target);
                            CurrentState = PdStates.WalkToPath;
                            time = Game.GameTime;
                            break;

                        case PdStates.StopForPlayer:
                            Ped.Task.StandStill(50000);
                            CurrentState = PdStates.StopForPlayer;
                            break;

                        case PdStates.LookAtPlayer:
                            Ped.Task.TurnTo(Game.Player.Character);
                            CurrentState = PdStates.LookAtPlayer;
                            break;

                        case PdStates.TalkToPlayer:
                            CurrentState = PdStates.TalkToPlayer;
                            Function.Call(Hash._PLAY_AMBIENT_SPEECH1, Ped, "GENERIC_HOWS_IT_GOING", "SPEECH_PARAMS_FORCE");
                            break;

                        case PdStates.Normal:
                            Working = true;
                            Target = World.GetNextPositionOnSidewalk(Ped.Position);
                            if (Ped != null) SetBlip();
                            if (Ped.CurrentPedGroup == Game.Player.Character.CurrentPedGroup) Ped.LeaveGroup();
                            if (Ped.CurrentVehicle != null && Ped.CurrentVehicle == Game.Player.Character.CurrentVehicle)
                            {
                                TaskSequence sequence = new TaskSequence();
                                sequence.AddTask.LeaveVehicle();
                                sequence.AddTask.GuardCurrentPosition();
                                Ped.Task.PerformSequence(sequence);
                                sequence.Dispose();
                            }
                            else
                            {
                                Ped.Task.GuardCurrentPosition();
                            }
                            Ped.BlockPermanentEvents = true;
                            CurrentState = PdStates.Normal;
                            break;

                        case PdStates.Dealing:
                            CurrentState = PdStates.Dealing;
                            break;

                        case PdStates.FollowPlayer:
                            Working = false;
                            if (Ped != null) SetBlip();
                            Ped.Task.ClearAll();
                            CurrentState = PdStates.FollowPlayer;
                            break;

                        case PdStates.FollowingPlayer:
                            CurrentState = PdStates.FollowingPlayer;
                            break;


                        case PdStates.Rest:
                            Target = DealerHandler.GetCloseHealPosition();
                            if (DealerHandler.HealPositionType != HealPositionTypes.None && Target.DistanceTo(Ped.Position) < 30)
                            {
                                if (!Ped.IsInVehicle() || Ped.IsInVehicle() && Ped.CurrentVehicle.Speed < 1)
                                {
                                    if (DealerHandler.HealPositionType == HealPositionTypes.Hospital && Game.Player.Money >= (Ped.MaxHealth - Ped.Health) * 50)
                                    {
                                        Ped.LeaveGroup();
                                        Ped.BlockPermanentEvents = true;
                                        HealPosType = DealerHandler.HealPositionType;
                                        Ped.Task.RunTo(Target);
                                        CurrentState = PdStates.Rest;
                                    }
                                    else if (DealerHandler.HealPositionType == HealPositionTypes.House)
                                    {
                                        Ped.LeaveGroup();
                                        Ped.BlockPermanentEvents = true;
                                        HealPosType = DealerHandler.HealPositionType;
                                        Ped.Task.RunTo(Target);
                                        CurrentState = PdStates.Rest;
                                    }
                                }
                            }
                            break;

                        case PdStates.Resting:
                            XDocument dealerDoc1 = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
                            DealerHandler.SaveDealer(this, dealerDoc1);
                            dealerDoc1.Save("scripts\\LSLife\\LSLife_Dealers.xml");
                            CurrentState = PdStates.Resting;
                            break;
                    }
                }
                else
                {
                    switch (CurrentState)
                    {
                        case PdStates.WalkToPath:

                            if (Ped.Position.DistanceTo(Target) <= 2f)
                            {
                                NewState = PdStates.Normal;
                            }
                            if (playerNear()) NewState = PdStates.StopForPlayer;
                            break;

                        case PdStates.Defend:
                            if (driveByHandler.AmountOfDriveBy() == 0)
                            {
                                NewState = PdStates.Normal;
                                Ped.BlockPermanentEvents = true;
                            }
                            break;

                        case PdStates.Normal:
                            if (playerNear()) NewState = PdStates.StopForPlayer;
                            else if (Ped.Position.DistanceTo(Target) > 1) NewState = PdStates.WalkToPath;
                            else NewState = PdStates.Dealing;
                            break;

                        case PdStates.StopForPlayer:
                            if (Ped.IsStopped) NewState = PdStates.LookAtPlayer;
                            if (!playerNear()) NewState = PdStates.Normal;
                            break;

                        case PdStates.LookAtPlayer:
                            if (!Ped.IsHeadtracking(player.Character)) Ped.Task.LookAt(player.Character);
                            else NewState = PdStates.TalkToPlayer;
                            if (!playerNear()) NewState = PdStates.Normal;
                            break;

                        case PdStates.TalkToPlayer:
                            if (!playerNear())
                            {
                                if (DealerHandler.currentDealer == this && WorkerMainMenu.Visible) WorkerMainMenu.Visible = false;
                                NewState = PdStates.Normal;
                                Function.Call(Hash._PLAY_AMBIENT_SPEECH2, Ped, "GENERIC_BYE", "SPEECH_PARAMS_FORCE");
                            }
                            break;

                        case PdStates.Dealing:
                            Target = World.GetNextPositionOnSidewalk(Ped.Position + Ped.Velocity);
                            if (playerNear()) NewState = PdStates.StopForPlayer;
                            else if (Ped.Position.DistanceTo(Target) > 2.3f && Function.Call<bool>(Hash.IS_POINT_ON_ROAD, Ped.Position.X, Ped.Position.Y, Ped.Position.Z)) NewState = PdStates.WalkToPath;
                            else if (TotalDrugs(Drugs) > 0) Deal();
                            break;

                        case PdStates.FollowPlayer:
                            if (player.Character.CurrentPedGroup != Ped.CurrentPedGroup && player.Character.Position.DistanceTo(Ped.Position) < 100)
                            {
                                Ped.BlockPermanentEvents = false;
                                player.Character.CurrentPedGroup.Add(Ped, false);
                                Function.Call(Hash._0x2E2F4240B3F24647, Ped.Handle, player.Character.CurrentPedGroup.Handle, false);
                                NewState = PdStates.FollowingPlayer;
                            }
                            else if (player.Character.CurrentPedGroup == Ped.CurrentPedGroup)
                            {
                                Ped.BlockPermanentEvents = false;
                                Function.Call(Hash._0x2E2F4240B3F24647, Ped.Handle, player.Character.CurrentPedGroup.Handle, false);
                                NewState = PdStates.FollowingPlayer;
                            }
                            break;

                        case PdStates.FollowingPlayer:
                            if (Ped.Health < (Ped.MaxHealth * 0.9f))
                            {
                                NewState = PdStates.Rest;
                            }
                            if (player.Character.IsDead || Ped.CurrentPedGroup != player.Character.CurrentPedGroup)
                            {
                                Ped.BlockPermanentEvents = true;
                                NewState = PdStates.FollowPlayer;
                            }
                            break;

                        case PdStates.Rest:
                            if (Ped.Position.DistanceTo(Target) < 2)
                            {
                                if (HealPosType == HealPositionTypes.House)
                                {
                                    //Ped.Position = HouseHandler.CurrentHouse.exitPos;
                                }
                                health = Ped.Health;
                                NewState = PdStates.Resting;
                            }
                            break;
                        case PdStates.Resting:
                            //someHealingStuff
                            if (Ped.Health < Ped.MaxHealth)
                            {
                                if (HealPosType == HealPositionTypes.Hospital)
                                {
                                    Ped.Health = Ped.MaxHealth;
                                    Game.Player.Money -= (Ped.MaxHealth - Ped.Health) * 50;
                                    PlayTheSound();
                                }
                                else Ped.Health++;
                            }
                            else
                            {
                                NewState = PdStates.FollowPlayer;
                            }

                            break;

                    }
                }
                if (Ped.Position.DistanceTo(playerPos) > 200 && area != playerArea && NewState != PdStates.FollowPlayer && NewState != PdStates.FollowingPlayer) DespawnPed();
            }

            else if (Pos.DistanceTo(playerPos) <= 200)
            {
                if (CurrentState != PdStates.Dead)
                {
                    SpawnPed();
                }
            }
        }

        public void GiveArmourToDealer()
        {
            Ped.Armor = 200;
            dArmour = 200;
        }

        bool CheckIfDead()
        {
            if (!Ped.IsAlive)
            {
                RemoveBlip();
                Ped.IsPersistent = false;
                Ped = null;
                return true;
            }
            else return false;
        }

        void Deal()
        {
            if (Ped != null)
            {
                if (cPed != null && !cPed.Exists())
                {
                    lPed = cPed;
                    cPed = null;
                    if (DEBUG) UI.Notify("pDealer Customer doesnt exist");
                }
                if (cPed != null && cPed.Exists() && cPed.IsDead)
                {
                    lPed = cPed;
                    cPed = null;
                    if (DEBUG) UI.Notify("pDealer Customer doesnt exist");
                }
                if (cPed == null)
                {
                    if (rnd.Next(0, 5) == 0)
                    {
                        if (DEBUG) UI.Notify("pDealer Getting Customer");
                        var peds = lPeds.GetPeds;
                        if (peds.ToList().Count > 0)
                        {
                            foreach (Ped ped in peds)
                            {
                                if (!CheckIfPedUsed(ped, false, false) && ped != lPed)
                                {
                                    cPed = ped;
                                    cPed.FreezePosition = false;
                                    cPed.IsPersistent = true;
                                    if (DEBUG) { cPed.AddBlip(); }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (DEBUG) { UI.Notify("pDealer found no peds"); }
                        }
                    }

                }
                else
                {
                    switch (dealState)
                    {

                        case DealState.GetNear:
                            float dist = cPed.Position.DistanceTo(Ped.Position);
                            if (dist > 100)
                            {
                                RemovePedFromWorld(cPed, false);
                                cPed = null;
                            }
                            else if (cPed.Position.DistanceTo(Ped.Position) > 10)
                            {
                                MovePed(cPed, Ped.Position);
                            }
                            else
                            {
                                dealState = DealState.Approach;
                                cPed.Task.LookAt(Ped);
                                Ped.Task.TurnTo(cPed);
                            }
                            break;
                        case DealState.Approach:
                            if (cPed.Position.DistanceTo(Ped.Position) > 2) { MovePed(cPed, Ped.Position); }
                            else
                            {
                                cPed.Task.TurnTo(Ped);
                                Ped.Task.TurnTo(cPed);
                                dealState = DealState.InPosition;
                                time = Game.GameTime;
                            }
                            break;
                        case DealState.InPosition:
                            if (Game.GameTime > time + 1000)
                            {
                                if (DEBUG) { UI.Notify("Doing Deal"); }
                                dealState = DealState.DoingDeal;
                                cPed.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                                Ped.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_shopkeeper", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                                time = Game.GameTime;
                            }
                            break;
                        case DealState.DoingDeal:
                            if (Game.GameTime > time + 2000)
                            {
                                dealState = DealState.WalkingAway;
                                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, cPed, "GENERIC_THANKS", "SPEECH_PARAMS_STANDARD");
                                cPed.Task.WanderAround();
                                Ped.Task.GuardCurrentPosition();
                            }
                            break;
                        case DealState.WalkingAway:
                            if (cPed.Position.DistanceTo(Ped.Position) > 4)
                            {
                                dealState = DealState.GetNear;
                                if (DEBUG) { UI.Notify("Deal Finished"); }
                                RemovePedFromWorld(cPed, false);
                                cPed.CurrentBlip.Remove();
                                lPed = cPed;
                                cPed = null;
                            }
                            break;
                    }
                }
            }
            else
            {
                if (cPed != null)
                {
                    if (DEBUG) UI.Notify("aDealer Removing Customer to Far Away");
                    RemovePedFromWorld(cPed, false);
                    cPed = null;
                    if (dealState != DealState.GetNear) dealState = DealState.GetNear;
                }
            }
        }

        void SetAbilities()
        {
            Function.Call(Hash.SET_PED_COMBAT_ABILITY, Ped, 100);
            Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, Ped, 2);
            Ped.Accuracy = 100;
            Ped.ShootRate = 100;
            Ped.FiringPattern = FiringPattern.FullAuto;
            Ped.CanWrithe = false;
        }

        public void SpawnPed()
        {
            //Pos.Z = World.GetGroundHeight(Pos);
            Ped = World.CreatePed(pHash, Pos);
            if (Ped != null)
            {
                Function.Call(Hash._0x2E2F4240B3F24647, Ped.Handle, Game.Player.Character.CurrentPedGroup.Handle, false);
                SetImmuneToPlayer(Ped);
                AddBlip(Ped, BlipColor.Blue, "Dealer", true, true);
                SetBlip();
                Ped.Armor = dArmour;
                GiveSavedWeapons();
                RemoveLocationBlip();
                Ped.Health = 200;
                Ped.BlockPermanentEvents = true;
                Ped.IsPersistent = true;
                SetAbilities();
                Ped.RelationshipGroup = playersTeam;
                Ped.Weapons.Select(Ped.Weapons.BestWeapon);
                Ped.CanSwitchWeapons = true;
                if (CurrentState == PdStates.FollowPlayer && Game.Player.Character.CurrentPedGroup != Ped.CurrentPedGroup && playerPos.DistanceTo(Ped.Position) < 100)
                {
                    Game.Player.Character.CurrentPedGroup.Add(Ped, false);
                    Function.Call(Hash._0x2E2F4240B3F24647, Ped.Handle, Game.Player.Character.CurrentPedGroup.Handle, false);
                }
                if (pData.Count > 0)
                {
                    SetPedVariation(Ped, pData);
                }
            }
        }

        bool isFollowing()
        {
            if (NewState == PdStates.FollowPlayer || NewState == PdStates.FollowingPlayer || CurrentState == PdStates.FollowPlayer || CurrentState == PdStates.FollowingPlayer) return true;
            else return false;
        }

        void GiveSavedWeapons()
        {
            GivePedWeapons(Ped, dWeapons);
            Ped.Weapons.Select(Ped.Weapons.BestWeapon);
        }

        public void DespawnPed()
        {
            if (Ped != null)
            {
                if (DEBUG) UI.Notify("Dealer DESPAWNED");
                Pos = Ped.Position;
                RemoveBlip();
                Ped.Delete();
                Ped = null;
                CreateLocationBlip();
                NotImmuneToPlayer(Ped);
            }
        }

        public void SimulateSales(int _hours)
        {
            int nDealers = 0;
            Area pArea = areas.Find(a => a.Name == area);
            int startMoney = Money;
            if (pArea == null)
            {
                if (areaTypes.TryGetValue(area, out AreaType type))
                {
                    areas.Add(new Area(area, LsDrugs(), type));
                    if (DEBUG) UI.Notify("~r~Added area for dealer " + area);
                }
                else UI.Notify("~r~LsLife - Error getting area type for Area " + area);
            }
            if (pArea != null && pArea.Drugs.Count > 0)
            {
                if (pArea.GangPresance > 0)
                {
                    nDealers = 2;
                }
                if (isDealing && playerArea == pArea.Name)
                {
                    if (nDealers == 2) nDealers += 1;
                    else nDealers = 2;
                }
                Random rInt = new Random();
                for (int i = 0; i < _hours * 4; i++)
                {
                    foreach (Drug drug in pArea.Drugs)
                    {

                        int rIntMax = (int)(10f * drug.Demand);
                        if (nDealers > 0) rIntMax = (int)(10f * (drug.Demand / nDealers));

                        if (drug.Demand > 1) { rIntMax = 10; }
                        if (rInt.Next(0, 11 - rIntMax) <= 3)
                        {
                            //UI.Notify("Sale" + drug.Demand + " " + drug.Name);
                            if (Drugs[drug.Name] > 0)
                            {
                                int amount = rnd.Next(1, 15);
                                if (Drugs[drug.Name] < amount) amount = Drugs[drug.Name];
                                Money += amount * drug.PricePerG;
                                Drugs[drug.Name] -= amount;
                                drug.Supplied = true;
                                //if (DEBUG) UI.Notify("~r~Sold " + drug.Name + "*" + amount);
                            }
                            else
                            {
                                drug.Supplied = false;

                            }
                        }
                    }
                }
                if (DEBUG)
                {
                    if (startMoney != Money)
                    {
                        UI.Notify("Made $~g~" + (Money - startMoney));
                    }
                    else { UI.Notify("Made nothing"); }
                }
            }
        }

        public void GiveMoneyToPlayer()
        {
            Money -= (Money / 100) * 20;
            Game.Player.Money += Money;
            Money = 0;
            PlayTheSound();
        }

        void PromtPlayerToTalk()
        {
            if (!LsMenuPool.IsAnyMenuOpen())
            {
                if (!zeeClose())
                {
                    if (player.Character.IsStopped && playerPos.DistanceTo(Ped.Position) < 2.5f)
                    {
                        if (IsCustomerNear())
                        {
                            if (Game.GameTime > displayHelpTimer + 5000)
                            {
                                Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                displayHelpTimer = Game.GameTime;
                                DisplayHelpText("Press ~INPUT_CONTEXT~ to Talk to this person.");
                            }
                            if (Game.IsControlJustReleased(2, GTA.Control.Context))
                            {
                                Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                WorkerMainMenu.CurrentSelection = 3;
                                DealerHandler.currentDealer = this;
                                SetDrugDesc();
                                SetFollowButton();
                                SetArmourButton();
                                if (!WorkerMainMenu.Visible) { WorkerMainMenu.Visible = true; }
                            }
                        }
                        //else DbugString("customer close");

                    }
                    //else DbugString("Player not stopped or not close enough");
                }
                //else DbugString("Zee close");
            }
        }

        void PromtToFollow()
        {
            if (!LsMenuPool.IsAnyMenuOpen() && player.Character.IsStopped && playerPos.DistanceTo(Ped.Position) < 10 && player.Character.IsInVehicle() && Ped.CurrentPedGroup != player.Character.CurrentPedGroup && IsCustomerNear())
            {
                if (Game.GameTime > displayHelpTimer + 5000)
                {
                    Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                    displayHelpTimer = Game.GameTime;
                    DisplayHelpText("Press ~INPUT_VEH_HORN~ to make your dealer Follow you");
                }
                if (Game.IsControlJustPressed(2, GTA.Control.VehicleHorn))
                {
                    Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                    DealerHandler.currentDealer = this;
                    NewState = PdStates.FollowPlayer;
                }
            }
        }

        void SetFollowButton()
        {
            if (CurrentState == PdStates.FollowPlayer || CurrentState == PdStates.FollowingPlayer)
            {
                wFollow.Text = "Work here";
            }
            else
            {
                wFollow.Text = "Follow me";
            }
        }

        void SetArmourButton()
        {
            if (playerArmours == 0) wArmour.Text = "No armour left";
            else wArmour.Text = "Give armour";
        }



        public void SetDrugDesc()
        {
            SetWorkerMenuHealth(Ped);
            wWeed.Maximum = PlayerInventory["Weed"];
            wCrack.Maximum = PlayerInventory["Crack"];
            wCocaine.Maximum = PlayerInventory["Cocaine"];

            wWeed.Description = "Has " + GramsToOz(Drugs["Weed"]) + " Weed.";
            wCrack.Description = "Has " + GramsToOz(Drugs["Crack"]) + " Crack.";
            wCocaine.Description = "Has " + GramsToOz(Drugs["Cocaine"]) + " Cocaine.";
            int cMoney = Money - ((Money / 100) * 20);
            if (Money > 100) { wCollect.Description = "Dealer has $" + IntToMoney(Money) + " gets 20% cut, Collect $" + IntToMoney(cMoney) + "."; }
            else { wCollect.Description = "Dealer hasnt made any money yet"; }
        }

        bool playerNear()
        {
            if (Ped.Position.DistanceTo(playerPos) <= 5)
            {
                return true;
            }
            else return false;

        }

        void CreateLocationBlip()
        {
            if (blip == null)
            {
                blip = World.CreateBlip(Pos);
                blip.IsShortRange = true;
                blip.Name = "Dealer Location";
            }
        }
        public void RemoveLocationBlip()
        {
            if (blip != null)
            {
                blip.Remove();
                blip = null;
            }
        }

        void SetBlip()
        {
            if (NewState == PdStates.FollowPlayer || CurrentState == PdStates.FollowPlayer)
            {
                Ped.CurrentBlip.Color = BlipColor.BlueDark;
            }
            else
            {
                Ped.CurrentBlip.Color = BlipColor.Purple;
            }
            Ped.CurrentBlip.Name = "Dealer";
            Ped.CurrentBlip.Scale = 0.5f;
            Ped.CurrentBlip.IsShortRange = true;
        }
        void RemoveBlip()
        {
            Ped.CurrentBlip.Remove();
        }

        void SetStartDrugs()
        {
            Drugs.Add("Weed", rnd.Next(0, 28));
            Drugs.Add("Crack", rnd.Next(0, 28));
            Drugs.Add("Cocaine", rnd.Next(0, 28));
        }
    }
}
