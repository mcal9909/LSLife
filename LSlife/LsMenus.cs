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
using static LSlife.LsFunctions;

namespace LSlife
{
    static class LsMenus
    {
        public static void OnstartDealSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == StartDeal)
            {

                sellingWeed = sellWeed.Checked;
                sellingCrack = sellCrack.Checked;
                sellingCocaine = sellCocaine.Checked;
                if (!sellingWeed && !sellingCrack && !sellingCocaine)
                {
                    LsFunctions.SendTextMsg("You need to select a drug to sell before you can start dealing.");                    
                }
                else
                if (!isDealing)
                {
                    sender.Visible = !sender.Visible;
                    if (ratChance > 0)
                    {
                        LsFunctions.SendTextMsg("Maybe you should lay low for a bit youre way too hot round there.");
                    }
                    else
                    if (TotalDrugsCarried + stashCocain + stashCrack + stashWeed + drugCarWeed + drugCarCrack + drugCarCocain > 0)
                    {
                        LsFunctions.SendTextMsg("Ok, ill put the word out.");
                        Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                        isDealing = true;
                        lastStreet = World.GetStreetName(new Vector2(Game.Player.Character.Position.X, Game.Player.Character.Position.Y));
                        thisStreet = lastStreet;
                        timeStartedDealing = Game.GameTime;
                        heatBar = new BarTimerBar("Heat");
                        heatBar.Percentage = 0f;
                        timerBarPool.Add(heatBar);
                        heatBar.BackgroundColor = Color.Black;
                        heatBar.ForegroundColor = Color.White;
                    }
                    else
                    {
                        LsFunctions.SendTextMsg("You got no drugs to sell.");
                    }
                }
            }
        }
        public static void OnZeeMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == buyDrugs)
            {
                sender.Visible = !sender.Visible;
                if (ratChance > 0)
                {
                    UI.Notify("Maybe you should lay low for a bit youre way too hot round there.");
                }
                else
                {
                    if (drugDealer1 == null)
                    {
                        callDealer(false);
                        UI.Notify("Yo man, sending someone to meet you.");
                    }
                    else
                    {
                        UI.Notify("Someones already on there way mate.");
                    }
                }
            }
            if (item == buyWeapons)
            {
                sender.Visible = !sender.Visible;
                if (ratChance > 0)
                {
                    UI.Notify("Maybe you should lay low for a bit youre way too hot round there.");
                }
                else
                {
                    if (weaponDealer1 == null)
                    {
                        callDealer(true);
                        UI.Notify("Yo man, sending someone to meet you.");
                    }
                    else
                    {
                        UI.Notify("Someones already on there way mate.");
                    }
                }
            }
            if (item == buyLift)
            {
                if (wepDealer == null)
                {
                    sender.Visible = !sender.Visible;
                    wepDealer = new Dealer(PedHash.ArmLieut01GMM, Game.Player.Character.Position, "INGOT", playerGroup);
                }
            }
        }
        public static void OnWepMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {

            if (item == melee)
            {
                int total = meleePrice[melee.Index] - LsFunctions.Discount(meleePrice[melee.Index]);
                if (Game.Player.Money >= total)
                {
                    Enum.TryParse<WeaponHash>(meleeWeps[melee.Index], out WeaponHash weaponHash);
                    if (!Game.Player.Character.Weapons.HasWeapon(weaponHash))
                    {
                        Game.Player.Money -= total;
                        dealer1Money += total;
                        dealer1Rep += total / 100;
                        Game.Player.Character.Weapons.Give(weaponHash, 0, true, true);
                        UpdateSave();

                    }
                }
                else
                if (dealer1Debt < 100000 && dealer1Debt + total < 100000)
                {
                    Enum.TryParse<WeaponHash>(meleeWeps[melee.Index], out WeaponHash weaponHash);
                    if (!Game.Player.Character.Weapons.HasWeapon(weaponHash))
                    {
                        dealer1Debt -= total;
                        dealer1Rep += total / 100;
                        Game.Player.Character.Weapons.Give(weaponHash, 0, true, true);
                        UpdateSave();
                    }
                }
            }
            if (item == pistol)
            {
                int total = pistolPrice[pistol.Index] - LsFunctions.Discount(pistolPrice[pistol.Index]);
                if (pistol.Index == 3)
                {
                    if (Game.Player.Character.Weapons.Current.Group == WeaponGroup.Pistol)
                    {
                        if (Game.Player.Money >= total)
                        {
                            Game.Player.Money -= total;
                            dealer1Money += total;
                            dealer1Rep += total / 100;
                            Game.Player.Character.Weapons.Current.Ammo += 50;
                            UpdateSave();
                        }
                        else
                        {
                            if (dealer1Debt < 100000 && dealer1Debt + total < 100000)
                            {
                                dealer1Debt += total;
                                Game.Player.Character.Weapons.Current.Ammo += 50;
                                UpdateSave();
                            }
                        }
                    }
                    else
                    {
                        if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.Pistol))
                        {
                            Game.Player.Character.Weapons.Select(WeaponHash.Pistol);
                            if (Game.Player.Money >= total)
                            {
                                Game.Player.Money -= total;
                                dealer1Money += total;
                                dealer1Rep += total / 100;
                                Game.Player.Character.Weapons.Current.Ammo += 50;
                                UpdateSave();
                            }
                            else
                            {
                                if (dealer1Debt < 100000 && dealer1Debt + total < 100000)
                                {
                                    dealer1Debt += total;
                                    Game.Player.Character.Weapons.Current.Ammo += 50;
                                    UpdateSave();
                                }
                            }
                        }
                        else
                        if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.Pistol50))
                        {
                            Game.Player.Character.Weapons.Select(WeaponHash.Pistol50);
                            if (Game.Player.Money >= total)
                            {
                                Game.Player.Money -= total;
                                dealer1Money += total;
                                dealer1Rep += total / 100;
                                Game.Player.Character.Weapons.Current.Ammo += 50;
                                UpdateSave();
                            }
                            else
                            {
                                if (dealer1Debt < 100000 && dealer1Debt + total < 100000)
                                {
                                    dealer1Debt += total;
                                    Game.Player.Character.Weapons.Current.Ammo += 50;
                                    UpdateSave();
                                }
                            }
                        }
                        else
                        if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.HeavyPistol))
                        {
                            Game.Player.Character.Weapons.Select(WeaponHash.HeavyPistol);
                            if (Game.Player.Money >= total)
                            {
                                Game.Player.Money -= total;
                                dealer1Money += total;
                                dealer1Rep += total / 100;
                                Game.Player.Character.Weapons.Current.Ammo += 50;
                                UpdateSave();
                            }
                            else
                            {
                                if (dealer1Debt < 100000 && dealer1Debt + total < 100000)
                                {
                                    dealer1Debt += total;
                                    Game.Player.Character.Weapons.Current.Ammo += 50;
                                    UpdateSave();
                                }
                            }
                        }
                    }
                }
                else
                if (Game.Player.Money >= total)
                {
                    Enum.TryParse<WeaponHash>(pistolWeps[pistol.Index], out WeaponHash weaponHash);
                    if (!Game.Player.Character.Weapons.HasWeapon(weaponHash))
                    {
                        Game.Player.Money -= total;
                        dealer1Money += total;
                        dealer1Rep += total / 100;
                        Game.Player.Character.Weapons.Give(weaponHash, 100, true, true);
                        UpdateSave();
                    }
                }
                else
                if (dealer1Debt < 100000 && dealer1Debt + total < 100000)
                {
                    Enum.TryParse<WeaponHash>(pistolWeps[pistol.Index], out WeaponHash weaponHash);
                    if (!Game.Player.Character.Weapons.HasWeapon(weaponHash))
                    {
                        dealer1Debt += total;
                        Game.Player.Character.Weapons.Give(weaponHash, 100, true, true);
                        UpdateSave();
                    }
                }
            }
            if (item == armour)
            {
                int total = 250 - LsFunctions.Discount(250);
                if (Game.Player.Money >= total)
                {
                    Game.Player.Money -= total;
                    dealer1Money += total;
                    dealer1Rep += total / 100;
                    Game.Player.Character.Armor = 200;
                    UpdateSave();
                }
                else
                if (dealer1Debt < 100000 && dealer1Debt + total < 100000)
                {
                    dealer1Debt -= total;
                    dealer1Rep += total / 100;
                    Game.Player.Character.Armor = 200;
                    UpdateSave();
                }
            }
            if (item == dismiss)
            {
                if (weaponDealer1 != null)
                {
                    weaponDealer1.CurrentBlip.Remove();
                    wepMenu.Visible = !wepMenu.Visible;
                    weaponDealer1.Task.WanderAround();
                    weaponDealer1.MarkAsNoLongerNeeded();
                    weaponDealer1.LeaveGroup();
                    weaponDealer1 = null;
                    readyToDeal = false;
                }
            }
        }
        #region Dealer Menus
        //weed menu
        public static void onWeedMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == WeedBuy && dealer1Weed > 0)
            {

                var inGrams = bushWeed.Value * 28;
                int total = bushWeed.Value * (weed1Price - Discount(weed1Price));
                PlayerInventory["Weed"] += inGrams;
                bool sale = false;
                if (Game.Player.Money >= total)
                {
                    dealer1Rep += total / 100;
                    Game.Player.Money -= total;
                    dealer1Money += total;
                    drugDealer1.Money = dealer1Money;
                    UI.ShowSubtitle("Nice doing business with you.", 4000);
                    sale = true;
                }
                else
                {
                    if (dealer1Debt + total > 100000)
                    {
                        UI.ShowSubtitle("You owe me to much already", 4000);
                        sale = false;
                    }
                    else
                    {

                        dealer1Debt += total;
                        payDebt.Text = ("Pay Debt $" + dealer1Debt);
                        UI.ShowSubtitle("Ill add this one to youre bill, you now owe me $" + dealer1Debt, 4000);
                        sale = true;
                    }

                }
                if (sale)
                {
                    bushWeed.Maximum -= bushWeed.Value;
                    dealer1Weed = bushWeed.Maximum;
                    total = bushWeed.Value * (weed1Price - Discount(weed1Price));
                    WeedBuy.Text = "Buy ~g~" + bushWeed.Value + " ~s~for $~g~" + total;
                    putWeedStash.Maximum = PlayerInventory["Weed"];
                    dealerMainMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    weedMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    crackMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    cocainMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    bushWeed.Text = "Weed $" + (weed1Price - Discount(weed1Price)) + "oz";
                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "playerInventory")
                        {
                            foreach (XElement e in element.Elements())
                            {
                                if (e.Name == "weed")
                                {
                                    e.Value = PlayerInventory["Weed"].ToString();
                                }
                            }
                        }
                        if (element.Name == "dealer1")
                        {
                            foreach (XElement e in element.Elements())
                            {
                                if (e.Name == "debt")
                                {
                                    e.Value = dealer1Debt.ToString();
                                }
                                if (e.Name == "weed")
                                {
                                    e.Value = dealer1Weed.ToString();
                                }
                                if (e.Name == "money")
                                {
                                    e.Value = dealer1Money.ToString();
                                }
                                if (e.Name == "rep")
                                {

                                    e.Value = dealer1Rep.ToString();
                                }
                            }
                        }
                    }
                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                    TotalDrugsCarried = 0;
                    foreach (KeyValuePair<string, int> entry in PlayerInventory)
                    {
                        TotalDrugsCarried += PlayerInventory[entry.Key];
                    }
                }
            }
        }
        //CRACK MENUS
        public static void onCrackMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == CrackBuy && dealer1Crack > 0)
            {
                var inGrams = crack.Value * 28;
                int total = crack.Value * (crack1Price - Discount(crack1Price));
                PlayerInventory["Crack"] += inGrams;
                bool sale = false;
                if (Game.Player.Money >= total)
                {
                    dealer1Rep += total / 100;
                    Game.Player.Money -= total;
                    dealer1Money += total;
                    drugDealer1.Money = dealer1Money;
                    UI.ShowSubtitle("Nice doing business with you.", 4000);
                    sale = true;

                }
                else
                {
                    if (dealer1Debt + total > 100000)
                    {
                        UI.ShowSubtitle("You owe me to much already", 4000);
                        sale = false;
                    }
                    else
                    {

                        dealer1Debt += total;
                        payDebt.Text = ("Pay Debt $" + dealer1Debt);
                        UI.ShowSubtitle("Ill add this one to youre bill, you now owe me $" + dealer1Debt, 4000);
                        sale = true;
                    }
                }
                if (sale)
                {
                    crack.Maximum -= crack.Value;
                    dealer1Crack = crack.Maximum;
                    total = crack.Value * (crack1Price - Discount(crack1Price));
                    CrackBuy.Text = "Buy ~g~" + crack.Value + " ~w~for $~g~" + total;
                    putCrackStash.Maximum = PlayerInventory["Crack"];
                    dealerMainMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    weedMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    crackMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    cocainMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    crack.Text = "Crack $" + (crack1Price - Discount(crack1Price)) + "oz";
                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "playerInventory")
                        {
                            foreach (XElement e in element.Elements())
                            {
                                if (e.Name == "crack")
                                {
                                    e.Value = PlayerInventory["Crack"].ToString();
                                }
                            }
                        }
                        if (element.Name == "dealer1")
                        {
                            foreach (XElement e in element.Elements())
                            {
                                if (e.Name == "debt")
                                {
                                    e.Value = dealer1Debt.ToString();
                                }
                                if (e.Name == "crack")
                                {
                                    e.Value = dealer1Crack.ToString();
                                }
                                if (e.Name == "money")
                                {
                                    e.Value = dealer1Money.ToString();
                                }
                                if (e.Name == "rep")
                                {
                                    e.Value = dealer1Rep.ToString();
                                }
                            }
                        }
                    }
                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                    TotalDrugsCarried = 0;
                    foreach (KeyValuePair<string, int> entry in PlayerInventory)
                    {
                        TotalDrugsCarried += PlayerInventory[entry.Key];
                    }
                }
            }
        }
        //COCAIN MENU
        public static void onCocainMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == CocainBuy && dealer1Cocain > 0)
            {
                var inGrams = cocaine.Value * 28;
                int total = cocaine.Value * (cocain1Price - Discount(cocain1Price));
                PlayerInventory["Cocaine"] += inGrams;
                bool sale = false;
                if (Game.Player.Money >= total)
                {
                    dealer1Rep += total / 100;
                    Game.Player.Money -= total;
                    dealer1Money += total;
                    drugDealer1.Money = dealer1Money;
                    UI.ShowSubtitle("Nice doing business with you.", 4000);
                    sale = true;
                }
                else
                {
                    if (dealer1Debt + total > 100000)
                    {
                        UI.ShowSubtitle("You owe me to much already", 4000);
                        sale = false;
                    }
                    else
                    {

                        dealer1Debt += total;
                        payDebt.Text = ("Pay Debt $" + dealer1Debt);
                        UI.ShowSubtitle("Ill add this one to youre bill, you now owe me $" + dealer1Debt, 4000);
                        sale = true;
                    }
                }
                if (sale)
                {
                    cocaine.Maximum -= cocaine.Value;
                    dealer1Cocain = cocaine.Maximum;
                    total = cocaine.Value * (cocain1Price - Discount(cocain1Price));
                    CocainBuy.Text = "Buy ~g~" + cocaine.Value + " ~w~for $~g~" + total;
                    putCocainStash.Maximum = PlayerInventory["Cocaine"];
                    dealerMainMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    weedMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    crackMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    cocainMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep);
                    cocaine.Text = "Cocaine $" + (cocain1Price - Discount(cocain1Price)) + "oz";
                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "playerInventory")
                        {
                            foreach (XElement e in element.Elements())
                            {
                                if (e.Name == "cocain")
                                {
                                    e.Value = PlayerInventory["Cocaine"].ToString();
                                }
                            }
                        }
                        if (element.Name == "dealer1")
                        {
                            foreach (XElement e in element.Elements())
                            {
                                if (e.Name == "debt")
                                {
                                    e.Value = dealer1Debt.ToString();
                                }
                                if (e.Name == "cocain")
                                {
                                    e.Value = dealer1Cocain.ToString();
                                }
                                if (e.Name == "money")
                                {
                                    e.Value = dealer1Money.ToString();
                                }
                                if (e.Name == "rep")
                                {
                                    double.TryParse(e.Value, out dealer1Rep);

                                    e.Value = dealer1Rep.ToString();
                                }
                            }
                        }
                    }
                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                    TotalDrugsCarried = 0;
                    foreach (KeyValuePair<string, int> entry in PlayerInventory)
                    {
                        TotalDrugsCarried += PlayerInventory[entry.Key];
                    }
                }
            }
        }
        //Dealer Main Menu
        public static void onDealerMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == payDebt && dealer1Debt > 0)
            {
                if (Game.Player.Money > 0)
                {
                    int total = 0;
                    if (Game.Player.Money > dealer1Debt) { total = dealer1Debt; }
                    else { total = Game.Player.Money; }
                    dealer1Debt -= total;
                    Game.Player.Money -= total;
                    dealer1Money += total;
                    drugDealer1.Money = dealer1Money;
                    UI.ShowSubtitle("Thanks man", 4000);
                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "dealer1")
                        {
                            foreach (XElement e in element.Elements())
                            {
                                if (e.Name == "debt")
                                {
                                    e.Value = dealer1Debt.ToString();
                                }
                                if (e.Name == "money")
                                {
                                    e.Value = dealer1Money.ToString();
                                }
                                if (e.Name == "rep")
                                {
                                    double.TryParse(e.Value, out dealer1Rep);
                                    dealer1Rep += total / 500;
                                    e.Value = dealer1Rep.ToString();
                                    dealerMainMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)LsFunctions.CurrentRepLvl(dealer1Rep);
                                    weedMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)LsFunctions.CurrentRepLvl(dealer1Rep);
                                    crackMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)LsFunctions.CurrentRepLvl(dealer1Rep);
                                    cocainMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)LsFunctions.CurrentRepLvl(dealer1Rep);
                                }
                            }
                        }
                    }
                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                    if (dealer1Debt == 0)
                    {
                        payDebt.Text = "No debt.";
                    }
                    else
                    {
                        payDebt.Text = "Pay Debt $" + dealer1Debt;
                    }
                }
            }
            if (item == dismissDealer)
            {
                if (drugDealer1 != null)
                {
                    drugDealer1.CurrentBlip.Remove();
                    dealerMainMenu.Visible = !dealerMainMenu.Visible;
                    drugDealer1.Task.WanderAround();
                    drugDealer1.MarkAsNoLongerNeeded();
                    drugDealer1.LeaveGroup();
                    drugDealer1 = null;
                    readyToDeal = false;
                }
            }
        }
        #endregion
        // Vehicle Menu
        public static void onPutVehicleMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == putWeedVeh)
            {
                drugCarWeed += putWeedVeh.Value;
                PlayerInventory["Weed"] -= putWeedVeh.Value;
                putWeedVeh.Maximum -= putWeedVeh.Value;
                takeWeedVeh.Maximum = drugCarWeed;
                TotalDrugsCarried = 0;
                putWeedVeh.Text = "Weed " + PlayerInventory["Weed"] + "g";
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "weed")
                            {
                                e.Value = PlayerInventory["Weed"].ToString();
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "weed")
                            {
                                e.Value = drugCarWeed.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == putCrackVeh)
            {
                drugCarCrack += putCrackVeh.Value;
                PlayerInventory["Crack"] -= putCrackVeh.Value;
                putCrackVeh.Maximum -= putCrackVeh.Value;
                takeCrackVeh.Maximum = drugCarCrack;
                TotalDrugsCarried = 0;
                putCrackVeh.Text = "Crack " + PlayerInventory["Crack"] + "g";
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = PlayerInventory["Crack"].ToString();
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = drugCarCrack.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == putCocainVeh)
            {
                drugCarCocain += putCocainVeh.Value;
                PlayerInventory["Cocaine"] -= putCocainVeh.Value;
                putCocainVeh.Maximum -= putCocainVeh.Value;
                takeCocainVeh.Maximum = drugCarCocain;
                TotalDrugsCarried = 0;
                putCocainVeh.Text = "Cocaine " + PlayerInventory["Cocaine"] + "g";
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = PlayerInventory["Cocaine"].ToString();
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = drugCarCocain.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == putMoneyVeh)
            {
                drugCarMoney += putMoneyVeh.Value;
                Game.Player.Money -= putMoneyVeh.Value;
                putMoneyVeh.Maximum -= putMoneyVeh.Value;
                takeMoneyVeh.Maximum = drugCarMoney;
                putMoneyVeh.Text = "Money $" + Game.Player.Money;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "money")
                            {
                                e.Value = drugCarMoney.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }
            if (item == putWeaponVeh)
            {
                if (putWeaponVeh.Items.Count > 0)
                {
                    vehWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
                    int ID = 0;
                    int newID = 0;
                    if (vehWeaponsXML.Descendants().Count() > 0)
                    {
                        foreach (XElement element in vehWeaponsXML.Descendants())
                        {
                            if (element.Name == "weapon")
                            {
                                ID++;
                                if (ID.ToString() != element.FirstAttribute.Value)
                                { newID = ID; }
                                //Console.WriteLine(element.Name);
                                //Console.WriteLine(element.FirstAttribute.Value);
                            }
                        }
                    }
                    XElement w = playerWeapons[putWeaponVeh.Index];
                    if (w != null)
                    {
                        Enum.TryParse(playerWeps[putWeaponVeh.Index], out WeaponHash wep);
                        if (wep != WeaponHash.Unarmed)
                        {
                            w.SetAttributeValue("id", newID);
                            var rootElement = vehWeaponsXML.Element("weapons");
                            rootElement?.Add(w);
                            vehWeaponsXML.Save("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
                            Game.Player.Character.Weapons.Remove(wep);
                            playerWeapons.Remove(w);
                            playerWeps.RemoveAt(putWeaponVeh.Index);
                            vehWeapons.Add(w);
                            vehWeps.Add(putWeaponVeh.Items[putWeaponVeh.Index]);
                        }

                    }
                }
            }
            UpdateCarStash();
        }
        public static void onTakeVehicleMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == takeWeedVeh)
            {
                PlayerInventory["Weed"] += takeWeedVeh.Value;
                drugCarWeed -= takeWeedVeh.Value;
                takeWeedVeh.Maximum -= takeWeedVeh.Value;
                putWeedVeh.Maximum = PlayerInventory["Weed"];
                TotalDrugsCarried = 0;
                takeWeedVeh.Text = "Weed " + drugCarWeed + "g";
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "weed")
                            {
                                e.Value = PlayerInventory["Weed"].ToString();
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "weed")
                            {
                                e.Value = drugCarWeed.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == takeCrackVeh)
            {
                PlayerInventory["Crack"] += takeCrackVeh.Value;
                drugCarCrack -= takeCrackVeh.Value;
                takeCrackVeh.Maximum -= takeCrackVeh.Value;
                putCrackVeh.Maximum = PlayerInventory["Crack"];
                TotalDrugsCarried = 0;
                takeCrackVeh.Text = "Crack " + drugCarCrack + "g";
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = PlayerInventory["Crack"].ToString();
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = drugCarCrack.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == takeCocainVeh)
            {
                PlayerInventory["Cocaine"] += takeCocainVeh.Value;
                drugCarCocain -= takeCocainVeh.Value;
                takeCocainVeh.Maximum -= takeCocainVeh.Value;
                putCocainStash.Maximum = PlayerInventory["Cocaine"];
                TotalDrugsCarried = 0;
                takeCocainVeh.Text = "Cocaine " + drugCarCocain + "g";
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = PlayerInventory["Cocaine"].ToString();
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = drugCarCocain.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == takeMoneyVeh)
            {
                Game.Player.Money += takeMoneyVeh.Value;
                drugCarMoney -= takeMoneyVeh.Value;
                takeMoneyVeh.Maximum -= takeMoneyVeh.Value;
                putMoneyVeh.Maximum = Game.Player.Money;
                takeMoneyVeh.Text = "Money $" + drugCarMoney;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "money")
                            {
                                e.Value = drugCarMoney.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }
            if (item == takeWeaponVeh)
            {
                if (takeWeaponVeh.Items.Count > 0)
                {
                    vehWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
                    XElement w = vehWeapons[takeWeaponVeh.Index];
                    XElement HASH = null;

                    foreach (XElement e in w.Elements())
                    {
                        if (e.Name == "HASH") { HASH = e; }
                    }
                    if (HASH != null)
                    {
                        Enum.TryParse(HASH.Value, out WeaponHash wep);
                        if (wep != WeaponHash.Unarmed)
                        {

                            foreach (XElement element in vehWeaponsXML.Descendants())
                            {
                                int ammo = 0;
                                if (element.Name == "HASH")
                                {
                                    if (element.Value == HASH.Value)
                                    {
                                        XElement rootElement = element.Parent;
                                        foreach (XElement e in rootElement.Elements())
                                        {
                                            if (e.Name == "AMMO")
                                            {
                                                Int32.TryParse(e.Value, out ammo);
                                            }
                                        }
                                        Game.Player.Character.Weapons.Give(wep, ammo, true, false);
                                        Game.Player.Character.Weapons.Select(wep, true);
                                        foreach (XElement e in rootElement.Elements())
                                        {
                                            if (e.Name == "COMPONENT")
                                            {
                                                Enum.TryParse(e.Value, out WeaponComponent wepcomp);
                                                Game.Player.Character.Weapons.Current.SetComponent(wepcomp, true);
                                            }
                                        }
                                        playerWeapons.Add(element);
                                        playerWeps.Add(element.Value);
                                        vehWeapons.Remove(w);
                                        rootElement.Remove();
                                        vehWeps.RemoveAt(takeWeaponVeh.Index);
                                        vehWeaponsXML.Save("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            UpdateCarStash();
        }
        public static void onSliderChange(UIMenuSliderItem sender, int index)
        {
            //VEHICLE SLIDERS
            if (sender == takeWeedVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take " + takeWeedVeh.Value + "g";
            }
            if (sender == putWeedVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put " + putWeedVeh.Value + "g";
            }

            if (sender == takeCrackVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take " + takeCrackVeh.Value + "g";
            }
            if (sender == putCrackVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put " + putCrackVeh.Value + "g";
            }

            if (sender == takeCocainVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take " + takeCocainVeh.Value + "g";
            }
            if (sender == putCocainVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put " + putCocainVeh.Value + "g";
            }

            if (sender == putMoneyVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put $" + putMoneyVeh.Value;
            }
            if (sender == takeMoneyVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take $" + takeMoneyVeh.Value;
            }

            //STASH SLIDERS
            if (sender == takeWeedStash)
            {
                stashTakeMenu.Subtitle.Caption = "Take " + takeWeedStash.Value + "g";
            }
            if (sender == putWeedStash)
            {
                stashPutMenu.Subtitle.Caption = "Put " + putWeedStash.Value + "g";
            }
            if (sender == takeCrackStash)
            {
                stashTakeMenu.Subtitle.Caption = "Take " + takeCrackStash.Value + "g";
            }
            if (sender == putCrackStash)
            {
                stashPutMenu.Subtitle.Caption = "Put " + putCrackStash.Value + "g";
            }
            if (sender == takeCocainStash)
            {
                stashTakeMenu.Subtitle.Caption = "Take " + takeCocainStash.Value + "g";
            }
            if (sender == putCocainStash)
            {
                stashPutMenu.Subtitle.Caption = "Put " + putCocainStash.Value + "g";
            }
            if (sender == takeMoneyStash)
            {
                stashTakeMenu.Subtitle.Caption = "Take $" + takeMoneyStash.Value;
            }
            if (sender == putMoneyStash)
            {
                stashPutMenu.Subtitle.Caption = "Put $" + putMoneyStash.Value;
            }

            //DEALER SLIDERS
            if (sender == bushWeed)
            {
                int total = bushWeed.Value * (weed1Price - LsFunctions.Discount(weed1Price));
                WeedBuy.Text = "Buy ~g~" + bushWeed.Value + " ~s~for $~g~" + total;
            }
            if (sender == crack)
            {
                int total = crack.Value * (crack1Price - LsFunctions.Discount(crack1Price));
                CrackBuy.Text = "Buy ~g~" + crack.Value + " ~s~for $~g~" + total;
            }
            if (sender == cocaine)
            {
                int total = cocaine.Value * (cocain1Price - LsFunctions.Discount(cocain1Price));
                CocainBuy.Text = "Buy ~g~" + cocaine.Value + " ~s~for $~g~" + total;
            }
        }
        public static void onListChange(UIMenuListItem sender, int index)
        {
            if (sender == takeWeaponStash)
            {
                stashTakeMenu.Subtitle.Caption = "Take " + stashWeps[takeWeaponStash.Index];
            }
            if (sender == putWeaponStash)
            {
                stashPutMenu.Subtitle.Caption = "Put " + playerWeps[putWeaponStash.Index];
            }
            if (sender == takeWeaponVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take " + vehWeps[takeWeaponVeh.Index];
            }
            if (sender == putWeaponVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put " + playerWeps[putWeaponVeh.Index];
            }
            if (sender == stashPutMultiplier)
            {
                putWeedStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
                putCrackStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
                putCocainStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
                putMoneyStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
            }
            if (sender == stashTakeMultiplier)
            {
                takeWeedStash.Multiplier = (int)multiplier[stashTakeMultiplier.Index];
                takeCrackStash.Multiplier = (int)multiplier[stashTakeMultiplier.Index];
                takeCocainStash.Multiplier = (int)multiplier[stashTakeMultiplier.Index];
                takeMoneyStash.Multiplier = (int)multiplier[stashTakeMultiplier.Index];
            }
            if (sender == vehPutMultiplier)
            {
                putWeedVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
                putCrackVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
                putCocainVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
                putMoneyVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
            }
            if (sender == vehTakeMultiplier)
            {
                takeWeedVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
                takeCrackVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
                takeCocainVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
                takeMoneyVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
            }
            if (sender == melee)
            {
                melee.Text = "Mele $" + (meleePrice[melee.Index] - LsFunctions.Discount(meleePrice[melee.Index]));
            }
            if (sender == pistol)
            {
                pistol.Text = "Pistol $" + (pistolPrice[pistol.Index] - LsFunctions.Discount(pistolPrice[pistol.Index]));
            }

        }
        //PLAYER TO STASH MENUS
        public static void onStashMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == putWeedStash)
            {
                stashWeed += putWeedStash.Value;
                PlayerInventory["Weed"] -= putWeedStash.Value;
                putWeedStash.Maximum -= putWeedStash.Value;
                takeWeedStash.Maximum = stashWeed;
                TotalDrugsCarried = 0;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "weed")
                            {
                                e.Value = PlayerInventory["Weed"].ToString();
                            }
                        }
                    }
                    if (element.Name == "playerStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "weed")
                            {
                                e.Value = stashWeed.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == putCrackStash)
            {
                stashCrack += putCrackStash.Value;
                PlayerInventory["Crack"] -= putCrackStash.Value;
                putCrackStash.Maximum -= putCrackStash.Value;
                takeCrackStash.Maximum = stashCrack;
                TotalDrugsCarried = 0;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = PlayerInventory["Crack"].ToString();
                            }
                        }
                    }
                    if (element.Name == "playerStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = stashCrack.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == putCocainStash)
            {
                stashCocain += putCocainStash.Value;
                PlayerInventory["Cocaine"] -= putCocainStash.Value;
                putCocainStash.Maximum -= putCocainStash.Value;
                takeCocainStash.Maximum = stashCocain;
                TotalDrugsCarried = 0;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = PlayerInventory["Cocaine"].ToString();
                            }
                        }
                    }
                    if (element.Name == "playerStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = stashCocain.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == putMoneyStash)
            {
                stashMoney += putMoneyStash.Value;
                Game.Player.Money -= putMoneyStash.Value;
                putMoneyStash.Maximum -= putMoneyStash.Value;
                takeMoneyStash.Maximum = stashMoney;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "money")
                            {
                                e.Value = stashMoney.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }
            if (item == putWeaponStash)
            {
                if (putWeaponStash.Items.Count > 0)
                {
                    stashWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Weapons.xml");
                    int ID = 0;
                    int newID = 0;
                    if (stashWeaponsXML.Descendants().Count() > 0)
                    {
                        foreach (XElement element in stashWeaponsXML.Descendants())
                        {
                            if (element.Name == "weapon")
                            {
                                ID++;
                                if (ID.ToString() != element.FirstAttribute.Value)
                                { newID = ID; }
                            }
                        }
                    }
                    XElement w = playerWeapons[putWeaponStash.Index];
                    if (w != null)
                    {
                        Enum.TryParse(playerWeps[putWeaponStash.Index], out WeaponHash wep);
                        if (wep != WeaponHash.Unarmed)
                        {
                            w.SetAttributeValue("id", newID);
                            var rootElement = stashWeaponsXML.Element("weapons");
                            rootElement?.Add(w);
                            stashWeaponsXML.Save("scripts\\LSLife\\LsLife_Weapons.xml");
                            Game.Player.Character.Weapons.Remove(wep);
                            playerWeapons.Remove(w);
                            playerWeps.RemoveAt(putWeaponStash.Index);
                            stashWeapons.Add(w);
                            stashWeps.Add(putWeaponStash.Items[putWeaponStash.Index]);
                        }

                    }
                }
            }
            UpdateInventory();
        }
        //STASH TO PLAYER MENUS
        public static void onPlayerMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == takeWeedStash)
            {
                PlayerInventory["Weed"] += takeWeedStash.Value;
                stashWeed -= takeWeedStash.Value;
                takeWeedStash.Maximum -= takeWeedStash.Value;
                putWeedStash.Maximum = PlayerInventory["Weed"];
                TotalDrugsCarried = 0;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "weed")
                            {
                                e.Value = PlayerInventory["Weed"].ToString();
                            }
                        }
                    }
                    if (element.Name == "playerStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "weed")
                            {
                                e.Value = stashWeed.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == takeCrackStash)
            {
                PlayerInventory["Crack"] += takeCrackStash.Value;
                stashCrack -= takeCrackStash.Value;
                takeCrackStash.Maximum -= takeCrackStash.Value;
                putCrackStash.Maximum = PlayerInventory["Crack"];
                TotalDrugsCarried = 0;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = PlayerInventory["Crack"].ToString();
                            }
                        }
                    }
                    if (element.Name == "playerStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = stashCrack.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == takeCocainStash)
            {
                PlayerInventory["Cocaine"] += takeCocainStash.Value;
                stashCocain -= takeCocainStash.Value;
                takeCocainStash.Maximum -= takeCocainStash.Value;
                putCocainStash.Maximum = PlayerInventory["Cocaine"];
                TotalDrugsCarried = 0;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = PlayerInventory["Cocaine"].ToString();
                            }
                        }
                    }
                    if (element.Name == "playerStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = stashCocain.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            if (item == takeMoneyStash)
            {
                Game.Player.Money += takeMoneyStash.Value;
                stashMoney -= takeMoneyStash.Value;
                takeMoneyStash.Maximum -= takeMoneyStash.Value;
                putMoneyStash.Maximum = Game.Player.Money;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "money")
                            {
                                e.Value = stashMoney.ToString();
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }
            if (item == takeWeaponStash)
            {
                if (takeWeaponStash.Items.Count > 0)
                {
                    stashWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Weapons.xml");
                    XElement w = stashWeapons[takeWeaponStash.Index];
                    XElement HASH = null;

                    foreach (XElement e in w.Elements())
                    {
                        if (e.Name == "HASH") { HASH = e; }
                    }
                    if (HASH != null)
                    {
                        Enum.TryParse(HASH.Value, out WeaponHash wep);
                        if (wep != WeaponHash.Unarmed)
                        {

                            foreach (XElement element in stashWeaponsXML.Descendants())
                            {
                                int ammo = 0;
                                if (element.Name == "HASH")
                                {
                                    if (element.Value == HASH.Value)
                                    {
                                        XElement rootElement = element.Parent;
                                        foreach (XElement e in rootElement.Elements())
                                        {
                                            if (e.Name == "AMMO")
                                            {
                                                Int32.TryParse(e.Value, out ammo);
                                            }
                                        }
                                        Game.Player.Character.Weapons.Give(wep, ammo, true, false);
                                        Game.Player.Character.Weapons.Select(wep, true);
                                        foreach (XElement e in rootElement.Elements())
                                        {
                                            if (e.Name == "COMPONENT")
                                            {
                                                Enum.TryParse(e.Value, out WeaponComponent wepcomp);
                                                Game.Player.Character.Weapons.Current.SetComponent(wepcomp, true);
                                            }
                                        }
                                        playerWeapons.Add(element);
                                        playerWeps.Add(element.Value);
                                        stashWeapons.Remove(w);
                                        rootElement.Remove();
                                        stashWeps.RemoveAt(takeWeaponStash.Index);
                                        stashWeaponsXML.Save("scripts\\LSLife\\LsLife_Weapons.xml");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            UpdateInventory();
        }
        //CUSTOMERS
        public static void onCustomerMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == sell)
            {
                jobOfferTimer = Game.GameTime;
                if (PlayerInventory[customerSellTo.Drug.Name] >= customerSellTo.amount)
                {
                    switch (customerSellTo.Drug.Name)
                    {
                        case "Weed":
                            if (areas[areaIndex].AreaType == AreaType.Normal)
                            {
                                if (areas[areaIndex].DemandNotMet == true)
                                {
                                    areas[areaIndex].DemandNotMet = false;
                                }
                            }
                            break;
                        case "Crack":
                            if (areas[areaIndex].AreaType == AreaType.Poor)
                            {
                                if (areas[areaIndex].DemandNotMet == true)
                                {
                                    areas[areaIndex].DemandNotMet = false;
                                }
                            }
                            break;
                        case "Cocaine":
                            if (areas[areaIndex].AreaType == AreaType.Rich)
                            {
                                if (areas[areaIndex].DemandNotMet == true)
                                {
                                    areas[areaIndex].DemandNotMet = false;
                                }
                            }
                            break;
                    }
                    if (customerSellTo.FarAway)
                    {
                        if (jobAccepted) { jobAccepted = false; }
                    }
                    if (Game.Player.Character.IsInVehicle())
                    {
                        MakeStreetSale(customerSellTo, true);
                        customerMainMenu.Visible = !customerMainMenu.Visible;
                        lastTimeDealDone = Game.GameTime;
                    }
                    else
                    {
                        MakeStreetSale(customerSellTo, false);
                        customerMainMenu.Visible = !customerMainMenu.Visible;
                        lastTimeDealDone = Game.GameTime;
                    }
                }

            }
            if (item == turnAway)
            {
                switch (customerSellTo.Drug.Name)
                {
                    case "Weed":
                        if (areas[areaIndex].AreaType == AreaType.Normal)
                        {
                            if (areas[areaIndex].DemandNotMet == false)
                            {
                                areas[areaIndex].DemandNotMet = true;
                                areas[areaIndex].HighDemandTimer = Game.GameTime;
                            }
                        }
                        break;
                    case "Crack":
                        if (areas[areaIndex].AreaType == AreaType.Poor)
                        {
                            if (areas[areaIndex].DemandNotMet == false)
                            {
                                areas[areaIndex].DemandNotMet = true;
                                areas[areaIndex].HighDemandTimer = Game.GameTime;
                            }
                        }
                        break;
                    case "Cocaine":
                        if (areas[areaIndex].AreaType == AreaType.Rich)
                        {
                            if (areas[areaIndex].DemandNotMet == false)
                            {
                                areas[areaIndex].DemandNotMet = true;
                                areas[areaIndex].HighDemandTimer = Game.GameTime;
                            }
                        }
                        break;
                }
                jobOfferTimer = Game.GameTime;
                customerMainMenu.Visible = !customerMainMenu.Visible;
                if (customerSellTo.FarAway)
                {
                    if (jobAccepted) { jobAccepted = false; }
                    jobs.Remove(customerSellTo);
                    RemovePedFromWorld(customerSellTo.Ped, true);
                    lastTimeDealDone = Game.GameTime;
                    oldCustomers.Add(customerSellTo.Ped);
                }
                else
                {
                    if (customerSellTo.Vehicle != null && customerSellTo.Vehicle.Exists())
                    {
                        customerSellTo.Ped.Task.PerformSequence(PerformTaskSeq(1, customerSellTo.Vehicle));
                        RemovePedFromWorld(customerSellTo.Ped, false);

                    }
                    else
                    {
                        LsFunctions.RemovePedFromWorld(customerSellTo.Ped, true);
                    }
                    customers.Remove(customerSellTo);
                    lastTimeDealDone = Game.GameTime;
                    oldCustomers.Add(customerSellTo.Ped);
                    customerSellTo = null;
                }
            }
        }
    }
}
