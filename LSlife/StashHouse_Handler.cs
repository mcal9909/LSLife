using GTA;
using GTA.Math;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class StashHouseHandler
    {

        public List<StashHouse> Houses { get; private set; } = new List<StashHouse>();
        public StashHouse CurrentHouse = null;
        //public bool Inside = false;


        public StashHouseHandler()
        {
            LoadHouses();
            if (DEBUG) UI.Notify("Loaded " + Houses.Count + " houses");
        }

        public void Ontick()
        {

            foreach (StashHouse house in Houses.Where(r => r.entrancePos.DistanceTo2D(playerPos) < 30 || r.exitPos.DistanceTo(playerPos) < 40))
            {
                house.OnTick();
            }
        }



        private void LoadHouses()
        {

            if (!File.Exists("scripts\\LSLife\\LSLife_StashHouses.xml"))
            {
                UI.Notify("~r~Missing StashHouses.xml");
            }
            else
            {
                XDocument StashDoc;
                StashDoc = XDocument.Load("scripts\\LSLife\\LSLife_StashHouses.xml");
                foreach (XElement e in StashDoc.Descendants())
                {
                    if (e.Name == "house")
                    {
                        int id = 0;
                        int.TryParse(e.FirstAttribute.Value, out id);
                        //UI.Notify("house ID = " + id);
                        Vector3 entrance = Vector3.Zero;
                        Vector3 exit = Vector3.Zero;
                        Vector3 stash = Vector3.Zero;
                        Vector3 health = Vector3.Zero;
                        int price = 0;
                        bool purchased = false;
                        int weed = 0;
                        int crack = 0;
                        int cocain = 0;
                        int armour = 0;
                        int money = 0;
                        bool inside = false;
                        bool found = false;
                        foreach (XElement lEle in LSLifeSave.Descendants())
                        {
                            if (lEle.Name == "playerStash")
                            {
                                found = true;
                                lEle.Remove();
                                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                                break;
                            }
                        }
                        List<XElement> sWeapons = new List<XElement>();
                        List<dynamic> sWeps = new List<dynamic>();
                        foreach (XElement el in e.Elements())
                        {
                            if (el.Name == "entrancePos")
                            {
                                foreach (XElement ele in el.Elements())
                                {
                                    if (ele.Name == "X") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out entrance.X);
                                    if (ele.Name == "Y") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out entrance.Y);
                                    if (ele.Name == "Z") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out entrance.Z);
                                }
                            }
                            if (el.Name == "exitPos")
                            {
                                foreach (XElement ele in el.Elements())
                                {
                                    if (ele.Name == "X") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out exit.X);
                                    if (ele.Name == "Y") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out exit.Y);
                                    if (ele.Name == "Z") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out exit.Z);

                                }
                            }
                            if (el.Name == "stashPos")
                            {
                                foreach (XElement ele in el.Elements())
                                {
                                    if (ele.Name == "X") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out stash.X);
                                    if (ele.Name == "Y") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out stash.Y);
                                    if (ele.Name == "Z") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out stash.Z);
                                }
                            }
                            if (el.Name == "healthPos")
                            {
                                foreach (XElement ele in el.Elements())
                                {
                                    if (ele.Name == "X") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out health.X);
                                    if (ele.Name == "Y") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out health.Y);
                                    if (ele.Name == "Z") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out health.Z);
                                }
                            }

                            if (el.Name == "price") int.TryParse(el.Value, out price);
                            if (el.Name == "purchased") bool.TryParse(el.Value, out purchased);
                            if (el.Name == "inside") bool.TryParse(el.Value, out inside);
                            if (el.Name == "stash")
                            {
                                foreach (XElement ele in el.Elements())
                                {
                                    if (ele.Name == "weed")
                                    {
                                        if (found)
                                        {
                                            ele.Value = weed.ToString();
                                        }
                                        else
                                        {
                                            int.TryParse(ele.Value, out weed);
                                            if (DEBUG) UI.Notify("~y~Weed" + weed);
                                        }
                                    }
                                    if (ele.Name == "crack")
                                    {
                                        if (found)
                                        {
                                            ele.Value = crack.ToString();
                                        }
                                        else
                                        {
                                            int.TryParse(ele.Value, out crack);
                                        }
                                    }
                                    if (ele.Name == "cocain")
                                    {
                                        if (found)
                                        {
                                            ele.Value = cocain.ToString();
                                        }
                                        else
                                        {
                                            int.TryParse(ele.Value, out cocain);
                                        }

                                    }
                                    if (ele.Name == "money")
                                    {
                                        if (found) ele.Value = money.ToString();
                                        else int.TryParse(ele.Value, out money);

                                    }
                                    if (ele.Name == "armour")
                                    {
                                        if (found) ele.Value = armour.ToString();
                                        else int.TryParse(ele.Value, out armour);
                                    }
                                    if (ele.Name == "weapons")
                                    {
                                        //UI.Notify("found weapons");
                                        foreach (XElement wepEl in ele.Elements())
                                        {
                                            if (wepEl.Name == "weapon")
                                            {
                                                //UI.Notify("found weapon");

                                                foreach (XElement element in wepEl.Elements())
                                                {
                                                    if (element.Name == "HASH")
                                                    {
                                                        sWeapons.Add(wepEl);
                                                        sWeps.Add(element.Value);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (found)
                        {
                            StashDoc.Save("scripts\\LSLife\\LSLife_StashHouses.xml");
                        }

                        Houses.Add(new StashHouse(id, entrance, exit, stash, health, purchased, price, inside, weed, crack, cocain, money, armour, sWeapons, sWeps));
                    }
                }
            }
        }

        private void CreateStashHouseXML()
        {

        }

        public void SaveHouse(StashHouse _house)
        {
            XDocument StashDoc = XDocument.Load("scripts\\LSLife\\LSLife_StashHouses.xml");
            foreach (XElement e in StashDoc.Descendants())
            {
                if (e.Name == "house" && e.FirstAttribute.Value == _house.id.ToString())
                {
                    //UI.Notify("house found");
                    foreach (XElement el in e.Elements())
                    {
                        if (el.Name == "stash")
                        {
                            //UI.Notify("stash Found");
                            foreach (XElement ele in el.Elements())
                            {
                                if (ele.Name == "weed") ele.Value = _house.Weed.ToString();
                                if (ele.Name == "crack") ele.Value = _house.Crack.ToString();
                                if (ele.Name == "cocain") ele.Value = _house.Cocain.ToString();
                                if (ele.Name == "money") ele.Value = _house.Money.ToString();
                                if (ele.Name == "armour") ele.Value = _house.Armor.ToString();
                                XElement wEle = new XElement("weapons");
                                foreach (XElement wep in _house.Weapons)
                                {
                                    wEle.Add(wep);
                                    //UI.Notify(wep.Name.ToString());
                                }
                                //UI.Notify(wEle.Elements().Count().ToString());                                
                                if (ele.Name == "weapons")
                                {
                                    ele.Remove();
                                    el.Add(wEle);
                                    break;

                                }



                            }

                        }
                    }
                    StashDoc.Save("scripts\\LSLife\\LSLife_StashHouses.xml");


                }
            }
        }

        public int TotalStashDrugs()
        {
            int drugs = 0;
            foreach (StashHouse house in Houses)
            {
                drugs += (house.Weed + house.Crack + house.Cocain);
            }
            return drugs;
        }

        public void UpdateMenu(StashHouse _house)
        {



            takeWeedStash.Maximum = _house.Weed;
            takeCrackStash.Maximum = _house.Crack;
            takeCocainStash.Maximum = _house.Cocain;
            takeMoneyStash.Maximum = _house.Money;
            takeWeedStash.Text = "Weed " + GramsToOz(_house.Weed);
            takeCrackStash.Text = "Crack " + GramsToOz(_house.Crack);
            takeCocainStash.Text = "Cocaine " + GramsToOz(_house.Cocain);
            takeMoneyStash.Text = "Money $" + IntToMoney(_house.Money);
            takeWeaponStash.Items = _house.Weps;
            takeWeaponStash.Index = 0;
            takeArmourStash.Items = new List<dynamic>();
            for (int t = 0; t < _house.Armor + 1; t++)
            {
                takeArmourStash.Items.Add(t);
            }
            putWeedStash.Maximum = PlayerInventory["Weed"];
            putCrackStash.Maximum = PlayerInventory["Crack"];
            putCocainStash.Maximum = PlayerInventory["Cocaine"];

            putWeedOzStash.Maximum = pWeedOunce;
            putCrackOzStash.Maximum = pCrackOunce;
            putCocainOzStash.Maximum = pCocaineOunce;

            putMoneyStash.Maximum = player.Money;

            putWeedStash.Text = "Weed " + GramsToOz(PlayerInventory["Weed"]);
            putCrackStash.Text = "Crack " + GramsToOz(PlayerInventory["Crack"]);
            putCocainStash.Text = "Cocaine " + GramsToOz(PlayerInventory["Cocaine"]);

            putWeedOzStash.Text = "Weed ~r~" + pWeedOunce + "~s~z";
            putCrackOzStash.Text = "Crack ~r~" + pCrackOunce + "~s~z";
            putCocainOzStash.Text = "Cocaine ~r~" + pCocaineOunce + "~s~z";

            putMoneyStash.Text = "Money $" + IntToMoney(player.Money);
            putWeaponStash.Index = 0;
            putWeaponStash.Items = playerWeps;
            putArmourStash.Items = new List<dynamic>();
            for (int t = 0; t < playerArmours + 1; t++)
            {
                putArmourStash.Items.Add(t);
            }
        }
    }
}
