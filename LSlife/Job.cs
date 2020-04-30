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
    public class Job
    {
        private Ped ped;
        private Vector3 target;
        public List<string> drugs = new List<string>();
        private int wealth;
        public string drug;
        public int amount;
        public Job(Ped ped, Vector3 target, int wealth)
        {
            //init
            Wealth = wealth;
            Ped = ped;
            Target = target;
            ped.AddBlip();
            ped.CurrentBlip.IsFriendly = true;
            ped.CurrentBlip.Scale = 0.5f;
            ped.CurrentBlip.IsShortRange = true;
            ped.IsPersistent = true;
            //ped.Task.ClearAll();
            //ped.Task.GoTo(target);
            if (Wealth > 0) drugs.Add("Weed");
            if (Wealth < 5) drugs.Add("Crack");
            if (Wealth > 4) drugs.Add("Cocaine");
            Random rnd = new Random();
            int max = drugs.Count;
            int i = rnd.Next(0, max);
            drug = drugs[i];
            amount = rnd.Next(1, Wealth + 2);
        }
        public Vector3 Target { get => target; set => target = value; }
        public Ped Ped { get => ped; set => ped = value; }
        public int Wealth { get => wealth; set => wealth = value; }
    }
}
