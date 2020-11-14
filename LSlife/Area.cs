using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using static LSlife.LSL;

namespace LSlife
{
    public class Area
    {
        public string Name;
        public List<Drug> Drugs = new List<Drug>();
        public AreaType AreaType = AreaType.Normal;
        public jurisdictionType PoliceType = jurisdictionType.LSPD;
        public int CopPresance = 0;
        public int GangPresance = 0;
        public float Reputation = 0;
        public int Heat = 0;                    
        public int Dealers = 0;
        public bool DealerKilled = false;
        public int DemandBuffTime = 0;
        public int DemandBuffLength = 0;


        public Area(string name, List<Drug> _drugs, AreaType areaType)
        {
            Name = name;
            Drugs = _drugs;
            AreaType = areaType;
            PoliceType = jurisdiction[Name];
            switch (AreaType)
            {
                case AreaType.Poor:
                    CopPresance = 1;
                    GangPresance = 6;
                    break;
                case AreaType.Normal:
                    CopPresance = 1;
                    GangPresance = 5;
                    break;
                case AreaType.Rich:
                    CopPresance = 1;
                    GangPresance = 5;
                    break;
            }
            foreach (Drug drug in Drugs)
            {
                switch (AreaType)
                {
                    case AreaType.Poor:
                        if (drug.AreaType == AreaType.Poor)
                        {
                            drug.Demand = 1.0f + AddRandomAmount();
                        }
                        if (drug.AreaType == AreaType.Normal)
                        {
                            drug.Demand = 0.8f + AddRandomAmount();
                        }
                        if (drug.AreaType == AreaType.Rich)
                        {
                            drug.Demand = 0.5f + AddRandomAmount();
                        }
                        drug.Delay = 10000;
                        break;
                    case AreaType.Normal:
                        if (drug.AreaType == AreaType.Poor)
                        {
                            drug.Demand = 0.7f + AddRandomAmount();
                        }
                        if (drug.AreaType == AreaType.Normal)
                        {
                            drug.Demand = 1f + AddRandomAmount();
                        }
                        if (drug.AreaType == AreaType.Rich)
                        {
                            drug.Demand = 0.7f + AddRandomAmount();
                        }
                        drug.Delay = 10000;
                        break;
                    case AreaType.Rich:
                        if (drug.AreaType == AreaType.Poor)
                        {
                            drug.Demand = 0.5f + AddRandomAmount();
                        }
                        if (drug.AreaType == AreaType.Normal)
                        {
                            drug.Demand = 0.8f + AddRandomAmount();
                        }
                        if (drug.AreaType == AreaType.Rich)
                        {
                            drug.Demand = 1f + AddRandomAmount();
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

        float AddRandomAmount()
        {
            return Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 0.25f);
        }

        public void GetCustomer(List<Ped> peds)
        {
            if (Drugs.Where(d => d.SelectCustomer).Count() > 0 && peds.Count > 0)
            {

                foreach (Drug drug in Drugs.Where(d => d.SelectCustomer))
                {
                    Random rInt = new Random();
                    int rIntMax = (int)(10f * drug.Demand);
                    if (drug.Demand > 1) { rIntMax = 10; }
                    if (rInt.Next(0, 11 - rIntMax) == 0)
                    {
                        int pCount = peds.Count;
                        int pIndex = 0;
                        pIndex = rInt.Next(0, pCount);
                        if (pCount == 1) pIndex = 0;
                        //UI.Notify("pindex" + pCount);
                        drug.Customer = peds[pIndex];
                        peds.Remove(drug.Customer);
                        drug.TimeOfLastCustomer = Game.GameTime;
                        if (peds.Count == 0) break;
                    }
                }
            }
        }

        public void KillDealer()
        {
            DealerKilled = true;
            GangPresance -= 1;
            DemandBuffTime = Game.GameTime;
            DemandBuffLength = 60000 * (10 - GangPresance);
        }

        public bool CanSpawnDealer()
        {
            if (DealerKilled)
            {
                if (Game.GameTime > DemandBuff())
                {
                    DealerKilled = false;
                    return true;
                }
                else return false;
            }
            else return true;
        }

        public int DemandBuff()
        {
            return DemandBuffTime + DemandBuffLength;
        }

        internal bool DemandMet()
        {
            foreach(Drug d in Drugs)
            {
                if(d.Supplied == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
