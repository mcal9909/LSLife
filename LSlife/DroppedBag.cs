using GTA;
using GTA.Math;
using System.Collections.Generic;
using System.Xml.Linq;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class DroppedBag
    {
        int weed;
        int crack;
        int cocaine;
        int money;
        int timeSpawned;
        int timeToLive = 120000;
        List<XElement> weapons = null;
        Vector3 pos;
        Blip blip;
        Pickup pickup;

        public DroppedBag(string _subtitleString, int _weed, int _crack, int _cocaine, int _money, Vector3 _pos)
        {
            weed = _weed;
            crack = _crack;
            cocaine = _cocaine;
            money = _money;
            pos = _pos;
            Innit(_subtitleString);
        }

        public DroppedBag(string _subtitleString, int _weed, int _crack, int _cocaine, int _money, List<XElement> _weapons, Vector3 _pos)
        {
            weed = _weed;
            crack = _crack;
            cocaine = _cocaine;
            money = _money;
            weapons = _weapons;
            pos = _pos;
            Innit(_subtitleString);
        }

        void Innit(string _subtitle)
        {
            CreateBlip();
            CreatePickup();
            UI.ShowSubtitle(_subtitle, 5000);
            timeSpawned = Game.GameTime;
        }

        public bool CheckIfPickedUp()
        {
            if (!pickup.ObjectExists())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddStuffToPlayer()
        {
            PlayerInventory["Weed"] += weed;
            PlayerInventory["Crack"] += crack;
            PlayerInventory["Cocaine"] += cocaine;
            Game.Player.Money += money;
            if (weapons != null) GivePedWeapons(Game.Player.Character, weapons);
            UI.ShowSubtitle("You picked up " + GramsToOz(weed) + " Weed, " + GramsToOz(crack) + " Crack, " + GramsToOz(cocaine) + " Cocaine and $~g~" + money, 5000);
            UpdateInventory();
        }

        void CreateBlip()
        {
            blip = World.CreateBlip(pos);
            blip.Color = BlipColor.Green;
            blip.Name = "Bag";
            blip.IsShortRange = true;
        }

        void RemoveBlip()
        {
            blip.Remove();
        }

        public bool CanDespawn()
        {
            if (Game.GameTime > timeSpawned + timeToLive) { return true; }
            else return false;
        }

        public void RemovePickup()
        {
            pickup.Delete();
            RemoveBlip();
            if (DEBUG) DbugString("Pickup Deleted");
        }

        void CreatePickup()
        {
            pickup = World.CreatePickup(PickupType.CustomScript, pos, new Model("prop_big_bag_01"), 0);
            while (pickup == null)
            {
                Script.Wait(0);
            }
        }
    }
}
