using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static LSlife.Enums;
using static LSlife.LsFunctions;
using static LSlife.LSL;

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
                    int total = TotalDrugsCarried + HouseHandler.TotalStashDrugs();
                    if (LSL.pStashVehicle != null) total += LSL.pStashVehicle.TotalDrugs();
                    sender.Visible = !sender.Visible;
                    if (ratChance > 0)
                    {

                        LsFunctions.SendTextMsg("Maybe you should lay low for a bit youre way too hot round there.");
                    }
                    else
                    if (total > 0)
                    {
                        LsFunctions.SendTextMsg("Ok, ill put the word out.");
                        Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                        isDealing = true;
                        lastStreet = World.GetStreetName(new Vector2(Game.Player.Character.Position.X, Game.Player.Character.Position.Y));
                        thisStreet = lastStreet;
                        timeStartedDealing = Game.GameTime;
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
            if (item == buyWeapons)
            {
                sender.Visible = !sender.Visible;
                if (ratChance > 0)
                {
                    SendTextMsg("Maybe you should lay low for a bit youre way too hot round there.");
                }
                else
                {
                    if (weaponDealer1 == null)
                    {
                        callDealer(true);
                        SendTextMsg("Yo man, sending someone to meet you.");
                    }
                    else
                    {
                        RemovePedFromWorld(weaponDealer1, true);
                        weaponDealer1 = null;
                        SendTextMsg("Ok ill tell them to head home.");
                    }
                }
            }
            if (item == buyLift)
            {
                if (wepDealer == null)
                {
                    sender.Visible = !sender.Visible;
                    wepDealer = new Dealer(PedHash.ArmLieut01GMM, Game.Player.Character.Position + (Game.Player.Character.ForwardVector * 2), "INGOT", playerGroup);
                    SendTextMsg("Sure, Will be there in a min.");
                }
                else
                {
                    RemovePedFromWorld(wepDealer.ped, true);
                    wepDealer.vehicle?.MarkAsNoLongerNeeded();
                    wepDealer.vehicle?.CurrentBlip?.Remove();
                    wepDealer = null;
                    SendTextMsg("Ok ill tell them to head home.");
                }
            }
            if (item == payDebt)
            {
                if (player.Money > 0)
                {
                    int toPay = 0;
                    if (dealer1Debt < player.Money)
                    {
                        toPay = dealer1Debt;
                    }
                    else
                    {
                        toPay = player.Money;
                    }
                    dealer1Debt -= toPay;
                    player.Money -= toPay;
                    dealer1Money += toPay;
                    Dealer1AddRep(MoneyToRep(toPay, true));
                    payDebt.Text = "Pay Debt $" + IntToMoney(dealer1Debt);
                    PlayTheSound();
                    SaveDealer();
                    Script.Wait(500);
                    if (dealer1Debt > 0)
                    {
                        SendTextMsg("Thanks for paying some of that debt off");
                    }
                    else
                    {
                        SendTextMsg("Thanks, all the debt is payed off");
                    }
                }
                else SendTextMsg("You idiot, you havent got any money to send me!");
            }
        }
        public static void OnWepMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == melee)
            {
                int total = meleePrice[melee.Index] - Discount(meleePrice[melee.Index]);
                if (Game.Player.Money >= total)
                {
                    Enum.TryParse<WeaponHash>(meleeWeps[melee.Index], out WeaponHash weaponHash);
                    if (!Game.Player.Character.Weapons.HasWeapon(weaponHash))
                    {
                        HandOverGoods(weaponDealer1, player.Character);
                        Game.Player.Money -= total;
                        PlayTheSound();
                        dealer1Money += total;
                        dealer1Rep += total / 100;
                        Game.Player.Character.Weapons.Give(weaponHash, 0, true, true);
                        UpdateSave();
                    }
                }
                else
                if (dealer1Debt < zeeDebtCap() && dealer1Debt + total <= zeeDebtCap())
                {
                    Enum.TryParse<WeaponHash>(meleeWeps[melee.Index], out WeaponHash weaponHash);
                    if (!Game.Player.Character.Weapons.HasWeapon(weaponHash))
                    {
                        HandOverGoods(weaponDealer1, player.Character);
                        PlayTheSound();
                        dealer1Debt += total;
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
                            if (dealer1Debt < zeeDebtCap() && dealer1Debt + total <= zeeDebtCap())
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
                                if (dealer1Debt < zeeDebtCap() && dealer1Debt + total <= zeeDebtCap())
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
                                if (dealer1Debt < zeeDebtCap() && dealer1Debt + total <= zeeDebtCap())
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
                                if (dealer1Debt < zeeDebtCap() && dealer1Debt + total <= zeeDebtCap())
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
                if (dealer1Debt < zeeDebtCap() && dealer1Debt + total <= zeeDebtCap())
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
                    if (IsPlayerArmoursMaxed())
                    {
                        UI.ShowSubtitle("You cant carry any more", 4000);
                    }
                    else
                    {
                        Game.Player.Money -= total;
                        dealer1Money += total;
                        dealer1Rep += total / 100;
                        GiveArmourToPlayer();
                        UpdateInventory();
                    }

                }
                else if (dealer1Debt < zeeDebtCap() && dealer1Debt + total <= zeeDebtCap())
                {
                    if (IsPlayerArmoursMaxed())
                    {
                        UI.ShowSubtitle("You cant carry any more", 4000);
                    }
                    else
                    {
                        dealer1Debt += total;
                        dealer1Rep += total / 100;
                        GiveArmourToPlayer();
                        UpdateInventory();
                    }
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
            if (item == WeedBuy && dealer1Weed > 0 && bushWeed.Value > 0)
            {
                int total = bushWeed.Value * (weed1Price - Discount(weed1Price));
                NewOrder.OrderWeed(bushWeed.Value, total);
                bushWeed.Maximum -= bushWeed.Value;
                dealer1Weed = bushWeed.Maximum;
                WeedBuy.Text = "Order ~g~" + bushWeed.Value + " ~s~for $" + IntToMoney(total);
                PlaceOrder.Text = "Place Order $" + IntToMoney(NewOrder.Bill);
                DbugString("Weed Order changed");
                UpdateOrderMenuDesc();
            }
        }
        //CRACK MENUS
        public static void onCrackMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == CrackBuy && dealer1Crack > 0 && crack.Value > 0)
            {
                int total = crack.Value * (crack1Price - Discount(crack1Price));
                NewOrder.OrderCrack(crack.Value, total);
                crack.Maximum -= crack.Value;
                dealer1Crack = crack.Maximum;
                CrackBuy.Text = "Order ~g~" + crack.Value + " ~w~for $~g~" + IntToMoney(total);
                PlaceOrder.Text = "Place Order $" + IntToMoney(NewOrder.Bill);
                DbugString("Crack Order changed");
                UpdateOrderMenuDesc();
            }
        }
        //COCAIN MENU
        public static void onCocainMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == CocainBuy && dealer1Cocain > 0)
            {
                int total = cocaine.Value * (cocain1Price - Discount(cocain1Price));
                NewOrder.OrderCocaine(cocaine.Value, total);
                cocaine.Maximum -= cocaine.Value;
                dealer1Cocain = cocaine.Maximum;
                CocainBuy.Text = "Order ~g~" + cocaine.Value + " ~w~for $~g~" + IntToMoney(total);
                PlaceOrder.Text = "Place Order $" + IntToMoney(NewOrder.Bill);
                DbugString("Cocaine Order changed");
                UpdateOrderMenuDesc();
            }
        }
        //Dealer Main Menu
        public static void OnOrderDrugsMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == PlaceOrder)
            {
                if (NewOrder != null)
                {
                    if (NewOrder.OrderValid())
                    {
                        NewOrder.Placed = true;
                        callDealer(false);
                        LsMenuPool.CloseAllMenus();
                    }
                }
            }
        }
        #endregion
        // Vehicle Menu
        public static void onPutVehicleMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == putWeedVeh)
            {
                LSL.pStashVehicle.drugCarWeed += putWeedVeh.Value;
                PlayerInventory["Weed"] -= putWeedVeh.Value;
                putWeedVeh.Maximum -= putWeedVeh.Value;
                takeWeedVeh.Maximum = LSL.pStashVehicle.drugCarWeed;
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
                                break;
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "weed")
                            {
                                e.Value = LSL.pStashVehicle.drugCarWeed.ToString();
                                break;
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
            if (item == putWeedOzVeh)
            {
                LSL.pStashVehicle.drugCarWeedOz += putWeedOzVeh.Value;
                pWeedOunce -= putWeedOzVeh.Value;
                putWeedOzVeh.Maximum -= putWeedOzVeh.Value;
                takeWeedOzVeh.Maximum = LSL.pStashVehicle.drugCarWeedOz;
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
                                break;
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
                                break;
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }
            if (item == putCrackVeh)
            {
                LSL.pStashVehicle.drugCarCrack += putCrackVeh.Value;
                PlayerInventory["Crack"] -= putCrackVeh.Value;
                putCrackVeh.Maximum -= putCrackVeh.Value;
                takeCrackVeh.Maximum = LSL.pStashVehicle.drugCarCrack;
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
                                break;
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = LSL.pStashVehicle.drugCarCrack.ToString();
                                break;
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
            if (item == putCrackOzVeh)
            {
                LSL.pStashVehicle.drugCarCrackOz += putCrackOzVeh.Value;
                pCrackOunce -= putCrackOzVeh.Value;
                putCrackOzVeh.Maximum -= putCrackOzVeh.Value;
                takeCrackOzVeh.Maximum = LSL.pStashVehicle.drugCarCrackOz;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crackOz")
                            {
                                e.Value = pCrackOunce.ToString();
                                break;
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crackOz")
                            {
                                e.Value = LSL.pStashVehicle.drugCarCrackOz.ToString();
                                break;
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }
            if (item == putCocainVeh)
            {
                LSL.pStashVehicle.drugCarCocain += putCocainVeh.Value;
                PlayerInventory["Cocaine"] -= putCocainVeh.Value;
                putCocainVeh.Maximum -= putCocainVeh.Value;
                takeCocainVeh.Maximum = LSL.pStashVehicle.drugCarCocain;
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
                                break;
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = LSL.pStashVehicle.drugCarCocain.ToString();
                                break;
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
            if (item == putCocainOzVeh)
            {
                LSL.pStashVehicle.drugCarCocainOz += putCocainOzVeh.Value;
                pCocaineOunce -= putCocainOzVeh.Value;
                putCocainOzVeh.Maximum -= putCocainOzVeh.Value;
                takeCocainOzVeh.Maximum = LSL.pStashVehicle.drugCarCocainOz;

                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocaineOz")
                            {
                                e.Value = pCocaineOunce.ToString();
                                break;
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocainOz")
                            {
                                e.Value = LSL.pStashVehicle.drugCarCocainOz.ToString();
                                break;
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");

            }

            if (item == putMoneyVeh)
            {
                LSL.pStashVehicle.drugCarMoney += putMoneyVeh.Value;
                Game.Player.Money -= putMoneyVeh.Value;
                putMoneyVeh.Maximum -= putMoneyVeh.Value;
                takeMoneyVeh.Maximum = LSL.pStashVehicle.drugCarMoney;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "money")
                            {
                                e.Value = LSL.pStashVehicle.drugCarMoney.ToString();
                                break;
                            }
                        }
                        break;
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
                            LSL.pStashVehicle.vehWeapons.Add(w);
                            LSL.pStashVehicle.vehWeps.Add(putWeaponVeh.Items[putWeaponVeh.Index]);
                        }

                    }
                }
            }
            LSL.pStashVehicle.UpdateCarStash();
        }
        public static void onTakeVehicleMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == takeWeedVeh)
            {
                PlayerInventory["Weed"] += takeWeedVeh.Value;
                LSL.pStashVehicle.drugCarWeed -= takeWeedVeh.Value;
                takeWeedVeh.Maximum -= takeWeedVeh.Value;
                putWeedVeh.Maximum = PlayerInventory["Weed"];
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
                                break;
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "weed")
                            {
                                e.Value = LSL.pStashVehicle.drugCarWeed.ToString();
                                break;
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
            if (item == takeWeedOzVeh)
            {
                pWeedOunce += takeWeedOzVeh.Value;
                LSL.pStashVehicle.drugCarWeedOz -= takeWeedOzVeh.Value;
                takeWeedOzVeh.Maximum -= takeWeedOzVeh.Value;
                putWeedOzVeh.Maximum = pWeedOunce;
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
                                break;
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
                                break;
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }

            if (item == takeCrackVeh)
            {
                PlayerInventory["Crack"] += takeCrackVeh.Value;
                LSL.pStashVehicle.drugCarCrack -= takeCrackVeh.Value;
                takeCrackVeh.Maximum -= takeCrackVeh.Value;
                putCrackVeh.Maximum = PlayerInventory["Crack"];
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
                                break;
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = LSL.pStashVehicle.drugCarCrack.ToString();
                                break;
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }

            if (item == takeCrackOzVeh)
            {
                pCrackOunce += takeCrackOzVeh.Value;
                LSL.pStashVehicle.drugCarCrackOz -= takeCrackOzVeh.Value;
                takeCrackOzVeh.Maximum -= takeCrackOzVeh.Value;
                putCrackOzVeh.Maximum = pCrackOunce;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crackOz")
                            {
                                e.Value = pCrackOunce.ToString();
                                break;
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "crack")
                            {
                                e.Value = LSL.pStashVehicle.drugCarCrackOz.ToString();
                                break;
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }

            if (item == takeCocainVeh)
            {
                PlayerInventory["Cocaine"] += takeCocainVeh.Value;
                LSL.pStashVehicle.drugCarCocain -= takeCocainVeh.Value;
                takeCocainVeh.Maximum -= takeCocainVeh.Value;
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
                                break;
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = LSL.pStashVehicle.drugCarCocain.ToString();
                                break;
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

            if (item == takeCocainOzVeh)
            {
                pCocaineOunce += takeCocainOzVeh.Value;
                LSL.pStashVehicle.drugCarCocainOz -= takeCocainOzVeh.Value;
                takeCocainOzVeh.Maximum -= takeCocainOzVeh.Value;
                //putCocainOzStash.Maximum = pCocaineOunce;                
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocainOz")
                            {
                                e.Value = pCocaineOunce.ToString();
                                break;
                            }
                        }
                    }
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocainOz")
                            {
                                e.Value = LSL.pStashVehicle.drugCarCocainOz.ToString();
                                break;
                            }
                        }
                        
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }



            if (item == takeMoneyVeh)
            {
                Game.Player.Money += takeMoneyVeh.Value;
                LSL.pStashVehicle.drugCarMoney -= takeMoneyVeh.Value;
                takeMoneyVeh.Maximum -= takeMoneyVeh.Value;
                putMoneyVeh.Maximum = Game.Player.Money;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "money")
                            {
                                e.Value = LSL.pStashVehicle.drugCarMoney.ToString();
                                break;
                            }
                        }
                        break;
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }
            if (item == takeWeaponVeh)
            {
                if (takeWeaponVeh.Items.Count > 0)
                {
                    vehWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
                    XElement w = LSL.pStashVehicle.vehWeapons[takeWeaponVeh.Index];
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
                                        LSL.pStashVehicle.vehWeapons.Remove(w);
                                        rootElement.Remove();
                                        LSL.pStashVehicle.vehWeps.RemoveAt(takeWeaponVeh.Index);
                                        vehWeaponsXML.Save("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            LSL.pStashVehicle.UpdateCarStash();
        }
        public static void onSliderChange(UIMenuSliderItem sender, int index)
        {
            //VEHICLE SLIDERS           
            if (sender == takeWeedVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take " + GramsToOz(takeWeedVeh.Value);
            }
            else
            if (sender == putWeedVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put " + GramsToOz(putWeedVeh.Value);
            }
            if (sender == takeWeedOzVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take ~r~" + takeWeedOzVeh.Value + "~s~z";
            }
            else
            if (sender == putWeedOzVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put ~r~" + putWeedOzVeh.Value + "~s~z";
            }
            else
            if (sender == takeCrackVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take " + GramsToOz(takeCrackVeh.Value);
            }
            else
            if (sender == putCrackVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put " + GramsToOz(putCrackVeh.Value);
            }
            if (sender == takeCrackOzVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take ~r~" + takeCrackOzVeh.Value + "~s~z";
            }
            else
            if (sender == putCrackOzVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put ~r~" + putCrackOzVeh.Value + "~s~z";
            }
            else
            if (sender == takeCocainVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take " + GramsToOz(takeCocainVeh.Value);
            }
            else
            if (sender == putCocainVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put " + GramsToOz(putCocainVeh.Value);
            }
            else
            if (sender == takeCocainOzVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take ~r~" + takeCocainOzVeh.Value + "~s~z";
            }
            else
            if (sender == putCocainOzVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put ~r~" + putCocainOzVeh.Value + "~s~z";
            }
            else
            if (sender == putMoneyVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put $" + IntToMoney(putMoneyVeh.Value);
            }
            else
            if (sender == takeMoneyVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take $" + IntToMoney(takeMoneyVeh.Value);
            }

            //STASH SLIDERS
            else
            if (sender == takeWeedStash)
            {
                stashTakeMenu.Subtitle.Caption = "Take " + GramsToOz(takeWeedStash.Value);
            }
            else
            if (sender == putWeedStash)
            {
                stashPutMenu.Subtitle.Caption = "Put " + GramsToOz(putWeedStash.Value);
            }
            else
            if (sender == takeCrackStash)
            {
                stashTakeMenu.Subtitle.Caption = "Take " + GramsToOz(takeCrackStash.Value);
            }
            else
            if (sender == putCrackStash)
            {
                stashPutMenu.Subtitle.Caption = "Put " + GramsToOz(putCrackStash.Value);
            }
            else
            if (sender == takeCocainStash)
            {
                stashTakeMenu.Subtitle.Caption = "Take " + GramsToOz(takeCocainStash.Value);
            }
            else
            if (sender == putCocainStash)
            {
                stashPutMenu.Subtitle.Caption = "Put " + GramsToOz(putCocainStash.Value);
            }
            else
            if (sender == takeMoneyStash)
            {
                stashTakeMenu.Subtitle.Caption = "Take $" + IntToMoney(takeMoneyStash.Value);
            }
            else if (sender == putWeedOzStash)
            {
                stashPutMenu.Subtitle.Caption = "Put ~r~" + putWeedOzStash.Value + "~s~z";
            }
            else if (sender == putCrackOzStash)
            {
                stashPutMenu.Subtitle.Caption = "Put ~r~" + putCrackOzStash.Value + "~s~z";
            }
            else if (sender == putCocainOzStash)
            {
                stashPutMenu.Subtitle.Caption = "Put ~r~" + pCocaineOunce + "~s~z";
            }
            else
            if (sender == putMoneyStash)
            {
                stashPutMenu.Subtitle.Caption = "Put $" + IntToMoney(putMoneyStash.Value);
            }

            //DEALER SLIDERS
            else
            if (sender == bushWeed)
            {
                int total = bushWeed.Value * (weed1Price - Discount(weed1Price));
                WeedBuy.Text = "Order ~g~" + bushWeed.Value + " ~s~for $" + IntToMoney(total);
            }
            else
            if (sender == crack)
            {
                int total = crack.Value * (crack1Price - Discount(crack1Price));
                CrackBuy.Text = "Order ~g~" + crack.Value + " ~s~for $" + IntToMoney(total);
            }
            else
            if (sender == cocaine)
            {
                int total = cocaine.Value * (cocain1Price - Discount(cocain1Price));
                CocainBuy.Text = "Order ~g~" + cocaine.Value + " ~s~for $" + IntToMoney(total);
            }
        }
        public static void onListChange(UIMenuListItem sender, int index)
        {
            if (sender == takeWeaponStash)
            {
                stashTakeMenu.Subtitle.Caption = "Take " + sender.Items[takeWeaponStash.Index];
            }
            else
            if (sender == putWeaponStash)
            {
                stashPutMenu.Subtitle.Caption = "Put " + sender.Items[putWeaponStash.Index];
            }
            else
            if (sender == takeWeaponVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take " + sender.Items[takeWeaponVeh.Index];
            }
            else
            if (sender == putWeaponVeh)
            {
                vehStashPutMenu.Subtitle.Caption = "Put " + sender.Items[putWeaponVeh.Index];
            }
            else
            if (sender == stashPutMultiplier)
            {
                putWeedStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
                putCrackStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
                putCocainStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
                putMoneyStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
                putWeedOzStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
                putCrackOzStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
                putCocainOzStash.Multiplier = (int)multiplier[stashPutMultiplier.Index];
            }
            else
            if (sender == stashTakeMultiplier)
            {
                takeWeedStash.Multiplier = (int)multiplier[stashTakeMultiplier.Index];
                takeCrackStash.Multiplier = (int)multiplier[stashTakeMultiplier.Index];
                takeCocainStash.Multiplier = (int)multiplier[stashTakeMultiplier.Index];
                takeMoneyStash.Multiplier = (int)multiplier[stashTakeMultiplier.Index];
            }
            else
            if (sender == vehPutMultiplier)
            {
                putWeedVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
                putCrackVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
                putCocainVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
                putMoneyVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
                putWeedOzVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
                putCrackOzVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
                putCocainOzVeh.Multiplier = (int)multiplier[vehPutMultiplier.Index];
            }
            else
            if (sender == vehTakeMultiplier)
            {
                takeWeedVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
                takeCrackVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
                takeCocainVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
                takeMoneyVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
                takeWeedOzVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
                takeCrackOzVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
                takeCocainOzVeh.Multiplier = (int)multiplier[vehTakeMultiplier.Index];
            }
            else
            if (sender == melee)
            {
                melee.Text = "Mele $" + IntToMoney(meleePrice[melee.Index] - LsFunctions.Discount(meleePrice[melee.Index]));
            }
            else
            if (sender == pistol)
            {
                pistol.Text = "Pistol $" + IntToMoney(pistolPrice[pistol.Index] - LsFunctions.Discount(pistolPrice[pistol.Index]));
            }

        }
        //PLAYER TO STASH MENUS
        public static void onStashMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == putWeedStash)
            {
                HouseHandler.CurrentHouse.Weed += putWeedStash.Value;
                PlayerInventory["Weed"] -= putWeedStash.Value;
            }
            else
            if (item == putCrackStash)
            {
                HouseHandler.CurrentHouse.Crack += putCrackStash.Value;
                PlayerInventory["Crack"] -= putCrackStash.Value;
            }
            else
            if (item == putCocainStash)
            {
                HouseHandler.CurrentHouse.Cocain += putCocainStash.Value;
                PlayerInventory["Cocaine"] -= putCocainStash.Value;
            }
            else
            if (item == putMoneyStash)
            {
                HouseHandler.CurrentHouse.Money += putMoneyStash.Value;
                Game.Player.Money -= putMoneyStash.Value;
            }
            else
            if (item == putWeedOzStash)
            {
                HouseHandler.CurrentHouse.Weed += putWeedOzStash.Value * 28;
                pWeedOunce -= putWeedOzStash.Value;
            }
            else if (item == putCrackOzStash)
            {
                HouseHandler.CurrentHouse.Crack += putCrackOzStash.Value * 28;
                pCrackOunce -= putCrackOzStash.Value;
            }
            else if (item == putCocainOzStash)
            {
                HouseHandler.CurrentHouse.Cocain += putCocainOzStash.Value * 28;
                pCocaineOunce -= putCocainOzStash.Value;
            }
            else
            if (item == putArmourStash)
            {
                int amount = putArmourStash.Index;
                if (amount > 0 && playerArmours > 0)
                {
                    playerArmours -= amount;
                    HouseHandler.CurrentHouse.Armor += amount;
                    foreach (XElement ele in LSLifeSave.Descendants())
                    {
                        if (ele.Name == "playerInventory")
                        {
                            foreach (XElement el in ele.Elements())
                            {
                                if (el.Name == "armour")
                                {
                                    el.Value = playerArmours.ToString();
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                }
            }

            else
            if (item == putWeaponStash)
            {
                if (putWeaponStash.Items.Count > 0)
                {
                    int ID = 0;
                    int newID = 0;
                    if (HouseHandler.CurrentHouse.Weapons.Count > 0)
                    {
                        foreach (XElement element in HouseHandler.CurrentHouse.Weapons)
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
                        //UI.Notify(wep.ToString());
                        if (wep != WeaponHash.Unarmed)
                        {
                            w.SetAttributeValue("id", newID);
                            HouseHandler.CurrentHouse.Weapons.Add(w);
                            HouseHandler.CurrentHouse.Weps.Add(putWeaponStash.Items[putWeaponStash.Index]);
                            Game.Player.Character.Weapons.Remove(wep);
                            playerWeapons.Remove(w);
                            playerWeps.RemoveAt(putWeaponStash.Index);


                        }

                    }
                }
            }
            UpdateInventory();
            HouseHandler.UpdateMenu(HouseHandler.CurrentHouse);
            HouseHandler.SaveHouse(HouseHandler.CurrentHouse);
        }
        //STASH TO PLAYER MENUS
        public static void onPlayerMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == takeWeedStash)
            {
                PlayerInventory["Weed"] += takeWeedStash.Value;
                HouseHandler.CurrentHouse.Weed -= takeWeedStash.Value;
            }
            else
            if (item == takeCrackStash)
            {
                PlayerInventory["Crack"] += takeCrackStash.Value;
                HouseHandler.CurrentHouse.Crack -= takeCrackStash.Value;

            }
            else
            if (item == takeCocainStash)
            {
                PlayerInventory["Cocaine"] += takeCocainStash.Value;
                HouseHandler.CurrentHouse.Cocain -= takeCocainStash.Value;

            }
            else
            if (item == takeMoneyStash)
            {
                Game.Player.Money += takeMoneyStash.Value;
                HouseHandler.CurrentHouse.Money -= takeMoneyStash.Value;
            }
            else
            if (item == takeArmourStash)
            {
                int amount = takeArmourStash.Index;
                int max = 5;
                if (Game.Player.Character.Armor < 100) max = 6;
                if (amount > 0 && HouseHandler.CurrentHouse.Armor > 0 && amount + playerArmours <= max)
                {
                    if (Game.Player.Character.Armor < 100)
                    {
                        Game.Player.Character.Armor = 100;
                        amount--;
                    }
                    playerArmours += amount;
                    HouseHandler.CurrentHouse.Armor -= amount;
                    foreach (XElement ele in LSLifeSave.Descendants())
                    {
                        if (ele.Name == "playerInventory")
                        {
                            foreach (XElement el in ele.Elements())
                            {
                                if (el.Name == "armour")
                                {
                                    el.Value = playerArmours.ToString();
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                }
            }
            else
            if (item == takeWeaponStash)
            {
                if (takeWeaponStash.Items.Count > 0)
                {
                    //stashWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Weapons.xml");
                    XElement w = HouseHandler.CurrentHouse.Weapons[takeWeaponStash.Index];
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
                            bool hasWeapon = false;
                            int ammo = 0;
                            foreach (XElement aEl in w.Descendants())
                            {
                                if (aEl.Name == "AMMO")
                                {
                                    Int32.TryParse(aEl.Value, out ammo);
                                    break;
                                }
                            }
                            if (!Game.Player.Character.Weapons.HasWeapon(wep))
                            {
                                Game.Player.Character.Weapons.Give(wep, ammo, true, true);
                                Game.Player.Character.Weapons.Select(wep);
                            }
                            else
                            {
                                hasWeapon = true;
                                Game.Player.Character.Weapons.Select(wep);
                                Game.Player.Character.Weapons.Current.Ammo += ammo;
                            }
                            if (!hasWeapon)
                            {
                                playerWeapons.Add(w);
                                playerWeps.Add(HASH.Value);
                                foreach (XElement aEl in w.Descendants())
                                {
                                    if (aEl.Name == "COMPONENT")
                                    {
                                        Enum.TryParse(aEl.Value, out WeaponComponent wepcomp);
                                        Game.Player.Character.Weapons.Current.SetComponent(wepcomp, true);
                                    }
                                }
                            }
                            HouseHandler.CurrentHouse.Weapons.Remove(w);
                            HouseHandler.CurrentHouse.Weps.RemoveAt(takeWeaponStash.Index);
                        }
                    }
                }
            }
            UpdateInventory();
            HouseHandler.UpdateMenu(HouseHandler.CurrentHouse);
            HouseHandler.SaveHouse(HouseHandler.CurrentHouse);
        }
        //CUSTOMERS
        public static void onCustomerMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == sell)
            {
                jobOfferTimer = Game.GameTime;
                if (PlayerInventory[customerSellTo.Drug.Name] >= customerSellTo.amount)
                {
                    foreach(Drug d in areas[areaIndex].Drugs)
                    {
                        if(d.Name == customerSellTo.Drug.Name)
                        {
                            d.Supplied = true;
                        }
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
            else
            if (item == turnAway)
            {
                foreach (Drug d in areas[areaIndex].Drugs)
                {
                    if (d.Name == customerSellTo.Drug.Name)
                    {
                        d.Supplied = false;
                    }
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
                        customerSellTo.Ped.Task.PerformSequence(PerformTaskSeq(1, customerSellTo.Vehicle, Vector3.Zero));
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

        public static void HireDealerFollowSelect(UIMenu sender, UIMenuItem selectedItem)
        {
            hireDealerMainMenu.Visible = false;
            if (aDealer != null)
            {
                aDealer.NewState = DStates.Normal;
            }

        }

        //Hire dealer menus
        public static void HireDealerOfferSelect(UIMenu sender, UIMenuItem selectedItem)
        {
            hireDealerMainMenu.Visible = false;

            if (aDealer != null)
            {
                aDealer.NewState = DStates.Normal;
                if (aDealer.LastOffer < hireDealerOfferAmount.Value)
                {
                    if (aDealer.DecidePlayerOffer(hireDealerOfferAmount.Value))
                    {
                        Function.Call(Hash._PLAY_AMBIENT_SPEECH1, aDealer.dPed, "GENERIC_YES", "SPEECH_PARAMS_FORCE");
                        ChangeAdealerToPdealer();
                        UI.ShowSubtitle("Sure i will work for you.", 4000);
                        Game.Player.Money -= hireDealerOfferAmount.Value;
                        PlayTheSound();
                        hireDealerOfferAmount.Maximum = Game.Player.Money;
                        if (driveByHandler.AmountOfDriveBy() == 0 && rnd.Next(0, 5) == 0)
                        {
                            SpawnDriveBy();
                        }

                    }
                    else
                    {
                        int rNumber = rnd.Next(0, 3);
                        if (rNumber == 0)
                        {
                            aDealer.NewState = DStates.AttackPlayer;
                            Function.Call(Hash._PLAY_AMBIENT_SPEECH1, aDealer.dPed, "GENERIC_CURSE_HIGH", "SPEECH_PARAMS_FORCE");
                            //UI.ShowSubtitle("Nah, im just gona kill you and take your money.", 4000);
                            if (driveByHandler.AmountOfDriveBy() == 0 && rnd.Next(0, 5) == 0)
                            {
                                SpawnDriveBy();
                            }
                        }
                        else
                        {
                            Function.Call(Hash._PLAY_AMBIENT_SPEECH1, aDealer.dPed, "GENERIC_NO", "SPEECH_PARAMS_FORCE");
                            UI.ShowSubtitle("Your kidding me, Im worth more than that!", 4000);
                            aDealer.SetLastOfferAmount(hireDealerOfferAmount.Value);
                        }
                    }
                }
                else
                {
                    int rNumber = rnd.Next(0, 3);
                    if (rNumber == 0)
                    {
                        aDealer.NewState = DStates.AttackPlayer;
                        Function.Call(Hash._PLAY_AMBIENT_SPEECH1, aDealer.dPed, "GENERIC_CURSE_HIGH", "SPEECH_PARAMS_FORCE");
                        UI.ShowSubtitle("Nah, im just gona kill you and take your money.", 4000);
                        if (driveByHandler.AmountOfDriveBy() == 0 && rnd.Next(0, 5) == 0)
                        {
                            SpawnDriveBy();
                        }
                    }
                    else UI.ShowSubtitle("Offer me more and i might accept.", 4000);
                }
            }
        }
        public static void HireDealerOfferAmount_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
        {
            hireDealerMakeOffer.Text = "Offer $" + IntToMoney(sender.Value);
        }

        //PlayerDealer meuns

        public static void WArmour_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (playerArmours > 0)
            {
                HandOverGoods(player.Character, DealerHandler.currentDealer.Ped);
                if (DealerHandler.currentDealer?.Ped?.Armor < 200)
                {
                    DealerHandler.currentDealer.Ped.Armor = 200;
                    DealerHandler.currentDealer.dArmour = DealerHandler.currentDealer.Ped.Armor;
                    playerArmours -= 1;
                    if (playerArmours == 0) selectedItem.Text = "No armour left";
                    PlayTheSound();
                    UpdateInventory();
                    SetWorkerMenuHealth(DealerHandler.currentDealer.Ped);
                }
                else UI.ShowSubtitle("I already have enough armour.", 4000);
            }
        }

        public static void WWeapons_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (DEBUG) UI.Notify("Transfer weapon");
            if (wWeapons.Items.Count > 0)
            {
                HandOverGoods(player.Character, DealerHandler.currentDealer.Ped);
                if (Enum.TryParse(playerWeps[wWeapons.Index], out WeaponHash wep))
                {
                    if (wep != WeaponHash.Unarmed)
                    {
                        XElement w = playerWeapons[wWeapons.Index];
                        if (w != null)
                        {
                            XDocument dealersDoc = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
                            if (dealersDoc.Descendants().Count() > 0)
                            {
                                foreach (XElement element in dealersDoc.Descendants())
                                {
                                    if (element.Name == "dealer")
                                    {
                                        bool found = false;
                                        foreach (XElement e in element.Elements())
                                        {
                                            if (e.Name == "id" && e.Value == DealerHandler.currentDealer.ID.ToString())
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                        if (found)
                                        {
                                            bool hasWeapon = false;
                                            XElement dWeapon = null;
                                            foreach (XElement e in element.Descendants())
                                            {
                                                if (e.Name == "HASH" && e.Value == wep.ToString())
                                                {
                                                    UI.Notify("Dealer found " + wep);
                                                    hasWeapon = true;
                                                    dWeapon = e.Parent;
                                                    break;
                                                }
                                            }

                                            if (hasWeapon)
                                            {
                                                dWeapon.ReplaceWith(w);
                                            }
                                            else
                                            {
                                                element.Add(w);
                                            }
                                            dealersDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
                                            int ammo = 1000;
                                            foreach (XElement e in w.Descendants())
                                            {
                                                if (e.Name == "AMMO")
                                                {
                                                    Int32.TryParse(e.Value, out ammo);
                                                    DealerHandler.currentDealer?.Ped.Weapons.Give(wep, ammo, true, true);
                                                    DealerHandler.currentDealer?.Ped?.Weapons.Select(wep);
                                                    PlayTheSound();
                                                }
                                                if (e.Name == "COMPONENT")
                                                {
                                                    if (Enum.TryParse(e.Value, out WeaponComponent wepcomp))
                                                    {
                                                        DealerHandler.currentDealer?.Ped?.Weapons.Current.SetComponent(wepcomp, true);
                                                    }
                                                }
                                            }
                                            Game.Player.Character.Weapons.Remove(wep);
                                            playerWeapons.Remove(w);
                                            playerWeps.RemoveAt(wWeapons.Index);
                                            wWeapons.Items = playerWeps;
                                            wWeapons.Index = 0;
                                            if (DEBUG) UI.Notify("Saving Dealer Weapon " + wep.ToString() + " ammo:" + ammo);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (DEBUG) UI.Notify("No match found in playerWeapons");
                    }
                }
                else if (DEBUG) UI.Notify("No such Hash found " + playerWeps[wWeapons.Index]);
            }
            else if (DEBUG) UI.Notify("NO weapons found");
        }

        public static void WFollow_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (DealerHandler.currentDealer != null)
            {
                DealerHandler.needsSave = true;
                if (DealerHandler.currentDealer.Working)
                {
                    DealerHandler.currentDealer.NewState = PdStates.FollowPlayer;
                    wFollow.Text = "Work here";
                    UI.ShowSubtitle("Ok", 3000);
                }
                else if (DealerHandler.dealers.FindAll(d => d.area == playerArea && d != DealerHandler.currentDealer && d.Working).Count == 0)
                {
                    DealerHandler.currentDealer.NewState = PdStates.Normal;
                    wFollow.Text = "Follow me";
                    UI.ShowSubtitle("Ok", 3000);
                }
                else
                {
                    UI.ShowSubtitle("You got someone else working here.", 4000);
                }
            }
            LsMenuPool.CloseAllMenus();
            Script.Wait(1000);
        }

        public static void WConfirm_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            int i = 0;
            foreach (PlayerDealer dealer in DealerHandler.dealers)
            {
                if (dealer == DealerHandler.currentDealer) break;
                else i++;
            }
            bool change = false;
            if (wWeed.Value > 0)
            {
                DealerHandler.dealers[i].Drugs["Weed"] += wWeed.Value;
                PlayerInventory["Weed"] -= wWeed.Value;
                wWeed.Value = 0;
                wWeed.Text = "Weed " + GramsToOz(wWeed.Value);
                change = true;
            }
            if (wCrack.Value > 0)
            {
                DealerHandler.dealers[i].Drugs["Crack"] += wCrack.Value;
                PlayerInventory["Crack"] -= wCrack.Value;
                wCrack.Value = 0;
                wCrack.Text = "Crack " + GramsToOz(wCrack.Value);
                change = true;
            }
            if (wCocaine.Value > 0)
            {
                DealerHandler.dealers[i].Drugs["Cocaine"] += wCocaine.Value;
                PlayerInventory["Cocaine"] -= wCocaine.Value;
                wCocaine.Value = 0;
                wCocaine.Text = "Coke " + GramsToOz(wCocaine.Value);
                change = true;
            }
            if (change)
            {
                HandOverGoods(player.Character, DealerHandler.currentDealer.Ped);
                UpdateInventory();
                DealerHandler.dealers[i].SetDrugDesc();
                DealerHandler.needsSave = true;
            }
        }

        public static void WCollect_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            int i = 0;
            foreach (PlayerDealer dealer in DealerHandler.dealers)
            {
                if (dealer == DealerHandler.currentDealer) break;
                else i++;
            }
            if (DealerHandler.dealers[i].Money >= 100)
            {
                HandOverGoods(player.Character, DealerHandler.currentDealer.Ped);
                DealerHandler.dealers[i].GiveMoneyToPlayer();
                DealerHandler.dealers[i].SetDrugDesc();
                WorkerMainMenu.CurrentSelection = 3;
            }
        }

        public static void WCocaine_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
        {
            wCocaine.Text = "Coke " + GramsToOz(sender.Value);
        }

        public static void WCrack_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
        {
            wCrack.Text = "Crack " + GramsToOz(sender.Value);
        }

        public static void WWeed_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
        {
            wWeed.Text = "Weed " + GramsToOz(sender.Value);
        }
    }
}
