using GTA;
using GTA.Math;
using GTA.Native;
using System.Linq;
using static GTA.Script;
using static LSlife.LsFunctions;
using static LSlife.LSL;


namespace LSlife
{
    static class Prison
    {
        public static LsTimer SentanceTime = null;
        static int BailAmount = -1;
        public enum PrisonStates { None, PlayerEnterCar, CopEnterCar, ApproachBarrier, ApproachGate1, ApproachGate2, FinalApproach, FinalPos }
        public static PrisonStates CurrentState { get; set; } = PrisonStates.ApproachGate2;
        static PrisonStates NewState = PrisonStates.None;
        public static bool Started = false;
        public static bool Ready = false;
        static Ped cop = null;
        static Vehicle copCar = null;
        static Vector3 PrisonPos = new Vector3(1946.057f, 2617.254f, 45.63307f);
        static Vector3 BarrierWaitPos = new Vector3(1899.201f, 2609.867f, 45.34259f);
        static Vector3 Gate1WaitPos = new Vector3(1851.723f, 2608.119f, 45.23964f);
        static Vector3 Gate2WaitPos = new Vector3(1824.335f, 2607.923f, 45.18952f);
        static Vector3 Gate2ExitPos = new Vector3(1798.774f, 2606.744f, 45.16935f);
        static Vector3 FinalPos = new Vector3(1690.612f, 2604.468f, 45.16944f);
        static Gate PriGate1 = null;
        static Gate PriGate2 = null;
        static bool DEBUG = LSL.DEBUG;
        static float Speed = 0;



        public static void PrisonTick()
        {
            DriverSpeed(cop, copCar);
            CancelWantedLevels();
            // If we have a new state do this stuff
            if (NewState != CurrentState)
            {
                bool stateChanged = false;
                switch (NewState)
                {
                    case PrisonStates.None:
                        if (Ready)
                        {
                            Game.Player.IgnoredByPolice = true;
                            Game.Player.IgnoredByEveryone = true;
                            float heading;
                            Vector3 pos;
                            Vector3 playerPos = Game.Player.Character.Position;
                            OutputArgument outArgA = new OutputArgument();
                            OutputArgument outArgB = new OutputArgument();

                            if (Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING, playerPos.X, playerPos.Y, playerPos.Z, outArgA, outArgB, 0, 1077936128, 0))
                            {
                                heading = outArgB.GetResult<float>(); //getting heading
                                pos = outArgA.GetResult<Vector3>();
                                if (copCar == null)
                                {
                                    copCar = World.CreateVehicle(VehicleHash.Police, pos, heading);
                                    copCar.AddBlip();
                                    copCar.CurrentBlip.Sprite = BlipSprite.CopCar;
                                    if (DEBUG) { UI.Notify("transport Spawned" + copCar); }
                                }
                            }
                            if (cop == null) cop = World.CreatePed(PedHash.Cop01SMY, Game.Player.Character.Position + ((Game.Player.Character.ForwardVector * -1) * 1.5f));

                            if (copCar != null && cop != null)
                            {

                                Function.Call(Hash.REQUEST_ANIM_DICT, "mp_arresting");
                                while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, "mp_arresting"))
                                {
                                    Wait(100);
                                }
                                Function.Call(Hash.TASK_PLAY_ANIM, Game.Player.Character, "mp_arresting", "idle", 8.0f, 1.0f, -1, 49, .1f, 0, 0, 0);
                                Game.Player.Character.Task.EnterVehicle(copCar, VehicleSeat.RightRear);
                                if (Function.Call<bool>(Hash.IS_PLAYER_CONTROL_ON))
                                {
                                    Function.Call(Hash.SET_PLAYER_CONTROL, Game.Player.Handle, false, 256);
                                }
                                Function.Call(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY, cop.Handle, Game.Player.Character.Handle, 1, 1, 0, 1, -1, 10, true);
                                CurrentState = PrisonStates.None;
                                stateChanged = true;
                            }
                        }
                        break;

                    case PrisonStates.PlayerEnterCar:
                        cop.Task.ClearAll();
                        cop.AlwaysKeepTask = true;
                        cop.BlockPermanentEvents = true;
                        cop.Task.EnterVehicle(copCar, VehicleSeat.Driver);
                        CurrentState = PrisonStates.PlayerEnterCar;
                        stateChanged = true;
                        break;

                    case PrisonStates.CopEnterCar:
                        Function.Call(Hash.SET_DRIVER_ABILITY, cop.Handle, 100f);
                        cop.Task.DriveTo(copCar, PrisonPos, 5, Speed, (int)DrivingStyle.SometimesOvertakeTraffic);
                        CurrentState = PrisonStates.CopEnterCar;
                        stateChanged = true;
                        break;

                    case PrisonStates.ApproachBarrier:
                        cop.Task.DriveTo(copCar, BarrierWaitPos, 1, Speed, (int)DrivingStyle.IgnoreLights);
                        CurrentState = PrisonStates.ApproachBarrier;
                        stateChanged = true;
                        break;

                    case PrisonStates.ApproachGate1:
                        cop.Task.DriveTo(copCar, Gate1WaitPos, 1, Speed, 262144);
                        CurrentState = PrisonStates.ApproachGate1;
                        stateChanged = true;
                        break;

                    case PrisonStates.ApproachGate2:
                        cop.Task.DriveTo(copCar, Gate2WaitPos, 1, Speed, 262144);
                        CurrentState = PrisonStates.ApproachGate2;
                        stateChanged = true;
                        break;

                    case PrisonStates.FinalApproach:
                        cop.Task.DriveTo(copCar, Gate2ExitPos, 1, Speed, 262144);
                        CurrentState = PrisonStates.FinalApproach;
                        stateChanged = true;
                        break;

                    case PrisonStates.FinalPos:
                        cop.Task.DriveTo(copCar, FinalPos, 1, Speed, 262144);

                        CurrentState = PrisonStates.FinalPos;
                        stateChanged = true;
                        break;
                }
                if (stateChanged) UI.Notify("Switching to state " + NewState);
            }
            //if not do this stuff
            else
            {
                switch (CurrentState)
                {
                    case PrisonStates.None:
                        if (Game.Player.Character.IsInVehicle(copCar) && !cop.IsInVehicle(copCar))
                        {
                            NewState = PrisonStates.PlayerEnterCar;
                        }
                        break;

                    case PrisonStates.PlayerEnterCar:
                        if (cop.IsInVehicle(copCar))
                        {
                            Wait(1000);
                            NewState = PrisonStates.CopEnterCar;
                        }
                        break;

                    case PrisonStates.CopEnterCar:

                        if (copCar.Position.DistanceTo(PrisonPos) < 50)
                        {
                            NewState = PrisonStates.ApproachBarrier;
                        }
                        break;

                    case PrisonStates.ApproachBarrier:

                        if (copCar.Position.DistanceTo(BarrierWaitPos) <= 2)
                        {
                            NewState = PrisonStates.ApproachGate1;
                        }
                        break;

                    case PrisonStates.ApproachGate1:
                        //Function.Call(Hash._DOOR_CONTROL, "prop_sec_barrier_ld_01a", 1896.7380f, 2606.1790, 45.7093, false, 0f, 50f, 0f);
                        if (copCar.Position.DistanceTo(Gate1WaitPos) <= 5)
                        {
                            if (PriGate1 == null)
                            {
                                if (pGate1() != null)
                                {
                                    PriGate1 = new Gate(pGate1());
                                    PriGate1.Prop.FreezePosition = false;
                                    NewState = PrisonStates.ApproachGate2;
                                    UI.Notify("Gate Found " + pGate1().Model.Hash);
                                }
                            }
                        }
                        break;

                    case PrisonStates.ApproachGate2:
                        PriGate1.Unlock();
                        if (copCar.Position.DistanceTo(Gate2WaitPos) <= 3)
                        {
                            if (PriGate2 == null)
                            {
                                if (pGate2() != null)
                                {
                                    PriGate2 = new Gate(pGate2());
                                    PriGate2.Prop.FreezePosition = false;
                                    NewState = PrisonStates.FinalApproach;
                                }
                            }
                        }
                        break;

                    case PrisonStates.FinalApproach:
                        PriGate2.Unlock();
                        if (copCar.Position.DistanceTo(Gate2ExitPos) <= 3)
                        {
                            NewState = PrisonStates.FinalPos;
                        }
                        break;

                    case PrisonStates.FinalPos:
                        if (copCar.Position.DistanceTo2D(FinalPos) < 10)
                        {
                            PlayerExitPrison();
                        }
                        break;
                }
            }
        }

        static void DriverSpeed(Ped p, Vehicle v)
        {

            if (v != null && p != null && v.Driver.Handle == p.Handle)
            {
                float roadSpeed = GetRoadSpeed(v.Position + v.Velocity);
                if (roadSpeed != Speed)
                {
                    float increment = 0.01f;
                    Speed = roadSpeed;

                    if (v.Speed > Speed)
                    {
                        p.MaxDrivingSpeed = v.Speed - (increment * Game.LastFrameTime);
                        p.DrivingSpeed = v.Speed - (increment * Game.LastFrameTime);
                    }
                    else
                    {
                        p.MaxDrivingSpeed = Speed;
                        p.DrivingSpeed = Speed;
                    }

                }
            }
        }

        static void CancelWantedLevels()
        {
            if (Game.MaxWantedLevel != 0)
            {
                Game.MaxWantedLevel = 0;
            }
        }

        static void PlayerExitPrison()
        {
            Function.Call(Hash.SET_PLAYER_CONTROL, Game.Player.Handle, true, 256);
            Game.Player.CanControlCharacter = true;
            entitiesCleanUp.Add(copCar);
            entitiesCleanUp.Add(cop);
            cop = null;
            copCar = null;
            Game.MaxWantedLevel = 5;
            Game.Player.Character.Position = Gate1WaitPos;
            Started = false;
            NewState = PrisonStates.None;
        }

        public static Prop pGate1()
        {
            int doorHash = 741314661;
            Prop prop = null;
            var props = World.GetNearbyProps(new Vector3(1844.9984f, 2604.8125f, 44.6398f), 10f);
            if (props.Count() > 0)
            {
                foreach (Prop p in props)
                {
                    if (p.Model.Hash == doorHash)
                    {
                        prop = p;
                        UI.Notify("Gate1 found");
                        break;
                    }
                }
            }
            return prop;
        }

        public static Prop pGate2()
        {
            int doorHash = 741314661;
            Prop prop = null;
            var props = World.GetNearbyProps(new Vector3(1818.5428f, 2604.8125f, 44.6110f), 10f);
            if (props.Count() > 0)
            {
                foreach (Prop p in props)
                {
                    if (p.Model.Hash == doorHash)
                    {
                        prop = p;
                        UI.Notify("Gate2 found");
                        break;
                    }
                }
            }
            return prop;
        }

        public class Gate
        {
            private readonly bool DEBUG = true;
            public Prop Prop;
            private bool Locked = true;
            public Gate(Prop prop)
            {
                this.Prop = prop;
            }
            public void Unlock()
            {
                Wait(10);
                Function.Call(Hash._DOOR_CONTROL, this.Prop.Model.Hash, this.Prop.Position.X, this.Prop.Position.Y, this.Prop.Position.Z, false, 0f, 50f, 0f);
                //if (this.DEBUG) { UI.Notify(this.Prop.Model.Hash + " Door unlocked " + this.Prop.Position); }
            }
            public void Lock()
            {
                Wait(10);
                Function.Call(Hash._DOOR_CONTROL, this.Prop.Model.Hash, this.Prop.Position.X, this.Prop.Position.Y, this.Prop.Position.Z, true, 0f, 50f, 0f);
                //if (this.DEBUG) { UI.Notify(this.Prop.Model.Hash + " Door locked " + this.Prop.Position); }
            }
        }

        static float changeSpeed(float fromVal, float toVal, float duration)
        {
            return 0;
        }

        static float MaxSpeed()
        {
            LSL.GetRoadFlags(copCar.Position);
            return 0f;
        }
    }
}
