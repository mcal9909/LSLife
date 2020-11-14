using GTA;
using GTA.Math;
using GTA.Native;
using System.Linq;
using static LSlife.Enums;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class AreaDealer
    {
        private bool DEBUG = false;
        public Ped dPed;
        public Vehicle Vehicle = null;
        public DStates CurrentState = DStates.NoState;
        public DStates NewState = DStates.Normal;
        public DealState dealState = DealState.GetNear;
        public bool Detected = false;
        Vector3 servePos = Vector3.Zero;
        public LsTimer cTimer = new LsTimer(5, true, false);
        public Ped cPed = null;
        int time;
        public int LastCustomer { get; private set; } = 0;
        public int LastOffer { get; private set; } = 0;
        public string Area;

        public AreaDealer(Ped p)
        {
            DEBUG = LSL.DEBUG;
            dPed = p;
            dPed.IsPersistent = true;

            //dPed.BlockPermanentEvents = true;
            dPed.Armor = 100;
            WeaponHash[] weaponHashes = { WeaponHash.Pistol, WeaponHash.PumpShotgun, WeaponHash.SawnOffShotgun, WeaponHash.MicroSMG };
            dPed.Weapons.Give(weaponHashes[rnd.Next(0, weaponHashes.Count())], 100, false, true);
            dPed.RelationshipGroup = rival_nutral;
            SetRandomCombatStats(dPed);
            dPed.FiringPattern = FiringPattern.FullAuto;
            dPed.CanSwitchWeapons = true;

            Function.Call(Hash.SET_PED_COMBAT_ABILITY, dPed, 100);
            if (dPed.IsInVehicle()) { Vehicle = dPed.CurrentVehicle; }
            DoState();
            CheckState();
            if (DEBUG) { dPed.AddBlip(); }
            dPed.Money = rnd.Next(200, 2000);
            Area = World.GetZoneNameLabel(dPed.Position);
        }

        public void CheckState()
        {
            servePos = World.GetNextPositionOnSidewalk(dPed.Position + dPed.ForwardVector);
            switch (CurrentState)
            {
                case DStates.WalkToPath:
                    if (dPed.Position.DistanceTo(servePos) < 1)
                    {
                        NewState = DStates.Normal;
                    }
                    if (playerNear()) NewState = DStates.StopForPlayer;
                    break;

                case DStates.Normal:
                    if (dPed.Position.DistanceTo(servePos) < 1)
                    {
                        NewState = DStates.Dealing;
                    }
                    else NewState = DStates.WalkToPath;
                    if (playerNear()) NewState = DStates.StopForPlayer;
                    if (ShouldAttackPlayer()) NewState = DStates.AttackPlayer;
                    break;

                case DStates.Dealing:
                    if (cPed == null && dPed.Position.DistanceTo(servePos) > 2.5f)
                    {
                        NewState = DStates.WalkToPath;
                    }
                    if (playerNear()) NewState = DStates.StopForPlayer;
                    if (ShouldAttackPlayer()) NewState = DStates.AttackPlayer;
                    break;

                case DStates.NoState:
                    break;

                case DStates.StopForPlayer:
                    if (dPed.IsStopped) NewState = DStates.LookAtPlayer;
                    if (!playerNear()) NewState = DStates.Normal;
                    if (ShouldAttackPlayer()) NewState = DStates.AttackPlayer;
                    if (cPed != null)
                    {
                        RemovePedFromWorld(cPed, true);
                        cPed.CurrentBlip?.Remove();
                        cPed = null;
                    }
                    break;

                case DStates.LookAtPlayer:
                    if (!dPed.IsHeadtracking(Game.Player.Character)) dPed.Task.LookAt(Game.Player.Character);
                    else NewState = DStates.TalkToPlayer;
                    if (!playerNear()) NewState = DStates.Normal;
                    if (ShouldAttackPlayer()) NewState = DStates.AttackPlayer;
                    break;
                case DStates.TalkToPlayer:
                    if (!playerNear())
                    {
                        NewState = DStates.Normal;
                        Function.Call(Hash._PLAY_AMBIENT_SPEECH2, dPed, "GENERIC_BYE", "SPEECH_PARAMS_FORCE");
                    }
                    if (ShouldAttackPlayer()) NewState = DStates.AttackPlayer;
                    break;

                case DStates.AttackPlayer:
                    if (driveByHandler.GetDriveBies.Count < 1 && rnd.Next(0, areas[areaIndex].GangPresance) + 1 != 0)
                    {
                        SpawnDriveBy();
                    }
                    if (!ShouldAttackPlayer())
                    {
                        NewState = DStates.Normal;
                        if (!Detected && dPed.CurrentBlip != null || dPed.CurrentBlip.Exists())
                        {
                            dPed.CurrentBlip.Remove();
                        }
                    }

                    break;
            }
        }
        public void DoState()
        {
            if (dPed != null)
            {
                if (dPed.RelationshipGroup != rival_nutral && !dPed.IsInCombat)
                {
                    if (dPed.RelationshipGroup != rival_enemy)
                    {
                        DbugString("~g~ Rival dealer relasion ship changed to rival_enemy");
                    }
                    else
                    {
                        DbugString("~r~ Rival dealer relasion ship changed to something. ~n~Changing it to rival_nutral");
                        dPed.RelationshipGroup = rival_nutral;
                    }
                }
                if (NewState != CurrentState)
                {

                    if (DEBUG) UI.Notify("adealer state = " + NewState);
                    switch (NewState)
                    {
                        case DStates.WalkToPath:
                            dPed.Task.ClearAll();
                            dPed.Task.GoTo(servePos);
                            CurrentState = DStates.WalkToPath;
                            break;

                        case DStates.Normal:
                            if (Vehicle != null)
                            {
                                if (dPed.CurrentVehicle.Driver.Handle != dPed.Handle)
                                {
                                    dPed.CurrentVehicle.Driver.Task.DriveTo(dPed.CurrentVehicle, World.GetNextPositionOnSidewalk(dPed.CurrentVehicle.Position), 3, 5);
                                }
                                else
                                {
                                    dPed.Task.DriveTo(dPed.CurrentVehicle, World.GetNextPositionOnSidewalk(dPed.CurrentVehicle.Position), 3, 5);
                                }
                            }
                            else
                            {
                                dPed.Task.WanderAround();
                            }
                            CurrentState = DStates.Normal;
                            break;

                        case DStates.Dealing:
                            cTimer.Reset();
                            cTimer.Start();
                            dPed.Task.GuardCurrentPosition();
                            CurrentState = DStates.Dealing;
                            break;

                        case DStates.StopForPlayer:
                            if (dPed.IsWalking || dPed.IsRunning) dPed.Task.Wait(20000);
                            CurrentState = DStates.StopForPlayer;
                            break;

                        case DStates.LookAtPlayer:
                            dPed.Task.TurnTo(Game.Player.Character);
                            CurrentState = DStates.LookAtPlayer;
                            break;

                        case DStates.TalkToPlayer:
                            Function.Call(Hash._PLAY_AMBIENT_SPEECH1, dPed, "GENERIC_HOWS_IT_GOING", "SPEECH_PARAMS_FORCE");
                            CurrentState = DStates.TalkToPlayer;
                            break;

                        case DStates.AttackPlayer:
                            dPed.RelationshipGroup = rival_enemy;
                            dPed.Task.ClearAll();
                            AddBlip(dPed, BlipColor.Red, "Rival Dealer", false, false);
                            dPed.CanSwitchWeapons = true;
                            dPed.Weapons.Select(dPed.Weapons.BestWeapon);
                            dPed.Task.FightAgainst(Game.Player.Character);
                            CurrentState = DStates.AttackPlayer;
                            break;

                        case DStates.NoState:
                            CurrentState = DStates.NoState;
                            break;
                    }
                }
                else
                {
                    switch (CurrentState)
                    {
                        case DStates.Dealing:
                            if (!cTimer.Finished()) { cTimer.Tick(); }
                            else
                            {
                                Deal();
                            }
                            break;
                    }
                }
            }
            else UI.Notify("~r~Area Dealer Ped NULL something wrong");
        }

        private bool ShouldAttackPlayer()
        {
            if (CanSee(dPed, Game.Player.Character))
            {
                if (dPed.Position.DistanceTo(playerPos) < 200)
                {
                    if (CurrentState == DStates.AttackPlayer)
                        return true;
                    //if(areas.)   // 
                    if (Game.Player.Character.GetMeleeTarget() == dPed || Game.Player.IsTargetting(dPed))
                        return true;
                    if (dPed.Position.DistanceTo(playerPos) < 50)
                    {
                        if (dPed.IsInCombatAgainst(Game.Player.Character) || Game.Player.IsAiming)
                            return true;
                    }
                }
                return false;
            }
            else
            {
                if (CurrentState == DStates.AttackPlayer)
                {
                    return true;
                }
                return false;
                //if (playerPos.DistanceTo(dPed.Position) > 100)
                //    return false;
                ///if (driveByHandler.GetDriveBies.Count > 0 && driveByHandler.GetDriveBies.First().DefendPosition.DistanceTo(playerPos) > 300)
                //    return false;

            }
        }

        public void PromtPlayerToTalk()
        {
            if (!LsMenuPool.IsAnyMenuOpen())
            {
                bool canHire = true;
                string reason = "";

                if (canHire)
                {
                    if (DealerHandler.dealers.Count >= (int)CurrentRepLvl(dealer1Rep))
                    {
                        canHire = false;
                        reason = "You need more rep before hiring more dealers.";
                    }
                }
                if (canHire)
                {
                    if (Game.GameTime > displayHelpTimer + 4000)
                    {
                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                        displayHelpTimer = Game.GameTime;
                        DisplayHelpText("Press ~INPUT_SPRINT~ to Talk to this person.");
                    }
                    if (Game.IsControlJustPressed(2, GTA.Control.Sprint))
                    {
                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                        hireDealerMainMenu.Subtitle.Caption = "Test";
                        hireDealerMainMenu.Visible = !hireDealerMainMenu.Visible;
                        hireDealerMainMenu.CurrentSelection = 0;
                        hireDealerOfferAmount.Maximum = Game.Player.Money;
                    }
                }
                else
                {
                    if (Game.GameTime > displayHelpTimer + 4000)
                    {
                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                        displayHelpTimer = Game.GameTime;
                        DisplayHelpText(reason);
                    }
                }
            }
        }

        bool playerNear()
        {
            if (dPed.Position.DistanceTo(playerPos) < 4 && CanSee(dPed, Game.Player.Character) || CanHearPlayer(dPed))
            {
                return true;
            }
            else return false;

        }

        public bool DecidePlayerOffer(int amount)
        {
            int max = 100 - (amount / 1000);
            if (max < 1) max = 1;
            int chance = rnd.Next(0, max + areas[areaIndex].GangPresance);
            if (chance < 20)
            {
                return true;
            }
            else return false;
        }

        public void SetLastOfferAmount(int _amount)
        {
            LastOffer = _amount;
        }

        void Deal()
        {
            if (Game.Player.Character.Position.DistanceTo(dPed.Position) < 300)
            {
                if (cPed != null && !cPed.Exists())
                {
                    cPed = null;
                    if (DEBUG) UI.Notify("aDealer Customer doesnt exist");
                }
                if (cPed == null)
                {
                    if (DEBUG) UI.Notify("aDealer Getting Customer");
                    var peds = lPeds.GetPeds;
                    if (peds.ToList().Count > 0)
                    {
                        foreach (Ped ped in peds)
                        {
                            if (!CheckIfPedUsed(ped, false, false))
                            {
                                cPed = ped;
                                cPed.FreezePosition = false;
                                cPed.IsPersistent = true;
                                LastCustomer = cPed.Handle;
                                if (DEBUG) { cPed.AddBlip(); }
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (DEBUG) { UI.Notify("aDealer found no peds"); }
                        cTimer.Reset();
                    }
                }
                else
                {
                    switch (dealState)
                    {

                        case DealState.GetNear:
                            servePos = dPed.Position;
                            float dist = cPed.Position.DistanceTo(servePos);
                            if (dist > 100)
                            {
                                RemovePedFromWorld(cPed, false);
                                cPed = null;
                                cTimer.Reset();
                            }
                            else if (cPed.Position.DistanceTo(servePos) > 10)
                            {
                                MovePed(cPed, servePos);
                            }
                            else
                            {
                                dealState = DealState.Approach;
                                cPed.Task.LookAt(dPed);
                                dPed.Task.TurnTo(cPed);
                            }
                            break;
                        case DealState.Approach:
                            servePos = dPed.Position;
                            if (cPed.Position.DistanceTo(servePos) > 2) { MovePed(cPed, servePos); }
                            else
                            {
                                cPed.Task.TurnTo(dPed);
                                dPed.Task.TurnTo(cPed);
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
                                dPed.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_shopkeeper", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                                time = Game.GameTime;
                            }
                            break;
                        case DealState.DoingDeal:
                            if (Game.GameTime > time + 2000)
                            {
                                dealState = DealState.WalkingAway;
                                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, cPed, "GENERIC_THANKS", "SPEECH_PARAMS_STANDARD");
                                cPed.Task.WanderAround();
                                dPed.Task.GuardCurrentPosition();
                            }
                            break;
                        case DealState.WalkingAway:
                            servePos = dPed.Position;
                            if (cPed.Position.DistanceTo(servePos) > 4)
                            {
                                dealState = DealState.GetNear;
                                if (DEBUG) { UI.Notify("Deal Finished"); }
                                RemovePedFromWorld(cPed, false);
                                cPed.CurrentBlip.Remove();
                                cPed = null;
                                cTimer.Reset();
                                cTimer.Start();
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
    }
}
