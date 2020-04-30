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

namespace LSFunctions
{
    
    public class LsFunc
    {
        public static void SendTextMsg(string str)
        {
            UI.Notify(str);
        }

        public static void PedBag(Ped ped, bool give)
        {
            if (give)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 9, 1, 0, 0);
            }
            else
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 9, 0, 0, 0);
            }
        }
        public static void DisplayHelpText(string text)
        {
            InputArgument[] arguments = new InputArgument[] { "STRING" };
            Function.Call(Hash._0x8509B634FBE7DA11, arguments);
            InputArgument[] argumentArray2 = new InputArgument[] { text };
            Function.Call(Hash._0x6C188BE134E074AA, argumentArray2);
            InputArgument[] argumentArray3 = new InputArgument[] { 0, 0, 1, -1 };
            Function.Call(Hash._0x238FFE5C7B0498A6, argumentArray3);
        }
    }
}
