using GTA;
using System.Xml.Linq;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class DrugOrder
    {
        public int weed { get; private set; } = 0;
        public int crack { get; private set; } = 0;
        public int cocaine { get; private set; } = 0;
        public int Bill { get; private set; } = 0;

        public bool Placed = false;

        bool payed = false;
        public bool pickedUp = false;

        public DrugOrder()
        {
        }

        public void OrderWeed(int amount, int price)
        {
            weed += amount;
            Bill += price;
        }

        public void OrderCrack(int amount, int price)
        {
            crack += amount;
            Bill += price;
        }
        public void OrderCocaine(int amount, int price)
        {
            cocaine += amount;
            Bill += price;
        }

        public bool PayBill()
        {
            if (CanPayBill())
            {
                payed = true;
                int cash = player.Money;

                int toPay = 0;
                if (cash < Bill)
                {

                    toPay = cash;
                }
                else
                {
                    toPay = Bill;

                }
                player.Money = player.Money - toPay;
                dealer1Money += toPay;
                if (toPay > 0)
                {
                    Dealer1AddRep(toPay / 100);
                }
                if (Bill - toPay > 0)
                {
                    dealer1Debt += (Bill - toPay);
                }
                SaveDealer();
                PlayTheSound();
                payed = true;
                return true;
            }
            else return false;
        }

        public bool CanPayBill()
        {
            int _bill = Bill;
            _bill -= player.Money;
            if (dealer1Debt + _bill <= zeeDebtCap())
            {
                return true;
            }
            else return false;
        }

        public void PickupDrugs()
        {
            if (PlayerCanPickup())
            {
                if (weed > 0) pWeedOunce += weed;
                if (crack > 0) pCrackOunce += crack;
                if (cocaine > 0) pCocaineOunce += cocaine;
                PlayTheSound();
                pickedUp = true;
            }
            else
            {
                StashVehicle stash;
                if (LSL.pStashVehicle?.drugCar != null)
                {
                    stash = LSL.pStashVehicle;
                    if (stash.drugCar.Position.DistanceTo(ZeesDealers.dealerCar.Position) < 20)
                    {
                        Game.FadeScreenOut(1000);
                        Script.Wait(2000);
                        player.Character.SetIntoVehicle(stash.drugCar, VehicleSeat.Driver);
                        if (weed > 0) pWeedOunce += weed;
                        if (crack > 0) pCrackOunce += crack;
                        if (cocaine > 0) pCocaineOunce += cocaine;
                        PlayTheSound();
                        pickedUp = true;
                        Game.FadeScreenIn(500);
                    }
                }
                else UI.ShowSubtitle("You need a stash vehicle to pickup an order this big.");
            }
            if (pickedUp) SaveOrder();
        }

        void SaveOrder()
        {            
            LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
            foreach (XElement element in LSLifeSave.Descendants())
            {
                if (element.Name == "playerInventory")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weedOz")
                        {
                            e.Value = pWeedOunce.ToString();
                        }
                        if (e.Name == "crackOz")
                        {
                            e.Value = pCrackOunce.ToString();
                        }
                        if (e.Name == "cocaineOz")
                        {
                            e.Value = pCocaineOunce.ToString();
                        }
                    }
                }
                if (element.Name == "drugCarStash")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weedOz")
                        {
                            e.Value = LSL.pStashVehicle.drugCarWeedOz.ToString();
                        }
                        if (e.Name == "crackOz")
                        {
                            e.Value = LSL.pStashVehicle.drugCarCrackOz.ToString();
                        }
                        if (e.Name == "cocainOz")
                        {
                            e.Value = LSL.pStashVehicle.drugCarCocainOz.ToString();
                        }
                    }
                }
                if (element.Name == "dealer1")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weed")
                        {
                            e.Value = LSL.dealer1Weed.ToString();
                        }
                        if (e.Name == "crack")
                        {
                            e.Value = LSL.dealer1Crack.ToString();
                        }
                        if (e.Name == "cocain")
                        {
                            e.Value = LSL.dealer1Cocain.ToString();
                        }
                        if (e.Name == "debt")
                        {
                            e.Value = LSL.dealer1Debt.ToString();
                        }
                        if (e.Name == "money")
                        {
                            e.Value = LSL.dealer1Money.ToString();
                        }
                    }
                }
            }
            LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
        }




        public bool PlayerCanPickup()
        {
            if (TotalDrugs(PlayerInventory) + OrderSizeInGrams() < pMaxDrugs) return true;
            else return false;
        }
        int OrderSizeInGrams()
        {
            return (weed * 28) + (crack * 28) + (cocaine * 28);
        }

        internal bool OrderValid()
        {
            if (weed == 0 && crack == 0 && cocaine == 0)
            {
                SendTextMsg("Your order is empty");
                return false;
            }
            else return true;
        }

        internal void PutBackDrugs()
        {
            dealer1Weed += weed;
            dealer1Crack += crack;
            dealer1Cocain += cocaine;
            SaveDealer();
        }

        internal void ResetOrder()
        {
            weed = 0;
            crack = 0;
            cocaine = 0;
            Bill = 0;
            payed = false;
            pickedUp = false;
            Placed = false;            
        }
    }
}
