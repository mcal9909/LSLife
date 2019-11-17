using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;
using BetterChasesPlus;
using iFruitAddon2;



public enum Nodetype { AnyRoad, Road, Offroad, Water }
public static Vector3 GenerateSpawnPos(Vector3 desiredPos, Nodetype roadtype, bool sidewalk)
{

        Vector3 finalpos = Vector3.Zero;
        bool ForceOffroad = false;


        OutputArgument outArgA = new OutputArgument();
        int NodeNumber = 1;
        int type = 0;

        if (roadtype == Nodetype.AnyRoad) type = 1;
        if (roadtype == Nodetype.Road) type = 0;
        if (roadtype == Nodetype.Offroad) { type = 1; ForceOffroad = true; }
        if (roadtype == Nodetype.Water) type = 3;


        int NodeID = Function.Call<int>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID, desiredPos.X, desiredPos.Y, desiredPos.Z, NodeNumber, type, 300f, 300f);

        if (ForceOffroad)
        {
            for (int i = 0; i < 100; i++)
            {
                if (!Function.Call<bool>(Hash._GET_IS_SLOW_ROAD_FLAG, NodeID))
                {
                    NodeID = Function.Call<int>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID, desiredPos.X, desiredPos.Y, desiredPos.Z, NodeNumber, type, 300f, 300f);
                    NodeNumber++;
                }
            }
        }

        Function.Call(Hash.GET_VEHICLE_NODE_POSITION, NodeID, outArgA);
        finalpos = outArgA.GetResult<Vector3>();

        if (sidewalk) finalpos = World.GetNextPositionOnSidewalk(finalpos);
        return finalpos;

    }


    public enum PathnodeFlags
    {
        Slow = 1,
        Two = 2,
        Intersection = 4,
        Eight = 8, SlowTraffic = 12, ThirtyTwo = 32, Freeway = 64, FourWayIntersection = 128, BigIntersectionLeft = 512
    }
    public static string GetRoadFlags(Vector3 pos)
    {
        OutputArgument outArgA = new OutputArgument();
        OutputArgument outArgB = new OutputArgument();
        if (Function.Call<bool>(Hash.GET_VEHICLE_NODE_PROPERTIES, pos.X, pos.Y, pos.Z, outArgA, outArgB))
        {
            int busy = outArgA.GetResult<int>();
            int flags = outArgB.GetResult<int>();

            string d = "";
            foreach (int flag in Enum.GetValues(typeof(PathnodeFlags)).Cast<PathnodeFlags>())
            {

                if ((flag & flags) != 0) d += " " + (PathnodeFlags)flag;
            }
            return d;  // DisplayHelpTextThisFrame("Flags: " + d);
        }
        return "";
    }
    public static bool RoadHasFlag(Vector3 pos, PathnodeFlags flag)
    {
        OutputArgument outArgA = new OutputArgument();
        OutputArgument outArgB = new OutputArgument();
        if (Function.Call<bool>(Hash.GET_VEHICLE_NODE_PROPERTIES, pos.X, pos.Y, pos.Z, outArgA, outArgB))
        {
            int busy = outArgA.GetResult<int>();
            int flags = outArgB.GetResult<int>();
            if ((flags & (int)flag) != 0) return true;
        }
        return false;
    }

    public static bool IntHasFlag(int number, int flag)
    {

        if ((number & (int)flag) != 0) return true;
        return false;
    }

