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
    public class Drug
    {
        public string Name;
        public int PricePerG;
        public AreaType AreaType;
        public float Demand = 0;
        public float Supply = 0;
        public float Delay = 15000;
        public int TimeOfLastCustomer = Game.GameTime;
        public bool SelectCustomer = false;
        public Ped Customer = null;
        public Drug(string name, int price, AreaType areaType)
        {
            Name = name;
            PricePerG = price;
            AreaType = areaType;
        }
    }
}
