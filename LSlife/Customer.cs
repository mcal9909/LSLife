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

namespace LSlife
{    
    public class Customer
    {
        private Ped ped;
        private Vector3 target;
        private Vehicle vehicle;
        public List<string> drugs = new List<string>();
        public Drug Drug;
        public int amount;
        private bool farAway;
        Random rnd = new Random();
        public Customer(Ped ped, Vector3 target, bool farAway, Vehicle vehicle, Drug drug, int playerLevel)
        {
            //init
            FarAway = farAway;
            Ped = ped;
            Target = target;
            Vehicle = vehicle;
            Drug = drug;
            ped.AddBlip();
            ped.CurrentBlip.IsFriendly = true;
            ped.CurrentBlip.Scale = 0.5f;
            ped.CurrentBlip.IsShortRange = true;
            ped.IsPersistent = true;
            ped.Task.ClearAll();
            ped.BlockPermanentEvents = true;
            if (FarAway) { amount = rnd.Next(10 + playerLevel, 50 + playerLevel); }
            else { amount = rnd.Next(1, 10); }
            int extraMoney = rnd.Next(0, 500);
            Ped.Money = (amount * Drug.PricePerG) + extraMoney;
            if (!FarAway && vehicle == null) { Ped.Task.GoTo(target); }
            //if (!FarAway && vehicle != null) { PerformTaskSeq(2, vehicle); }
        }
        public Vector3 Target { get => target; set => target = value; }
        public Ped Ped { get => ped; set => ped = value; }
        public Vehicle Vehicle { get => vehicle; set => vehicle = value; }
        public bool FarAway { get => farAway; set => farAway = value; }
    }
}
