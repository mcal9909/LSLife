using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;
using iFruitAddon2;
using System.IO;
using Control = GTA.Control;
using static LSlife.LSL;

namespace LSlife
{
    public class Pig
    {
        private readonly bool DEBUG = false;
        public Ped Ped;
        public Vehicle Vehicle = null;
        public enum States { NoState, Normal, ApproachPlayer, WatchPlayer };
        public States CurrentState = States.NoState;
        public States NewState = States.Normal;
        public Vector3 destination = Vector3.Zero;
        public Pig(Ped p)
        {
            Ped = p;
            destination = Game.Player.Character.Position;
            if (Ped.IsInVehicle()) { Vehicle = Ped.CurrentVehicle; }
            DoState();
            CheckState();
        }
        public void CheckState()
        {
            switch (CurrentState)
            {
                case States.Normal:
                    if (Ped.Position.DistanceTo(Game.Player.Character.Position) > 80)
                    {
                        NewState = States.ApproachPlayer;
                    }
                    break;

                case States.WatchPlayer:
                    if (Ped.Position.DistanceTo(Game.Player.Character.Position) > 80)
                    {
                        NewState = States.ApproachPlayer;
                    }
                    break;

                case States.ApproachPlayer:
                    if (Ped.Position.DistanceTo(Game.Player.Character.Position) <= 15)
                    {
                        NewState = States.Normal;
                    }
                    if (destination.DistanceTo(Game.Player.Character.Position) > 5)
                    {
                        destination = Game.Player.Character.Position;
                        if (Vehicle != null)
                        {
                            Ped.Task.DriveTo(Vehicle, World.GetNextPositionOnStreet(destination, true), 15, 10, (int)DrivingStyle.Normal);
                        }
                        else
                        {
                            Ped.Task.WanderAround(destination, 15f);
                        }
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
                if (DEBUG) { UI.Notify("Change state detected"); }
                switch (NewState)
                {
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
                        Ped.Task.LookAt(Game.Player.Character);
                        CurrentState = States.WatchPlayer;
                        if (DEBUG) { UI.Notify("Changing to watch"); }
                        break;

                    case States.ApproachPlayer:
                        destination = Game.Player.Character.Position;
                        if (Vehicle != null)
                        {
                            Ped.Task.DriveTo(Vehicle, World.GetNextPositionOnStreet(destination, true), 15, 10, (int)DrivingStyle.Normal);
                        }
                        else
                        {
                            Ped.Task.WanderAround(destination, 15f);
                        }
                        CurrentState = States.ApproachPlayer;
                        if (DEBUG) { UI.Notify("Changing to approach"); }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
