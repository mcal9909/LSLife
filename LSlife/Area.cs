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
    public class Area
    {
        public string Name;
        public List<Drug> Drugs = new List<Drug>();
        public AreaType AreaType = AreaType.Normal;
        public int CopPresance = 0;
        public int GangPresance = 0;
        public float Reputation = 0;
        public int Heat = 0;
        public bool DemandNotMet = false;
        public int HighDemandTimer = Game.GameTime;
        public int TimeLastActive = Game.GameTime;
        public int Dealers = 0;

        public Area(string name, List<Drug> drugs, AreaType areaType)
        {
            Name = name;
            Drugs = drugs;
            AreaType = areaType;
            switch (AreaType)
            {
                case AreaType.Poor:
                    CopPresance = 1;
                    GangPresance = 5;
                    break;
                case AreaType.Normal:
                    CopPresance = 2;
                    GangPresance = 2;
                    break;
                case AreaType.Rich:
                    CopPresance = 5;
                    GangPresance = 1;
                    break;
            }
            foreach (Drug drug in Drugs)
            {
                Random rng = new Random();
                double range = (double)0.25 - (double)0.0;
                double sample = rng.NextDouble();
                double scaled = (sample * range) + 0.0;
                float f = (float)scaled;
                switch (AreaType)
                {
                    case AreaType.Poor:
                        if (drug.AreaType == AreaType.Poor)
                        {
                            drug.Demand = 1.0f + f;
                        }
                        if (drug.AreaType == AreaType.Normal)
                        {
                            drug.Demand = 0.5f + f;
                        }
                        if (drug.AreaType == AreaType.Rich)
                        {
                            drug.Demand = 0.1f + f;
                        }
                        drug.Delay = 10000;
                        break;
                    case AreaType.Normal:
                        if (drug.AreaType == AreaType.Poor)
                        {
                            drug.Demand = 0.2f + f;
                        }
                        if (drug.AreaType == AreaType.Normal)
                        {
                            drug.Demand = 0.8f + f;
                        }
                        if (drug.AreaType == AreaType.Rich)
                        {
                            drug.Demand = 0.3f + f;
                        }
                        drug.Delay = 10000;
                        break;
                    case AreaType.Rich:
                        if (drug.AreaType == AreaType.Poor)
                        {
                            drug.Demand = 0.1f + f;
                        }
                        if (drug.AreaType == AreaType.Normal)
                        {
                            drug.Demand = 0.5f + f;
                        }
                        if (drug.AreaType == AreaType.Rich)
                        {
                            drug.Demand = 1f + f;
                        }
                        drug.Delay = 10000;
                        break;
                }
            }
            List<Drug> SortedList = Drugs.OrderByDescending(d => d.Demand).ToList();
            Drugs = SortedList;
        }
        public bool NeedCustomer()
        {
            bool getCustomer = false;
            foreach (Drug drug in Drugs)
            {
                if (Game.GameTime > drug.TimeOfLastCustomer + (drug.Delay / (drug.Demand / Dealers)))
                {
                    drug.SelectCustomer = true;
                    getCustomer = true;
                }
            }
            if (getCustomer) { return true; }
            else { return false; }
        }
        public void GetCustomer(List<Ped> peds)
        {
            {
                foreach (Drug drug in Drugs)
                {
                    Random rInt = new Random();
                    int rIntMax = (int)(10f * drug.Demand);
                    if (drug.Demand > 1) { rIntMax = 10; }
                    if (rInt.Next(0, 11 - rIntMax) == 0)
                    {
                        if (drug.SelectCustomer == true)
                        {
                            if (peds.Count == 0) { break; }
                            else
                            {

                                int pIndex = rInt.Next(0, peds.Count);
                                drug.Customer = peds[pIndex];
                                peds.Remove(drug.Customer);
                                drug.TimeOfLastCustomer = Game.GameTime;
                            }
                        }
                    }
                }
            }
        }
    }
}
