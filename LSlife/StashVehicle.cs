using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using static GTA.Script;
using static LSlife.LsFunctions;
using static LSlife.LSL;
using Control = GTA.Control;

namespace LSlife
{
    public class StashVehicle
    {
        public bool drugCarWanted = false;
        public bool drugCarDestroyed = true;
        bool drugCarSaved = true;
        public Blip drugCarBlip = null;
        public int drugCarWeed;
        public int drugCarWeedOz;
        public int drugCarCrack;
        public int drugCarCrackOz;
        public int drugCarCocain;
        public int drugCarCocainOz;
        public int drugCarMoney;
        public int drugCarArmour = 0;
        public Vehicle drugCar = null;
        public Vector3 CurrentPosition;
        string reg;
        public List<XElement> vehWeapons = new List<XElement>();
        public List<dynamic> vehWeps = new List<dynamic>();

        public StashVehicle()
        {
            // Load car drugs.
            foreach (XElement element in LSL.LSLifeSave.Descendants())
            {
                if (element.Name == "drugCarStash")
                {
                    bool armourFound = false;
                    bool foundOz = false;
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weed")
                        {
                            Int32.TryParse(e.Value, out drugCarWeed);
                        }
                        if (e.Name == "weedOz")
                        {
                            Int32.TryParse(e.Value, out drugCarWeedOz);
                            foundOz = true;
                        }
                        if (e.Name == "crack")
                        {
                            Int32.TryParse(e.Value, out drugCarCrack);
                        }
                        if (e.Name == "crackOz")
                        {
                            Int32.TryParse(e.Value, out drugCarCrackOz);
                            foundOz = true;
                        }
                        if (e.Name == "cocain")
                        {
                            Int32.TryParse(e.Value, out drugCarCocain);
                        }
                        if (e.Name == "cocainOz")
                        {
                            Int32.TryParse(e.Value, out drugCarCocainOz);
                            foundOz = true;
                        }
                        if (e.Name == "money")
                        {
                            Int32.TryParse(e.Value, out drugCarMoney);
                        }
                        if (e.Name == "armour")
                        {
                            armourFound = true;
                            int.TryParse(e.Value, out drugCarArmour);
                            //UI.Notify("dArmour=" + drugCarArmour);
                        }
                    }
                    if (!armourFound)
                    {
                        element.Add(new XElement("armour", 0));
                        LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                    }
                    if (!foundOz)
                    {
                        element.Add(new XElement("weedOz", 0));
                        element.Add(new XElement("crackOz", 0));
                        element.Add(new XElement("cocainOz", 0));
                        LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                    }
                }
            }
            // load car weapons data.
            foreach (XElement element in VehicleXml.Descendants())
            {
                if (element.Name == "Vehicle")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "Destroyed")
                        {
                            bool.TryParse(e.Value, out drugCarDestroyed);
                        }
                        if (e.Name == "PosX")
                        {
                            float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out CurrentPosition.X);
                            if (CurrentPosition.X == 0) float.TryParse(e.Value, out CurrentPosition.X);
                        }
                        if (e.Name == "PosY")
                        {
                            float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out CurrentPosition.Y);
                            if (CurrentPosition.Y == 0) float.TryParse(e.Value, out CurrentPosition.Y);
                        }
                        if (e.Name == "PosZ")
                        {
                            float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out CurrentPosition.Z);
                            if (CurrentPosition.Z == 0) float.TryParse(e.Value, out CurrentPosition.Z);
                        }
                        if (e.Name == "NumPlate")
                        {
                            reg = e.Value;
                        }
                    }
                }
            }
            // load car data.
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

        public void OnTick()
        {   // EveryTick
            if (drugCar != null && drugCar.Exists())
            {
                Vector3 backPos = Vector3.Add(drugCar.Position, Vector3.Multiply(drugCar.ForwardVector * -1, (drugCar.Model.GetDimensions().Y / 2) + 0.5f));

                if (DrawMarkers && drugCar.Driver != player.Character && playerPos.DistanceTo(drugCar.Position) < 10)
                {
                    World.DrawMarker(MarkerType.UpsideDownCone, backPos, Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 0.5f), Color.Green);
                }
                if (drugCar.CurrentBlip.IsShortRange == false)
                {
                    drugCar.CurrentBlip.IsShortRange = true;
                    drugCar.CurrentBlip.Sprite = BlipSprite.PersonalVehicleCar;
                    drugCar.CurrentBlip.Name = "Drug Car";
                    drugCar.CurrentBlip.Color = BlipColor.Green;
                }
                if (!player.Character.IsInVehicle() && player.Character.Position.DistanceTo(backPos) <= 1f && drugCar.Model.IsCar)
                {
                    bool trunkFound = false;
                    if (drugCar.HasBone("boot")) { trunkFound = true; }
                    if (trunkFound)
                    {
                        if (!drugCar.IsDoorOpen(VehicleDoor.Trunk))
                        {
                            if (Game.GameTime > displayHelpTimer + 5000 && !LsMenuPool.IsAnyMenuOpen() && player.Character.IsStopped)
                            {
                                displayHelpTimer = Game.GameTime;
                                LsFunctions.DisplayHelpText("~INPUT_SPRINT~ to open trunk.");

                            }
                            if (Game.IsControlJustPressed(2, Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && player.Character.IsStopped)
                            {
                                drugCar.OpenDoor(VehicleDoor.Trunk, false, false);
                                if (PlayerInventory.Count > 0)
                                {
                                    putWeedVeh.Maximum = PlayerInventory["Weed"];
                                    putCrackVeh.Maximum = PlayerInventory["Crack"];
                                    putCocainVeh.Maximum = PlayerInventory["Cocaine"];
                                }
                                putMoneyVeh.Maximum = player.Money;
                                takeWeaponVeh.Items = vehWeps;
                                takeMoneyVeh.Maximum = drugCarMoney;
                                putArmourVeh.Items = new List<dynamic>();
                                for (int t = 0; t < playerArmours + 1; t++)
                                {
                                    putArmourVeh.Items.Add(t);
                                }
                                takeArmourVeh.Items = new List<dynamic>();
                                for (int t = 0; t < drugCarArmour + 1; t++)
                                {
                                    takeArmourVeh.Items.Add(t);
                                }
                                UpdateCarStash();
                                vehStashMainMenu.Visible = !vehStashMainMenu.Visible;
                            }
                        }
                        else
                        {
                            if (Game.GameTime > displayHelpTimer + 5000 && !LsMenuPool.IsAnyMenuOpen() && player.Character.IsStopped)
                            {
                                displayHelpTimer = Game.GameTime;
                                DisplayHelpText("~INPUT_SPRINT~ to close trunk.");
                            }
                            if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && player.Character.IsStopped)
                            {
                                drugCar.CloseDoor(VehicleDoor.Trunk, false);
                            }
                        }
                    }
                    else
                    {
                        if (!drugCar.IsDoorOpen(VehicleDoor.BackLeftDoor) && !LsMenuPool.IsAnyMenuOpen() && player.Character.IsStopped)
                        {
                            if (Game.GameTime > displayHelpTimer + 3000)
                            {
                                displayHelpTimer = Game.GameTime;
                                LsFunctions.DisplayHelpText("~INPUT_SPRINT~ to back doors.");

                            }
                            if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && player.Character.IsStopped)
                            {
                                drugCar.OpenDoor(VehicleDoor.BackLeftDoor, true, false);
                                drugCar.OpenDoor(VehicleDoor.BackRightDoor, true, false);

                                if (PlayerInventory.Count > 0)
                                {
                                    putWeedVeh.Maximum = PlayerInventory["Weed"];
                                    putCrackVeh.Maximum = PlayerInventory["Crack"];
                                    putCocainVeh.Maximum = PlayerInventory["Cocaine"];
                                }
                                putMoneyVeh.Maximum = player.Money;
                                takeWeaponVeh.Items = vehWeps;
                                takeMoneyVeh.Maximum = drugCarMoney;
                                putArmourVeh.Items = new List<dynamic>();
                                for (int t = 0; t < playerArmours + 1; t++)
                                {
                                    putArmourVeh.Items.Add(t);
                                }
                                takeArmourVeh.Items = new List<dynamic>();
                                for (int t = 0; t < drugCarArmour + 1; t++)
                                {
                                    takeArmourVeh.Items.Add(t);
                                }
                                UpdateCarStash();
                                vehStashMainMenu.Visible = !vehStashMainMenu.Visible;
                            }
                        }
                        else
                        {
                            if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen() && player.Character.IsStopped)
                            {
                                displayHelpTimer = Game.GameTime;
                                DisplayHelpText("~INPUT_SPRINT~ to close back doors.");
                            }
                            if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && player.Character.IsStopped)
                            {
                                drugCar.CloseDoor(VehicleDoor.BackLeftDoor, false);
                                drugCar.CloseDoor(VehicleDoor.BackRightDoor, false);
                            }
                        }
                    }
                }
            }

        }

        public void OnDelayedTick()
        {
            //check if we need to spawn a car
            if (drugCarDestroyed == false)
            {
                if (drugCar == null || !drugCar.Exists())
                {
                    if (playerPos.DistanceTo(CurrentPosition) <= 200)
                    {
                        if (drugCarBlip != null)
                        {
                            drugCarBlip.Remove();
                            drugCarBlip = null;
                        }
                        var vehicles = World.GetAllVehicles();
                        foreach (Vehicle veh in vehicles)
                        {

                            if (veh.Position.DistanceTo(CurrentPosition) < 6 && veh.NumberPlate == reg)
                            {
                                if (veh.CurrentBlip != null)
                                {
                                    veh.CurrentBlip.Remove();
                                }
                                drugCar = veh;
                                drugCar.IsPersistent = true;
                                drugCar.EngineHealth = 1000;
                                drugCar.Health = 1000;
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
                            }
                        }
                        if (drugCar == null || !drugCar.Exists())
                        {
                            if (DEBUG) { UI.Notify("Trying to load vehicle"); }
                            VehicleLoad();
                        }
                    }
                    else if (drugCarBlip == null)
                    {
                        drugCarBlip = World.CreateBlip(CurrentPosition);
                        drugCarBlip.IsShortRange = true;
                        drugCarBlip.IsFriendly = true;
                        drugCarBlip.Sprite = BlipSprite.PersonalVehicleCar;
                        drugCarBlip.Name = "Drug Car";
                        drugCarBlip.Color = BlipColor.Green;
                    }
                }

            }
            if (drugCar != null && drugCar.Exists())
            {
                if (drugCar.CurrentBlip.Exists())
                {
                    if (drugCar.Driver == player.Character)
                    {
                        drugCar.CurrentBlip.Remove();
                    }
                }
                if (player.Character.IsInVehicle())
                {
                    if (playerWanted && drugCar.Handle == player.Character.CurrentVehicle.Handle && drugCarWanted == false)
                    {
                        drugCarWanted = true;
                    }
                    if (drugCarSaved == true && drugCar.Driver == player.Character)
                    {
                        CurrentPosition = drugCar.Position;
                        drugCarSaved = false;
                        if (drugCar.IsInvincible == true)
                        {
                            drugCar.IsInvincible = false;
                        }
                    }
                }
                if (!playerWanted && drugCarWanted)
                {
                    drugCarWanted = false;
                }

                if (drugCarSaved == false && CurrentPosition != drugCar.Position && !player.Character.IsInVehicle())
                {
                    CurrentPosition = drugCar.Position;
                    VehicleSave(drugCar);
                    drugCarSaved = true;
                }
                if (drugCar.IsDead || drugCar.EngineHealth <= -500)
                {
                    DbugString("Vehicle Destroyed");
                    drugCarWanted = false;
                    drugCar.IsPersistent = false;
                    drugCar.MarkAsNoLongerNeeded();
                    drugCar.CurrentBlip.Remove();
                    drugCar = null;
                    drugCarDestroyed = true;
                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "drugCarStash")
                        {
                            foreach (XElement carStashEle in element.Elements())
                            {
                                if (carStashEle.Name == "weed")
                                {
                                    carStashEle.Value = "0";
                                }
                                if (carStashEle.Name == "crack")
                                {
                                    carStashEle.Value = "0";
                                }
                                if (carStashEle.Name == "cocain")
                                {
                                    carStashEle.Value = "0";
                                }
                                if (carStashEle.Name == "money")
                                {
                                    carStashEle.Value = "0";
                                }
                            }
                        }
                    }
                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                    XDocument veh = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
                    foreach (XElement element in veh.Descendants())
                    {
                        if (element.Name == "Destroyed")
                        {
                            element.Value = drugCarDestroyed.ToString();
                        }
                    }
                    veh.Save("scripts\\LSLife\\LSLife_Vehicles.xml");
                }

                if (drugCarDestroyed == false && drugCar != null && playerPos.DistanceTo(CurrentPosition) > 200)
                {
                    if (drugCar.Exists())
                    {
                        if (player.Character.IsInVehicle() && player.Character.CurrentVehicle != drugCar)
                        {
                            if (drugCarBlip == null)
                            {
                                drugCarBlip = World.CreateBlip(drugCar.Position);
                                drugCarBlip.IsShortRange = true;
                                drugCarBlip.IsFriendly = true;
                                drugCarBlip.Sprite = BlipSprite.PersonalVehicleCar;
                                drugCarBlip.Name = "Drug Car";
                                drugCarBlip.Color = BlipColor.Green;
                            }

                            drugCar.CurrentBlip.Remove();
                            drugCar.Delete();
                            drugCar = null;

                        }
                        if (!player.Character.IsInVehicle())
                        {
                            if (drugCarBlip == null)
                            {
                                drugCarBlip = World.CreateBlip(drugCar.Position);
                                drugCarBlip.IsShortRange = true;
                                drugCarBlip.IsFriendly = true;
                                drugCarBlip.Sprite = BlipSprite.PersonalVehicleCar;
                                drugCarBlip.Name = "Drug Car";
                                drugCarBlip.Color = BlipColor.Green;
                            }
                            drugCar.CurrentBlip.Remove();
                            drugCar.Delete();
                            drugCar = null;

                        }
                    }
                }

            }
        }

        public int TotalDrugs()
        {
            return drugCarWeed + drugCarCrack + drugCarCocain;
        }

        public void SpawnCar()
        {
            if (drugCar == null)
            {
                VehicleLoad();
            }
        }

        public void RemoveOldCar()
        {
            drugCarDestroyed = true;
            if (drugCarBlip != null)
            {
                drugCarBlip.Remove();
            }
            drugCar.IsPersistent = false;
            drugCar.MarkAsNoLongerNeeded();
            if (drugCar.CurrentBlip != null)
            {
                drugCar.CurrentBlip.Remove();
            }
            XDocument veh = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
            foreach (XElement element in veh.Descendants())
            {
                if (element.Name == "Destroyed")
                {
                    element.Value = drugCarDestroyed.ToString();
                }
            }
            veh.Save("scripts\\LSLife\\LSLife_Vehicles.xml");
            drugCar = null;

        }

        public void ImpoundCar()
        {
            if (drugCarWanted && drugCar != null)
            {
                SendTextMsg("Storage Vehicle has been confiscated, and its contents destroyed");
                drugCarDestroyed = true;
                XDocument veh = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
                foreach (XElement element in veh.Descendants())
                {
                    if (element.Name == "Destroyed")
                    {
                        element.Value = drugCarDestroyed.ToString();
                        break;
                    }
                }
                veh.Save("scripts\\LSLife\\LSLife_Vehicles.xml");
                drugCarWanted = false;
                drugCar.IsPersistent = false;
                drugCar.MarkAsNoLongerNeeded();
                drugCar.CurrentBlip.Remove();
                drugCar.Delete();
                drugCar = null;
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "drugCar")
                    {
                        foreach (XElement drugCarEle in element.Elements())
                        {
                            if (drugCarEle.Name == "HASH")
                            {
                                drugCarEle.Value = "";
                                break;
                            }
                        }
                    }

                    if (element.Name == "drugCarStash")
                    {
                        foreach (XElement carStashEle in element.Elements())
                        {
                            if (carStashEle.Name == "weed")
                            {

                                carStashEle.Value = "0";
                            }
                            if (carStashEle.Name == "crack")
                            {
                                carStashEle.Value = "0";
                            }
                            if (carStashEle.Name == "cocain")
                            {
                                carStashEle.Value = "0";
                            }
                            if (carStashEle.Name == "money")
                            {
                                carStashEle.Value = "0";
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }
        }

        void VehicleLoad()
        {
            string drugCarName = "";
            int drugCarHash = -1;
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
            int livery = -1;
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
                            drugCarName = e.Value;
                        }
                        if (e.Name == "Hash")
                        {
                            int.TryParse(e.Value, out drugCarHash);
                        }
                        if (e.Name == "PosX")
                        {

                            float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float x);
                            if (x == 0) float.TryParse(e.Value, out x);
                            if (x != 0) CurrentPosition.X = x;
                        }
                        if (e.Name == "PosY")
                        {
                            float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float y);
                            if (y == 0) float.TryParse(e.Value, out y);
                            if (y != 0) CurrentPosition.Y = y;
                        }
                        if (e.Name == "PosZ")
                        {
                            float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float z);
                            if (z == 0) float.TryParse(e.Value, out z);
                            if (z != 0) CurrentPosition.Z = z;
                        }
                        if (e.Name == "Heading")
                        {
                            float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out drugCarHead);
                            if (drugCarHead == 0) float.TryParse(e.Value, out drugCarHead);
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
                            reg = plate;
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
                        if (e.Name == "Livery") int.TryParse(e.Value, out livery);
                        if (e.Name == "CustomTire")
                        {
                            bool.TryParse(e.Value, out customTire);
                        }
                    }
                }
            }
            Model model = new Model(drugCarName);
            var vehicles = World.GetAllVehicles().ToList().FindAll(v => v.NumberPlate == reg);
            if (DEBUG)
            {
                UI.Notify("" + vehicles.Count);
                UI.Notify("~r~expected cord:" + CurrentPosition);
            }

            if (vehicles.Count == 0)
            {
                if (model.IsInCdImage)
                {
                    drugCar = World.CreateVehicle(new Model(drugCarName), CurrentPosition, drugCarHead);
                }
                else
                {
                    model = new Model(drugCarHash);
                    if (model.IsInCdImage)
                    {
                        drugCar = World.CreateVehicle(model, CurrentPosition, drugCarHead);
                    }
                }
            }
            else drugCar = vehicles[0];
            Wait(5);
            if (DEBUG)
            {
                UI.Notify("" + drugCarName + " | " + CurrentPosition);
            }
            if (drugCar != null)
            {
                drugCar.InstallModKit();
                drugCar.PrimaryColor = drugCarPriCol;
                drugCar.SecondaryColor = drugCarSecCol;
                drugCar.Health = health;
                drugCar.EngineHealth = engineHealth;
                drugCar.WheelType = wheelType;
                drugCar.EngineRunning = false;
                drugCar.IsInvincible = false;
                drugCar.CanTiresBurst = canTiresBurst;
                drugCar.NumberPlateType = plateType;
                drugCar.NumberPlate = plate;
                drugCar.WindowTint = tint;
                drugCar.DirtLevel = dirt;
                drugCar.FuelLevel = fuel;
                drugCar.Livery = livery;
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
            else UI.Notify("~r~Failed to load vehicle");
        }

        public void VehicleSave(Vehicle veh)
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
                parentElement.Add(new XElement("Hash", veh.Model.Hash));
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
                parentElement.Add(new XElement("Livery", veh.Livery));
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
                drugCarDestroyed = false;
                CurrentPosition = drugCar.Position;
            }
        }

        public void UpdateCarStash()
        {
            vehStashPutMenu.FormatDescriptions = true;
            vehStashTakeMenu.FormatDescriptions = true;
            takeWeedVeh.Maximum = drugCarWeed;
            takeCrackVeh.Maximum = drugCarCrack;
            takeCocainVeh.Maximum = drugCarCocain;


            string sDescription = "Grams of Weed. Ready to be sold on the streets or given to a worker.";
            putWeedVeh.Description = sDescription;
            takeWeedVeh.Description = sDescription;

            sDescription = "Grams of Crack. Ready to be sold on the streets or given to a worker.";
            putCrackVeh.Description = sDescription;
            takeCrackVeh.Description = sDescription;

            sDescription = "Grams of Cocaine. Ready to be sold on the streets or given to a worker.";
            putCocainVeh.Description = sDescription;
            takeCocainVeh.Description = sDescription;


            putWeedVeh.Text = "Weed " + GramsToOz(PlayerInventory["Weed"]);
            takeWeedVeh.Text = "Weed " + GramsToOz(drugCarWeed);
            putCrackVeh.Text = "Crack " + GramsToOz(PlayerInventory["Crack"]);
            takeCrackVeh.Text = "Crack " + GramsToOz(drugCarCrack);
            putCocainVeh.Text = "Cocaine " + GramsToOz(PlayerInventory["Cocaine"]);
            takeCocainVeh.Text = "Cocaine " + GramsToOz(drugCarCocain);

            takeWeedOzVeh.Maximum = drugCarWeedOz;
            takeCrackOzVeh.Maximum = drugCarCrackOz;
            takeCocainOzVeh.Maximum = drugCarCocainOz;
            putWeedOzVeh.Maximum = pWeedOunce;
            putCrackOzVeh.Maximum = pCrackOunce;
            putCocainOzVeh.Maximum = pCocaineOunce;

            sDescription = "Oz packs of Weed. Must be deposited at a Stash house before it can be sold.";
            putWeedOzVeh.Description = sDescription;
            takeWeedOzVeh.Description = sDescription;

            sDescription = "Oz packs of Crack. Must be deposited at a Stash house before it can be sold.";
            putCrackOzVeh.Description = sDescription;
            takeCrackOzVeh.Description = sDescription;

            sDescription = "Oz packs of Cocaine. Must be deposited at a Stash house before it can be sold.";
            putCocainOzVeh.Description = sDescription;
            takeCocainOzVeh.Description = sDescription;

            putWeedOzVeh.Text = "Weed ~r~" + pWeedOunce + "~s~z";
            takeWeedOzVeh.Text = "Weed ~r~" + drugCarWeedOz + "~s~z";
            putCrackOzVeh.Text = "Crack ~r~" + pCrackOunce + "~s~z";
            takeCrackOzVeh.Text = "Crack ~r~" + drugCarCrackOz + "~s~z";
            putCocainOzVeh.Text = "Cocaine ~r~" + pCocaineOunce + "~s~z";
            takeCocainOzVeh.Text = "Cocaine ~r~" + drugCarCocainOz + "~s~z";

            putMoneyVeh.Text = "Money $" + IntToMoney(player.Money);
            takeMoneyVeh.Text = "Money $" + IntToMoney(drugCarMoney);
            takeMoneyStash.Maximum = drugCarMoney;
            if (DEBUG) DbugString("dmon:" + drugCarMoney + " max:" + takeMoneyVeh.Maximum);
            takeMoneyVeh.Multiplier = 1;
            playerWeapons.Clear();
            playerWeps.Clear();
            int i = 0;
            foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
            {
                bool hasPedGotWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, player.Character, (int)weapon);
                int ammunitionInWeapon = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, player.Character, (int)weapon);
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
                        bool hasWeaponGotComponent = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, player.Character, (int)weapon, (int)component);
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
    }
}
