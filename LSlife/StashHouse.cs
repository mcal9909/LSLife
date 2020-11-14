using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using static GTA.Script;
using static LSlife.Enums;
using static LSlife.LsFunctions;
using static LSlife.LSL;
using Control = GTA.Control;

namespace LSlife
{
    public class StashHouse
    {
        public int id;
        public Vector3 entrancePos { get; private set; }
        public Vector3 exitPos { get; private set; }
        Vector3 stashPos;
        Blip blip;
        bool purchased;
        int price;
        Color color = Color.Yellow;
        public bool inside = false;
        public int Weed = 0;
        public int Crack = 0;
        public int Cocain = 0;
        public int WeedOz = 0;
        public int CrackOz = 0;
        public int CocainOz = 0;
        public int Money = 0;
        public int Armor = 0;
        public List<XElement> Weapons;
        public List<dynamic> Weps;
        public Vector3 houseHealthPickPos;
        public bool pickupSpawned = false;
        public static Pickup healthPickup;


        public StashHouse(int _id, Vector3 _entrancePos, Vector3 _exitPos, Vector3 _stashPos, Vector3 _health, bool _purchased, int _price, bool _inside, int _Weed, int _Crack, int _Cocain, int _Money, int _Armor, List<XElement> _weapons, List<dynamic> _weps)
        {
            id = _id;
            entrancePos = _entrancePos;
            exitPos = _exitPos;
            stashPos = _stashPos;
            purchased = _purchased;
            price = _price;
            blip = World.CreateBlip(entrancePos);
            blip.Sprite = (BlipSprite)40;
            blip.Name = "Stash House";
            SetBlip();
            inside = _inside;
            //if (inside) HouseHandler.Inside = inside;
            Weapons = _weapons;
            Weps = _weps;
            Weed = _Weed;
            Crack = _Crack;
            Cocain = _Cocain;
            Money = _Money;
            Armor = _Armor;
            houseHealthPickPos = _health;

            if (purchased)
            {
                color = Color.Green;

            }
        }

        public void OnTick()
        {
            if (!inside)
            {
                if (pickupSpawned)
                {
                    if (healthPickup != null && healthPickup.ObjectExists())
                    {
                        DbugString("Health Deleted");
                        healthPickup.Delete();
                        pickupSpawned = false;
                    }
                }
                if (DrawMarkers && playerPos.DistanceTo(entrancePos) < 30) World.DrawMarker(MarkerType.VerticalCylinder, entrancePos, Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 1f), color);
                if (player.Character.IsStopped)
                {
                    if (playerPos.DistanceTo(entrancePos) < 2)
                    {
                        if (purchased)
                        {
                            if (Game.GameTime > displayHelpTimer + 3000)
                            {
                                Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                displayHelpTimer = Game.GameTime;
                                DisplayHelpText("Press ~INPUT_CONTEXT~ to enter StashHouse.");

                            }
                            if (Game.IsControlJustReleased(2, Control.Context))
                            {
                                Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                player.Character.FreezePosition = true;
                                foreach (PlayerDealer playerDealer in DealerHandler.dealers)
                                {
                                    if (playerDealer.NewState == PdStates.FollowingPlayer)
                                    {
                                        playerDealer.NewState = PdStates.FollowPlayer;
                                        playerDealer.Ped.LeaveGroup();
                                    }
                                }
                                Game.FadeScreenOut(300);
                                Wait(300);
                                player.Character.Position = exitPos;
                                Wait(400);
                                player.Character.FreezePosition = false;
                                Game.FadeScreenIn(300);
                                inside = true;
                                //HouseHandler.Inside = true;                                
                                SetInside(true);
                            }
                        }
                        else
                        {
                            if (Game.GameTime > displayHelpTimer + 3000)
                            {
                                Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                displayHelpTimer = Game.GameTime;
                                DisplayHelpText("Press ~INPUT_CONTEXT~ To Purchase For $" + IntToMoney(price));
                            }

                            if (Game.IsControlJustReleased(2, Control.Context))
                            {
                                if (player.Money >= price)
                                {
                                    player.Money -= price;
                                    purchased = true;
                                    SetBlip();
                                    SetPurchased(true);
                                }
                                else UI.ShowSubtitle("You do not have enough money to purchase this property");
                            }
                        }
                    }
                }
            }
            else
            {
                if (DrawMarkers) World.DrawMarker(MarkerType.UpsideDownCone, new Vector3(stashPos.X, stashPos.Y, stashPos.Z - 0.5f), Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 0.5f), Color.Green);
                if (pickupSpawned)
                {
                    if (healthPickup != null && !healthPickup.ObjectExists())
                    {
                        DbugString("Health Deleted");
                        healthPickup.Delete();
                        healthPickup = null;
                        pickupSpawned = false;
                    }
                }
                else
                {
                    //UI.Notify("" + houseHealthPickPos);

                    pickupSpawned = true;
                    healthPickup = World.CreatePickup(PickupType.Health, houseHealthPickPos, Vector3.Zero, new Model("prop_ld_health_pack"), 100);
                    DbugString("Spawned Health");

                }


                if (!LsMenuPool.IsAnyMenuOpen() && playerPos.DistanceTo(stashPos) <= 1f)
                {
                    if (Game.GameTime > displayHelpTimer + 3000)
                    {
                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                        displayHelpTimer = Game.GameTime;
                        DisplayHelpText("Press ~INPUT_CONTEXT~ to open stash.");
                    }
                    if (Game.IsControlJustReleased(2, GTA.Control.Context))
                    {
                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                        UpdateInventory();

                        HouseHandler.UpdateMenu(this);
                        HouseHandler.CurrentHouse = this;
                        stashMainMenu.Visible = !stashMainMenu.Visible;
                    }
                }
                if (playerPos.DistanceTo(exitPos) < 2)
                {
                    if (Game.GameTime > displayHelpTimer + 10000)
                    {
                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                        displayHelpTimer = Game.GameTime;
                        DisplayHelpText("Press ~INPUT_CONTEXT~ to exit StashHouse.");

                    }
                    if (Game.IsControlJustReleased(2, Control.Context))
                    {
                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                        player.Character.FreezePosition = true;

                        Game.FadeScreenOut(300);
                        Wait(300);
                        player.Character.Position = entrancePos;
                        Wait(400);
                        player.Character.FreezePosition = false;
                        Game.FadeScreenIn(300);
                        inside = false;
                        //HouseHandler.Inside = false;
                        SetInside(false);
                    }
                }
                if (stashMainMenu.Visible && playerPos.DistanceTo(stashPos) > 1f) stashMainMenu.Visible = false;
            }
        }

        private void SetInside(bool v)
        {
            XDocument save = XDocument.Load("scripts\\LSLife\\LSLife_StashHouses.xml");
            foreach (XElement e in save.Descendants())
            {
                if (e.Name == "house" && e.FirstAttribute.Value == id.ToString())
                {
                    //UI.Notify("Found House");
                    foreach (XElement el in e.Elements())
                    {
                        if (el.Name == "inside")
                        {
                            el.Value = v.ToString();
                            save.Save("scripts\\LSLife\\LSLife_StashHouses.xml");
                            break;
                        }
                    }
                }
            }
        }

        private void SetPurchased(bool v)
        {
            XDocument save = XDocument.Load("scripts\\LSLife\\LSLife_StashHouses.xml");
            foreach (XElement e in save.Descendants())
            {
                if (e.Name == "house" && e.FirstAttribute.Value == id.ToString())
                {
                    //UI.Notify("Found House");
                    foreach (XElement el in e.Elements())
                    {
                        if (el.Name == "purchased")
                        {
                            el.Value = v.ToString();
                            save.Save("scripts\\LSLife\\LSLife_StashHouses.xml");
                            break;
                        }
                    }
                }
            }
        }

        public void RemoveBlip()
        {
            blip.Remove();
        }

        public void SetBlip()
        {
            if (purchased)
            {
                color = Color.Green;
                blip.Color = BlipColor.Green;
            }
            else
            {
                blip.Color = BlipColor.WhiteNotPure;
            }
        }
    }
}
