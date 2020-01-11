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
    public class LSL : Script
    {
        bool DEBUG = true;
        bool started = false;
        bool EnableBlips;
        ScriptSettings iniSettings;

        //UIStuff
        MenuPool LsMenuPool;
        //DEALER MENU
        UIMenu dealerMainMenu;
        UIMenu weedMenu;
        UIMenuSliderItem bushWeed;
        UIMenu crackMenu;
        UIMenuSliderItem crack;
        UIMenu cocainMenu;
        UIMenuSliderItem cocain;
        UIMenuItem WeedBuy;
        UIMenuItem CrackBuy;
        UIMenuItem CocainBuy;
        UIMenuItem payDebt;
        UIMenuItem dismissDealer;
        //STASH MENU
        UIMenu stashMainMenu;
        UIMenu stashTakeMenu;
        UIMenu stashPutMenu;
        UIMenuSliderItem putWeedStash;
        UIMenuSliderItem takeWeedStash;
        UIMenuSliderItem putCrackStash;
        UIMenuSliderItem takeCrackStash;
        UIMenuSliderItem putCocainStash;
        UIMenuSliderItem takeCocainStash;
        UIMenuSliderItem putMoneyStash;
        UIMenuSliderItem takeMoneyStash;
        UIMenuListItem putWeaponStash;
        UIMenuListItem takeWeaponStash;
        UIMenuListItem stashPutMultiplier;
        UIMenuListItem stashTakeMultiplier;

        //Vehicle stash MENU
        UIMenu vehStashMainMenu;
        UIMenu vehStashTakeMenu;
        UIMenu vehStashPutMenu;
        UIMenuSliderItem putWeedVeh;
        UIMenuSliderItem takeWeedVeh;
        UIMenuSliderItem putCrackVeh;
        UIMenuSliderItem takeCrackVeh;
        UIMenuSliderItem putCocainVeh;
        UIMenuSliderItem takeCocainVeh;
        UIMenuSliderItem putMoneyVeh;
        UIMenuSliderItem takeMoneyVeh;
        UIMenuListItem putWeaponVeh;
        UIMenuListItem takeWeaponVeh;
        UIMenuListItem vehPutMultiplier;
        UIMenuListItem vehTakeMultiplier;

        List<dynamic> multiplier = new List<dynamic>() { { 1 }, { 10 }, { 100 }, { 1000 }, { 10000 }, { 100000 } };

        //Customer Menu
        UIMenu customerMainMenu;
        UIMenuItem sell;
        UIMenuItem turnAway;       

        // TIMER BAR STUFF
        TimerBarPool timerBarPool = new TimerBarPool();
        BarTimerBar heatBar;

        //Main menu and options 
        Keys MenuKey;
        Keys StopDealing;
        Keys SetDrugCar;

        //XML STUFF
        XDocument vehWeaponsXML;
        XDocument stashWeaponsXML;
        XDocument LSLifeSave;




        float displayHelpTimer = Game.GameTime;
        
        int day;
        int hour;
        int dayUpdateTimer;


        //Ifruit stuff
        CustomiFruit iFruit;

        // Dealers
        //DealerIntro dealerIntro1;
        Ped drugDealer1;
        int dealerTimer;
        Blip stashBlip;
        bool readyToDeal;
        bool dealer1Angry;
        int weed1Price = 1000;
        int crack1Price = 5000;
        int cocain1Price = 10000;
        int dealer1Weed;
        int dealer1Crack;
        int dealer1Cocain;
        int dealer1Money;
        int dealer1ReloadDay;
        bool dealer1ReloadOrder = false;
        //int dealer1RepLevel;
        int dealer1ReloadHour = 20;
        bool dealer1Reloaded = false;
        int dealer1Debt;


        // player stuff
        bool removedStuff = true;
        bool isAtHome = false;
        bool isDealing = false;
        bool chilling = false;
        Player player = Game.Player;
        Ped playerPed = Game.Player.Character;
        Dictionary<string, int> areas = new Dictionary<string, int>()
            {
                { "ALAMO"    ,1  },
                {"ALTA"      ,10 },
                {"BHAMCA"    ,1  },
                {"BANHAMC"   ,8  },
                {"BANNING"   ,1  },
                {"JAIL"      ,10 },
                {"BRADP"     ,1  },
                {"BURTON"    ,10 },
                {"CALAFB"    ,0  },
                {"CCREAK"    ,0  },
                {"CHAMH"     ,1  },
                {"CMSW"      ,0  },
                {"CHU"       ,8  },
                {"CYPRE"     ,1  },
                {"DAVIS"     ,1  },
                {"ZQ_UAR"    ,0  },
                {"DELPE"     ,10 },
                {"DELBE"     ,10 },
                {"DOWNT"     ,10 },
                {"DTVINE"    ,10 },
                {"EAST_V"    ,5  },
                {"EBURO"     ,0  },
                {"ELGORL"    ,0  },
                {"ELYSIAN"   ,0  },
                {"ARMYB"     ,10 },
                {"GALFISH"   ,0  },
                {"DESRT"     ,0  },
                {"GRAPES"    ,1  },
                {"GREATC"    ,8  },
                {"golf"      ,10 },
                {"HARMO"     ,1  },
                {"HAWICK"    ,9  },
                {"HUMLAB"    ,10 },
                {"LMESA"     ,2  },
                {"LOSPUER"   ,1  },
                {"LAGO"      ,0  },
                {"LDAM"      ,0  },
                {"LACT"      ,0  },
                {"LEGSQU"    ,8  },
                {"KOREAT"    ,7  },
                {"AIRP"      ,10 },
                {"STAD"      ,5  },
                {"MIRR"      ,4  },
                {"SKID"      ,1  },
                {"MORN"      ,9  },
                {"MTCHIL"    ,0  },
                {"MTGORDO"   ,0  },
                {"MTJOSE"    ,0  },
                {"MURRI"     ,2  },
                {"NOOSE"     ,10 },
                {"NCHU"      ,0  },
                {"PBLUFF"    ,5  },
                {"OCEANA"    ,0  },
                {"PALETO"    ,1  },
                {"PALCOV"    ,0  },
                {"PALFOR"    ,1  },
                {"PALMPOW"   ,5  },
                {"PALHIGH"   ,0  },
                {"PBOX"      ,8  },
                {"ZP_ORT"    ,2  },
                {"PROCOB"    ,1  },
                {"DELSOL"    ,9  },
                {"RANCHO"    ,1  },
                {"CANNY"     ,0  },
                {"RTRAK"     ,1  },
                {"MOVIE"     ,10 },
                {"RICHM"     ,10 },
                {"RGLEN"     ,10 },
                {"ROCKF"     ,10 },
                {"WINDF"     ,0  },
                {"SanAnd"    ,0  },
                {"SANCHIA"   ,0  },
                {"SANDY"     ,2  },
                {"SLAB"      ,1  },
                {"STRAW"     ,2  },
                {"TATAMO"    ,0  },
                {"TERMINA"   ,2  },
                {"TEXTI"     ,7  },
                {"TONGVAH"   ,9  },
                {"TONGVAV"   ,9  },
                {"VESP"      ,8  },
                {"BEACH"     ,8  },
                {"VCANA"     ,7  },
                {"VINE"      ,10 },
                {"CHIL"      ,9  },
                {"HORS"      ,8  },
                {"WVINE"     ,10 },
                {"ZANCUDO"   ,0  } };
        Random rnd = new Random();
        Dictionary<string, int> PlayerInventory = new Dictionary<string, int>();
        int TotalDrugsCarried = 0;
        bool playerWanted = false;
        Vehicle drugCar = null;
        bool drugCarWanted = false;
        bool notFined = true;
        bool playerDead = false;
        List<dynamic> playerWeps = new List<dynamic>();
        List<XElement> playerWeapons = new List<XElement>();
        bool playerHasBag = false;

        bool drugCarDestroyed = true;
        bool drugCarSpawned = false;
        bool drugCarSaved = true;
        Blip drugCarBlip = null;
        int drugCarWeed;
        int drugCarCrack;
        int drugCarCocain;
        int drugCarMoney;

        List<XElement> vehWeapons = new List<XElement>();
        List<dynamic> vehWeps = new List<dynamic>();

        //Player Dealing
        PedGroup playerGroup;
        int timeStartedDealing;
        int timeDealing = 0;
        Vector3 pedTarget;
        Ped attacker;
        bool attackerAttacking = false;
        bool attackerPickupSpawned = false;
        Pickup attackerPickup;
        int rayTimer = 0;
        List<XElement> attackerWeapons = new List<XElement>();
        Dictionary<string, int> attackerInventory = new Dictionary<string, int>();
        List<Ped> oldCustomers = new List<Ped>();
        List<Ped> angryPeds = new List<Ped>();
        List<Customer> customers = new List<Customer>();
        List<Customer> jobs = new List<Customer>();
        string thisStreet;
        string lastStreet;
        float lastTimePedSelected = Game.GameTime;
        float lastTimeDealDone = Game.GameTime;
        float lastAngryTime = Game.GameTime;
        float ratTimer = 0;
        int ratChance;
        int rat;
        int lastCustomerCheckTimer;
        Customer customerSellTo;
        int jobOfferTimer;
        int jobAcceptTimer;
        bool jobOffer = false;
        bool jobAccepted = false;
        int wealth = 0;
        bool firstRun = true;



        //player housing
        Vector3 houseTeleIn = new Vector3(151.36f, -1007.56f, -99f);
        Vector3 houseTeleOut = new Vector3(-112.616f, -1479.229f, 37f);
        Vector3 houseStash = new Vector3(151.9f, -1001.4f, -99f);        
        int stashWeed;
        int stashCrack;
        int stashCocain;
        int stashMoney;
        List<XElement> stashWeapons = new List<XElement>();
        List<dynamic> stashWeps = new List<dynamic>();
        Vector3 houseHealthPickPos = new Vector3(154.6751f, -1007.43f, -99.2f);
        bool pickupSpawned = false;
        Pickup healthPickup;


        //WEAPONS DEALER STUFF
        Dealer wepDealer;
        UIText myUIText;


        // Main Loop
        public LSL()
        {
            Setup();

            Tick += onTick;
            KeyDown += onKeyDown;

            Interval = 2;

            //blips
            if (EnableBlips)
            {
                stashBlip = World.CreateBlip(new Vector3(-112.616f, -1479.229f, 30f));                
                Function.Call(Hash.SET_BLIP_SPRITE, stashBlip, 40);
                stashBlip.Color = BlipColor.Green;
                stashBlip.IsShortRange = true;
                stashBlip.Name = "Stash house";
            }
        }


        void Setup()
        {

            iniSettings = ScriptSettings.Load("scripts\\LSlife.ini");                                    
            DEBUG = iniSettings.GetValue<bool>("OPTIONS", "DEBUG", false);
            LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");

            foreach (XElement element in LSLifeSave.Descendants())
            {
                if (element.Name == "playerInventory")
                {
                    int number;
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weed")
                        {
                            Int32.TryParse(e.Value, out number);
                            PlayerInventory.Add("Weed", number);
                        }
                        if (e.Name == "crack")
                        {
                            Int32.TryParse(e.Value, out number);
                            PlayerInventory.Add("Crack", number);
                        }
                        if (e.Name == "cocain")
                        {
                            Int32.TryParse(e.Value, out number);
                            PlayerInventory.Add("Cocain", number);
                        }
                    }
                }
                if (element.Name == "playerStash")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weed")
                        {
                            Int32.TryParse(e.Value, out stashWeed);
                        }
                        if (e.Name == "crack")
                        {
                            Int32.TryParse(e.Value, out stashCrack);
                        }
                        if (e.Name == "cocain")
                        {
                            Int32.TryParse(e.Value, out stashCocain);
                        }
                        if (e.Name == "money")
                        {
                            Int32.TryParse(e.Value, out stashMoney);

                        }
                    }
                }
                if (element.Name == "drugCarStash")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weed")
                        {
                            Int32.TryParse(e.Value, out drugCarWeed);
                        }
                        if (e.Name == "crack")
                        {
                            Int32.TryParse(e.Value, out drugCarCrack);
                        }
                        if (e.Name == "cocain")
                        {
                            Int32.TryParse(e.Value, out drugCarCocain);
                        }
                        if (e.Name == "money")
                        {
                            Int32.TryParse(e.Value, out drugCarMoney);
                        }
                    }
                }
                if (element.Name == "dealer1")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "debt")
                        {
                            Int32.TryParse(e.Value, out dealer1Debt);
                            if (dealer1Debt > 100000)
                            {
                                dealer1Debt = 100000;
                                dealer1Angry = true;
                            }
                            else
                            {
                                dealer1Angry = false;
                            }
                        }
                        if (e.Name == "weed")
                        {
                            Int32.TryParse(e.Value, out dealer1Weed);
                        }
                        if (e.Name == "crack")
                        {
                            Int32.TryParse(e.Value, out dealer1Crack);
                        }
                        if (e.Name == "cocain")
                        {
                            Int32.TryParse(e.Value, out dealer1Cocain);
                        }
                        if (e.Name == "money")
                        {
                            Int32.TryParse(e.Value, out int tempInt);
                            if (tempInt > 10000000)
                            {
                                dealer1Money = 10000000;
                            }
                            else
                            {
                                dealer1Money = tempInt;
                            }
                        }
                        if (e.Name == "reloaded")
                        {
                            Boolean.TryParse(e.Value, out dealer1Reloaded);
                        }
                    }
                }
            }
            XDocument VehicleXml = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
            foreach(XElement vehicles in VehicleXml.Descendants())
            {
                if (vehicles.Name == "Vehicle")
                {
                    foreach(XElement e in vehicles.Elements())
                    {
                        if(e.Name == "Destroyed")
                        {
                            bool.TryParse(e.Value, out drugCarDestroyed);
                        }
                    }
                }
            }
            string keysMainMenu = iniSettings.GetValue("KEYS", "MAIN", "F10");
            Enum.TryParse(keysMainMenu, out Keys mainMenuKey);
            MenuKey = mainMenuKey;
            string keysStopDealing = iniSettings.GetValue("KEYS", "STOPDEALING", "F11");
            Enum.TryParse(keysStopDealing, out Keys mainStopDealing);
            StopDealing = mainStopDealing;
            string keysSetDrugCar = iniSettings.GetValue("KEYS", "SETDRUGCAR", "F12");
            Enum.TryParse(keysSetDrugCar, out Keys mainSetDrugCar);
            SetDrugCar = mainSetDrugCar;

            foreach (KeyValuePair<string, int> entry in PlayerInventory)
            {
                TotalDrugsCarried += PlayerInventory[entry.Key];
            }
            EnableBlips = true;
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
                    if(DEBUG)
                    {
                        UI.Notify("Added " + weapon + " to playerWeapons list.");
                    }
                    
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
            //    
            //    
            //    
            //    {
            //        
            //        foreach (KeyValuePair<string, string> weaponComponent in weaponComponentItems)
            //        {
            //            int componentHash = Function.Call<int>(Hash.GET_HASH_KEY, weaponComponent.Key);
            //            bool hasWeaponGotComponent = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character, (int)weapon, componentHash);
            //            if (hasWeaponGotComponent)
            //            {                // Add the values to a list so you can retreive their values later. Including the ammo count.

            //                playerWeapon[1] = ammunitionInWeapon;
            //                playerWeapon[3] = componentHash;
            //                playerWeapons.Add(playerWeapon);
            //            }            
            //            else
            //            {               // Also add these values to a list. Including the ammo count. 
            //                playerWeapon[0] = (int)weapon;
            //                playerWeapon[1] = ammunitionInWeapon;
            //                playerWeapon[3] = 0;
            //                playerWeapons.Add(playerWeapon);
            //            }
            //        }
            //    }
            //}

            //NATIVEUI STUFF
            LsMenuPool = new MenuPool();

            //DEALER MENU
            dealerMainMenu = new UIMenu("Dealer", "What you after?");
            LsMenuPool.Add(dealerMainMenu);
            // WEED
            weedMenu = LsMenuPool.AddSubMenu(dealerMainMenu, "Weed");
            bushWeed = new UIMenuSliderItem("Weed $" + weed1Price + "oz");
            weedMenu.AddItem(bushWeed);
            bushWeed.Maximum = dealer1Weed;
            bushWeed.Multiplier = 1;
            WeedBuy = new UIMenuItem("Buy: " + bushWeed.Value);
            weedMenu.AddItem(WeedBuy);
            bushWeed.OnSliderChanged += onSliderChange;
            weedMenu.OnItemSelect += onWeedMenuSelect;
            //CRACK 
            crackMenu = LsMenuPool.AddSubMenu(dealerMainMenu, "Crack");
            crack = new UIMenuSliderItem("Crack $" + crack1Price + "oz");
            crackMenu.AddItem(crack);
            crack.Maximum = dealer1Crack;
            crack.Multiplier = 1;
            CrackBuy = new UIMenuItem("Buy: " + crack.Value);
            crackMenu.AddItem(CrackBuy);
            crack.OnSliderChanged += onSliderChange;
            crackMenu.OnItemSelect += onCrackMenuSelect;
            //COCAIN
            cocainMenu = LsMenuPool.AddSubMenu(dealerMainMenu, "Cocain");
            cocain = new UIMenuSliderItem("Cocain $" + cocain1Price + "oz");
            cocainMenu.AddItem(cocain);
            cocain.Maximum = dealer1Cocain;
            cocain.Multiplier = 1;
            CocainBuy = new UIMenuItem("Buy: " + cocain.Value);
            cocainMenu.AddItem(CocainBuy);
            cocain.OnSliderChanged += onSliderChange;
            cocainMenu.OnItemSelect += onCocainMenuSelect;
            //dismis
            dismissDealer = new UIMenuItem("Nothing man, see ya");
            dealerMainMenu.AddItem(dismissDealer);
            payDebt = new UIMenuItem("Pay Debt $" + dealer1Debt);
            dealerMainMenu.AddItem(payDebt);
            dealerMainMenu.OnItemSelect += onDealerMenuSelect;

            //STASH MENU
            stashMainMenu = new UIMenu("Safehouse Stash", "Select an option.");
            LsMenuPool.Add(stashMainMenu);
            stashTakeMenu = LsMenuPool.AddSubMenu(stashMainMenu, "Take");
            stashPutMenu = LsMenuPool.AddSubMenu(stashMainMenu, "Stash");
            //stash weed
            putWeedStash = new UIMenuSliderItem("Weed");
            stashPutMenu.AddItem(putWeedStash);
            putWeedStash.Maximum = PlayerInventory["Weed"];
            putWeedStash.Multiplier = 1;
            takeWeedStash = new UIMenuSliderItem("Weed");
            stashTakeMenu.AddItem(takeWeedStash);
            takeWeedStash.Maximum = stashWeed;
            takeWeedStash.Multiplier = 1;
            putWeedStash.OnSliderChanged += onSliderChange;
            takeWeedStash.OnSliderChanged += onSliderChange;
            //stash crack
            putCrackStash = new UIMenuSliderItem("Crack");
            stashPutMenu.AddItem(putCrackStash);
            putCrackStash.Maximum = PlayerInventory["Crack"];
            putCrackStash.Multiplier = 1;
            takeCrackStash = new UIMenuSliderItem("Crack");
            stashTakeMenu.AddItem(takeCrackStash);
            takeCrackStash.Maximum = stashCrack;
            takeCrackStash.Multiplier = 1;
            putCrackStash.OnSliderChanged += onSliderChange;
            takeCrackStash.OnSliderChanged += onSliderChange;
            //stash COCAIN
            putCocainStash = new UIMenuSliderItem("Cocain");
            stashPutMenu.AddItem(putCocainStash);
            putCocainStash.Maximum = PlayerInventory["Cocain"];
            putCocainStash.Multiplier = 1;
            takeCocainStash = new UIMenuSliderItem("Cocain");
            stashTakeMenu.AddItem(takeCocainStash);
            takeCocainStash.Maximum = stashCocain;
            takeCocainStash.Multiplier = 1;
            putCocainStash.OnSliderChanged += onSliderChange;
            takeCocainStash.OnSliderChanged += onSliderChange;
            //stash MONEY
            putMoneyStash = new UIMenuSliderItem("Money $");
            stashPutMenu.AddItem(putMoneyStash);
            putMoneyStash.Maximum = player.Money;
            putMoneyStash.Multiplier = 100;
            takeMoneyStash = new UIMenuSliderItem("Money $");
            stashTakeMenu.AddItem(takeMoneyStash);
            takeMoneyStash.Maximum = stashMoney;
            takeMoneyStash.Multiplier = 100;
            putMoneyStash.OnSliderChanged += onSliderChange;
            takeMoneyStash.OnSliderChanged += onSliderChange;
            //STASH MULTIPLIER
            stashPutMultiplier = new UIMenuListItem("Multiplier", multiplier, 0);
            stashPutMenu.AddItem(stashPutMultiplier);
            stashPutMultiplier.OnListChanged += onListChange;
            stashTakeMultiplier = new UIMenuListItem("Multiplier", multiplier, 0);
            stashTakeMenu.AddItem(stashTakeMultiplier);
            stashTakeMultiplier.OnListChanged += onListChange;

            //STASH WEAPONS
            if (playerWeapons.Count > 0)
            {
                putWeaponStash = new UIMenuListItem("Stash Weapon: ", playerWeps, 0);
                stashPutMenu.AddItem(putWeaponStash);
                putWeaponStash.OnListChanged += onListChange;
            }
            if (stashWeps.Count > 0)
            {
                takeWeaponStash = new UIMenuListItem("Take Weapon :", stashWeps, 0);
                stashTakeMenu.AddItem(takeWeaponStash);
                takeWeaponStash.OnListChanged += onListChange;
            }
            stashPutMenu.OnItemSelect += onStashMenuSelect;
            stashTakeMenu.OnItemSelect += onPlayerMenuSelect;

            ///VEHCILE MENUS
            vehStashMainMenu = new UIMenu("Vehicle Stash", "Select");
            LsMenuPool.Add(vehStashMainMenu);
            vehStashTakeMenu = LsMenuPool.AddSubMenu(vehStashMainMenu, "Take");
            vehStashPutMenu = LsMenuPool.AddSubMenu(vehStashMainMenu, "Stash");
            //stash weed
            putWeedVeh = new UIMenuSliderItem("Weed " +PlayerInventory["Weed"]+ "g");
            vehStashPutMenu.AddItem(putWeedVeh);
            putWeedVeh.Maximum = PlayerInventory["Weed"];
            putWeedVeh.Multiplier = 1;
            takeWeedVeh = new UIMenuSliderItem("Weed " + drugCarWeed + "g");
            vehStashTakeMenu.AddItem(takeWeedVeh);
            takeWeedVeh.Maximum = drugCarWeed;
            takeWeedVeh.Multiplier = 1;
            putWeedVeh.OnSliderChanged += onSliderChange;
            takeWeedVeh.OnSliderChanged += onSliderChange;
            //stash crack
            putCrackVeh = new UIMenuSliderItem("Crack " + PlayerInventory["Crack"] + "g");
            vehStashPutMenu.AddItem(putCrackVeh);
            putCrackVeh.Maximum = PlayerInventory["Crack"];
            putCrackVeh.Multiplier = 1;
            takeCrackVeh = new UIMenuSliderItem("Crack " + drugCarCrack + "g");
            vehStashTakeMenu.AddItem(takeCrackVeh);
            takeCrackVeh.Maximum = drugCarCrack;
            takeCrackVeh.Multiplier = 1;
            putCrackVeh.OnSliderChanged += onSliderChange;
            takeCrackVeh.OnSliderChanged += onSliderChange;
            //stash COCAIN
            putCocainVeh = new UIMenuSliderItem("Cocain " + PlayerInventory["Cocain"] + "g");
            vehStashPutMenu.AddItem(putCocainVeh);
            putCocainVeh.Maximum = PlayerInventory["Cocain"];
            putCocainVeh.Multiplier = 1;
            takeCocainVeh = new UIMenuSliderItem("Cocain " + drugCarCocain + "g");
            vehStashTakeMenu.AddItem(takeCocainVeh);
            takeCocainVeh.Maximum = drugCarCocain;
            takeCocainVeh.Multiplier = 1;
            putCocainVeh.OnSliderChanged += onSliderChange;
            takeCocainVeh.OnSliderChanged += onSliderChange;
            //stash MONEY
            putMoneyVeh = new UIMenuSliderItem("Money $" + player.Money);
            vehStashPutMenu.AddItem(putMoneyVeh);
            putMoneyVeh.Maximum = player.Money;
            putMoneyVeh.Multiplier = 100;
            takeMoneyVeh = new UIMenuSliderItem("Money $" + drugCarMoney);
            vehStashTakeMenu.AddItem(takeMoneyVeh);
            takeMoneyVeh.Maximum = drugCarMoney;
            takeMoneyVeh.Multiplier = 100;
            putMoneyVeh.OnSliderChanged += onSliderChange;
            takeMoneyVeh.OnSliderChanged += onSliderChange;
            //VEH STASH MULIPLIER
            vehPutMultiplier = new UIMenuListItem("Multiplier", multiplier, 0);
            vehStashPutMenu.AddItem(vehPutMultiplier);
            vehPutMultiplier.OnListChanged += onListChange;
            vehTakeMultiplier = new UIMenuListItem("Multiplier", multiplier, 0);
            vehStashTakeMenu.AddItem(vehTakeMultiplier);
            vehTakeMultiplier.OnListChanged += onListChange;

            //VEHICLE WEAPONS
            if (playerWeapons.Count > 0)
            {
                putWeaponVeh = new UIMenuListItem("Stash Weapon: ", playerWeps, 0);
                vehStashPutMenu.AddItem(putWeaponVeh);
                putWeaponVeh.OnListChanged += onListChange;
            }
            if (vehWeps.Count > 0)
            {
                takeWeaponVeh = new UIMenuListItem("Take Weapon :", vehWeps, 0);
                vehStashTakeMenu.AddItem(takeWeaponVeh);
                takeWeaponVeh.OnListChanged += onListChange;
            }
            vehStashPutMenu.OnItemSelect += onPutVehicleMenuSelect;
            vehStashTakeMenu.OnItemSelect += onTakeVehicleMenuSelect;


            //Customer Menu
            customerMainMenu = new UIMenu("Customer", "Select an option");
            LsMenuPool.Add(customerMainMenu);
            sell = new UIMenuItem("drug");
            customerMainMenu.AddItem(sell);
            turnAway = new UIMenuItem("cya");
            customerMainMenu.AddItem(turnAway);
            customerMainMenu.OnItemSelect += onCustomerMenuSelect;
            LsMenuPool.RefreshIndex();

            ////Ifruit setup
            iFruit = new CustomiFruit();
            // Buydrugs contact (wait 4 seconds (4000ms) before picking up the phone)
            iFruitContact contactA = new iFruitContact("Zee - Buy drugs.");
            contactA.Answered += ContactAnswered;   // Linking the Answered event with our function
            contactA.DialTimeout = 1000;            // Delay before answering
            contactA.Active = true;                 // true = the contact is available and will answer the phone
            contactA.Icon = ContactIcon.Blank;      // Contact's icon
            iFruit.Contacts.Add(contactA);      // Add the contact to the phone
                                                // Selldrugs contact
            iFruitContact contactB = new iFruitContact("Zee - Sell drugs.");
            contactB.Answered += ContactAnswered;   // Linking the Answered event with our function
            contactB.DialTimeout = 1000;            // Delay before answering
            contactB.Active = true;                 // true = the contact is available and will answer the phone
            contactB.Icon = ContactIcon.Blank;      // Contact's icon
            iFruit.Contacts.Add(contactB);                                        // Add the contact to the phone
                                                                                  // Weapons contact                                      
            iFruitContact contactC = new iFruitContact("Zee - Buy weapons.");
            contactC.Answered += ContactAnswered;
            contactC.DialTimeout = 1000;
            contactC.Active = true;
            contactC.Icon = ContactIcon.MP_ProfBoss;
            iFruit.Contacts.Add(contactC);
            //timestuff

            day = Function.Call<int>(Hash.GET_CLOCK_DAY_OF_WEEK);
            hour = Function.Call<int>(Hash.GET_CLOCK_HOURS);
            dayUpdateTimer = Game.GameTime;
            jobOfferTimer = Game.GameTime;
            
            //Loaded
            //UI.Notify("LS Life Alpha Loaded, Give Zee a call.");
        }

        //WEED MENU
        void onWeedMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == WeedBuy && dealer1Weed > 0)
            {
                
                var inGrams = bushWeed.Value * 28;
                int total = bushWeed.Value * weed1Price;
                PlayerInventory["Weed"] += inGrams;
                bool sale = false;
                if (player.Money >= total)
                {
                    player.Money -= total;
                    dealer1Money += total;
                    drugDealer1.Money = dealer1Money;
                    UI.ShowSubtitle("Nice doing business with you.", 5000);
                    sale = true;
                }
                else
                {
                    if (dealer1Debt + total > 100000)
                    {
                        UI.ShowSubtitle("You owe me to much already", 5000);
                        sale = false;
                    }
                    else
                    {
                        
                        dealer1Debt += total;
                        payDebt.Text = ("Pay Debt $" + dealer1Debt);
                        UI.ShowSubtitle("Ill add this one to youre bill, you now owe me $" + dealer1Debt, 5000);
                        sale = true;
                    }
                    
                }
                if (sale)
                {
                    bushWeed.Maximum -= bushWeed.Value;
                    dealer1Weed = bushWeed.Maximum;
                    total = bushWeed.Value * weed1Price;
                    WeedBuy.Text = "Buy ~g~" + bushWeed.Value + " ~w~for $~g~" + total;
                    putWeedStash.Maximum = PlayerInventory["Weed"];
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
        void onCrackMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == CrackBuy && dealer1Crack > 0)
            {
                var inGrams = crack.Value * 28;
                int total = crack.Value * crack1Price;
                PlayerInventory["Crack"] += inGrams;
                bool sale = false;
                if (player.Money >= total)
                {
                    player.Money -= total;
                    dealer1Money += total;
                    drugDealer1.Money = dealer1Money;
                    UI.ShowSubtitle("Nice doing business with you.", 5000);
                    sale = true;
                }
                else
                {
                    if (dealer1Debt + total > 100000)
                    {
                        UI.ShowSubtitle("You owe me to much already", 5000);
                        sale = false;
                    }
                    else
                    {

                        dealer1Debt += total;
                        payDebt.Text = ("Pay Debt $" + dealer1Debt);
                        UI.ShowSubtitle("Ill add this one to youre bill, you now owe me $" + dealer1Debt, 5000);
                        sale = true;
                    }
                }
                if (sale)
                {
                    crack.Maximum -= crack.Value;
                    dealer1Crack = crack.Maximum;
                    total = crack.Value * crack1Price;
                    CrackBuy.Text = "Buy ~g~" + crack.Value + " ~w~for $~g~" + total;
                    putCrackStash.Maximum = PlayerInventory["Crack"];
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
        void onCocainMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == CocainBuy && dealer1Cocain > 0)
            {
                var inGrams = cocain.Value * 28;
                int total = cocain.Value * cocain1Price;
                PlayerInventory["Cocain"] += inGrams;
                bool sale = false;
                if (player.Money >= total)
                {
                    player.Money -= total;
                    dealer1Money += total;
                    drugDealer1.Money = dealer1Money;
                    UI.ShowSubtitle("Nice doing business with you.", 5000);
                    sale = true;
                }
                else
                {
                    if (dealer1Debt + total > 100000)
                    {
                        UI.ShowSubtitle("You owe me to much already", 5000);
                        sale = false;
                    }
                    else
                    {

                        dealer1Debt += total;
                        payDebt.Text = ("Pay Debt $" + dealer1Debt);
                        UI.ShowSubtitle("Ill add this one to youre bill, you now owe me $" + dealer1Debt, 5000);
                        sale = true;
                    }
                }
                if (sale)
                {
                    cocain.Maximum -= cocain.Value;
                    dealer1Cocain = cocain.Maximum;
                    total = cocain.Value * cocain1Price;
                    CocainBuy.Text = "Buy ~g~" + cocain.Value + " ~w~for $~g~" + total;
                    putCocainStash.Maximum = PlayerInventory["Cocain"];
                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                    foreach (XElement element in LSLifeSave.Descendants())
                    {
                        if (element.Name == "playerInventory")
                        {
                            foreach (XElement e in element.Elements())
                            {
                                if (e.Name == "cocain")
                                {
                                    e.Value = PlayerInventory["Cocain"].ToString();
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
        void onDealerMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == payDebt && dealer1Debt > 0)
            {
                if (player.Money > 0)
                {
                    int total = 0;
                    if (player.Money > dealer1Debt) { total = dealer1Debt; }
                    else { total = player.Money; }
                    dealer1Debt -= total;
                    player.Money -= total;
                    dealer1Money += total;
                    drugDealer1.Money = dealer1Money;
                    UI.ShowSubtitle("Thanks man", 5000);
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

        // Vehicle Menu
        void onPutVehicleMenuSelect(UIMenu sender, UIMenuItem item, int index)
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
                PlayerInventory["Cocain"] -= putCocainVeh.Value;                
                putCocainVeh.Maximum -= putCocainVeh.Value;
                takeCocainVeh.Maximum = drugCarCocain;
                TotalDrugsCarried = 0;
                putCocainVeh.Text = "Cocain " + PlayerInventory["Crack"] + "g";
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = PlayerInventory["Cocain"].ToString();
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
                player.Money -= putMoneyVeh.Value;
                putMoneyVeh.Maximum -= putMoneyVeh.Value;
                takeMoneyVeh.Maximum = drugCarMoney;
                putMoneyVeh.Text = "Money $" + player.Money;
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
                            playerPed.Weapons.Remove(wep);
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
        void onTakeVehicleMenuSelect(UIMenu sender, UIMenuItem item, int index)
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
                PlayerInventory["Cocain"] += takeCocainVeh.Value;
                drugCarCocain -= takeCocainVeh.Value;               
                takeCocainVeh.Maximum -= takeCocainVeh.Value;
                putCocainStash.Maximum = PlayerInventory["Cocain"];
                TotalDrugsCarried = 0;
                takeCocainVeh.Text = "Cocain " + drugCarCocain + "g";
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                foreach (XElement element in LSLifeSave.Descendants())
                {
                    if (element.Name == "playerInventory")
                    {
                        foreach (XElement e in element.Elements())
                        {
                            if (e.Name == "cocain")
                            {
                                e.Value = PlayerInventory["Cocain"].ToString();
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
                player.Money += takeMoneyVeh.Value;
                drugCarMoney -= takeMoneyVeh.Value;
                takeMoneyVeh.Maximum -= takeMoneyVeh.Value;
                putMoneyVeh.Maximum = player.Money;
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
                                        playerPed.Weapons.Give(wep, ammo, true, false);
                                        playerPed.Weapons.Select(wep, true);
                                        foreach (XElement e in rootElement.Elements())
                                        {
                                            if (e.Name == "COMPONENT")
                                            {
                                                Enum.TryParse(e.Value, out WeaponComponent wepcomp);
                                                playerPed.Weapons.Current.SetComponent(wepcomp, true);
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

        void onSliderChange(UIMenuSliderItem sender, int index)
        {
            //VEHICLE SLIDERS
            if(sender == takeWeedVeh)
            {
                vehStashTakeMenu.Subtitle.Caption = "Take " + takeWeedVeh.Value + "g";
            }
            if(sender == putWeedVeh)
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
                int total = bushWeed.Value * weed1Price;
                WeedBuy.Text = "Buy ~g~" + bushWeed.Value + " ~w~for $~g~" + total;
            }
            if (sender == crack)
            {
                int total = crack.Value * crack1Price;
                CrackBuy.Text = "Buy ~g~" + crack.Value + " ~w~for $~g~" + total;
            }
            if (sender == cocain)
            {
                int total = cocain.Value * cocain1Price;
                CocainBuy.Text = "Buy ~g~" + cocain.Value + " ~w~for $~g~" + total;
            }
        }

        //PLAYER TO STASH MENUS
        void onListChange(UIMenuListItem sender, int index)
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

        }

        void onStashMenuSelect(UIMenu sender, UIMenuItem item, int index)
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
                PlayerInventory["Cocain"] -= putCocainStash.Value;                
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
                                e.Value = PlayerInventory["Cocain"].ToString();
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
                player.Money -= putMoneyStash.Value;
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
                            playerPed.Weapons.Remove(wep);
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
        void onPlayerMenuSelect(UIMenu sender, UIMenuItem item, int index)
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
                PlayerInventory["Cocain"] += takeCocainStash.Value;
                stashCocain -= takeCocainStash.Value;
                takeCocainStash.Maximum -= takeCocainStash.Value;
                putCocainStash.Maximum = PlayerInventory["Cocain"];
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
                                e.Value = PlayerInventory["Cocain"].ToString();
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
                player.Money += takeMoneyStash.Value;
                stashMoney -= takeMoneyStash.Value;
                takeMoneyStash.Maximum -= takeMoneyStash.Value;
                putMoneyStash.Maximum = player.Money;
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
                                        playerPed.Weapons.Give(wep, ammo, true, false);
                                        playerPed.Weapons.Select(wep, true);
                                        foreach (XElement e in rootElement.Elements())
                                        {
                                            if (e.Name == "COMPONENT")
                                            {
                                                Enum.TryParse(e.Value, out WeaponComponent wepcomp);
                                                playerPed.Weapons.Current.SetComponent(wepcomp, true);
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
        void onCustomerMenuSelect(UIMenu sender, UIMenuItem item, int index)
        {
            if (item == sell)
            {
                jobOfferTimer = Game.GameTime;
                if (PlayerInventory[customerSellTo.drug] >= customerSellTo.amount)
                {
                    if (playerPed.IsInVehicle())
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
                jobOfferTimer = Game.GameTime;
                customerMainMenu.Visible = !customerMainMenu.Visible;
                if (customerSellTo.FarAway)
                {
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
                        RemovePedFromWorld(customerSellTo.Ped, true);
                    }
                    customers.Remove(customerSellTo);                    
                    lastTimeDealDone = Game.GameTime;
                    oldCustomers.Add(customerSellTo.Ped);
                }
            }
        }

        //on game tick ASHASHASHSAH!!!!!!!!!!!!



        void onTick(object sender, EventArgs e)
        {
            if (!Game.IsLoading)
            {
                if (firstRun)
                {
                    if (playerPed != null)
                    {
                        firstRun = false;
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, playerPed, 9, 0, 0, 0);
                    }                    
                }

                if (DEBUG)
                {
                    int awepsCount = 0;
                    int pwepsCount = 0;
                    //myUIText = new UIText("Weed:" +dealer1Weed+" | Crack:"+dealer1Crack+" | Cocain:"+dealer1Cocain+" | Reloaded:"+dealer1Reloaded+" | Ordered:"+dealer1ReloadOrder+" | Day delivery:"+dealer1ReloadDay+" | Current day:"+day, new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                    foreach (XElement element in attackerWeapons)
                    {
                        if (element.Name == "weapon")
                        {
                            awepsCount += 1;
                        }
                    }
                    foreach(XElement element in playerWeapons)
                    {
                        if (element.Name == "weapon")
                        {
                            pwepsCount += 1;
                        }
                    }
                    string s = "AWeps =" + awepsCount + "PWeps =" + pwepsCount + " drugs " + attackerInventory.Count;
                    myUIText = new UIText(s, new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                    myUIText.Draw();
                }       
                                
                if (LsMenuPool != null)
                {
                    LsMenuPool.ProcessMenus();
                }
                iFruit.Update();
                if (timerBarPool != null)
                {
                    if(!LsMenuPool.IsAnyMenuOpen()) { timerBarPool.Draw(); }                    
                }
                // 300ms delay
                if (Game.GameTime > dayUpdateTimer + 350)
                {
                    if (attackerPickupSpawned)
                    {
                        if (!attackerPickup.ObjectExists())
                        {
                            attackerPickup.Delete();
                            attackerPickupSpawned = false;                            
                            WeaponHash weaponHash = WeaponHash.Unarmed;
                            int ammo = 0;
                            foreach (XElement element in attackerWeapons)
                            {
                                if (element.Name == "weapon")
                                {
                                    foreach (XElement wep in element.Elements())
                                    {
                                        if (wep.Name == "HASH")
                                        {
                                            Enum.TryParse(wep.Value, out weaponHash);
                                        }
                                        if (wep.Name == "AMMO")
                                        {
                                            Int32.TryParse(wep.Value, out ammo);
                                        }
                                        if (weaponHash != WeaponHash.Unarmed)
                                        {
                                            if (ammo == 100) { ammo = 100; }
                                            playerPed.Weapons.Give(weaponHash, ammo, true, true);
                                            playerPed.Weapons.Select(weaponHash, true);
                                            foreach (XElement c in element.Elements())
                                            {
                                                if (c.Name == "COMPONENT")
                                                {
                                                    Enum.TryParse(c.Value, out WeaponComponent wepcomp);
                                                    playerPed.Weapons.Current.SetComponent(wepcomp, true);
                                                }
                                            }
                                            if (DEBUG)
                                            {
                                                UI.Notify("Gave " + weaponHash + " ammo = " + ammo);
                                            }
                                        }
                                    }
                                }
                            }
                            attackerWeapons.Clear();
                            foreach (KeyValuePair<string, int> entry in attackerInventory)
                            {
                                PlayerInventory[entry.Key] = entry.Value;
                            }
                            attackerInventory.Clear();

                            LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                            foreach (XElement element in LSLifeSave.Descendants())
                            {
                                if (element.Name == "playerInventory")
                                {
                                    foreach (XElement drug in element.Elements())
                                    {
                                        if (drug.Name == "weed")
                                        {
                                            drug.Value = PlayerInventory["Weed"].ToString();
                                        }
                                        if (drug.Name == "crack")
                                        {
                                            drug.Value = PlayerInventory["Crack"].ToString();
                                        }
                                        if (drug.Name == "cocain")
                                        {
                                            drug.Value = PlayerInventory["Cocain"].ToString();
                                        }
                                    }
                                }
                            }
                            LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                        }
                    }
                    playerGroup = playerPed.CurrentPedGroup;
                    dayUpdateTimer = Game.GameTime;
                    string playerArea = World.GetZoneNameLabel(playerPed.Position);
                    DateTimeUpdate();
                    if (areas.ContainsKey(playerArea))
                    {
                        areas.TryGetValue(playerArea, out wealth);
                    }
                    if (player.IsPlaying && notFined != true) { notFined = true; }
                    if (playerPed != null && !playerPed.IsAlive && !playerDead)
                    {
                        if (drugCarSpawned)
                        {
                            drugCar.InstallModKit();
                            drugCarSpawned = false;
                        }
                        playerDead = true;
                        if (isDealing) { isDealing = false; }
                        var killer = playerPed.GetKiller();
                        if (killer != null)
                        {
                            if (killer is Ped p)
                            {
                                if (p != null && p.IsHuman && !p.IsPlayer)
                                {
                                    if (Function.Call<int>(Hash.GET_PED_TYPE, p.Handle) != 6)
                                    {
                                        if (attacker == null)
                                        {
                                            //UI.Notify("Yo, found the dude that got you.");
                                            attacker = p;
                                            attacker.IsPersistent = true;
                                            attacker.Task.WanderAround();
                                            attacker.Money = player.Money;
                                            player.Money -= player.Money;
                                            angryPeds.Remove(attacker);
                                            attacker.AddBlip();
                                            attacker.CurrentBlip.IsFriendly = false;
                                            attacker.CurrentBlip.Name = "Your Killer";
                                            WeaponHash weaponHash = WeaponHash.Unarmed;
                                            int ammo = 0;
                                            playerPed.Weapons.RemoveAll();
                                            attacker.Armor = 100;
                                            
                                            foreach (XElement element in playerWeapons)
                                            {
                                                if (element.Name == "weapon")
                                                {
                                                    attackerWeapons.Add(element);
                                                    foreach(XElement wep in element.Elements())
                                                    {
                                                        if (wep.Name == "HASH")
                                                        {
                                                            Enum.TryParse(wep.Value, out weaponHash);                                                            
                                                        }
                                                        if (wep.Name == "AMMO")
                                                        {
                                                            Int32.TryParse(wep.Value, out ammo);
                                                        }
                                                        if (weaponHash != WeaponHash.Unarmed)
                                                        {
                                                            attacker.Weapons.Give(weaponHash, ammo, true, true);                                                            
                                                        }                                                        
                                                    }
                                                }
                                            }
                                            attacker.DropsWeaponsOnDeath = true;
                                            playerWeapons.Clear();
                                            playerWeps.Clear();
                                            attacker.Weapons.Select(attacker.Weapons.BestWeapon);
                                            UI.Notify("Killer has equiped " + attacker.Weapons.Current.Name + " & and has equiped Body Armour");
                                            foreach (KeyValuePair<string, int> entry in PlayerInventory)
                                            {
                                                attackerInventory.Add(entry.Key, entry.Value);                                                
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (playerPed != null && !player.IsPlaying)
                    {
                        if (isDealing == true) { isDealing = false; }
                        if (notFined && playerWanted)
                        {
                            notFined = false;
                            playerWanted = false;
                            if (drugCarWanted && drugCar != null)
                            {
                                UI.Notify("Storage Vehicle has been confiscated, and its contents destroyed");
                                drugCarDestroyed = true;
                                drugCarDestroyed = true;
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
                                drugCarWanted = false;
                                drugCar.IsPersistent = false;
                                drugCar.MarkAsNoLongerNeeded();
                                drugCar.CurrentBlip.Remove();
                                drugCar.Delete();
                                drugCar = null;
                                
                                int drugCarDrugs = 0;
                                drugCarDrugs += drugCarWeed;
                                drugCarDrugs += drugCarCrack;
                                drugCarDrugs += drugCarCocain;
                                TotalDrugsCarried += drugCarDrugs;
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
                            if (TotalDrugsCarried > 0)
                            {
                                UI.Notify("They took $" + player.Money + ", all your Drugs and Weapons.");
                                playerPed.Weapons.RemoveAll();
                                player.Money -= player.Money;
                                TotalDrugsCarried = 0;
                                if (PlayerInventory.Keys.ToList().Count > 0)
                                {
                                    foreach (var drug in PlayerInventory.Keys.ToList())
                                    {
                                        PlayerInventory[drug] = 0;
                                    }
                                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                                    foreach (XElement element in LSLifeSave.Descendants())
                                    {
                                        if (element.Name == "playerInventory")
                                        {
                                            foreach (XElement pe in element.Elements())
                                            {
                                                if (pe.Name == "weed")
                                                {
                                                    pe.Value = "0";
                                                }
                                                if (pe.Name == "crack")
                                                {
                                                    pe.Value = "0";
                                                }
                                                if (pe.Name == "cocain")
                                                {
                                                    pe.Value = "0";
                                                }
                                            }
                                        }
                                    }
                                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                                }
                            }
                        }
                        else
                        if (playerDead)
                        {
                            playerDead = false;
                            if (TotalDrugsCarried > 0)
                            {
                                playerPed.Weapons.RemoveAll();
                                UI.Notify("Someone robbed your $" + player.Money + ", all your Drugs and Weapons.");
                                player.Money -= player.Money;
                                TotalDrugsCarried = 0;
                                if (PlayerInventory.Keys.ToList().Count > 0)
                                {
                                    foreach (var drug in PlayerInventory.Keys.ToList())
                                    {
                                        PlayerInventory[drug] = 0;
                                    }
                                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                                    foreach (XElement element in LSLifeSave.Descendants())
                                    {
                                        if (element.Name == "playerInventory")
                                        {
                                            foreach (XElement pe in element.Elements())
                                            {
                                                if (pe.Name == "weed")
                                                {
                                                    pe.Value = "0";
                                                }
                                                if (pe.Name == "crack")
                                                {
                                                    pe.Value = "0";
                                                }
                                                if (pe.Name == "cocain")
                                                {
                                                    pe.Value = "0";
                                                }
                                            }
                                        }
                                    }
                                    LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                                }
                            }
                        }
                    }
                    if (angryPeds.Count > 0)
                    {
                        int pIndex = 0;
                        foreach (Ped p in angryPeds)
                        {
                            if (p == null)
                            {
                                angryPeds.RemoveAt(pIndex);
                            }
                            pIndex++;
                        }
                    }
                    if (playerPed != null && playerPed.IsAlive && player.IsPlaying)
                    {
                        Vector3 vehPos = Vector3.Zero;                        
                        int handle = 0;
                        string reg = "";
                        string model = "";
                        if (!drugCarDestroyed)
                        {
                            XDocument VehicleXml = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
                            foreach (XElement v in VehicleXml.Descendants())
                            {
                                if (v.Name == "Vehicle")
                                {
                                    foreach (XElement vEl in v.Elements())
                                    {
                                        if (vEl.Name == "PosX") { float.TryParse(vEl.Value, out vehPos.X); }
                                        if (vEl.Name == "PosY") { float.TryParse(vEl.Value, out vehPos.Y); }
                                        if (vEl.Name == "PosZ") { float.TryParse(vEl.Value, out vehPos.Z); }
                                        if (vEl.Name == "Handle") { Int32.TryParse(vEl.Value, out handle); }
                                        if (vEl.Name == "Name") { model = vEl.Value; }
                                        if (vEl.Name == "NumPlate") { reg = vEl.Value; }

                                    }
                                }
                            }
                        }                        
                        if (Game.Player != player)
                        {
                            player = Game.Player;
                            playerPed = Game.Player.Character;
                            UI.Notify("LS Life: Player Character changed");
                        }                            
                        if (attacker != null)
                        {
                            if (attacker.IsDead)
                            {
                                RemovePedFromWorld(attacker, true);
                                if (!attackerPickupSpawned)
                                {
                                    attackerPickupSpawned = true;
                                    attackerPickup = World.CreatePickup(PickupType.MoneyMedBag, attacker.Position, Vector3.Zero, new Model("prop_big_bag_01"), 100);
                                    //foreach (XElement element in playerWeapons)
                                    //{
                                    //    if (element.Name == "weapon")
                                    //    {
                                    //        attackerWeapons.Add(element);
                                    //        UI.Notify("added " + element.Name);
                                    //    }
                                    //}
                                }
                                attacker = null;
                            }
                            else
                            {
                                float distance = attacker.Position.DistanceTo(playerPed.Position);
                                if ( distance < 60)
                                {                                   
                                    if (!attackerAttacking)
                                    {
                                        if (distance < 10)
                                        {

                                            attackerAttacking = true;
                                            attacker.Task.FightAgainst(playerPed);
                                        }
                                        else
                                        if (Game.GameTime > rayTimer + 1000)
                                        {
                                            rayTimer = Game.GameTime;
                                            RaycastResult raycast = World.RaycastCapsule(attacker.Position + attacker.ForwardVector, playerPed.Position, 60, IntersectOptions.Everything);
                                            if (raycast.HitEntity == playerPed)
                                            {
                                                attackerAttacking = true;
                                                attacker.Task.FightAgainst(playerPed);
                                            }
                                        }
                                        
                                    }
                                    else
                                    {
                                        attackerAttacking = false;
                                        attacker.Task.WanderAround();
                                    }
                                        
                                    
                                }
                            }
                        }                        
                        if (drugCarDestroyed == false && drugCar == null)
                        {                           
                            if (playerPed.Position.DistanceTo(vehPos) <= 200)
                            {
                                if (drugCarBlip != null)
                                {
                                    drugCarBlip.Remove();
                                    drugCarBlip = null;
                                }
                                var vehicles = World.GetAllVehicles();
                                foreach (Vehicle veh in vehicles)
                                {

                                    if (veh.Position.DistanceTo(vehPos) < 3 && veh.NumberPlate == reg)
                                    {
                                        if (veh.CurrentBlip != null)
                                        {
                                            veh.CurrentBlip.Remove();
                                        }
                                        drugCar = veh;
                                        drugCar.IsPersistent = true;
                                        drugCar.EngineHealth = 1000;
                                        drugCar.Health = 1000;
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
                                if (drugCar == null)
                                {
                                    VehicleLoad();
                                }
                            }
                        }
                        if (drugCar != null)
                        {

                            if (drugCar.Exists())
                            {
                                if (drugCar.CurrentBlip.Exists())
                                {
                                    if (drugCar.Driver == playerPed)
                                    {
                                        drugCar.CurrentBlip.Remove();
                                    }
                                }
                                if (playerPed.IsInVehicle())
                                {
                                    if (playerWanted && drugCar.Handle == playerPed.CurrentVehicle.Handle && drugCarWanted == false)
                                    {
                                        drugCarWanted = true;
                                        UI.Notify("Your storage vehicle has been identified.");
                                    }
                                    if (drugCarSaved == true && drugCar.Driver == playerPed)
                                    {
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
                                    UI.Notify("Police are not longer after your storage vehicle.");
                                }
                                if (drugCarSaved == false && vehPos != drugCar.Position && !playerPed.IsInVehicle())
                                {
                                    VehicleSave(drugCar);
                                    drugCarSaved = true;
                                }
                                if (drugCar.IsDead)
                                {
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
                                    foreach(XElement element in veh.Descendants())
                                    {
                                        if(element.Name == "Destroyed")
                                        {
                                            element.Value = drugCarDestroyed.ToString();
                                        }
                                    }
                                    veh.Save("scripts\\LSLife\\LSLife_Vehicles.xml");
                                }
                            }
                        }
                        if (TotalDrugsCarried >= 140 && !playerHasBag)
                        {
                            PedBag(playerPed, true);
                            playerHasBag = true;
                            playerPed.MaxSpeed = 6f;
                            
                        }
                        if (TotalDrugsCarried < 140 && playerHasBag)
                        {
                            PedBag(playerPed, false);
                            playerHasBag = false;
                            playerPed.MaxSpeed = 7f;
                        }
                        if (player.WantedLevel > 0 && playerWanted == false) { playerWanted = true; }
                        if (playerWanted && player.WantedLevel == 0) { playerWanted = false; }


                        if (drugCarDestroyed == false && drugCar != null && playerPed.Position.DistanceTo(drugCar.Position) > 200)
                        {
                            if (drugCar.Exists())
                            {
                                if (playerPed.IsInVehicle() && playerPed.CurrentVehicle != drugCar)
                                {
                                    drugCarBlip = World.CreateBlip(drugCar.Position);
                                    drugCarBlip.IsShortRange = true;
                                    drugCarBlip.IsFriendly = true;
                                    drugCarBlip.Sprite = BlipSprite.PersonalVehicleCar;
                                    drugCarBlip.Name = "Drug Car";
                                    drugCarBlip.Color = BlipColor.Green;
                                    drugCar.CurrentBlip.Remove();
                                    drugCar.Delete();
                                    drugCar = null;
                                }
                                if (!playerPed.IsInVehicle())
                                {
                                    drugCarBlip = World.CreateBlip(drugCar.Position);
                                    drugCarBlip.IsShortRange = true;
                                    drugCarBlip.IsFriendly = true;
                                    drugCarBlip.Sprite = BlipSprite.PersonalVehicleCar;
                                    drugCarBlip.Name = "Drug Car";
                                    drugCarBlip.Color = BlipColor.Green;
                                    drugCar.CurrentBlip.Remove();
                                    drugCar.Delete();
                                    drugCar = null;
                                }
                            }
                            

                        }
                        if (drugCarDestroyed == false && drugCar == null && drugCarBlip == null)
                        {
                            drugCarBlip = World.CreateBlip(vehPos);
                            drugCarBlip.IsShortRange = true;
                            drugCarBlip.IsFriendly = true;
                            drugCarBlip.Sprite = BlipSprite.PersonalVehicleCar;
                            drugCarBlip.Name = "Drug Car";
                            drugCarBlip.Color = BlipColor.Green;
                        }
                        if (drugDealer1 != null)
                        {
                            if (drugDealer1.IsDead)
                            {
                                RemovePedFromWorld(drugDealer1, true);
                                drugDealer1 = null;
                            }
                        }
                        if (player.WantedLevel > 0 && isDealing == true)
                        {
                            UI.Notify("Cant continue dealing, your wanted.");
                            isDealing = false;
                        }
                        if (isDealing == false && removedStuff == false)
                        {
                            removedStuff = true;
                            timerBarPool.Remove(heatBar);
                            if (customers.Count > 0)
                            {
                                foreach (Customer c in customers)
                                {
                                    if (c != null)
                                    {
                                        if (c.Ped.Exists())
                                        {
                                            RemovePedFromWorld(c.Ped, true);
                                        }
                                    }
                                }
                                customers.Clear();
                            }
                            if (oldCustomers.Count > 0)
                            {
                                foreach (Ped p in oldCustomers)
                                {
                                    if (p != null && p.Exists())
                                    {
                                        RemovePedFromWorld(p, true);
                                    }
                                }
                                oldCustomers.Clear();
                            }
                            if (jobs.Count > 0)
                            {
                                foreach (Customer c in jobs)
                                {
                                    if (c != null && c.Ped.Exists())
                                    {
                                        c.Ped.CurrentBlip.ShowRoute = false;
                                        RemovePedFromWorld(c.Ped, true);
                                    }
                                }
                                jobs.Clear();
                            }
                        }
                        if (isDealing == false)
                        {
                            if (ratChance != -100)
                            {
                                ratChance -= 1;
                            }
                        }
                        if (isDealing)
                        {
                            if (dealer1Money > 50000 && Game.GameTime > jobOfferTimer + 30000 && jobs.ToList().Count == 0)
                            {
                                jobOfferTimer = Game.GameTime;
                                int r = rnd.Next(0, 100);
                                if (r > 70)
                                {
                                    Vector3 spawnPos = GenerateSpawnPos(playerPed.Position.Around(300f), Nodetype.Offroad, true);
                                    var peds = World.GetNearbyPeds(playerPed.Position, 300f);
                                    var pedsList = peds.ToList();
                                    if (pedsList.Count > 0)
                                    {
                                        foreach (Ped p in peds)
                                        {
                                            if (!p.IsPlayer && p.IsHuman)
                                            {
                                                r = rnd.Next(0, 10);
                                                if (r > 5)
                                                {
                                                    Ped newPed = World.CreatePed(p.Model, spawnPos);
                                                    if (newPed != null)
                                                    {
                                                        jobs.Add(new Customer(newPed, spawnPos, 50, true, null));
                                                        if (jobs.Count > 0)
                                                        {
                                                            Customer c = jobs[0];
                                                            UI.Notify("Yo can you go drop " + c.amount + "g of " + c.drug + " to my pal for $" + c.amount * c.price + "?");
                                                            Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                                                            jobAcceptTimer = Game.GameTime;
                                                            jobOffer = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (drugDealer1 != null && drugDealer1.IsAlive)
                        {
                            var distanceToTarget = World.GetDistance(drugDealer1.Position, pedTarget);
                            var distanceToPlayer = World.GetDistance(drugDealer1.Position, playerPed.Position);
                            if (drugDealer1.Position.DistanceTo(playerPed.Position) > 15f && drugDealer1.CurrentPedGroup != playerPed.CurrentPedGroup)
                            {
                                if (Game.GameTime > dealerTimer + 2000 && playerPed.Position != pedTarget)
                                {
                                    dealerTimer = Game.GameTime;
                                    pedTarget = playerPed.Position;
                                    drugDealer1.Task.GoTo(playerPed);
                                }
                            }
                            else
                            {
                                if (drugDealer1.CurrentPedGroup != playerPed.CurrentPedGroup)
                                {
                                    drugDealer1.Task.ClearAll();
                                    Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, drugDealer1.Handle, playerGroup);
                                }
                            }
                            if (drugDealer1.IsStopped && distanceToPlayer <= 3f && playerPed.IsStopped && Game.GameTime > displayHelpTimer + 1000)
                            {
                                {
                                    displayHelpTimer = Game.GameTime;

                                    var peds = World.GetNearbyPeds(playerPed, 10f);
                                    if (peds.Count() > 3)
                                    {
                                        //DealerTaskUI = new UIText("Its to busy here.", new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                                        DisplayHelpText("Find some where quiter.");
                                        readyToDeal = false;
                                        if (LsMenuPool.IsAnyMenuOpen())
                                        {
                                            dealerMainMenu.Visible = !dealerMainMenu.Visible;
                                        }
                                    }
                                    else
                                    if (!readyToDeal && !LsMenuPool.IsAnyMenuOpen())
                                    {
                                        //DealerTaskUI = new UIText("This will do.", new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                                        DisplayHelpText("Press ~INPUT_CONTEXT~ to interact");
                                        readyToDeal = true;
                                    }
                                }
                            }
                            if (!drugDealer1.IsStopped)
                            {
                                readyToDeal = false;
                            }
                            if (drugDealer1 != null && !drugDealer1.IsAlive)
                            {
                                drugDealer1.IsPersistent = false;
                                drugDealer1.CurrentBlip.Remove();
                                drugDealer1.MarkAsNoLongerNeeded();
                                UI.Notify("Dealer is dead!");
                                drugDealer1 = null;
                                readyToDeal = false;
                            }
                        }
                    }
                }       

                // if we alive do this stuff each Tick.
                if (playerPed != null && playerPed.IsAlive && player.IsPlaying)
                {                    
                    if (drugCar != null)
                    {
                        if (drugCar.Exists())
                        {
                            Vector3 backPos = Vector3.Add(drugCar.Position, Vector3.Multiply(drugCar.ForwardVector * -1, (drugCar.Model.GetDimensions().Y / 2) + 0.5f));
                            if (drugCar.Driver != playerPed && playerPed.Position.DistanceTo(drugCar.Position) < 10)
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
                            if (!playerPed.IsInVehicle() && playerPed.Position.DistanceTo(backPos) <= 1f && drugCar.Model.IsCar)
                            {
                                bool trunkFound = false;
                                if (drugCar.HasBone("boot")) { trunkFound = true; }
                                if (trunkFound)
                                {
                                    if (!drugCar.IsDoorOpen(VehicleDoor.Trunk))
                                    {
                                        if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen() && playerPed.IsStopped)
                                        {
                                            displayHelpTimer = Game.GameTime;
                                            DisplayHelpText("~INPUT_SPRINT~ to open trunk.");
                                        }
                                        if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && playerPed.IsStopped)
                                        {
                                            drugCar.OpenDoor(VehicleDoor.Trunk, false, false);
                                            if (PlayerInventory.Count > 0)
                                            {
                                                putWeedVeh.Maximum = PlayerInventory["Weed"];
                                                putCrackVeh.Maximum = PlayerInventory["Crack"];
                                                putCocainVeh.Maximum = PlayerInventory["Cocain"];
                                            }
                                            putMoneyVeh.Maximum = player.Money;
                                            UpdateCarStash();
                                            vehStashMainMenu.Visible = !vehStashMainMenu.Visible;
                                        }
                                    }
                                    else
                                    {
                                        if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen() && playerPed.IsStopped)
                                        {
                                            displayHelpTimer = Game.GameTime;
                                            DisplayHelpText("~INPUT_SPRINT~ to close trunk.");
                                        }
                                        if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && playerPed.IsStopped)
                                        {
                                            drugCar.CloseDoor(VehicleDoor.Trunk, false);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!drugCar.IsDoorOpen(VehicleDoor.BackLeftDoor) && !LsMenuPool.IsAnyMenuOpen() && playerPed.IsStopped)
                                    {
                                        if (Game.GameTime > displayHelpTimer + 3000)
                                        {
                                            displayHelpTimer = Game.GameTime;
                                            DisplayHelpText("~INPUT_SPRINT~ to back doors.");
                                        }
                                        if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && playerPed.IsStopped)
                                        {
                                            drugCar.OpenDoor(VehicleDoor.BackLeftDoor, false, false);
                                            drugCar.OpenDoor(VehicleDoor.BackRightDoor, false, false);

                                            vehStashMainMenu.Visible = !vehStashMainMenu.Visible;
                                        }
                                    }
                                    else
                                    {
                                        if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen() && playerPed.IsStopped)
                                        {
                                            displayHelpTimer = Game.GameTime;
                                            DisplayHelpText("~INPUT_SPRINT~ to close back doors.");
                                        }
                                        if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && playerPed.IsStopped)
                                        {
                                            drugCar.CloseDoor(VehicleDoor.BackLeftDoor, false);
                                            drugCar.CloseDoor(VehicleDoor.BackRightDoor, false);
                                        }
                                    }
                                }
                            }                            
                        }
                    }
                    
                    Housing();                    
                    //Buying Drugs
                                      
                    if (drugDealer1 != null && readyToDeal)
                    {
                        if (Game.IsControlJustPressed(2, GTA.Control.Context) && !LsMenuPool.IsAnyMenuOpen())
                        {
                            if (LsMenuPool != null)
                            {
                                dealerMainMenu.Visible = !dealerMainMenu.Visible;
                            }
                        }
                        if (Game.IsControlJustPressed(2, GTA.Control.ContextSecondary) & chilling == false && !LsMenuPool.IsAnyMenuOpen())
                        {
                            Wait(10);
                            float dHeading = drugDealer1.Heading;
                            float pHeading = playerPed.Heading;
                            drugDealer1.Task.StartScenario("WORLD_HUMAN_SMOKING_POT", drugDealer1.Position);
                            drugDealer1.Heading = dHeading;
                            Wait(10);
                            playerPed.Task.StartScenario("WORLD_HUMAN_SMOKING_POT", playerPed.Position);
                            playerPed.Heading = pHeading;
                            chilling = true;
                        }
                        if (chilling)
                        {
                            if (Game.IsControlJustPressed(2, Control.MoveLeftRight) || Game.IsControlJustPressed(2, Control.MoveUpDown))
                            {
                                chilling = false;
                                playerPed.Task.ClearAll();
                                drugDealer1.Task.ClearAll();
                            }
                        }
                    }
                    //STREET DEALING                                  
                    if (isDealing == true)
                    {
                        if (removedStuff == true)
                        {
                            removedStuff = false;
                        }                        
                        if (jobs.Count > 0 && jobs[0] != null && !LsMenuPool.IsAnyMenuOpen())
                        {
                            if (jobOffer)
                            {
                                if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen())
                                {
                                    Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                    displayHelpTimer = Game.GameTime;
                                    DisplayHelpText("~INPUT_CONTEXT_SECONDARY~ to reject or ~INPUT_CONTEXT~ to accept");
                                }
                                if (Game.IsControlJustPressed(2, GTA.Control.ContextSecondary) && !LsMenuPool.IsAnyMenuOpen())
                                {
                                    jobOffer = false;
                                    if (jobs[0].Ped != null)
                                    {
                                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                        RemovePedFromWorld(jobs[0].Ped, true);
                                        jobs.Clear();
                                    }
                                }
                                if (Game.IsControlJustPressed(2, GTA.Control.Context) && !LsMenuPool.IsAnyMenuOpen())
                                {
                                    Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                    jobOffer = false;
                                    jobs[0].Ped.CurrentBlip.IsShortRange = false;
                                    jobs[0].Ped.CurrentBlip.ShowRoute = true;
                                    jobAccepted = true;
                                    Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                                    UI.Notify("Cool, Sent you a ping.");
                                }
                                if (Game.GameTime > jobAcceptTimer + 15000)
                                {
                                    Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                                    UI.Notify("Too slow man, got someone.");
                                    Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                    jobOffer = false;
                                    if (jobs[0].Ped != null)
                                    {
                                        RemovePedFromWorld(jobs[0].Ped, true);
                                        jobs.Clear();
                                    }
                                }
                            }
                            if (jobs.Count > 0 && jobAccepted)
                            {
                                foreach (Customer j in jobs)
                                {
                                    //j.Ped.CurrentBlip.ShowRoute = true;
                                    if (j != null)
                                    {
                                        if (j.Ped.CurrentPedGroup != playerPed.CurrentPedGroup && j.Ped.Position.DistanceTo(playerPed.Position) <= 10f)
                                        {
                                            j.Ped.Task.ClearAll();
                                            Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, j.Ped.Handle, playerGroup);
                                            j.Ped.CurrentBlip.ShowRoute = false;
                                        }
                                        if (j.Ped.IsInRangeOf(playerPed.Position, 2f))
                                        {
                                            if (TotalDrugsCarried == 0)
                                            {
                                                isDealing = false;
                                                UI.Notify("You got nothing to sell.");
                                                RemovePedFromWorld(j.Ped, true);
                                                jobs.Remove(j);
                                                lastTimeDealDone = Game.GameTime;
                                                jobAccepted = false;
                                                break;
                                            }
                                            else
                                            {
                                                if (playerPed.IsInVehicle() == true && j.Ped.IsInVehicle() == true)
                                                {
                                                    if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen())
                                                    {
                                                        displayHelpTimer = Game.GameTime;
                                                        DisplayHelpText("~INPUT_SPRINT~ to sell to this person.");
                                                    }
                                                    if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen())
                                                    {
                                                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                                        customerSellTo = j;
                                                        sell.Text = "Got " + j.amount + "g of " + j.drug + "?";
                                                        customerMainMenu.Subtitle.Caption = "You have " + PlayerInventory[j.drug] + "g.";
                                                        customerMainMenu.Visible = !customerMainMenu.Visible;
                                                        jobAccepted = false;
                                                        break;
                                                    }
                                                }
                                                if (playerPed.IsInVehicle() == false)
                                                {
                                                    if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen())
                                                    {
                                                        displayHelpTimer = Game.GameTime;
                                                        DisplayHelpText("Press ~INPUT_SPRINT~ to sell to this person.");
                                                    }
                                                    if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen())
                                                    {
                                                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                                        customerSellTo = j;
                                                        sell.Text = "Got " + j.amount + "g of " + j.drug + "?";
                                                        customerMainMenu.Subtitle.Caption = "You have " + PlayerInventory[j.drug] + "g of " + j.drug;
                                                        customerMainMenu.Visible = !customerMainMenu.Visible;
                                                        jobAccepted = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Get rid of or decide if old ped is angry
                        if (oldCustomers.ToList().Count > 0)
                        {
                            for (var i = 0; i < oldCustomers.Count; i++)
                            {

                                var ped = oldCustomers[i];
                                if (ped == null)
                                {
                                    oldCustomers.RemoveAt(i);
                                    break;
                                }
                                if (ped != null && !ped.Exists())
                                {
                                    oldCustomers.RemoveAt(i);
                                    break;
                                }
                                if (ped != null && ped.IsDead)
                                {
                                    oldCustomers.RemoveAt(i);
                                    break;
                                }
                                if (ped != null && ped.IsAlive)
                                {
                                    int anger = rnd.Next(0, 100);
                                    if (anger > 83 + wealth && Game.GameTime > lastAngryTime + 10000)
                                    {
                                        if (ped.Gender.Equals(Gender.Male))
                                        {
                                            ped.CanSwitchWeapons = true;
                                            ped.Weapons.Give(WeaponHash.Bat, 1, true, false);
                                            ped.Weapons.Select(ped.Weapons.BestWeapon);           
                                            ped.Task.FightAgainst(playerPed);
                                            lastAngryTime = Game.GameTime;

                                            oldCustomers.Remove(ped);
                                            angryPeds.Add(ped);
                                        }
                                    }
                                    if (anger < 86 + wealth && Game.GameTime > lastAngryTime + 10000)
                                    {
                                        lastAngryTime = Game.GameTime;
                                    }
                                }
                            }
                        }
                        //select ped for deal
                        if (customers.Count < 5 && jobs.Count == 0)
                        {
                            if (Game.GameTime > lastTimePedSelected + 5000)
                            {
                                lastTimePedSelected = Game.GameTime;
                                int getPed = rnd.Next(0, 100);
                                if (getPed > 65)
                                {
                                    
                                    Vector3 playerPos = Game.Player.Character.Position;
                                    foreach (Ped p in World.GetAllPeds())
                                    {
                                        if (p.IsHuman && !p.IsPlayer && p.IsAlive)
                                        {
                                            float dist = p.Position.DistanceTo(playerPos);
                                            //Ped is within 30m and its heading the player's way
                                            //if (dist < 50f && (p.Position + p.Velocity).DistanceTo(playerPos) < dist)
                                            if (dist < 40f)
                                            {
                                                //None of the current Customers has the same Handle as this ped
                                                //So this ped is not a registered customer
                                                if (customers.FindAll(r => r.Ped.Handle == p.Handle).Count == 0)
                                                {
                                                    if (oldCustomers.ToList().FindAll(r => r.Handle == p.Handle).Count == 0)
                                                    {
                                                        if (angryPeds.ToList().FindAll(r => r.Handle == p.Handle).Count == 0)
                                                        {
                                                            if (p.CurrentPedGroup != playerPed.CurrentPedGroup)
                                                            {
                                                                if (p.IsInVehicle() && p.CurrentVehicle.Driver == p)
                                                                {
                                                                    Vehicle vehicle = p.CurrentVehicle;
                                                                    customers.Add(new Customer(p, playerPos, wealth, false, vehicle));
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    customers.Add(new Customer(p, playerPos, wealth, false, null));
                                                                    break;
                                                                }                                                                
                                                            }                                                            
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //do the deal
                        if (customers.Count > 0)
                        {
                            if (Game.GameTime > lastCustomerCheckTimer + 370)
                            {                                
                                foreach (Customer c in customers)
                                {
                                    if (c != null)
                                    {
                                        if (c.Ped.Exists() && c.Ped.IsAlive && !c.Ped.IsPlayer)
                                        {
                                            if (c.Ped.Position.DistanceTo(playerPed.Position) > 60f)
                                            {
                                                customers.Remove(c);
                                                RemovePedFromWorld(c.Ped, true);
                                                break;
                                            }
                                            if (c.Ped.CurrentPedGroup != playerPed.CurrentPedGroup && c.Ped.Position.DistanceTo(playerPed.Position) > 3f)
                                            {
                                                if (c.Ped.IsInVehicle())
                                                {
                                                    c.Ped.Task.PerformSequence(PerformTaskSeq(2, c.Ped.CurrentVehicle));                                                    
                                                    lastCustomerCheckTimer = Game.GameTime;
                                                }
                                                else
                                                {
                                                    if (c.Target != playerPed.Position)
                                                    {
                                                        c.Target = playerPed.Position;
                                                        MoveCustomer(c, c.Target);
                                                        //c.Ped.Task.PerformSequence(PerformTaskSeq(2, c.Ped.CurrentVehicle));
                                                        lastCustomerCheckTimer = Game.GameTime;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }                            
                            foreach (Customer c in customers)
                            {
                                if (c != null)
                                {
                                    if (c.Ped.Exists() == true && c.Ped.IsAlive && !c.Ped.IsPlayer)
                                    {
                                        
                                        if (c.Ped.CurrentPedGroup != playerPed.CurrentPedGroup && c.Ped.Position.DistanceTo(playerPed.Position) <= 3f)
                                        {
                                            c.Ped.Task.ClearAll();
                                            Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, c.Ped.Handle, playerGroup);
                                            Function.Call(Hash._PLAY_AMBIENT_SPEECH1, c.Ped, "GENERIC_HI", "SPEECH_PARAMS_STANDARD");
                                        }
                                        if (c.Ped.IsInRangeOf(playerPed.Position, 2f))
                                        {
                                            if (TotalDrugsCarried == 0)
                                            {
                                                isDealing = false;
                                                UI.Notify("You ran out of drugs, go buy somemore.");
                                                RemovePedFromWorld(c.Ped, true);
                                                customers.Remove(c);
                                                lastTimeDealDone = Game.GameTime;
                                                break;
                                            }
                                            else
                                            {
                                                if (playerPed.IsInVehicle() == true && c.Ped.IsInVehicle() == true && c.Ped.CurrentVehicle.Speed < 1)
                                                {
                                                    if (Game.GameTime > displayHelpTimer + 3000)
                                                    {
                                                        displayHelpTimer = Game.GameTime;
                                                        DisplayHelpText("~INPUT_SPRINT~ to sell from car.");
                                                    }
                                                    if (Game.IsControlJustPressed(2, GTA.Control.Sprint))
                                                    {
                                                        customerSellTo = c;
                                                        sell.Text = "Got " + customerSellTo.amount + "g of " + customerSellTo.drug + "?";
                                                        customerMainMenu.Subtitle.Caption = "You have " + PlayerInventory[customerSellTo.drug] + "g.";
                                                        customerMainMenu.Visible = !customerMainMenu.Visible;
                                                        break;
                                                    }
                                                }
                                                if (playerPed.IsInVehicle() == false && playerPed.IsStopped && !playerPed.IsInCombat)
                                                {
                                                    if (Game.GameTime > displayHelpTimer + 3000)
                                                    {
                                                        displayHelpTimer = Game.GameTime;
                                                        DisplayHelpText("Press ~INPUT_SPRINT~ to sell to this person.");
                                                    }
                                                    if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen())
                                                    {
                                                        customerSellTo = c;
                                                        sell.Text = "Got " + customerSellTo.amount + "g of " + customerSellTo.drug + "?";
                                                        customerMainMenu.Subtitle.Caption = "You have " + PlayerInventory[customerSellTo.drug] + "g of " + customerSellTo.drug;
                                                        customerMainMenu.Visible = !customerMainMenu.Visible;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (c.Ped.IsDead)
                                    {
                                        customers.Remove(c);
                                        RemovePedFromWorld(c.Ped, true);
                                        break;
                                    }
                                }
                            }
                        }
                        if (Game.GameTime > ratTimer + 1000)
                        {
                            timeDealing = Game.GameTime - timeStartedDealing;
                            thisStreet = World.GetStreetName(new Vector2(playerPed.Position.X, playerPed.Position.Y));
                            ratTimer = Game.GameTime;
                            rat = rnd.Next(0, 1000);
                            if (thisStreet == lastStreet && rat < ratChance + (timeDealing / 60000) && timeDealing > 60000)
                            {
                                UI.Notify("Someone ratted on you.");
                                Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                                player.WantedLevel = 2;
                                isDealing = false;
                                ratChance = 0;
                            }

                            if (thisStreet != lastStreet)
                            {
                                lastStreet = thisStreet;
                                ratChance -= 10;
                                if (ratChance < -100)
                                { ratChance = -100; }
                                timeStartedDealing = Game.GameTime;                                
                            }
                            else
                            {
                                if (Game.GameTime > lastTimeDealDone + 10000)
                                {
                                    if (ratChance > -100)
                                    {

                                        ratChance--;
                                        ratChance += timeDealing / 180000;
                                        if (ratChance <= 0f)
                                        {
                                            if (heatBar.Percentage != 0f) { heatBar.Percentage = 0f; }
                                        }
                                        else
                                        {
                                            if (heatBar.Percentage > 0f) { heatBar.Percentage -= 0.1f; }
                                        }


                                    }

                                }
                                else
                                {
                                    if (ratChance < 100)
                                    {
                                        if (wealth == 1) { ratChance += wealth + (timeDealing / 60000); }
                                        else { ratChance += (wealth / 2) + (timeDealing / 60000); }
                                        if (ratChance > 0)
                                        {
                                            if (heatBar.Percentage < 1f) { heatBar.Percentage += 0.1f; }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (wepDealer != null)
                    {
                        if (!playerPed.IsInVehicle())
                        {
                            if (wepDealer.vehicle.Position.DistanceTo(playerPed.Position) < 3)
                            {
                                if (Game.GameTime > displayHelpTimer + 3000)
                                {
                                    displayHelpTimer = Game.GameTime;
                                    DisplayHelpText("Press ~INPUT_VEH_EXIT~ to enter.");
                                }
                                if (Game.IsControlJustPressed(2, GTA.Control.VehicleExit))
                                {
                                    playerPed.Task.EnterVehicle(wepDealer.vehicle, VehicleSeat.Passenger);
                                }
                            }
                        }
                        if (playerPed.IsInVehicle())
                        {
                            if (playerPed.CurrentVehicle.Handle == wepDealer.vehicle.Handle)
                            {
                                if (Game.IsWaypointActive && !wepDealer.isDriving)
                                {
                                    if (wepDealer.destination != World.GetNextPositionOnStreet(World.GetWaypointPosition()))
                                    {
                                        //float maxSpeed = Function.Call<float>(Hash._GET_VEHICLE_MAX_SPEED, wepDealer.vehicle.Handle);
                                        wepDealer.destination = World.GetNextPositionOnStreet(World.GetWaypointPosition());
                                        wepDealer.isDriving = true;
                                        wepDealer.vehicle.IsInvincible = true;
                                        wepDealer.ped.Task.ClearAll();
                                        wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 1, 40f, (int)DrivingStyle.SometimesOvertakeTraffic);
                                        UI.Notify("Leaving.");
                                        //Function.Call(Hash.SET_WAYPOINT_OFF);

                                    }
                                }
                                if (wepDealer.isDriving)
                                {
                                    if (wepDealer.vehicle.Position.DistanceTo(wepDealer.destination) < 10)
                                    {
                                        UI.Notify("Arived.");
                                        wepDealer.destination = Vector3.Zero;
                                        wepDealer.isDriving = false;
                                    }
                                }
                                if (!wepDealer.doneTutorial)
                                {
                                    wepDealer.doneTutorial = true;
                                    displayHelpTimer = Game.GameTime;
                                    DisplayHelpText("Press ~INPUT_CONTEXT~to interact or ~INPUT_CONTEXT_SECONDARY~to dismiss.");
                                }
                                if (Game.IsControlJustPressed(2, GTA.Control.ContextSecondary))
                                {
                                    playerPed.Task.LeaveVehicle(playerPed.CurrentVehicle, true);
                                    Wait(500);
                                    wepDealer.ped.IsPersistent = false;
                                    wepDealer.vehicle.IsPersistent = false;
                                    wepDealer.vehicle.CurrentBlip.Remove();
                                    Function.Call(Hash.TASK_VEHICLE_DRIVE_WANDER, wepDealer.ped.Handle, wepDealer.vehicle.Handle, 20f, (int)DrivingStyle.Normal);
                                    wepDealer = null;
                                }
                                if (Game.IsControlJustPressed(2, GTA.Control.Context))
                                {
                                    //playerPed.Task.LeaveVehicle(playerPed.CurrentVehicle, true);
                                }
                            }
                        }
                    }
                }
            }            
        }



        //!!!!!!!!!!!!!!!!!!!!!!

        void VehicleSave(Vehicle veh)
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
        
        void VehicleLoad()
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
                        }
                        if (e.Name == "EngineHealth")
                        {
                            Int32.TryParse(e.Value, out engineHealth);                            
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
            drugCar.InstallModKit();
            if (DEBUG)
            {
                UI.Notify("" + drugCarModel + " | " + drugCarPos + " | " + playerPed.Position);
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

        void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == SetDrugCar && playerPed.IsAlive)
            {
                if (playerPed.IsInVehicle())
                {
                    if (drugCar != null)
                    {
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
                    }
                    VehicleSave(playerPed.CurrentVehicle);
                    Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                    UI.Notify("" + drugCar.Model + " is now youre Stash Vehicle");
                }
                if (!playerPed.IsInVehicle())
                {
                    Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                    UI.Notify("Please enter a Vehicle.");
                }
            }
            if (e.KeyCode == StopDealing && playerPed.IsAlive)
            {
                if (isDealing)
                {
                    isDealing = false;
                    UI.Notify("Street dealing cancel");
                    Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                }
                if (!isDealing)
                {
                    UI.Notify("Street dealing is not in progress.");
                    Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                }

            }
            if (e.KeyCode == Keys.J && started == false)
            {
                if(DEBUG)
                {
                    float ground = World.GetGroundHeight(playerPed.Position);
                    //UI.Notify("" + Function.Call<bool>(Hash.IS_POINT_ON_ROAD, playerPed.Position.X, playerPed.Position.Y, playerPed.Position.Z));
                    if (!attackerPickupSpawned)
                    {
                        int drugs = 0;
                        attackerPickupSpawned = true;
                        attackerPickup = World.CreatePickup(PickupType.MoneyMedBag, playerPed.Position + (playerPed.ForwardVector * 2), Vector3.Zero, new Model("prop_big_bag_01"), 100);
                        foreach(XElement element in playerWeapons)
                        {
                            if (element.Name == "weapon")
                            {
                                attackerWeapons.Add(element);
                                UI.Notify("added " + element.Name);
                            }
                        }
                        foreach (KeyValuePair<string, int> entry in PlayerInventory)
                        {
                            attackerInventory.Add(entry.Key, entry.Value);
                            drugs += entry.Value;
                        }
                        UI.Notify("drugs = " + drugs);
                    }
                }
            }
                
            //if (e.KeyCode == Keys.J && started == false)
            //{                
            //    if (playerPed.IsInVehicle())
            //    {
            //        Vehicle veh = playerPed.CurrentVehicle;
            //        drugCar.RimColor = VehicleColor.MetallicRed;
            //        //foreach (VehicleHash veh2 in Enum.GetValues(typeof(VehicleHash)))
            //        //{

            //        //}
            //    }
            //}
            //{
            //    drugCar = World.CreateVehicle(new Model(drugCarModel), drugCarPos, drugCarHead);
            //    UI.Notify("" + drugCarModel + " | " + drugCarPos + " | " + playerPed.Position);
            //    drugCar.IsPersistent = true;
            //    drugCar.AddBlip();
            //    drugCar.CurrentBlip.IsShortRange = true;
            //    drugCar.CurrentBlip.IsFriendly = true;
            //    drugCar.CurrentBlip.Sprite = BlipSprite.PersonalVehicleCar;
            //    drugCar.CurrentBlip.Name = "Drug Car";
            //    drugCar.CurrentBlip.Color = BlipColor.Green;
            //    Function.Call(Hash.SET_VEHICLE_COLOURS, drugCar, drugCarPriCol, drugCarSecCol);
            //}
            //{
            //    if (playerPed.IsInVehicle())
            //    {
            //        started = true;
            //        Vehicle veh = playerPed.CurrentVehicle;
            //        XDocument vehiclesXML = XDocument.Load("scripts\\LSLife\\Vehicle.xml");                    
            //        var rootElement = vehiclesXML.Root;
            //        var parentElement = new XElement("Vehicle");
            //        var modsElement = new XElement("Mods");
            //        var neonsElement = new XElement("Neons");
            //        parentElement.Add(new XElement("Name", veh.DisplayName));
            //        parentElement.Add(new XElement("PosX", veh.Position.X));
            //        parentElement.Add(new XElement("PosY", veh.Position.Y));
            //        parentElement.Add(new XElement("PosZ", veh.Position.Z));
            //        parentElement.Add(new XElement("Heading", veh.Heading));
            //        parentElement.Add(new XElement("PrimaryCol", veh.PrimaryColor));
            //        parentElement.Add(new XElement("SecondaryCol", veh.SecondaryColor));
            //        parentElement.Add(new XElement("Health", veh.BodyHealth));
            //        parentElement.Add(new XElement("EngineHealth", veh.EngineHealth));
            //        parentElement.Add(new XElement("WheelType", veh.WheelType));
            //        parentElement.Add(new XElement("RimCol", veh.RimColor));
            //        parentElement.Add(new XElement("SmokeCol", veh.TireSmokeColor));
            //        parentElement.Add(new XElement("NeonCol", veh.NeonLightsColor));
            //        parentElement.Add(new XElement("TrimCol", veh.TrimColor));
            //        parentElement.Add(new XElement("DashCol", veh.DashboardColor));
            //        parentElement.Add(new XElement("NumPlateType", veh.NumberPlateType));
            //        parentElement.Add(new XElement("NumPlate", veh.NumberPlate));
            //        parentElement.Add(new XElement("WindowTint", veh.WindowTint));
            //        parentElement.Add(new XElement("DirtLevel", veh.DirtLevel));
            //        parentElement.Add(new XElement("CanTiresBurst", veh.CanTiresBurst));
            //        parentElement.Add(new XElement("Fuel", veh.FuelLevel));
            //        if(Function.Call<bool>(Hash.GET_VEHICLE_MOD_VARIATION, veh.Handle, (int)VehicleMod.FrontWheels))
            //        {
            //            parentElement.Add(new XElement("CustomTire", true));
            //        }
            //        else
            //        {
            //            parentElement.Add(new XElement("CustomTire", false));
            //        }
            //        foreach (VehicleMod mod in Enum.GetValues(typeof(VehicleMod)))
            //        {                                            
            //            modsElement.Add(new XElement(mod.ToString(), veh.GetMod(mod)));                                              
            //        }
            //        foreach (VehicleToggleMod mod in Enum.GetValues(typeof(VehicleToggleMod)))
            //        {                     
            //            modsElement.Add(new XElement(mod.ToString(), veh.IsToggleModOn(mod)));
            //        }
            //        foreach (VehicleNeonLight neon in Enum.GetValues(typeof(VehicleNeonLight)))
            //        {
            //            neonsElement.Add(new XElement(neon.ToString(), veh.IsNeonLightsOn(neon)));
            //        }
            //        parentElement.Add(modsElement);
            //        parentElement.Add(neonsElement);
            //        rootElement.Add(parentElement);
            //        vehiclesXML.Save("scripts\\LSLife\\Vehicle.xml");
            //    }                
            //}
            if (e.KeyCode == MenuKey)
            {
                if (playerPed != null && playerPed.IsAlive)
                {
                    //DisplayHelpText("You have " + TotalDrugsCarried + "g total.");
                    Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                    //UI.Notify("Debt $" + dealer1Debt + ", Totaldrugs:" + TotalDrugsCarried + "g, Dealing:" + isDealing);
                    UI.Notify("Weed " + PlayerInventory["Weed"] + "g, Crack " + PlayerInventory["Crack"] + "g, Coke " + PlayerInventory["Cocain"] + "g.");
                    //UI.Notify(""+playerPed.Position);
                    //Function.Call(Hash.SET_STATE_OF_CLOSEST_DOOR_OF_TYPE, 1417775309, )


                    //
                    //int ID = 0;
                    //int newID = 0;
                    //foreach (XElement element in xdoc.Descendants())
                    //{
                    //    if (element.Name == "weapon")
                    //    {
                    //        ID++;
                    //        if (ID.ToString() != element.FirstAttribute.Value)
                    //        { newID = ID; }
                    //        Console.WriteLine(element.Name);
                    //        Console.WriteLine(element.FirstAttribute.Value);
                    //    }
                    //}
                    //var weaponHash = playerPed.Weapons.Current.Hash;
                    //var parentElement = new XElement("weapon");
                    //parentElement.SetAttributeValue("id", newID);
                    //var weaponNameElement = new XElement("HASH", weaponHash);
                    //var weaponAmmoElement = new XElement("AMMO", "0");
                    //parentElement.Add(weaponNameElement);
                    //parentElement.Add(weaponAmmoElement);
                    //var rootElement = xdoc.Element("weapons");
                    //rootElement?.Add(parentElement);
                    //xdoc.Save("Scripts\\Weapons.xml");
                    //playerPed.Weapons.Remove(playerPed.Weapons.Current);


                    //if (jobs.Count == 0)
                    //{
                    //    Vector3 spawnPos = GenerateSpawnPos(playerPed.Position.Around(300f), Nodetype.Offroad, false);
                    //    Ped p = World.CreatePed(PedHash.ArmGoon02GMY, spawnPos);
                    //    jobs.Add(new Customer(p, spawnPos, 50, true));
                    //    Customer c = jobs[0];
                    //    UI.Notify("Yo can you go drop " + c.amount + "g of " + c.drug + " to my pal for $" + c.amount * c.price + "?");                        
                    //    Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                    //    jobAcceptTimer = Game.GameTime;
                    //    jobOffer = true;
                    //}
                }
            }
        }

        void DisplayHelpText(string text)
        {
            InputArgument[] arguments = new InputArgument[] { "STRING" };
            Function.Call(Hash._0x8509B634FBE7DA11, arguments);
            InputArgument[] argumentArray2 = new InputArgument[] { text };
            Function.Call(Hash._0x6C188BE134E074AA, argumentArray2);
            InputArgument[] argumentArray3 = new InputArgument[] { 0, 0, 1, -1 };
            Function.Call(Hash._0x238FFE5C7B0498A6, argumentArray3);
        }

        public enum PedVariationData
        {
            PED_VARIATION_FACE = 0,
            PED_VARIATION_HEAD = 1,
            PED_VARIATION_HAIR = 2,
            PED_VARIATION_TORSO = 3,
            PED_VARIATION_LEGS = 4,
            PED_VARIATION_HANDS = 5,
            PED_VARIATION_FEET = 6,
            PED_VARIATION_EYES = 7,
            PED_VARIATION_ACCESSORIES = 8,
            PED_VARIATION_TASKS = 9,
            PED_VARIATION_TEXTURES = 10,
            PED_VARIATION_TORSO2 = 11
        }

        void PedBag(Ped ped, bool give)
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

        void callDealer()
        {
            Vector3 dealer1pos;
            dealer1pos = World.GetNextPositionOnSidewalk(playerPed.Position.Around(60f));
            drugDealer1 = World.CreatePed(PedHash.ArmBoss01GMM, GenerateSpawnPos(dealer1pos, Nodetype.AnyRoad, true));
            pedTarget = playerPed.Position;
            drugDealer1.AddBlip();
            drugDealer1.CurrentBlip.IsFriendly = true;
            drugDealer1.CanSwitchWeapons = true;
            drugDealer1.Weapons.Give(WeaponHash.Pistol, 9999, false, true);
            drugDealer1.Armor = 100;
            Function.Call(Hash.SET_PED_COMBAT_ABILITY, drugDealer1, 100);
            readyToDeal = false;
            Wait(100);
            drugDealer1.Task.GoTo(pedTarget);
            dealerTimer = Game.GameTime;
            LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
            foreach (XElement element in LSLifeSave.Descendants())
            {                                             
                if (element.Name == "dealer1")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "debt")
                        {
                            Int32.TryParse(e.Value, out dealer1Debt);
                        }
                        if (e.Name == "weed")
                        {
                            Int32.TryParse(e.Value, out dealer1Weed);
                        }
                        if (e.Name == "crack")
                        {
                            Int32.TryParse(e.Value, out dealer1Crack);
                        }
                        if (e.Name == "cocain")
                        {
                            Int32.TryParse(e.Value, out dealer1Cocain);
                        }
                        if (e.Name == "money")
                        {
                            Int32.TryParse(e.Value, out dealer1Money);
                        }
                        if (e.Name == "reloaded")
                        {
                            Boolean.TryParse(e.Value, out dealer1Reloaded);
                        }
                    }
                }                
            }
            bushWeed.Maximum = dealer1Weed;
            crack.Maximum = dealer1Crack;
            cocain.Maximum = dealer1Cocain;
        }

        void Housing()
        {
            if (isAtHome == false)
            {
                if (playerPed.Position.DistanceTo(houseTeleIn) < 30f)
                {
                    DisplayHelpText("isAthome = " + isAtHome + ", changing it to true");
                    isAtHome = true;
                }

                if (playerPed.Position.DistanceTo(houseTeleOut) < 10f)
                {
                    World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(-112.616f, -1479.229f, 36f), Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 1f), Color.Green);
                    if (playerPed.Position.DistanceTo(houseTeleOut) < 1f)
                    {
                        DisplayHelpText("Press ~INPUT_CONTEXT~ to enter Safehouse.");
                    }
                    if (Game.IsControlJustReleased(2, GTA.Control.Context) && playerPed.Position.DistanceTo(houseTeleOut) < 2f)
                    {
                        playerPed.FreezePosition = true;
                        Game.FadeScreenOut(300);
                        Script.Wait(300);
                        playerPed.Position = houseTeleIn;
                        Script.Wait(300);
                        playerPed.FreezePosition = false;
                        Game.FadeScreenIn(300);
                        isAtHome = true;
                    }
                }
            }
            if (isAtHome == true)
            {
                World.DrawMarker(MarkerType.UpsideDownCone, new Vector3(houseStash.X, houseStash.Y, houseStash.Z-0.5f), Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 0.5f), Color.Green);
                if (pickupSpawned)
                {
                    if (!healthPickup.ObjectExists())
                    {
                        healthPickup.Delete();
                        pickupSpawned = false;
                    }
                }
                if (!pickupSpawned)
                {
                    pickupSpawned = true;
                    healthPickup = World.CreatePickup(PickupType.Health, houseHealthPickPos, Vector3.Zero, new Model("prop_ld_health_pack"), 100);

                }

                if (playerPed.Position.DistanceTo(houseTeleIn) < 1f)
                {
                    DisplayHelpText("Press ~INPUT_CONTEXT~ to exit Safehouse.");
                }
                if (Game.IsControlJustReleased(2, GTA.Control.Context) && playerPed.Position.DistanceTo(houseTeleIn) < 2f)
                {
                    playerPed.FreezePosition = true;
                    Game.FadeScreenOut(600);
                    Script.Wait(600);
                    playerPed.Position = houseTeleOut;
                    Script.Wait(300);
                    playerPed.FreezePosition = false;
                    Game.FadeScreenIn(300);
                    isAtHome = false;
                }
                if (playerPed.Position.DistanceTo(houseStash) < 1f)
                {
                    DisplayHelpText("Press ~INPUT_CONTEXT~ to open stash.");
                    if (Game.IsControlJustReleased(2, GTA.Control.Context) && !LsMenuPool.IsAnyMenuOpen())
                    {
                        UpdateInventory();
                        stashMainMenu.Visible = !stashMainMenu.Visible;
                    }
                }
            }
        }

        void UpdateCarStash()
        {
            putWeedVeh.Text = "Weed " + PlayerInventory["Weed"] + "g";
            takeWeedVeh.Text = "Weed " + drugCarWeed + "g";
            putCrackVeh.Text = "Crack " + PlayerInventory["Crack"] + "g";
            takeCrackVeh.Text = "Crack " + drugCarCrack + "g";
            putCocainVeh.Text = "Cocain " + PlayerInventory["Cocain"] + "g";
            takeCocainVeh.Text = "Cocain " + drugCarCocain + "g";
            putMoneyVeh.Text = "Money $ " + player.Money;
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

        void UpdateInventory()
        {

            putWeedStash.Maximum = PlayerInventory["Weed"];
            putCrackStash.Maximum = PlayerInventory["Crack"];
            putCocainStash.Maximum = PlayerInventory["Cocain"];
            putMoneyStash.Maximum = player.Money;
            putWeedStash.Text = "Weed " + PlayerInventory["Weed"] + "g.";
            putCrackStash.Text = "Crack " + PlayerInventory["Crack"] + "g.";
            putCocainStash.Text = "Cocain " + PlayerInventory["Cocain"] + "g.";
            putMoneyStash.Text = "Money $" + player.Money + ".";
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
            takeWeedStash.Maximum = stashWeed;
            takeCrackStash.Maximum = stashCrack;
            takeCocainStash.Maximum = stashCocain;
            takeMoneyStash.Maximum = stashMoney;
            takeWeedStash.Text = "Weed " + stashWeed + "g.";
            takeCrackStash.Text = "Crack " + stashCrack + "g.";
            takeCocainStash.Text = "Cocain " + stashCocain + "g.";
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

        void RemovePedFromWorld(Ped ped, bool clearTask)
        {
            ped.CurrentBlip.Remove();
            ped.LeaveGroup();
            if (clearTask)
            {
                ped.Task.ClearAll();
            }            
            ped.MarkAsNoLongerNeeded();
            ped.BlockPermanentEvents = true;
            ped.IsPersistent = false;
        }

        void DateTimeUpdate()
        {
            if (Function.Call<int>(Hash.GET_CLOCK_HOURS) != hour)
            {

                hour = Function.Call<int>(Hash.GET_CLOCK_HOURS);
                if (dealer1Weed == 0 || dealer1Crack == 0 || dealer1Cocain == 0)
                {
                    if (!dealer1ReloadOrder)
                    {
                        dealer1ReloadOrder = true;
                        if (day == 6)
                        {
                            dealer1ReloadDay = 0;
                        }
                        else
                        {
                            dealer1ReloadDay = day + 1;
                        }
                        UI.Notify("Zee has ordered a reload, will take about a day.");
                        Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                    }
                }

            }
            if (Function.Call<int>(Hash.GET_CLOCK_DAY_OF_WEEK) != day)
            {
                
                day = Function.Call<int>(Hash.GET_CLOCK_DAY_OF_WEEK);
                LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                if (day != dealer1ReloadDay && dealer1Reloaded)
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
                                }                                
                            }
                        }
                    }
                    
                }
                if (dealer1Weed > 0)
                {
                    dealer1Weed -= 10;
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
                            }
                        }
                    }
                }
                if (dealer1Crack > 0)
                {
                    dealer1Crack -= 1;
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
                            }
                        }
                    }
                }
                if (dealer1Cocain > 0)
                {
                    dealer1Cocain -= 1;
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
                            }
                        }
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
            }
            if (day == dealer1ReloadDay && !dealer1Reloaded)
            {
                dealer1ReloadOrder = false;
                dealer1Reloaded = true;
                if (dealer1Debt > 0)
                {
                    dealer1Debt += dealer1Debt / 4;
                }                
                if (dealer1Weed < 20) { dealer1Weed += 20; }
                if (dealer1Crack < 4) { dealer1Crack += 4; }
                if (dealer1Cocain < 2) { dealer1Cocain += 2; }
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
                UI.Notify("Zees runner Reloaded.");
                Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
            }
        }

        public static TaskSequence PerformTaskSeq(int index, Vehicle vehicle)
        {
            if (index == 0)
            {
                TaskSequence exitVehAndWalk = new TaskSequence();
                exitVehAndWalk.AddTask.LeaveVehicle();
                exitVehAndWalk.AddTask.WanderAround();
                return exitVehAndWalk;
            }            
            if (index == 1)
            {
                TaskSequence walkToVehicleDrive = new TaskSequence();
                walkToVehicleDrive.AddTask.GoTo(vehicle.Position + new Vector3(0,-5, 0));
                walkToVehicleDrive.AddTask.EnterVehicle(vehicle, VehicleSeat.Driver);
                walkToVehicleDrive.AddTask.CruiseWithVehicle(vehicle, 30f, (int)DrivingStyle.Normal);
                walkToVehicleDrive.AddTask.ClearAll();
                return walkToVehicleDrive;                
            }            
            if ( index == 2)
            {
                TaskSequence driveToPlayerAndExit = new TaskSequence();
                driveToPlayerAndExit.AddTask.DriveTo(vehicle, Game.Player.Character.Position, 10f, 20f, (int)DrivingStyle.Normal);
                driveToPlayerAndExit.AddTask.GoTo(Game.Player.Character.Position);
                return driveToPlayerAndExit;                
            }            
            else
            {
                return null;
            }
        }

        void MoveCustomer(Customer c, Vector3 target)
        {                             
            if (c.Ped.Position.DistanceTo(playerPed.Position) > 20)
            {
                if (Function.Call<bool>(Hash.IS_POINT_ON_ROAD, c.Ped.Position.X, c.Ped.Position.Y, c.Ped.Position.Z))
                {
                    c.Ped.Task.RunTo(target);
                }
                else
                {
                    c.Ped.Task.GoTo(target);
                }
            }
            else
            {
                c.Ped.Task.GoTo(target);
            }      
        }

        void MakeStreetSale(Customer c, bool inCar)
        {
            List<Ped> police = new List<Ped>();
            
            int price = 0;
            if (c.drug == "Weed") { price = 50; }
            if (c.drug == "Crack") { price = 200; }
            if (c.drug == "Cocain") { price = 400; }
            Ped p = c.Ped;                               
            if (inCar == false)
            {                
                foreach (Ped ped in World.GetNearbyPeds(playerPed.Position, 25))
                {
                    if (Function.Call<int>(Hash.GET_PED_TYPE, ped.Handle) == 6)
                    {
                        police.Add(ped);
                        ped.Task.LookAt(playerPed, 10000);
                    }
                }
                playerPed.Task.TurnTo(p);
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_HOWS_IT_GOING", "SPEECH_PARAMS_STANDARD");                
                Wait(1000);
                p.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                playerPed.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_shopkeeper", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                Wait(2000);
                player.Money += customerSellTo.amount * price;
                customerSellTo.Ped.Money -= customerSellTo.amount * customerSellTo.price;
                PlayerInventory[c.drug] -= c.amount;
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "LOCAL_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", 0);
                UI.Notify("Sold " + c.amount + "g, you got " + PlayerInventory[c.drug] + "g " + c.drug + " left.");
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_THANKS", "SPEECH_PARAMS_STANDARD");
                foreach (Ped ped in police)
                {
                    if (ped.Position.DistanceTo(playerPed.Position) < 30)
                    {
                        player.WantedLevel = 1;
                        break;
                    }
                }
                police.Clear();
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
                
                if (playerPed.CurrentVehicle.Model.IsCar)
                {
                    {
                        foreach (Ped ped in World.GetNearbyPeds(playerPed.Position, 20))
                        {
                            if (Function.Call<int>(Hash.GET_PED_TYPE, ped.Handle) == 6)
                            {
                                police.Add(ped);
                                ped.Task.LookAt(playerPed, 10000);
                            }
                        }
                    }                    
                }
                else
                {
                    foreach (Ped ped in World.GetNearbyPeds(playerPed.Position, 25))
                    {
                        if (Function.Call<int>(Hash.GET_PED_TYPE, ped.Handle) == 6)
                        {
                            police.Add(ped);
                            ped.Task.LookAt(playerPed, 10000);
                        }
                    }                    
                }
                
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_HOWS_IT_GOING", "SPEECH_PARAMS_STANDARD");
                Wait(1000);
                p.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                Wait(3000);
                player.Money += c.amount * price;
                customerSellTo.Ped.Money -= customerSellTo.amount * customerSellTo.price;
                PlayerInventory[c.drug] -= c.amount;
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "LOCAL_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", 0);
                UI.Notify("Sold " + c.amount + "g, you got " + PlayerInventory[c.drug] + "g " + c.drug + " left.");
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_THANKS", "SPEECH_PARAMS_STANDARD");
                foreach (Ped ped in police)
                {
                    if (ped.Position.DistanceTo(playerPed.Position) < 25)
                    {
                        player.WantedLevel = 1;
                        break;
                    }
                }
                police.Clear();                
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
                        if (e.Name == "weed" && c.drug == "Weed")
                        {
                            e.Value = PlayerInventory["Weed"].ToString();
                        }
                        if (e.Name == "crack" && c.drug == "Crack")
                        {
                            e.Value = PlayerInventory["Crack"].ToString();
                        }
                        if (e.Name == "cocain" && c.drug == "Cocain")
                        {
                            e.Value = PlayerInventory["Cocain"].ToString();
                        }
                    }
                }
            }
            LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");

        }

        private void ContactAnswered(iFruitContact contact)
        {
            // The contact has answered, we can execute our code

            if (contact.Name == "Zee - Buy drugs.")
            {
                if (drugDealer1 == null)
                {
                    callDealer();
                    UI.Notify("Yo man, sending someone to meet you.");
                }
                else
                {
                    UI.Notify("Someones already on there way mate.");
                }
            }
            if (contact.Name == "Zee - Sell drugs.")
            {
                if (PlayerInventory["Weed"] > 0 || PlayerInventory["Crack"] > 0 || PlayerInventory["Cocain"] > 0)
                {
                    UI.Notify("Got your call, will send some people youre way.");
                    isDealing = true;
                    lastStreet = World.GetStreetName(new Vector2(playerPed.Position.X, playerPed.Position.Y));
                    thisStreet = lastStreet;
                    timeStartedDealing = Game.GameTime;
                    heatBar = new BarTimerBar("Heat");
                    heatBar.Percentage = 0f; // Half full
                    //myBar.Percentage = 1.0f; // Full bar
                    timerBarPool.Add(heatBar);

                    // You can easily modify the bar color.
                    heatBar.BackgroundColor = Color.Black;
                    heatBar.ForegroundColor = Color.WhiteSmoke;
                }
                else
                {
                    UI.Notify("You got no drugs left, you should get some more");
                }
            }
            if (contact.Name == "Zee - Buy weapons.")
            {
                if (wepDealer == null)
                {
                    wepDealer = new Dealer(PedHash.ArmLieut01GMM, playerPed.Position, "INGOT", playerGroup);
                }
            }

            // We need to close the phone at a moment.
            // We can close it as soon as the contact pick up calling _iFruit.Close().
            // Here, we will close the phone in 5 seconds (5000ms).
            iFruit.Close(3000);
        }

        public class Customer
        {
            private Ped ped;
            private Vector3 target;
            private Vehicle vehicle;
            public List<string> drugs = new List<string>();
            private int wealth;
            public string drug;
            public int amount;
            private bool farAway;
            public int price = 0;

            public Customer(Ped ped, Vector3 target, int wealth, bool farAway, Vehicle vehicle)
            {
                //init
                FarAway = farAway;
                Wealth = wealth;
                Ped = ped;
                Target = target;
                Vehicle = vehicle;
                ped.AddBlip();
                ped.CurrentBlip.IsFriendly = true;
                ped.CurrentBlip.Scale = 0.5f;
                ped.CurrentBlip.IsShortRange = true;
                ped.IsPersistent = true;
                ped.Task.ClearAll();
                if (Wealth > 0) drugs.Add("Weed");
                if (Wealth <= 7) drugs.Add("Crack");
                if (Wealth >= 4) drugs.Add("Cocain");
                int minAmount = 1;
                if (FarAway) { minAmount = 10; drugs.Add("Crack"); }
                Random rnd = new Random();
                int max = drugs.Count;
                int i = rnd.Next(0, max);
                drug = drugs[i];
                amount = rnd.Next(minAmount, Wealth + 2);
                if (drug == "Weed") { price = 50; }
                if (drug == "Crack") { price = 200; }
                if (drug == "Cocain") { price = 400; }
                int extraMoney = rnd.Next(0, 500);
                Ped.Money = (amount * price) + extraMoney;
                if (!FarAway && vehicle == null) { Ped.Task.GoTo(target); }
                //if (!FarAway && vehicle != null) { PerformTaskSeq(2, vehicle); }
            }
            public Vector3 Target { get => target; set => target = value; }
            public Ped Ped { get => ped; set => ped = value; }
            public Vehicle Vehicle { get => vehicle; set => vehicle = value; }
            public int Wealth { get => wealth; set => wealth = value; }
            public bool FarAway { get => farAway; set => farAway = value; }
        }

        //public class DealerIntro
        //{
        //    private Ped ped;
        //    private Vector3 target;
        //    public DealerIntro(Ped ped, Vector3 target)
        //    {
        //        Ped = ped;
        //        Target = target;
        //    }
        //    public Vector3 Target { get => target; set => target = value; }
        //    public Ped Ped { get => ped; set => ped = value; }
        //}

        public class Dealer
        {
            public float maxSpeed;
            public Vector3 destination;
            public bool isDriving = false;
            public bool doneTutorial = false;
            public Ped ped;
            public Vector3 pos;
            private Model vehModel;
            public Vehicle vehicle;
            public Dealer(Model pedModel, Vector3 pos, Model vehModel, PedGroup group)
            {
                this.pos = pos;
                this.vehModel = vehModel;
                this.vehicle = World.CreateVehicle(this.vehModel, GenerateSpawnPos(pos.Around(300), Nodetype.AnyRoad, false));
                this.vehicle.IsPersistent = true;
                this.vehicle.AddBlip();
                this.vehicle.CurrentBlip.IsFriendly = true;
                this.ped = World.CreatePed(pedModel, this.vehicle.Position);
                this.ped.IsPersistent = true;
                this.ped.SetIntoVehicle(this.vehicle, VehicleSeat.Driver);
                Function.Call(Hash.SET_DRIVER_ABILITY, this.ped.Handle, 100f);
                Function.Call(Hash._SET_EXCLUSIVE_DRIVER, this.vehicle.Handle, this.ped.Handle);
                this.ped.Task.DriveTo(this.vehicle, World.GetNextPositionOnStreet(pos), 2f, 30f, (int)DrivingStyle.Normal);
                this.ped.StaysInVehicleWhenJacked = true;
                this.ped.BlockPermanentEvents = true;
                maxSpeed = Function.Call<float>(Hash._GET_VEHICLE_MAX_SPEED, this.vehicle);
            }
        }

        public class Job
        {
            private Ped ped;
            private Vector3 target;
            public List<string> drugs = new List<string>();
            private int wealth;
            public string drug;
            public int amount;
            public Job(Ped ped, Vector3 target, int wealth)
            {
                //init
                Wealth = wealth;
                Ped = ped;
                Target = target;
                ped.AddBlip();
                ped.CurrentBlip.IsFriendly = true;
                ped.CurrentBlip.Scale = 0.5f;
                ped.CurrentBlip.IsShortRange = true;
                ped.IsPersistent = true;
                //ped.Task.ClearAll();
                //ped.Task.GoTo(target);
                if (Wealth > 0) drugs.Add("Weed");
                if (Wealth < 5) drugs.Add("Crack");
                if (Wealth > 4) drugs.Add("Cocain");
                Random rnd = new Random();
                int max = drugs.Count;
                int i = rnd.Next(0, max);
                drug = drugs[i];
                amount = rnd.Next(1, Wealth + 2);
            }
            public Vector3 Target { get => target; set => target = value; }
            public Ped Ped { get => ped; set => ped = value; }
            public int Wealth { get => wealth; set => wealth = value; }
        }
        
        public enum Nodetype { AnyRoad, Road, Offroad, Water }
        public static Vector3 GenerateSpawnPos(Vector3 desiredPos, Nodetype roadtype, bool sidewalk)
        {
            Vector3 finalpos = Vector3.Zero;
            bool ForceOffroad = false;

            OutputArgument outArgA = new OutputArgument();
            int NodeNumber = 1;
            int type = 0;

            if (roadtype == Nodetype.AnyRoad) type = 1;
            if (roadtype == Nodetype.Road) type = 0;
            if (roadtype == Nodetype.Offroad) { type = 1; ForceOffroad = true; }
            if (roadtype == Nodetype.Water) type = 3;


            int NodeID = Function.Call<int>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID, desiredPos.X, desiredPos.Y, desiredPos.Z, NodeNumber, type, 200f, 200f);

            if (ForceOffroad)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (!Function.Call<bool>(Hash._GET_IS_SLOW_ROAD_FLAG, NodeID))
                    {
                        NodeID = Function.Call<int>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID, desiredPos.X, desiredPos.Y, desiredPos.Z, NodeNumber, type, 200f, 200f);
                        NodeNumber++;
                    }
                }
            }

            Function.Call(Hash.GET_VEHICLE_NODE_POSITION, NodeID, outArgA);
            finalpos = outArgA.GetResult<Vector3>();

            if (sidewalk) finalpos = World.GetNextPositionOnSidewalk(finalpos);
            return finalpos;

        }

        public enum PathnodeFlags
        {
            Slow = 1,
            Two = 2,
            Intersection = 4,
            Eight = 8, SlowTraffic = 12, ThirtyTwo = 32, Freeway = 64, FourWayIntersection = 128, BigIntersectionLeft = 512
        }
        public static string GetRoadFlags(Vector3 pos)
        {
            OutputArgument outArgA = new OutputArgument();
            OutputArgument outArgB = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_VEHICLE_NODE_PROPERTIES, pos.X, pos.Y, pos.Z, outArgA, outArgB))
            {
                int busy = outArgA.GetResult<int>();
                int flags = outArgB.GetResult<int>();

                string d = "";
                foreach (int flag in Enum.GetValues(typeof(PathnodeFlags)).Cast<PathnodeFlags>())
                {

                    if ((flag & flags) != 0) d += " " + (PathnodeFlags)flag;
                }
                return d;  // DisplayHelpTextThisFrame("Flags: " + d);
            }
            return "";
        }
        public static bool RoadHasFlag(Vector3 pos, PathnodeFlags flag)
        {
            OutputArgument outArgA = new OutputArgument();
            OutputArgument outArgB = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_VEHICLE_NODE_PROPERTIES, pos.X, pos.Y, pos.Z, outArgA, outArgB))
            {
                int busy = outArgA.GetResult<int>();
                int flags = outArgB.GetResult<int>();
                if ((flags & (int)flag) != 0) return true;
            }
            return false;
        }

        public static bool IntHasFlag(int number, int flag)
        {

            if ((number & (int)flag) != 0) return true;
            return false;
        }


        protected override void Dispose(bool A_0)
        {
            if (A_0)
            {
                if (drugDealer1 != null)
                {
                    drugDealer1.Delete();
                }
                if (stashBlip != null)
                {
                    stashBlip.Remove();
                }
                if (customers.Count >= 1)
                {
                    //int pedsDeleted = 0;
                    foreach (Customer c in customers)
                    {
                        Ped p = c.Ped;
                        //pedsDeleted++;
                        p.CurrentBlip.Remove();
                        p.Delete();
                    }
                    //UI.Notify("Deleted " + pedsDeleted + " peds.");
                }
                if (drugCar != null)
                {
                    if (drugCar.Exists())
                    {
                        drugCar.CurrentBlip.Remove();
                        drugCar.Delete();
                    }
                }
                if (jobs.Count > 0)
                {
                    foreach (Customer j in jobs)
                    {
                        j.Ped.CurrentBlip.Remove();
                        j.Ped.Delete();
                    }
                }
                if (wepDealer != null)
                {
                    if (wepDealer.vehicle != null)
                    {
                        wepDealer.vehicle.CurrentBlip.Remove();
                        wepDealer.vehicle.Delete();
                    }
                    if (wepDealer.ped != null)
                    {
                        //wepDealer.ped.CurrentBlip.Remove();
                        wepDealer.ped.Delete();
                    }
                }
            }
        }
    }
}
