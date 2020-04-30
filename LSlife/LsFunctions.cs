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
using static GTA.Script;
using GTA.Native;
using GTA.Math;
using NativeUI;
using iFruitAddon2;
using System.IO;
using Control = GTA.Control;
using static LSlife.LSL;

namespace LSlife
{
    static class LsFunctions
    {
        public static void DoGangGrowth()
        {
            foreach (Area area in areas)
            {
                int gChance = 50;
                int gTimer = 120000;
                if (area.Name == World.GetZoneNameLabel(stashBlip.Position))
                {
                    gChance = gChance * 2;
                }
                Random gRnd = new Random();
                if (gRnd.Next(0, gChance) == 0 && area.GangPresance < 10 && area.DemandNotMet)
                {
                    if (area.Name == areas[areaIndex].Name)
                    {
                        if (Game.GameTime > area.HighDemandTimer + gTimer + area.Reputation) { area.GangPresance += 1; area.HighDemandTimer = Game.GameTime; }
                    }
                    else
                    {
                        if (Game.GameTime > area.HighDemandTimer + gTimer + area.Reputation) { area.GangPresance += 1; area.HighDemandTimer = Game.GameTime; }
                    }
                }
            }
        }

        public static void SendTextMsg(string str)
        {
            Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
            UI.Notify(str);
        }

        public static void PedBag(Ped ped, bool give)
        {
            if (give)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 9, 1, 0, 0);
            }
            else
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 9, 0, 0, 0);
            }
        }

        public static void DisplayHelpText(string text)
        {
            InputArgument[] arguments = new InputArgument[] { "STRING" };
            Function.Call(Hash._0x8509B634FBE7DA11, arguments);
            InputArgument[] argumentArray2 = new InputArgument[] { text };
            Function.Call(Hash._0x6C188BE134E074AA, argumentArray2);
            InputArgument[] argumentArray3 = new InputArgument[] { 0, 0, 1, -1 };
            Function.Call(Hash._0x238FFE5C7B0498A6, argumentArray3);
        }

        public static int Discount(int price)
        {
            int discount = 0;
            if (CurrentRepLvl(dealer1Rep) >= 1)
            {
                discount = price / 100 * (int)CurrentRepLvl(dealer1Rep);
            }
            return discount;
        }

        public static double CurrentRepLvl(double rep)
        {
            double level;
            float mod = 0.04f;
            level = mod * Math.Sqrt(rep);
            return level;
        }

        public static void RemovePedFromWorld(Ped ped, bool clearTask)
        {
            ped.CurrentBlip.Remove();
            ped.LeaveGroup();
            if (clearTask)
            {
                ped.Task.ClearAll();
            }
            ped.MarkAsNoLongerNeeded();
            ped.BlockPermanentEvents = false;
            ped.IsPersistent = false;
            entitiesCleanUp.Add(ped);
        }

        public static void VehicleSave(Vehicle veh)
        {
            //Enum.TryParse(veh.Model.ToString(), out VehicleHash hash);
            //UI.Notify("" + veh.Model.ToString() + " | " + hash + " | " + veh.Model.Hash);
            if (veh.Model.IsCar)
            {
                XDocument vehiclesXML = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");

                var rootElement = vehiclesXML.Root;
                rootElement.RemoveAll();
                var parentElement = new XElement("Vehicle");
                var modsElement = new XElement("Mods");
                var toggleModsElement = new XElement("ToggleMods");
                var neonsElement = new XElement("Neons");
                parentElement.Add(new XElement("Destroyed", "false"));
                parentElement.Add(new XElement("Handle", veh.Handle.ToString()));
                parentElement.Add(new XElement("Name", veh.DisplayName));
                parentElement.Add(new XElement("PosX", veh.Position.X));
                parentElement.Add(new XElement("PosY", veh.Position.Y));
                parentElement.Add(new XElement("PosZ", veh.Position.Z));
                parentElement.Add(new XElement("Heading", veh.Heading));
                parentElement.Add(new XElement("PrimaryCol", veh.PrimaryColor));
                parentElement.Add(new XElement("SecondaryCol", veh.SecondaryColor));
                parentElement.Add(new XElement("Health", veh.BodyHealth));
                parentElement.Add(new XElement("EngineHealth", veh.EngineHealth));
                parentElement.Add(new XElement("WheelType", veh.WheelType));
                parentElement.Add(new XElement("RimCol", veh.RimColor));
                parentElement.Add(new XElement("SmokeCol", veh.TireSmokeColor));
                parentElement.Add(new XElement("NeonCol", veh.NeonLightsColor));
                parentElement.Add(new XElement("TrimCol", veh.TrimColor));
                parentElement.Add(new XElement("DashCol", veh.DashboardColor));
                parentElement.Add(new XElement("NumPlateType", veh.NumberPlateType));
                parentElement.Add(new XElement("NumPlate", veh.NumberPlate));
                parentElement.Add(new XElement("WindowTint", veh.WindowTint));
                parentElement.Add(new XElement("DirtLevel", veh.DirtLevel));
                parentElement.Add(new XElement("CanTiresBurst", veh.CanTiresBurst));
                parentElement.Add(new XElement("Fuel", veh.FuelLevel));
                if (Function.Call<bool>(Hash.GET_VEHICLE_MOD_VARIATION, veh.Handle, (int)VehicleMod.FrontWheels))
                {
                    parentElement.Add(new XElement("CustomTire", true));
                }
                else
                {
                    parentElement.Add(new XElement("CustomTire", false));
                }
                foreach (VehicleMod mod in Enum.GetValues(typeof(VehicleMod)))
                {
                    modsElement.Add(new XElement(mod.ToString(), veh.GetMod(mod)));
                }
                foreach (VehicleToggleMod mod in Enum.GetValues(typeof(VehicleToggleMod)))
                {
                    toggleModsElement.Add(new XElement(mod.ToString(), veh.IsToggleModOn(mod)));
                }
                foreach (VehicleNeonLight neon in Enum.GetValues(typeof(VehicleNeonLight)))
                {
                    neonsElement.Add(new XElement(neon.ToString(), veh.IsNeonLightsOn(neon)));
                }
                parentElement.Add(modsElement);
                parentElement.Add(toggleModsElement);
                parentElement.Add(neonsElement);
                rootElement.Add(parentElement);
                vehiclesXML.Save("scripts\\LSLife\\LSLife_Vehicles.xml");
                drugCar = veh;
                drugCar.IsPersistent = true;
                if (drugCar.CurrentBlip != null)
                {
                    drugCar.CurrentBlip.Remove();
                }
                drugCar.AddBlip();
                drugCar.CurrentBlip.IsShortRange = true;
                drugCar.CurrentBlip.IsFriendly = true;
                drugCar.CurrentBlip.Sprite = BlipSprite.PersonalVehicleCar;
                drugCar.CurrentBlip.Name = "Drug Car";
                drugCar.CurrentBlip.Color = BlipColor.Green;
            }
        }

        public static void VehicleLoad()
        {
            string drugCarModel = "";
            Vector3 drugCarPos = Vector3.Zero;
            float drugCarHead = 0;
            VehicleColor drugCarPriCol = VehicleColor.Blue;
            VehicleColor drugCarSecCol = VehicleColor.Blue;
            int health = 1000;
            int engineHealth = 1000;
            VehicleWheelType wheelType = VehicleWheelType.Tuner;
            VehicleColor rimCol = VehicleColor.BrushedSteel;
            bool canTiresBurst = false;
            NumberPlateType plateType = NumberPlateType.BlueOnWhite1;
            string plate = "";
            VehicleWindowTint tint = VehicleWindowTint.None;
            float dirt = 0;
            float fuel = 0;
            bool customTire = false;

            XDocument vehiclesXML = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");

            foreach (XElement veh in vehiclesXML.Descendants())
            {
                if (veh.Name == "Vehicle")
                {
                    foreach (XElement e in veh.Elements())
                    {
                        if (e.Name == "Name")
                        {
                            drugCarModel = e.Value;
                        }
                        if (e.Name == "PosX")
                        {
                            float.TryParse(e.Value, out drugCarPos.X);
                        }
                        if (e.Name == "PosY")
                        {
                            float.TryParse(e.Value, out drugCarPos.Y);
                        }
                        if (e.Name == "PosZ")
                        {
                            float.TryParse(e.Value, out drugCarPos.Z);
                        }
                        if (e.Name == "Heading")
                        {
                            float.TryParse(e.Value, out drugCarHead);
                        }
                        if (e.Name == "PrimaryCol")
                        {
                            Enum.TryParse<VehicleColor>(e.Value, out drugCarPriCol);
                        }
                        if (e.Name == "SecondaryCol")
                        {
                            Enum.TryParse<VehicleColor>(e.Value, out drugCarSecCol);
                        }
                        if (e.Name == "Health")
                        {
                            Int32.TryParse(e.Value, out health);
                            if (health < 100) { health = 1000; }
                        }
                        if (e.Name == "EngineHealth")
                        {
                            Int32.TryParse(e.Value, out engineHealth);
                            if (engineHealth < 100) { engineHealth = 1000; }
                        }
                        if (e.Name == "WheelType")
                        {
                            Enum.TryParse<VehicleWheelType>(e.Value, out wheelType);
                        }
                        if (e.Name == "RimCol")
                        {
                            Enum.TryParse<VehicleColor>(e.Value, out rimCol);
                        }
                        if (e.Name == "NumPlateType")
                        {
                            Enum.TryParse<NumberPlateType>(e.Value, out plateType);
                        }
                        if (e.Name == "NumPlate")
                        {
                            plate = e.Value;
                        }
                        if (e.Name == "WindowTint")
                        {
                            Enum.TryParse<VehicleWindowTint>(e.Value, out tint);
                        }
                        if (e.Name == "DirtLevel")
                        {
                            float.TryParse(e.Value, out dirt);
                        }
                        if (e.Name == "CanTiresBurst")
                        {
                            bool.TryParse(e.Value, out canTiresBurst);
                        }
                        if (e.Name == "Fuel")
                        {
                            float.TryParse(e.Value, out fuel);
                        }
                        if (e.Name == "CustomTire")
                        {
                            bool.TryParse(e.Value, out customTire);
                        }
                    }
                }
            }
            drugCar = World.CreateVehicle(new Model(drugCarModel), drugCarPos, drugCarHead);
            Wait(10);
            if (drugCar != null)
            {
                drugCar.InstallModKit();
                if (DEBUG)
                {
                    UI.Notify("" + drugCarModel + " | " + drugCarPos + " | " + Game.Player.Character.Position);
                }
                drugCar.PrimaryColor = drugCarPriCol;
                drugCar.SecondaryColor = drugCarSecCol;
                drugCar.Health = health;
                drugCar.EngineHealth = engineHealth;
                drugCar.WheelType = wheelType;
                drugCar.EngineRunning = true;
                drugCar.IsInvincible = true;
                drugCar.CanTiresBurst = canTiresBurst;
                drugCar.NumberPlateType = plateType;
                drugCar.NumberPlate = plate;
                drugCar.WindowTint = tint;
                drugCar.DirtLevel = dirt;
                drugCar.FuelLevel = fuel;
                drugCar.IsPersistent = true;
                drugCar.RimColor = rimCol;
                drugCar.Repair();
                if (drugCar.CurrentBlip != null)
                {
                    drugCar.CurrentBlip.Remove();
                }
                drugCar.AddBlip();
                drugCar.CurrentBlip.IsShortRange = true;
                drugCar.CurrentBlip.IsFriendly = true;
                drugCar.CurrentBlip.Sprite = BlipSprite.PersonalVehicleCar;
                drugCar.CurrentBlip.Name = "Drug Car";
                drugCar.CurrentBlip.Color = BlipColor.Green;
                foreach (XElement veh in vehiclesXML.Descendants())
                {
                    if (veh.Name == "Mods")
                    {
                        foreach (XElement m in veh.Elements())
                        {
                            if (m.Name == "FrontWheels")
                            {
                                Enum.TryParse<VehicleMod>(m.Name.ToString(), out VehicleMod mod);
                                Int32.TryParse(m.Value, out int val);
                                drugCar.SetMod(mod, val, customTire);
                                drugCar.RimColor = rimCol;
                            }
                            else
                            {
                                Enum.TryParse<VehicleMod>(m.Name.ToString(), out VehicleMod mod);
                                Int32.TryParse(m.Value, out int val);
                                drugCar.SetMod(mod, val, false);
                            }
                        }
                    }
                    if (veh.Name == "ToggleMods")
                    {
                        foreach (XElement t in veh.Elements())
                        {
                            Enum.TryParse<VehicleToggleMod>(t.Name.ToString(), out VehicleToggleMod toggleMod);
                            bool.TryParse(t.Value, out bool toggle);
                            drugCar.ToggleMod(toggleMod, toggle);
                        }
                    }
                }
            }
        }

        public static void UpdateCarStash()
        {
            putWeedVeh.Text = "Weed " + PlayerInventory["Weed"] + "g";
            takeWeedVeh.Text = "Weed " + drugCarWeed + "g";
            putCrackVeh.Text = "Crack " + PlayerInventory["Crack"] + "g";
            takeCrackVeh.Text = "Crack " + drugCarCrack + "g";
            putCocainVeh.Text = "Cocaine " + PlayerInventory["Cocaine"] + "g";
            takeCocainVeh.Text = "Cocaine " + drugCarCocain + "g";
            putMoneyVeh.Text = "Money $ " + Game.Player.Money;
            takeMoneyVeh.Text = "Money $ " + drugCarMoney;
            playerWeapons.Clear();
            playerWeps.Clear();
            int i = 0;
            foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
            {
                bool hasPedGotWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Game.Player.Character, (int)weapon);
                int ammunitionInWeapon = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Game.Player.Character, (int)weapon);
                if (hasPedGotWeapon)
                {

                    var parentElement = new XElement("weapon");
                    parentElement.SetAttributeValue("id", i);
                    var weaponNameElement = new XElement("HASH", weapon);
                    var weaponAmmoElement = new XElement("AMMO", ammunitionInWeapon);
                    parentElement.Add(weaponNameElement);
                    parentElement.Add(weaponAmmoElement);
                    foreach (WeaponComponent component in Enum.GetValues(typeof(WeaponComponent)))
                    {
                        bool hasWeaponGotComponent = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character, (int)weapon, (int)component);
                        if (hasWeaponGotComponent)
                        {
                            var weaponComponentElement = new XElement("COMPONENT", component);
                            parentElement.Add(weaponComponentElement);
                        }
                    }
                    playerWeapons.Add(parentElement);
                    playerWeps.Add(weapon.ToString());
                    i++;
                }
            }
            vehWeapons.Clear();
            vehWeps.Clear();
            vehWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
            foreach (XElement element in vehWeaponsXML.Descendants())
            {
                if (element.Name == "weapon")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "HASH")
                        {
                            vehWeapons.Add(element);
                            vehWeps.Add(e.Value);
                        }
                    }
                }
            }
        }

        public static void UpdateInventory()
        {
            foreach (KeyValuePair<string, int> entry in PlayerInventory)
            {
                TotalDrugsCarried += PlayerInventory[entry.Key];
            }
            putWeedStash.Maximum = PlayerInventory["Weed"];
            putCrackStash.Maximum = PlayerInventory["Crack"];
            putCocainStash.Maximum = PlayerInventory["Cocaine"];
            putMoneyStash.Maximum = Game.Player.Money;
            putWeedStash.Text = "Weed " + PlayerInventory["Weed"] + "g.";
            putCrackStash.Text = "Crack " + PlayerInventory["Crack"] + "g.";
            putCocainStash.Text = "Cocaine " + PlayerInventory["Cocaine"] + "g.";
            putMoneyStash.Text = "Money $" + Game.Player.Money + ".";
            playerWeapons.Clear();
            playerWeps.Clear();
            int i = 0;
            bool saveChanged = false;
            foreach (XElement element in LSLifeSave.Descendants())
            {
                if (element.Name == "playerInventory")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weed")
                        {
                            if(e.Value != PlayerInventory["Weed"].ToString())
                            {
                                e.Value = PlayerInventory["Weed"].ToString();
                                saveChanged = true;
                            }                            
                        }
                        if (e.Name == "crack")
                        {
                            if (e.Value != PlayerInventory["Crack"].ToString())
                            {
                                e.Value = PlayerInventory["Crack"].ToString();
                                saveChanged = true;
                            }
                        }
                        if (e.Name == "weed")
                        {
                            if (e.Value != PlayerInventory["Weed"].ToString())
                            {
                                e.Value = PlayerInventory["Crack"].ToString();
                                saveChanged = true;
                            }
                        }
                    }
                }
            }
            if(saveChanged) { LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml"); }

            foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
            {
                bool hasPedGotWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Game.Player.Character, (int)weapon);
                int ammunitionInWeapon = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Game.Player.Character, (int)weapon);
                if (hasPedGotWeapon)
                {

                    var parentElement = new XElement("weapon");
                    parentElement.SetAttributeValue("id", i);
                    var weaponNameElement = new XElement("HASH", weapon);
                    var weaponAmmoElement = new XElement("AMMO", ammunitionInWeapon);
                    parentElement.Add(weaponNameElement);
                    parentElement.Add(weaponAmmoElement);
                    foreach (WeaponComponent component in Enum.GetValues(typeof(WeaponComponent)))
                    {
                        bool hasWeaponGotComponent = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character, (int)weapon, (int)component);
                        if (hasWeaponGotComponent)
                        {
                            var weaponComponentElement = new XElement("COMPONENT", component);
                            parentElement.Add(weaponComponentElement);
                        }
                    }
                    playerWeapons.Add(parentElement);
                    playerWeps.Add(weapon.ToString());
                    i++;
                }
            }
            takeWeedStash.Maximum = stashWeed;
            takeCrackStash.Maximum = stashCrack;
            takeCocainStash.Maximum = stashCocain;
            takeMoneyStash.Maximum = stashMoney;
            takeWeedStash.Text = "Weed " + stashWeed + "g.";
            takeCrackStash.Text = "Crack " + stashCrack + "g.";
            takeCocainStash.Text = "Cocaine " + stashCocain + "g.";
            takeMoneyStash.Text = "Money $" + stashMoney + ".";
            stashWeapons.Clear();
            stashWeps.Clear();
            stashWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Weapons.xml");
            foreach (XElement element in stashWeaponsXML.Descendants())
            {
                if (element.Name == "weapon")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "HASH")
                        {
                            stashWeapons.Add(element);
                            stashWeps.Add(e.Value);
                        }
                    }
                }
            }
        }

        public static void DateTimeUpdate()
        {

            if (Function.Call<int>(Hash.GET_CLOCK_HOURS) != hour)
            {
                hour = Function.Call<int>(Hash.GET_CLOCK_HOURS);
                if (dealer1Weed == 0 || dealer1Crack == 0 || dealer1Cocain == 0)
                {
                    if (!dealer1ReloadOrder && !dealer1Reloaded)
                    {
                        dealer1ReloadOrder = true;
                        int waitTime = 10 - (int)LsFunctions.CurrentRepLvl(dealer1Rep);
                        int timeTodeliver = 0;
                        bool nextday = false;
                        if (hour + waitTime > 24) { timeTodeliver = 0 + (hour + waitTime - 24); nextday = true; }
                        else { timeTodeliver = hour + waitTime; nextday = false; }
                        dealer1ReloadHour = timeTodeliver;
                        if (nextday)
                        {
                            if (day == 6)
                            {

                                dealer1ReloadDay = 0;
                            }
                            else
                            {
                                dealer1ReloadDay = day + 1;
                            }
                        }
                        else
                        {
                            dealer1ReloadDay = day;
                        }
                        Enum.TryParse<DayGTA>(dealer1ReloadDay.ToString(), out DayGTA mDay);
                        SendTextMsg("Zee has ordered a reload, should come " + mDay.ToString() + " at " + dealer1ReloadHour + ":00.");
                    }
                }
                int timeReduction = 0;
                if (LsFunctions.CurrentRepLvl(dealer1Rep) > 36)
                {
                    timeReduction = 360000;
                }
                else
                {
                    timeReduction = (int)LsFunctions.CurrentRepLvl(dealer1Rep) * 10000;
                }
                if (!dealer1ReloadOrder && dealer1Reloaded && Game.GameTime > reloadTimer + (reloadTimerInterval - timeReduction))
                {
                    dealer1Reloaded = false;
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "dealer1")
                        {
                            foreach (XElement drugCarEle in element.Elements())
                            {
                                if (drugCarEle.Name == "reloaded")
                                {
                                    drugCarEle.Value = dealer1Reloaded.ToString();
                                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                                    break;
                                }
                            }
                        }
                    }

                }
                int dDay = 0;
                if (dealer1ReloadDay + 1 == 7) { dDay = 0; }
                else { dDay = dealer1ReloadDay + 1; }
                if (day == dealer1ReloadDay && !dealer1Reloaded && dealer1ReloadOrder && hour >= dealer1ReloadHour)
                {
                    dealer1ReloadOrder = false;
                    dealer1Reloaded = true;
                    reloadTimer = Game.GameTime;
                    if (dealer1Debt > 0)
                    {
                        float intrest = (float)0.05;
                        float intrestAmount = (float)dealer1Debt * intrest;
                        dealer1Debt += (int)intrestAmount;
                        SendTextMsg("Zee added $" + (int)intrestAmount + " to your bill, its now $" + dealer1Debt);                        
                    }
                    if (dealer1Money < 10000)
                    {
                        dealer1Money = 50000;
                    }
                    float weedMoney = 0;
                    float crackMoney = 0;
                    float cocainMoney = 0;
                    if (dealer1Weed < 40)
                    {
                        weedMoney = (float)dealer1Money * 0.75f;
                        dealer1Money -= (int)weedMoney;
                    }
                    if (dealer1Crack < 40)
                    {
                        crackMoney = (float)dealer1Money * 0.6f;
                        dealer1Money -= (int)crackMoney;
                    }
                    if (dealer1Cocain < 40)
                    {
                        cocainMoney = dealer1Money;
                        dealer1Money -= (int)cocainMoney;
                    }
                    int dWeed = (weed1Price / 2) - LsFunctions.Discount(weed1Price / 2);
                    int dCrack = (crack1Price / 2) - LsFunctions.Discount(crack1Price / 2);
                    int dCocaine = (cocain1Price / 2) - LsFunctions.Discount(cocain1Price / 2);
                    int finalWeed = (int)weedMoney / dWeed;
                    int finalCrack = (int)crackMoney / dCrack;
                    int finalCocain = (int)cocainMoney / dCocaine;
                    dealer1Weed += finalWeed;
                    dealer1Crack += finalCrack;
                    dealer1Cocain += finalCocain;
                    weedMoney -= finalWeed * dWeed;
                    crackMoney -= finalCrack * dCrack;
                    cocainMoney -= finalCocain * dCocaine;
                    dealer1Money += (int)weedMoney + (int)crackMoney + (int)cocainMoney;
                    SendTextMsg("Zee Reloaded. He got " + "Weed:" + finalWeed + " Crack:" + finalCrack + " Cocaine:" + finalCocain);                    
                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "dealer1")
                        {
                            foreach (XElement drugCarEle in element.Elements())
                            {
                                if (drugCarEle.Name == "debt")
                                {
                                    drugCarEle.Value = dealer1Debt.ToString();
                                }
                                if (drugCarEle.Name == "weed")
                                {
                                    drugCarEle.Value = dealer1Weed.ToString();
                                }
                                if (drugCarEle.Name == "crack")
                                {
                                    drugCarEle.Value = dealer1Crack.ToString();
                                }
                                if (drugCarEle.Name == "cocain")
                                {
                                    drugCarEle.Value = dealer1Cocain.ToString();
                                }
                                if (drugCarEle.Name == "money")
                                {
                                    drugCarEle.Value = dealer1Money.ToString();
                                }
                                if (drugCarEle.Name == "reloaded")
                                {
                                    drugCarEle.Value = dealer1Reloaded.ToString();
                                }
                            }
                        }
                    }
                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                }
                if (day == dDay && !dealer1Reloaded && dealer1ReloadOrder)
                {
                    dealer1ReloadOrder = false;
                    dealer1Reloaded = true;
                    reloadTimer = Game.GameTime;
                    if (dealer1Debt > 0)
                    {
                        float intrest = (float)0.05;
                        float intrestAmount = (float)dealer1Debt * intrest;
                        dealer1Debt += (int)intrestAmount;
                        SendTextMsg("Zee added $" + (int)intrestAmount + " to your bill, its now $" + dealer1Debt);
                       
                    }
                    if (dealer1Money < 10000)
                    {
                        dealer1Money = 50000;
                    }
                    float weedMoney = 0;
                    float crackMoney = 0;
                    float cocainMoney = 0;
                    if (dealer1Weed < 40)
                    {
                        weedMoney = (float)dealer1Money * 0.75f;
                        dealer1Money -= (int)weedMoney;
                    }
                    if (dealer1Crack < 40)
                    {
                        crackMoney = (float)dealer1Money * 0.6f;
                        dealer1Money -= (int)crackMoney;
                    }
                    if (dealer1Cocain < 40)
                    {
                        cocainMoney = dealer1Money;
                        dealer1Money -= (int)cocainMoney;
                    }
                    int dWeed = (weed1Price / 2) - LsFunctions.Discount(weed1Price / 2);
                    int dCrack = (crack1Price / 2) - LsFunctions.Discount(crack1Price / 2);
                    int dCocaine = (cocain1Price / 2) - LsFunctions.Discount(cocain1Price / 2);
                    int finalWeed = (int)weedMoney / dWeed;
                    int finalCrack = (int)crackMoney / dCrack;
                    int finalCocain = (int)cocainMoney / dCocaine;
                    dealer1Weed += finalWeed;
                    dealer1Crack += finalCrack;
                    dealer1Cocain += finalCocain;
                    weedMoney -= finalWeed * dWeed;
                    crackMoney -= finalCrack * dCrack;
                    cocainMoney -= finalCocain * dCocaine;
                    dealer1Money += (int)weedMoney + (int)crackMoney + (int)cocainMoney;
                    SendTextMsg("Zee Reloaded. He got " + "Weed:" + finalWeed + " Crack:" + finalCrack + " Cocaine:" + finalCocain);
                    if (DEBUG) { UI.Notify("Weed:" + finalWeed + " Crack:" + finalCrack + " Cocaine:" + finalCocain); }
                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "dealer1")
                        {
                            foreach (XElement drugCarEle in element.Elements())
                            {
                                if (drugCarEle.Name == "debt")
                                {
                                    drugCarEle.Value = dealer1Debt.ToString();
                                }
                                if (drugCarEle.Name == "weed")
                                {
                                    drugCarEle.Value = dealer1Weed.ToString();
                                }
                                if (drugCarEle.Name == "crack")
                                {
                                    drugCarEle.Value = dealer1Crack.ToString();
                                }
                                if (drugCarEle.Name == "cocain")
                                {
                                    drugCarEle.Value = dealer1Cocain.ToString();
                                }
                                if (drugCarEle.Name == "money")
                                {
                                    drugCarEle.Value = dealer1Money.ToString();
                                }
                                if (drugCarEle.Name == "reloaded")
                                {
                                    drugCarEle.Value = dealer1Reloaded.ToString();
                                }
                            }
                        }
                    }
                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                }
            }
            if (Function.Call<int>(Hash.GET_CLOCK_DAY_OF_WEEK) != day)
            {
                day = Function.Call<int>(Hash.GET_CLOCK_DAY_OF_WEEK);
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");

                if (dealer1Money > 1200000) { dealer1Money = 1200000; }
                if (dealer1Weed > 0)
                {
                    if (dealer1Weed > 100 + LsFunctions.CurrentRepLvl(dealer1Rep)) { dealer1Weed = 100 + (int)LsFunctions.CurrentRepLvl(dealer1Rep); }
                    if (dealer1Weed == 1) { dealer1Weed = 0; dealer1Money += (weed1Price / 2) + LsFunctions.Discount(weed1Price / 2); }
                    else { dealer1Weed -= dealer1Weed / 2; dealer1Money += ((weed1Price / 2) + LsFunctions.Discount(weed1Price / 2)) * dealer1Weed / 2; }

                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "dealer1")
                        {
                            foreach (XElement drugCarEle in element.Elements())
                            {
                                if (drugCarEle.Name == "weed")
                                {
                                    drugCarEle.Value = dealer1Weed.ToString();
                                }
                                if (drugCarEle.Name == "money")
                                {
                                    drugCarEle.Value = dealer1Money.ToString();
                                }
                            }
                        }
                    }
                }
                if (dealer1Crack > 0)
                {
                    if (dealer1Crack > 70 + LsFunctions.CurrentRepLvl(dealer1Rep)) { dealer1Crack = 70 + (int)LsFunctions.CurrentRepLvl(dealer1Rep); }
                    if (dealer1Crack == 1) { dealer1Crack = 0; dealer1Money += (crack1Price / 2) + LsFunctions.Discount(crack1Price / 2); }
                    else { dealer1Crack -= dealer1Crack / 2; dealer1Money += ((crack1Price / 2) + LsFunctions.Discount(crack1Price / 2)) * dealer1Crack / 2; }
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "dealer1")
                        {
                            foreach (XElement drugCarEle in element.Elements())
                            {
                                if (drugCarEle.Name == "crack")
                                {
                                    drugCarEle.Value = dealer1Crack.ToString();
                                }
                                if (drugCarEle.Name == "money")
                                {
                                    drugCarEle.Value = dealer1Money.ToString();
                                }
                            }
                        }
                    }
                }
                if (dealer1Cocain > 0)
                {
                    if (dealer1Cocain > 40 + LsFunctions.CurrentRepLvl(dealer1Rep)) { dealer1Cocain = 40 + (int)LsFunctions.CurrentRepLvl(dealer1Rep); }
                    if (dealer1Cocain == 1) { dealer1Cocain = 0; dealer1Money += (cocain1Price / 2) + LsFunctions.Discount(cocain1Price / 2); }
                    else { dealer1Cocain -= dealer1Cocain / 2; dealer1Money += ((cocain1Price / 2) + LsFunctions.Discount(cocain1Price / 2)) * dealer1Cocain / 2; }
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "dealer1")
                        {
                            foreach (XElement drugCarEle in element.Elements())
                            {
                                if (drugCarEle.Name == "cocain")
                                {
                                    drugCarEle.Value = dealer1Cocain.ToString();
                                }
                                if (drugCarEle.Name == "money")
                                {
                                    drugCarEle.Value = dealer1Money.ToString();
                                }
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }

        }

        public static Vector3 WeedPlaceLocation()
        {
            return new Vector3(610.726013f, -3059.59766f, 6.2584672f);
        }

        public static Prop LoadWeedPlace()
        {
            int doorHash = 464151082;
            Prop prop = null;
            var props = World.GetNearbyProps(Game.Player.Character.Position, 50f);
            Vector3 pos = WeedPlaceLocation();
            if (props.Count() > 0)
            {
                foreach (Prop p in props)
                {
                    if (p.Model.Hash == doorHash)
                    {
                        prop = p;
                        if (p.Position != pos) { p.Position = pos; }
                        if (DEBUG) { UI.Notify("Door found"); }
                        break;
                    }
                }
            }
            if (prop == null)
            {
                prop = World.CreateProp(doorHash, pos, new Vector3(0, 0, 0), false, false);
                if (DEBUG) { UI.Notify("Didnt find a door spawning one"); }
            }
            return prop;
        }

        public static void ContactAnswered(iFruitContact contact)
        {
            // The contact has answered, we can execute our code            
            if (contact.Name == "Zee")
            {

                if (isDealing)
                {
                    isDealing = false;
                    SendTextMsg("Street dealing cancel");
                }
                if (LsMenuPool != null)
                {
                    zeeMenu.Visible = !zeeMenu.Visible;
                    zeeMenu.CurrentSelection = 0;
                    
                }
            }
            // We need to close the phone at a moment.
            // We can close it as soon as the contact pick up calling _iFruit.Close().
            // Here, we will close the phone in 5 seconds (5000ms).
            iFruit.Close(3000);
        }

        public static void MovePed(Ped p, Vector3 target)
        {
            if (p.Position.DistanceTo(target) > 20 || Function.Call<bool>(Hash.IS_POINT_ON_ROAD, target.X, target.Y, target.Z))
            {
                p.Task.RunTo(target);
            }
            else
            {
                p.Task.GoTo(target);
            }
        }

        public static void MakeStreetSale(Customer c, bool inCar)
        {
            List<Ped> police = new List<Ped>();
            Ped p = c.Ped;
            int price = c.amount * c.Drug.PricePerG;
            float divder = customerSellTo.amount / 3.5f;
            if (divder >= 1)
            {
                price = (c.amount * c.Drug.PricePerG) - ((int)(c.amount / 3.5) * c.Drug.PricePerG);
            }
            foreach (Ped ped in World.GetNearbyPeds(Game.Player.Character.Position, 55))
            {
                if (Function.Call<int>(Hash.GET_PED_TYPE, ped.Handle) == 6)
                {
                    police.Add(ped);
                    ped.Task.LookAt(Game.Player.Character, 10000);
                }
            }
            if (inCar == false)
            {
                Game.Player.Character.Task.TurnTo(p);
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_HOWS_IT_GOING", "SPEECH_PARAMS_STANDARD");
                Wait(1000);
                p.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                Game.Player.Character.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_shopkeeper", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                Wait(2000);
                Game.Player.Money += price;
                c.Ped.Money -= price;
                PlayerInventory[c.Drug.Name] -= c.amount;
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "LOCAL_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", 0);
                UI.Notify("Sold " + c.Drug.Name + " " + c.amount + "g, you got " + PlayerInventory[c.Drug.Name] + "g left.");
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_THANKS", "SPEECH_PARAMS_STANDARD");
                oldCustomers.Add(p);
                if (c.Vehicle != null && c.Vehicle.Exists())
                {
                    c.Ped.Task.PerformSequence(PerformTaskSeq(1, c.Vehicle));
                }
                if (c.FarAway)
                {
                    jobs.Remove(c);
                    RemovePedFromWorld(p, true);
                }
                else
                {
                    customers.Remove(c);
                    if (c.Vehicle != null && c.Vehicle.Exists())
                    {
                        RemovePedFromWorld(p, false);
                    }
                    else
                    {
                        RemovePedFromWorld(p, true);
                    }

                }
                TotalDrugsCarried = 0;
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            else
            {
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_HOWS_IT_GOING", "SPEECH_PARAMS_STANDARD");
                Wait(1000);
                p.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                Wait(3000);
                Game.Player.Money += price;
                c.Ped.Money -= price;
                PlayerInventory[c.Drug.Name] -= c.amount;
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "LOCAL_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", 0);
                UI.Notify("Sold " + c.Drug.Name + " " + c.amount + "g, you got " + PlayerInventory[c.Drug.Name] + "g left.");
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_THANKS", "SPEECH_PARAMS_STANDARD");
                if (c.Vehicle != null && c.Vehicle.Exists())
                {
                    c.Ped.Task.PerformSequence(PerformTaskSeq(1, c.Vehicle));
                }
                else
                {
                    c.Ped.Task.PerformSequence(PerformTaskSeq(0, null));
                }
                oldCustomers.Add(p);
                if (c.FarAway)
                {
                    jobs.Remove(c);
                    RemovePedFromWorld(p, true);
                }
                else
                {
                    customers.Remove(c);
                    if (c.Vehicle != null && c.Vehicle.Exists())
                    {
                        RemovePedFromWorld(p, false);
                    }
                    else
                    {
                        RemovePedFromWorld(p, true);
                    }
                }
                TotalDrugsCarried = 0;
                foreach (KeyValuePair<string, int> entry in PlayerInventory)
                {
                    TotalDrugsCarried += PlayerInventory[entry.Key];
                }
            }
            LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
            foreach (XElement element in LSLifeSave.Descendants())
            {
                if (element.Name == "playerInventory")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weed" && c.Drug.Name == "Weed")
                        {
                            e.Value = PlayerInventory["Weed"].ToString();
                        }
                        if (e.Name == "crack" && c.Drug.Name == "Crack")
                        {
                            e.Value = PlayerInventory["Crack"].ToString();
                        }
                        if (e.Name == "cocain" && c.Drug.Name == "Cocaine")
                        {
                            e.Value = PlayerInventory["Cocaine"].ToString();
                        }
                    }
                }
            }
            if (aDealer != null)
            {
                if (rnd.Next(0, 10 + areas[areaIndex].GangPresance) == 0)
                {
                    UI.ShowSubtitle("Hey thought you might like to know, ~r~someone ~w~else selling over on ~y~" + World.GetStreetName(aDealer.dPed.Position) + "~w~.", 5000);
                    aDealer.dPed.AddBlip();
                    aDealer.Detected = true;
                }
            }
            LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            foreach (Ped pol in police)
            {
                if (Function.Call<bool>(Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY_IN_FRONT, pol, Game.Player.Character))
                {
                    SendTextMsg("They spotted you dealing, your gona be hot round there now.");
                    areas[areaIndex].Heat += 1000;
                    Game.Player.WantedLevel = 1;
                    DispatchAudio.Started = true;
                    break;
                }
            }
            customerSellTo = null;
        }
    }
}
