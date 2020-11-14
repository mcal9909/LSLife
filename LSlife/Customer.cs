using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using static LSlife.LsFunctions;
using static LSlife.LSL;

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
            Ped.RelationshipGroup = CustomerRel;
            AddBlip(ped, BlipColor.Blue, "Customer", true, true);
            ped.CurrentBlip.Scale = 0.5f;
            ped.IsPersistent = true;
            ped.Task.ClearAll();
            ped.BlockPermanentEvents = true;
            if (FarAway) { amount = rnd.Next(15 + (playerLevel * 2), 50 + (playerLevel * 2)); }
            else { amount = rnd.Next(1, 15 + playerLevel); }
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
