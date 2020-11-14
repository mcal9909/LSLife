using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static LSlife.Enums;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class PedDebugger
    {
        List<Ped> debugPeds = new List<Ped>();
        int dTime;
        int dInterval = 2000;

        public void OnTick()
        {
            if (Game.GameTime > dTime + dInterval)
            {
                foreach (Ped p in World.GetAllPeds().ToList())
                {
                    if (!debugPeds.Contains(p))
                    {
                        debugPeds.Add(p);
                    }
                }
            }
            foreach (Ped p in debugPeds.ToList())
            {

                if (p != null && p.Exists())
                {
                    if (p.IsOnScreen && p.IsVisible)
                    {

                        bool allowed = false;
                        if (!CheckIfPedUsed(p, true, true)) allowed = true;
                        string pString = SetPedDebugtext(p, allowed);
                        Vector2 tPos = World3DToScreen2d(new Vector3(p.GetBoneCoord(Bone.FB_Brow_Centre_000).X, p.GetBoneCoord(Bone.FB_Brow_Centre_000).Y, p.GetBoneCoord(Bone.FB_Brow_Centre_000).Z + 0.5f));

                        //Size size = Game.ScreenResolution;
                        tPos.X = 1280 * tPos.X;
                        tPos.Y = 720 * tPos.Y;

                        UIText pedInfo = new UIText(pString, new Point((int)tPos.X, (int)tPos.Y), 0.2f, Color.WhiteSmoke, 0, true, true, true);
                        pedInfo.Draw();
                    }
                }
                else
                {
                    debugPeds.Remove(p);
                }
            }
            if(pStashVehicle != null && pStashVehicle.drugCar != null && pStashVehicle.drugCar.Exists())
            {
                Vector2 tPos = World3DToScreen2d(pStashVehicle.drugCar.Position);
                tPos.X = 1280 * tPos.X;
                tPos.Y = 720 * tPos.Y;
                UIText pedInfo = new UIText(SetPedDebugtext(pStashVehicle.drugCar, true), new Point((int)tPos.X, (int)tPos.Y), 0.2f, Color.WhiteSmoke, 0, true, true, true);
                pedInfo.Draw();
            }
        }

        private static string SetPedDebugtext(Entity p, bool allowed)
        {
            if(pStashVehicle != null && p == pStashVehicle.drugCar)
            {
                Vehicle v = p as Vehicle;
                string str = p.Health.ToString()+" | "+pStashVehicle.drugCarDestroyed+" | "+pStashVehicle.drugCar.IsDead+" | "+v.BodyHealth+" | "+v.EngineHealth+" | "+v.IsDriveable;                
                return str;
            }
            if (p is Ped && p as Ped == player.Character)
            {
                string str = "";
                foreach (PathnodeFlags flag in GetRoadFlags(playerPos))
                {
                    str += " " + flag;
                }
                return str;
            }
            if (DealerHandler.dealers.Count > 0 && DealerHandler.dealers.Find(d => d.Ped == p) != null)
            {
                if(p.Alpha != 50) p.Alpha = 50;
                World.DrawMarker(MarkerType.DebugSphere, p.Position, Vector3.Zero, Vector3.Zero, new Vector3(0.5f, 0.5f, 0.5f), Color.Red);
                return DealerHandler.dealers.Find(d => d.Ped == p).NewState.ToString() + " > " + DealerHandler.dealers.Find(d => d.Ped == p).CurrentState.ToString();
                //foreach (eTaskTypeIndex task in Enum.GetValues(typeof(eTaskTypeIndex)))
                //{
                //    if (Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, p, (int)task))
                //    {
                //        return task.ToString();
                //    }
                //}
            }
            if (aDealer != null)
            {
                if (aDealer.dPed != null && p == aDealer.dPed)
                {
                    foreach (eTaskTypeIndex task in Enum.GetValues(typeof(eTaskTypeIndex)))
                    {
                        if (Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, p, (int)task))
                        {
                            return task.ToString();
                        }
                    }
                }
                else
                if (aDealer.cPed != null && p == aDealer.cPed) return aDealer.dealState.ToString();
            }
            if (driveByHandler != null && driveByHandler.GetDriveBies.Count > 0)
            {
                foreach (DriveBy d in driveByHandler.GetDriveBies)
                {
                    if (d.Peds.Count > 0 && d.Peds.Find(r => r == p) != null)
                    {
                        return d.DriveBySate.ToString();
                    }
                }

            }
            if (p == drugDealer1)
            {
                return dealer1Rep.ToString();
            }

            return allowed.ToString();
        }
    }
}
