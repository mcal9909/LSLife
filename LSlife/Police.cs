using GTA;
using GTA.Math;
using GTA.Native;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class Pig
    {
        private readonly bool DEBUG = false;
        public Ped Ped;
        public Vehicle Vehicle = null;
        public enum States { Destroy, NoState, driveToPlayer, Normal, ApproachPlayer, WatchPlayer, SearchPlayer };
        public States CurrentState = States.NoState;
        public States NewState = States.NoState;
        public Vector3 destination = Vector3.Zero;
        bool leader = false;
        int time;
        int delay = 20000;
        int timeSeen;
        public bool AskPlayerToComply { get; private set; } = false;
        public bool SirenActive { get; private set; } = false;
        public bool SirenSoundActive { get; private set; } = true;
        bool sayWait = false;
        public bool detected = false;
        public bool seenPlayer = false;

        Vector3 pPos = Vector3.Zero;


        public Pig(Ped _ped, bool _leader)
        {
            Ped = _ped;
            leader = _leader;
            if (Ped.CurrentVehicle != null) Vehicle = Ped.CurrentVehicle;
            destination = StopPosition();
            Ped.IsPersistent = true;
            Ped.AlwaysKeepTask = true;
            Ped.BlockPermanentEvents = true;
            Function.Call(Hash.SET_DRIVER_ABILITY, Ped, 100);
        }

        private Vector3 StopPosition()
        {
            Vector3 front = World.GetNextPositionOnStreet(Game.Player.Character.Position + Game.Player.Character.ForwardVector * 3);
            Vector3 back = World.GetNextPositionOnStreet(Game.Player.Character.Position + (Game.Player.Character.ForwardVector * -3) * 5);
            Vector3 final = Vector3.Zero;
            if (front.DistanceTo(Ped.Position) < back.DistanceTo(Ped.Position))
            {
                final = front;
            }
            else final = back;
            return final;
        }

        public void OnTick()
        {

            DoState();
            CheckState();
            if (Game.Player.WantedLevel > 0 || !isDealing) DeletePig();
        }

        public void CheckState()
        {
            switch (CurrentState)
            {
                case States.NoState:
                    NewState = States.driveToPlayer;
                    break;

                case States.driveToPlayer:
                    var pos = StopPosition();
                    if (destination != pos)
                    {
                        destination = pos;
                        NewState = States.NoState;
                        break;
                    }
                    if (Vehicle.Position.DistanceTo(destination) < 15 && Vehicle.IsStopped)
                    {
                        if (Vehicle.Position.DistanceTo(playerPos) > 50)
                        {
                            if (ratChance > 25)
                            {
                                destination = StopPosition();
                                NewState = States.NoState;
                            }
                            else
                            {
                                NewState = States.Destroy;
                            }
                        }
                        else
                        {
                            if (Game.Player.Character.CurrentVehicle != null)
                            {
                                if (Game.Player.Character.CurrentVehicle.Speed < 1)
                                {
                                    NewState = States.ApproachPlayer;
                                }
                                else
                                {
                                    if (Game.GameTime > time + 3000)
                                    {
                                        time = Game.GameTime;
                                        Function.Call(Hash.BLIP_SIREN, Vehicle.Handle);
                                    }
                                    if (!seenPlayer && CanSee(Ped, Game.Player.Character))
                                    {
                                        seenPlayer = true;
                                        timeSeen = Game.GameTime;
                                    }
                                    else
                                    {
                                        if (!CanSee(Ped, Game.Player.Character))
                                        {
                                            seenPlayer = false;
                                            if (Game.Player.WantedLevel < 2) Game.Player.WantedLevel = 2;
                                            areas[areaIndex].Heat += 1000;
                                        }
                                        else if (Game.GameTime > timeSeen + delay + 10000)
                                        {
                                            if (Game.Player.WantedLevel < 2) Game.Player.WantedLevel = 2;
                                            areas[areaIndex].Heat += 1000;
                                        }

                                    }
                                }
                            }
                            else NewState = States.ApproachPlayer;
                        }
                    }
                    if (leader)
                    {
                        if (Vehicle.Position.DistanceTo(destination) < 50)
                        {
                            if (!SirenActive)
                            {
                                Function.Call(Hash.SET_VEHICLE_SIREN, Vehicle.Handle, true);
                                SirenActive = true;
                            }
                            if (SirenSoundActive)
                            {
                                SirenSoundActive = false;
                                Function.Call(Hash.DISABLE_VEHICLE_IMPACT_EXPLOSION_ACTIVATION, Vehicle.Handle, true);
                            }
                        }
                    }
                    break;

                case States.Normal:
                    if (Ped.Position.DistanceTo(Game.Player.Character.Position) > 80)
                    {
                        NewState = States.ApproachPlayer;
                    }
                    break;

                case States.WatchPlayer:

                    if (Game.Player.WantedLevel > 0)
                    {
                        NewState = States.Destroy;
                        break;
                    }
                    if (Game.Player.WantedLevel < 3)
                    {
                        if (Game.Player.Character.CurrentVehicle != null && !Game.Player.Character.CurrentVehicle.IsStopped)
                        {
                            Game.Player.WantedLevel = 3;
                            areas[areaIndex].Heat += 2000;
                            break;
                        }
                        if (Game.GameTime > time + delay || playerPos.DistanceTo(pPos) > 2)
                        {
                            Game.Player.WantedLevel = 3;
                            areas[areaIndex].Heat += 1000;
                            break;
                        }
                    }
                    if (PlayerComplied && !Game.Player.Character.IsInVehicle())
                    {
                        AskPlayerToComply = false;
                        NewState = States.SearchPlayer;
                    }

                    break;

                case States.SearchPlayer:
                    if (Game.GameTime > time + 3000)
                    {
                        if (TotalDrugsCarried > 0)
                        {
                            if (leader)
                            {
                                UI.ShowSubtitle("Your busted.", 3000);
                                Game.Player.WantedLevel = 3;
                            }
                            NewState = States.Destroy;
                        }
                        else
                        {
                            if (leader) UI.ShowSubtitle("Ok we got our eye on you, get outa here.", 4000);
                            Game.Player.Character.Task.ClearAll();
                            NewState = States.Destroy;
                        }
                    }

                    break;


                case States.ApproachPlayer:
                    if (Game.Player.WantedLevel > 0)
                    {
                        NewState = States.Destroy;
                        break;
                    }
                    if (playerPos.DistanceTo(pPos) > 4 && Game.Player.WantedLevel < 3)
                    {
                        Game.Player.WantedLevel = 3;
                        areas[areaIndex].Heat += 2000;
                        break;
                    }
                    if (Ped.Position.DistanceTo(destination) < 1)
                    {
                        pPos = playerPos;
                        NewState = States.WatchPlayer;
                    }
                    if (!sayWait && !Ped.IsInVehicle() && Ped.IsWalking && playerPos.DistanceTo(Ped.Position) < 10)
                    {
                        sayWait = true;
                        Function.Call(Hash._PLAY_AMBIENT_SPEECH1, Ped, "WAIT", "SPEECH_PARAMS_STANDARD");
                    }
                    break;

                default:
                    break;
            }
        }
        public void DoState()
        {
            if (NewState != CurrentState)
            {
                if (DEBUG) { UI.Notify("Police new State " + NewState); }
                switch (NewState)
                {
                    case States.SearchPlayer:
                        if (leader)
                        {
                            TaskSequence searchPlayer = new TaskSequence();
                            searchPlayer.AddTask.GoTo(Ped.Position + (Ped.ForwardVector * 0.5f), false, 3000);
                            searchPlayer.AddTask.TurnTo(Game.Player.Character, 1000);
                            searchPlayer.AddTask.PlayAnimation("missexile3", "ex03_dingy_search_case_base_michael");
                            Ped.Task.PerformSequence(searchPlayer);
                        }
                        time = Game.GameTime;
                        CurrentState = States.SearchPlayer;
                        break;

                    case States.Destroy:
                        RemovePedFromWorld(Ped, true);
                        Ped.Task.Arrest(Game.Player.Character);
                        Ped = null;
                        if (Vehicle != null) Vehicle.IsPersistent = false; entitiesCleanUp.Add(Vehicle); Vehicle = null;
                        CurrentState = States.Destroy;
                        break;

                    case States.NoState:
                        CurrentState = States.NoState;
                        break;


                    case States.driveToPlayer:
                        destination = StopPosition();
                        if (Ped == Vehicle.Driver) Ped.Task.DriveTo(Vehicle, destination, 10, 12, (int)DrivingStyle.AvoidTraffic);
                        CurrentState = States.driveToPlayer;
                        break;

                    case States.Normal:
                        if (Vehicle != null)
                        {
                            Ped.Task.CruiseWithVehicle(Vehicle, 9f, (int)DrivingStyle.Normal);
                        }
                        else
                        {
                            Ped.Task.WanderAround();
                        }
                        CurrentState = States.Normal;
                        if (DEBUG) { UI.Notify("Changing to normal"); }
                        break;

                    case States.WatchPlayer:
                        Ped.Task.ClearAll();
                        Ped.Weapons.Give(WeaponHash.CombatPistol, 100, true, true);
                        Ped.Task.AimAt(Game.Player.Character, 20000);
                        time = Game.GameTime;
                        CurrentState = States.WatchPlayer;
                        if (Game.Player.Character.CurrentVehicle != null)
                        {
                            if (leader) UI.ShowSubtitle("Sir, please step out of the vehicle.", 4000);
                            AskPlayerToComply = true;
                        }
                        else
                        {
                            if (leader) UI.ShowSubtitle("Get down on the ground", 4000);
                            AskPlayerToComply = true;
                        }
                        if (DEBUG) { UI.Notify("Changing to watch"); }
                        break;

                    case States.ApproachPlayer:
                        if (Game.Player.Character.CurrentVehicle != null)
                        {
                            if (leader) destination = playerPos + (Game.Player.Character.RightVector * -1) * 2;
                            else destination = playerPos + (Game.Player.Character.RightVector * -1) * 2 + ((Game.Player.Character.ForwardVector * -1) * 2);
                        }
                        else
                        {
                            if (leader) destination = playerPos + (Game.Player.Character.ForwardVector) * 2;
                            else destination = playerPos + (Game.Player.Character.RightVector * -1) * 2 + ((Game.Player.Character.ForwardVector) * 2);
                        }
                        Ped.Task.GoTo(destination);
                        pPos = playerPos;
                        Ped.Task.LookAt(Game.Player.Character);
                        CurrentState = States.ApproachPlayer;
                        break;

                    default:
                        break;
                }
            }
        }

        public void DeletePig()
        {
            NewState = States.Destroy;
        }
    }
}
