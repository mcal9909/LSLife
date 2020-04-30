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
using static LSlife.LsFunctions;

namespace LSlife
{
    public class AreaDealer
    {
        private bool DEBUG = false;
        public Ped dPed;
        public Vehicle Vehicle = null;
        public enum dStates { NoState, Normal, Dealing };
        public dStates CurrentState = dStates.NoState;
        public dStates NewState = dStates.Normal;
        public enum DealState { GetNear, Approach, InPosition, DoingDeal, WalkingAway}
        public DealState dealState = DealState.GetNear;
        public bool Detected = false;
        Vector3 servePos = Vector3.Zero;
        public LsTimer cTimer = new LsTimer(15, true, false);
        public Ped cPed = null;
        int time;
        public bool dropDrugs = false;
        public Pickup dPickup = null;
        int LastCustomer = 0;

        public AreaDealer(Ped p)
        {
            DEBUG = LSL.DEBUG;
            dPed = p;
            dPed.IsPersistent = true;
            dPed.Armor = 100;
            dPed.Weapons.Give(WeaponHash.Pistol, 100, false, true);
            Function.Call(Hash.SET_PED_COMBAT_ABILITY, dPed, 100);
            if (dPed.IsInVehicle()) { Vehicle = dPed.CurrentVehicle; }
            DoState();
            CheckState();
            if (DEBUG) { dPed.AddBlip(); }
            Random rnd = new Random();
            dPed.Money = rnd.Next(200, 2000);
        }

        public void CheckState()
        {
            switch (CurrentState)
            {
                case dStates.Normal:
                    if (dPed.Position.DistanceTo(Game.Player.Character.Position) > 50 && !Function.Call<bool>(Hash.IS_POINT_ON_ROAD, dPed.Position.X, dPed.Position.Y, dPed.Position.Z))
                    {
                        NewState = dStates.Dealing;
                    }
                    break;
                case dStates.Dealing:
                    if (dPed.Position.DistanceTo(Game.Player.Character.Position) < 20 || Function.Call<bool>(Hash.IS_POINT_ON_ROAD, dPed.Position.X, dPed.Position.Y, dPed.Position.Z))
                    {
                        NewState = dStates.Normal;
                        if(cPed != null)
                        {
                            RemovePedFromWorld(cPed, false);
                            if (dealState != DealState.GetNear) dealState = DealState.GetNear;
                            cPed = null;
                        }
                    }
                    break;
                case dStates.NoState:
                    break;
            }
        }

        public void DoState()
        {
            if (NewState != CurrentState)
            {                
                switch (NewState)
                {
                    case dStates.Normal:
                        if (DEBUG) { UI.Notify("Change state Normal"); }
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
                        CurrentState = dStates.Normal;
                        if (DEBUG) { UI.Notify("Im moving"); }
                        break;

                    case dStates.Dealing:                        
                        cTimer.Reset();
                        cTimer.Start();                        
                        dPed.Task.Wait(60000 * 10);
                        CurrentState = dStates.Dealing;
                        if (DEBUG) { UI.Notify("Im Dealing"); }
                        break;

                    case dStates.NoState:
                        CurrentState = dStates.NoState;
                        break;
                }
            }
            else
            {
                switch(CurrentState)
                {
                    case dStates.Dealing:
                        if (!cTimer.Finished()) { cTimer.Tick();  }
                        else
                        {
                            Deal();
                        }                        
                        break;
                    case dStates.Normal:
                        break;
                    case dStates.NoState:
                        break;
                }
            }
        }

        void Deal()
        {
            if(Game.Player.Character.Position.DistanceTo(dPed.Position) < 300)
            {
                if(cPed != null && !cPed.Exists()) { cPed = null; }
                if (cPed == null)
                {
                    var peds = World.GetNearbyPeds(dPed, 100);
                    if (peds.ToList().Count > 0)
                    {
                        foreach (Ped ped in peds)
                        {
                            if (ped != null)
                            {
                                if (ped.Handle != LastCustomer && !ped.IsPlayer && ped.Handle != dPed.Handle && ped.IsHuman && ped.IsAlive && !ped.IsInVehicle() && !ped.IsInTrain)
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
                    }
                    else { if(DEBUG) UI.Notify("no peds found"); cTimer.Reset(); }

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
                if(cPed != null)
                {
                    RemovePedFromWorld(cPed, false);
                    cPed = null;
                    if (dealState != DealState.GetNear) dealState = DealState.GetNear;
                }
            }
        }        
    }
}
