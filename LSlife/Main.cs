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
using static LSlife.LsFunctions;
using static LSlife.LsMenus;


namespace LSlife
{
    public class LSL : Script
    {
        #region Globals
        public static bool DEBUG = true;
        public static bool started = false;
        public static bool EnableBlips;
        public static ScriptSettings iniSettings;

        //UIStuff
        public static MenuPool LsMenuPool;


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

        //Zee Menu

        public static UIMenu zeeMenu;
        public static UIMenu startDealing;
        public static UIMenuCheckboxItem sellWeed;
        public static UIMenuCheckboxItem sellCrack;
        public static UIMenuCheckboxItem sellCocaine;
        public static UIMenuItem StartDeal;
        public static UIMenuItem buyDrugs;
        public static UIMenuItem buyWeapons;
        public static UIMenuItem buyLift;

        //WEAPONS MENU
        public static UIMenu wepMenu;
        public static UIMenuListItem melee;
        public static List<dynamic> meleeWeps = new List<dynamic>() { {"Bat"}, {"Knife"}, { "Dagger" } };
        public static List<int> meleePrice = new List<int>() { { 5 }, { 100 }, { 2000 } };
        public static UIMenuListItem pistol;
        public static List<dynamic> pistolWeps = new List<dynamic>() { {"Pistol"}, { "Pistol50" }, { "HeavyPistol"}, { "Ammo" } };
        public static List<int> pistolPrice = new List<int>() { { 350 }, { 3750 }, { 4000 }, { 50 } };
        public static UIMenuListItem shotgun;
        public static List<dynamic> shotgunWeps = new List<dynamic>() { {"SawnOffShotgun"}, {"PumpShotgun"} };
        public static UIMenuListItem assaultRifle;
        public static List<dynamic> assaultRifleWeps = new List<dynamic>() { {"AssaultRifle"}, {"CarbineRifle"} };
        public static UIMenuListItem smg;
        public static List<dynamic> smgWeps = new List<dynamic>() { {"MicroSMG"}, {"SMG"} };
        public static UIMenuListItem mg;
        public static List<dynamic> mgWeps = new List<dynamic>() { {"MG"} };
        public static UIMenuListItem sniper;
        public static List<dynamic> sniperWeps = new List<dynamic>() {"SniperRifle"};
        public static UIMenuListItem heavy;
        public static List<dynamic> heavyWeps = new List<dynamic>() {"RPG"};
        public static UIMenuListItem thrown;
        public static List<dynamic> thrownWeps = new List<dynamic>() {"Grenade"};
        public static UIMenuItem armour;
        public static UIMenuItem dismiss;



        //DEALER MENU
        public static UIMenu dealerMainMenu;
        public static UIMenu weedMenu;
        public static UIMenuSliderItem bushWeed;
        public static UIMenu crackMenu;
        public static UIMenuSliderItem crack;
        public static UIMenu cocainMenu;
        public static UIMenuSliderItem cocaine;
        public static UIMenuItem WeedBuy;
        public static UIMenuItem CrackBuy;
        public static UIMenuItem CocainBuy;
        public static UIMenuItem payDebt;
        public static UIMenuItem dismissDealer;
        //STASH MENU
        public static UIMenu stashMainMenu;
        public static UIMenu stashTakeMenu;
        public static UIMenu stashPutMenu;
        public static UIMenuSliderItem putWeedStash;
        public static UIMenuSliderItem takeWeedStash;
        public static UIMenuSliderItem putCrackStash;
        public static UIMenuSliderItem takeCrackStash;
        public static UIMenuSliderItem putCocainStash;
        public static UIMenuSliderItem takeCocainStash;
        public static UIMenuSliderItem putMoneyStash;
        public static UIMenuSliderItem takeMoneyStash;
        public static UIMenuListItem putWeaponStash;
        public static UIMenuListItem takeWeaponStash;
        public static UIMenuListItem stashPutMultiplier;
        public static UIMenuListItem stashTakeMultiplier;

        //Vehicle stash MENU
        public static UIMenu vehStashMainMenu;
        public static UIMenu vehStashTakeMenu;
        public static UIMenu vehStashPutMenu;
        public static UIMenuSliderItem putWeedVeh;
        public static UIMenuSliderItem takeWeedVeh;
        public static UIMenuSliderItem putCrackVeh;
        public static UIMenuSliderItem takeCrackVeh;
        public static UIMenuSliderItem putCocainVeh;
        public static UIMenuSliderItem takeCocainVeh;
        public static UIMenuSliderItem putMoneyVeh;
        public static UIMenuSliderItem takeMoneyVeh;
        public static UIMenuListItem putWeaponVeh;
        public static UIMenuListItem takeWeaponVeh;
        public static UIMenuListItem vehPutMultiplier;
        public static UIMenuListItem vehTakeMultiplier;
        public static readonly List<dynamic> multiplier = new List<dynamic>() { { 1 }, { 10 }, { 100 }, { 1000 }, { 10000 }, { 100000 } };

        //Customer Menu
        public static UIMenu customerMainMenu;
        public static UIMenuItem sell;
        public static UIMenuItem turnAway;

        // TIMER BAR STUFF
        public static TimerBarPool timerBarPool = new TimerBarPool();
        public static BarTimerBar heatBar;

        //Main menu and options 
        public static Keys MenuKey;
        public static Keys StopDealing;
        public static Keys SetDrugCar;

        //XML STUFF
        public static XDocument vehWeaponsXML;
        public static XDocument stashWeaponsXML;
        public static XDocument LSLifeSave;




        public static float displayHelpTimer = Game.GameTime;

        public static int day;
        public static int hour;
        public static int dayUpdateTimer;


        //Ifruit stuff
        public static CustomiFruit iFruit;

        // Dealers
        //DealerIntro dealerIntro1;
        public static iFruitContact contactZee;
        public static Ped drugDealer1;
        public static Ped weaponDealer1;
        public static int dealerTimer;
        public static Blip stashBlip;
        public static bool readyToDeal;
        public static bool dealer1Angry;
        public static int weed1Price = 500;
        public static int crack1Price = 2500;
        public static int cocain1Price = 5000;
        public static int dealer1Weed;
        public static int dealer1Crack;
        public static int dealer1Cocain;
        public static int dealer1Money;
        public static int dealer1ReloadDay;
        public static bool dealer1ReloadOrder = false;
        //int dealer1RepLevel;
        public static int dealer1ReloadHour = 20;
        public static bool dealer1Reloaded = false;
        public static int dealer1Debt;
        public static double dealer1Rep;
        public static int reloadTimer;
        public static int reloadTimerInterval = 60000 * 10;

        // Police
        public static Pig pig = null;

        // Area Dealer / Gang stuff

        public static AreaDealer aDealer = null;        

        // WEED PLACE
        //int weedState = 0;
        //Door weedDoor;

        // Game.Player stuff
        public static bool removedStuff = true;
        public static bool isAtHome = false;
        public static bool isDealing = false;
        public static bool chilling = false;
        public static int areaCheckTimer;
        public enum AreaType
        {
            Poor = 0,
            Normal = 1,
            Rich = 2
        }
        public static Dictionary<string, AreaType> areaTypes = new Dictionary<string, AreaType>()
            {
                { "ALAMO"    , AreaType.Poor },
                {"ALTA"      , AreaType.Rich },
                {"BHAMCA"    , AreaType.Poor },
                {"BANHAMC"   , AreaType.Rich },
                {"BANNING"   , AreaType.Poor },
                {"JAIL"      , AreaType.Rich },
                {"BRADP"     , AreaType.Poor },
                {"BURTON"    , AreaType.Rich },
                {"CALAFB"    , AreaType.Poor },
                {"CCREAK"    , AreaType.Poor },
                {"CHAMH"     , AreaType.Poor },
                {"CMSW"      , AreaType.Poor },
                {"CHU"       , AreaType.Rich },
                {"CYPRE"     , AreaType.Poor },
                {"DAVIS"     , AreaType.Poor },
                {"ZQ_UAR"    , AreaType.Poor },
                {"DELPE"     , AreaType.Rich },
                {"DELBE"     , AreaType.Rich },
                {"DOWNT"     , AreaType.Rich },
                {"DTVINE"    , AreaType.Rich },
                {"EAST_V"    , AreaType.Normal },
                {"EBURO"     , AreaType.Poor },
                {"ELGORL"    , AreaType.Poor },
                {"ELYSIAN"   , AreaType.Poor },
                {"ARMYB"     , AreaType.Rich },
                {"GALFISH"   , AreaType.Poor },
                {"DESRT"     , AreaType.Poor },
                {"GRAPES"    , AreaType.Poor },
                {"GREATC"    , AreaType.Rich },
                {"golf"      , AreaType.Rich },
                {"HARMO"     , AreaType.Poor },
                {"HAWICK"    , AreaType.Rich },
                {"HUMLAB"    , AreaType.Rich },
                {"LMESA"     , AreaType.Poor },
                {"LOSPUER"   , AreaType.Poor },
                {"LAGO"      , AreaType.Poor },
                {"LDAM"      , AreaType.Poor },
                {"LACT"      , AreaType.Poor },
                {"LEGSQU"    , AreaType.Normal },
                {"KOREAT"    , AreaType.Rich },
                {"AIRP"      , AreaType.Rich },
                {"STAD"      , AreaType.Normal },
                {"MIRR"      , AreaType.Normal },
                {"SKID"      , AreaType.Poor },
                {"MORN"      , AreaType.Rich },
                {"MTCHIL"    , AreaType.Poor },
                {"MTGORDO"   , AreaType.Poor },
                {"MTJOSE"    , AreaType.Poor },
                {"MURRI"     , AreaType.Normal },
                {"NOOSE"     , AreaType.Rich },
                {"NCHU"      , AreaType.Poor },
                {"PBLUFF"    , AreaType.Rich },
                {"OCEANA"    , AreaType.Poor },
                {"PALETO"    , AreaType.Poor },
                {"PALCOV"    , AreaType.Poor },
                {"PALFOR"    , AreaType.Poor },
                {"PALMPOW"   , AreaType.Normal },
                {"PALHIGH"   , AreaType.Poor },
                {"PBOX"      , AreaType.Rich },
                {"ZP_ORT"    , AreaType.Normal },
                {"PROCOB"    , AreaType.Poor },
                {"DELSOL"    , AreaType.Rich },
                {"RANCHO"    , AreaType.Poor },
                {"CANNY"     , AreaType.Poor },
                {"RTRAK"     , AreaType.Poor },
                {"MOVIE"     , AreaType.Rich },
                {"RICHM"     , AreaType.Rich },
                {"RGLEN"     , AreaType.Rich },
                {"ROCKF"     , AreaType.Rich },
                {"WINDF"     , AreaType.Poor },
                {"SanAnd"    , AreaType.Poor },
                {"SANCHIA"   , AreaType.Poor },
                {"SANDY"     , AreaType.Normal },
                {"SLAB"      , AreaType.Poor },
                {"STRAW"     , AreaType.Normal },
                {"TATAMO"    , AreaType.Poor },
                {"TERMINA"   , AreaType.Poor },
                {"TEXTI"     , AreaType.Normal},
                {"TONGVAH"   , AreaType.Rich },
                {"TONGVAV"   , AreaType.Rich },
                {"VESP"      , AreaType.Rich },
                {"BEACH"     , AreaType.Rich },
                {"VCANA"     , AreaType.Rich },
                {"VINE"      , AreaType.Rich },
                {"CHIL"      , AreaType.Rich },
                {"HORS"      , AreaType.Rich },
                {"WVINE"     , AreaType.Rich },
                {"ZANCUDO"   , AreaType.Poor } };
        public static List<Area> areas = new List<Area>();
        public static int areaIndex = 0;
        public static string playerArea = "SanAnd";
        public static Random rnd = new Random();
        public static Dictionary<string, int> PlayerInventory = new Dictionary<string, int>();
        public static int TotalDrugsCarried = 0;
        public static bool playerWanted = false;
        public static Vehicle drugCar = null;
        public static bool drugCarWanted = false;
        public static bool notFined = true;
        public static bool playerDead = false;
        public static List<dynamic> playerWeps = new List<dynamic>();
        public static int newWepAmount = 0;
        public static List<XElement> playerWeapons = new List<XElement>();
        public static bool playerHasBag = false;
        public static int randomAttackTimer;
        public static int randomAttack = 0;
        public static bool randAttackersChasing = false;
        public static List<Ped> randAttackers = new List<Ped>();
        public static List<Vehicle> randAttackVehs = new List<Vehicle>();

        public static List<Entity> entitiesCleanUp = new List<Entity>();

        public static bool drugCarDestroyed = true;
        public static bool drugCarSpawned = false;
        public static bool drugCarSaved = true;
        public static Blip drugCarBlip = null;
        public static int drugCarWeed;
        public static int drugCarCrack;
        public static int drugCarCocain;
        public static int drugCarMoney;

        public static List<XElement> vehWeapons = new List<XElement>();
        public static List<dynamic> vehWeps = new List<dynamic>();

        //Player Dealing
        public static bool sellingWeed = false;
        public static bool sellingCrack = false;
        public static bool sellingCocaine = false;
        public static PedGroup playerGroup;
        public static int timeStartedDealing;
        public static int timeDealing = 0;
        public static Vector3 pedTarget;
        public static Ped attacker;
        public static bool attackerAttacking = false;
        public static bool attackerPickupSpawned = false;
        public static Pickup attackerPickup;
        public static int rayTimer = 0;
        public static List<XElement> attackerWeapons = new List<XElement>();
        public static Dictionary<string, int> attackerInventory = new Dictionary<string, int>();
        public static List<Ped> oldCustomers = new List<Ped>();
        public static List<Ped> angryPeds = new List<Ped>();
        public static List<Customer> customers = new List<Customer>();
        public static List<Customer> jobs = new List<Customer>();
        public static string thisStreet;
        public static string lastStreet;
        public static float lastTimePedSelected = Game.GameTime;
        public static float lastTimeDealDone = Game.GameTime;
        public static float lastAngryTime = Game.GameTime;
        public static float ratTimer = 0;
        public static int ratChance;
        public static int rat;
        public static int lastCustomerCheckTimer;
        public static Customer customerSellTo;
        public static int jobOfferTimer;
        public static int jobAcceptTimer;
        public static bool jobOffer = false;
        public static bool jobAccepted = false;
        public static int oneSecDelay;
        public static bool firstRun = true;
        public static List<Drug> LsDrugs = new List<Drug>();

        //Game.Player housing
        Vector3 houseTeleIn = new Vector3(151.36f, -1007.56f, -99f);
        Vector3 houseTeleOut = new Vector3(-112.616f, -1479.229f, 37f);
        Vector3 houseStash = new Vector3(151.9f, -1001.4f, -99f);
        public static int stashWeed;
        public static int stashCrack;
        public static int stashCocain;
        public static int stashMoney;
        public static List<XElement> stashWeapons = new List<XElement>();
        public static List<dynamic> stashWeps = new List<dynamic>();
        public static Vector3 houseHealthPickPos = new Vector3(154.6751f, -1007.43f, -99.2f);
        public static bool pickupSpawned = false;
        public static Pickup healthPickup;

        //WEAPONS DEALER STUFF
        public static Dealer wepDealer;
        public static UIText myUIText;
        #endregion
        public LSL()
        {
            Setup();
            Tick += onTick;
            KeyDown += onKeyDown;

            Interval = 1;

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
            LsDrugs.Add(new Drug("Weed", 50, AreaType.Normal));
            LsDrugs.Add(new Drug("Crack", 200, AreaType.Poor));
            LsDrugs.Add(new Drug("Cocaine", 400, AreaType.Rich));
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
                            PlayerInventory.Add("Cocaine", number);
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
                    bool foundRep = false;
                    foreach(XElement e in element.Elements())
                    {
                        if(e.Name == "rep")
                        {
                            foundRep = true;
                            break;
                        }
                    }
                    if (!foundRep)
                    {
                        XElement dealerRep = new XElement(name: "rep");
                        dealerRep.Value = "1";
                        element.Add(dealerRep);
                        LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                    }                    
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "debt")
                        {
                            Int32.TryParse(e.Value, out dealer1Debt);
                            if (dealer1Debt > 200000)
                            {
                                dealer1Debt = 200000;                                
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
                        if (e.Name == "rep")
                        {
                            double.TryParse(e.Value, out dealer1Rep);                            
                        }
                    }
                }
            }
            
            XDocument VehicleXml = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
            foreach (XElement vehicles in VehicleXml.Descendants())
            {
                if (vehicles.Name == "Vehicle")
                {
                    foreach (XElement e in vehicles.Elements())
                    {
                        if (e.Name == "Destroyed")
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
                    if (DEBUG)
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
            //NATIVEUI STUFF
            LsMenuPool = new MenuPool();

            //ZEE MENU
            zeeMenu = new UIMenu("Zee", "What you want?");
            LsMenuPool.Add(zeeMenu);
            startDealing = LsMenuPool.AddSubMenu(zeeMenu, "Dealing.");
            sellWeed = new UIMenuCheckboxItem("Sell Weed", true);
            sellCrack = new UIMenuCheckboxItem("Sell Crack", true);
            sellCocaine = new UIMenuCheckboxItem("Sell Cocaine", true);
            StartDeal = new UIMenuItem("Start Dealing.");
            startDealing.AddItem(sellWeed);
            startDealing.AddItem(sellCrack);
            startDealing.AddItem(sellCocaine);
            startDealing.AddItem(StartDeal);
            startDealing.OnItemSelect += OnstartDealSelect;
            buyDrugs = new UIMenuItem("Buy Drugs.");
            zeeMenu.AddItem(buyDrugs);
            buyWeapons = new UIMenuItem("Buy Weapons.");
            zeeMenu.AddItem(buyWeapons);
            buyLift = new UIMenuItem("Ask for a Lift.");
            zeeMenu.AddItem(buyLift);
            zeeMenu.OnItemSelect += OnZeeMenuSelect;


            // WEP MENU
            int mWprice = meleePrice[0] - Discount(meleePrice[0]);
            int pWprice = pistolPrice[0] - Discount(pistolPrice[0]);
            wepMenu = new UIMenu("Wepons", "What you want?");
            LsMenuPool.Add(wepMenu);
            melee = new UIMenuListItem("Melee $" + mWprice, meleeWeps, 0);
            wepMenu.AddItem(melee);
            pistol = new UIMenuListItem("Pistol $" + pWprice, pistolWeps, 0);
            wepMenu.AddItem(pistol);
            armour = new UIMenuItem("Armour $" + (250 - Discount(250)));
            wepMenu.AddItem(armour);
            dismiss = new UIMenuItem("Dismiss");
            wepMenu.AddItem(dismiss);
            wepMenu.OnItemSelect += OnWepMenuSelect;
            melee.OnListChanged += onListChange;
            pistol.OnListChanged += onListChange;




            //DEALER MENU
            
            dealerMainMenu = new UIMenu("Dealer", "Rep:" + dealer1Rep + " Lvl:" + (int)CurrentRepLvl(dealer1Rep));
            LsMenuPool.Add(dealerMainMenu);
            // WEED
            weedMenu = LsMenuPool.AddSubMenu(dealerMainMenu, "Weed");
            bushWeed = new UIMenuSliderItem("Weed $" + (weed1Price - Discount(weed1Price)) + "oz");
            weedMenu.AddItem(bushWeed);
            bushWeed.Maximum = dealer1Weed;
            bushWeed.Multiplier = 1;
            WeedBuy = new UIMenuItem("Buy: " + bushWeed.Value);
            weedMenu.AddItem(WeedBuy);
            bushWeed.OnSliderChanged += onSliderChange;
            weedMenu.OnItemSelect += onWeedMenuSelect;
            //CRACK 
            crackMenu = LsMenuPool.AddSubMenu(dealerMainMenu, "Crack");
            crack = new UIMenuSliderItem("Crack $" + (crack1Price- Discount(crack1Price)) + "oz");
            crackMenu.AddItem(crack);
            crack.Maximum = dealer1Crack;
            crack.Multiplier = 1;
            CrackBuy = new UIMenuItem("Buy: " + crack.Value);
            crackMenu.AddItem(CrackBuy);
            crack.OnSliderChanged += onSliderChange;
            crackMenu.OnItemSelect += onCrackMenuSelect;
            //COCAIN
            cocainMenu = LsMenuPool.AddSubMenu(dealerMainMenu, "Cocaine");
            cocaine = new UIMenuSliderItem("Cocaine $" + (cocain1Price- Discount(cocain1Price)) + "oz");
            cocainMenu.AddItem(cocaine);
            cocaine.Maximum = dealer1Cocain;
            cocaine.Multiplier = 1;
            CocainBuy = new UIMenuItem("Buy: " + cocaine.Value);
            cocainMenu.AddItem(CocainBuy);
            cocaine.OnSliderChanged += onSliderChange;
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
            putCocainStash = new UIMenuSliderItem("Cocaine");
            stashPutMenu.AddItem(putCocainStash);
            putCocainStash.Maximum = PlayerInventory["Cocaine"];
            putCocainStash.Multiplier = 1;
            takeCocainStash = new UIMenuSliderItem("Cocaine");
            stashTakeMenu.AddItem(takeCocainStash);
            takeCocainStash.Maximum = stashCocain;
            takeCocainStash.Multiplier = 1;
            putCocainStash.OnSliderChanged += onSliderChange;
            takeCocainStash.OnSliderChanged += onSliderChange;
            //stash MONEY
            putMoneyStash = new UIMenuSliderItem("Money $");
            stashPutMenu.AddItem(putMoneyStash);
            putMoneyStash.Maximum = Game.Player.Money;
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
            putWeedVeh = new UIMenuSliderItem("Weed " + PlayerInventory["Weed"] + "g");
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
            putCocainVeh = new UIMenuSliderItem("Cocaine " + PlayerInventory["Cocaine"] + "g");
            vehStashPutMenu.AddItem(putCocainVeh);
            putCocainVeh.Maximum = PlayerInventory["Cocaine"];
            putCocainVeh.Multiplier = 1;
            takeCocainVeh = new UIMenuSliderItem("Cocaine " + drugCarCocain + "g");
            vehStashTakeMenu.AddItem(takeCocainVeh);
            takeCocainVeh.Maximum = drugCarCocain;
            takeCocainVeh.Multiplier = 1;
            putCocainVeh.OnSliderChanged += onSliderChange;
            takeCocainVeh.OnSliderChanged += onSliderChange;
            //stash MONEY
            putMoneyVeh = new UIMenuSliderItem("Money $" + Game.Player.Money);
            vehStashPutMenu.AddItem(putMoneyVeh);
            putMoneyVeh.Maximum = Game.Player.Money;
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
            contactZee = new iFruitContact("Zee");            
            contactZee.Answered += ContactAnswered;   // Linking the Answered event with our function
            contactZee.DialTimeout = 1000;            // Delay before answering
            contactZee.Active = true;                 // true = the contact is available and will answer the phone
            contactZee.Icon = ContactIcon.Blank;      // Contact's icon
            iFruit.Contacts.Add(contactZee);      // Add the contact to the phone

            // WORD Drugs SETUP


            day = Function.Call<int>(Hash.GET_CLOCK_DAY_OF_WEEK);
            hour = Function.Call<int>(Hash.GET_CLOCK_HOURS);
            dayUpdateTimer = Game.GameTime;
            jobOfferTimer = Game.GameTime;
            randomAttackTimer = Game.GameTime;
            reloadTimer = Game.GameTime;
        }

        #region Menu Functions
        

        //WEED MENU
        

        
        #endregion

        void onTick(object sender, EventArgs e)
        {
            if (!Game.IsLoading)
            {
                if (firstRun)
                {
                    if (Game.Player.Character != null)
                    {                        
                        ratChance = -100;
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character, 9, 0, 0, 0);
                        playerArea = World.GetZoneNameLabel(Game.Player.Character.Position);
                        areaTypes.TryGetValue(playerArea, out AreaType aType);
                        areas.Add(new Area(playerArea, LsDrugs, aType));
                        areaIndex = 0;
                        if(areas[areaIndex].Name == World.GetZoneNameLabel(stashBlip.Position))
                        {
                            areas[areaIndex].GangPresance = 0;
                        }                        
                        firstRun = false;                        
                    }
                }
                if (DEBUG)
                {
                    int time = -1000;
                    Area area = areas[areaIndex];
                    if(aDealer != null)
                    {
                        if(aDealer.cTimer != null)
                        {                            
                            time = aDealer.cTimer.Elapsed();
                            myUIText = new UIText(time.ToString() + " fin:"+aDealer.cTimer.Finished() , new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                            myUIText.Draw();
                        }
                    }
                    //string debugData = "Areas: "+areas.Count+" aName:" + area.Name + " aHeat:" + area.Heat + "% aPolice:" + area.CopPresance + " aGang:" + area.GangPresance+"/"+area.Dealers + " RChance:"+ratChance+ "% aDrug:"+area.Drugs[0].Demand/area.Dealers+"/"+area.Drugs[0].Delay / (area.Drugs[0].Demand/area.Dealers) + "ms DemandNotMet"+area.DemandNotMet;                                        
                }

                if (LsMenuPool != null)
                {
                    LsMenuPool.ProcessMenus();
                }
                iFruit.Update();
                if (timerBarPool != null)
                {
                    if (!LsMenuPool.IsAnyMenuOpen() && isDealing) { timerBarPool.Draw(); }
                }
                if (Game.GameTime > oneSecDelay + 10000)
                {
                    oneSecDelay = Game.GameTime;
                    if (Game.Player.IsPlaying)
                    {
                        
                        if (!dealer1Angry && dealer1Debt > 170000)
                        {
                            if (DEBUG) { UI.Notify("dealer1 angry"); }
                            dealer1Angry = true;
                        }
                        if (randomAttack == 0)
                        {
                            if (isDealing && areas[areaIndex].GangPresance > 4 && timeDealing > 120000)
                            {
                                if (rnd.Next(0, 50) == 0)
                                {
                                    if (DEBUG) { UI.Notify("poor area drive-by"); }
                                    randomAttack = 1;
                                }
                            }
                            if (dealer1Angry && randomAttack == 0 && drugDealer1 == null)
                            {
                                if (rnd.Next(0, 10) == 0)
                                {
                                    if(DEBUG) { UI.Notify("dealer1 drive-by"); }
                                    randomAttack = 1;
                                }
                            }                            
                        }
                        if (entitiesCleanUp.Count > 0)
                        {
                            foreach (Entity ent in entitiesCleanUp.ToList())
                            {
                                if (ent == null)
                                {
                                    entitiesCleanUp.Remove(ent);
                                }
                                else
                                if (ent.Exists())
                                {
                                    if (ent == attacker) { entitiesCleanUp.Remove(ent); if (DEBUG) { UI.Notify("stoped delete attacker"); } }
                                    else
                                    {
                                        if (ent.Position.DistanceTo(Game.Player.Character.Position) > 200 && !ent.IsOnScreen)
                                        {
                                            entitiesCleanUp.Remove(ent);
                                            if (DEBUG) { UI.Notify("Delete:" + ent); }
                                            ent.Delete();
                                        }
                                    }
                                    
                                }
                            }
                        }
                        newWepAmount = 0;
                        foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
                        {
                            if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Game.Player.Character, (int)weapon))
                            {
                                newWepAmount++;
                            }
                        }
                        if (newWepAmount != playerWeps.Count)
                        {
                            if (DEBUG) { UI.Notify("Player weapon change detected"); }
                            playerWeps.Clear();
                            playerWeapons.Clear();
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
                                    if (DEBUG)
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
                        }
                        if(areas.Count > 0)
                        {
                            DoGangGrowth();
                        }
                    }                                        
                }
                if (Game.GameTime > areaCheckTimer + 1000)
                {
                    int dealers = 0;
                    if (isDealing) { dealers += 1; }
                    if (aDealer != null) { dealers += 1; }
                    areas[areaIndex].Dealers = dealers;
                    areaCheckTimer = Game.GameTime;
                    string newArea = World.GetZoneNameLabel(Game.Player.Character.Position);
                    if (playerArea != newArea)
                    {
                        if (aDealer != null && Game.Player.Character.Position.DistanceTo(aDealer.dPed.Position) > 300)
                        {
                            if(aDealer.cPed != null)
                            {
                                RemovePedFromWorld(aDealer.cPed, false);
                                
                            }
                            if (aDealer.dPickup != null) aDealer.dPickup.Delete();
                            RemovePedFromWorld(aDealer.dPed, false);
                            aDealer = null;                            
                        }
                        areas[areaIndex].DemandNotMet = true;
                        areas[areaIndex].TimeLastActive = Game.GameTime;
                        areas[areaIndex].HighDemandTimer = areas[areaIndex].TimeLastActive;
                        playerArea = newArea;
                        if (areas.FindAll(a => a.Name == playerArea).Count == 0)
                        {
                            areaTypes.TryGetValue(playerArea, out AreaType type);
                            areas.Add(new Area(playerArea, LsDrugs, type));
                            int aIndex = 0;
                            foreach (Area area in areas)
                            {
                                if (area.Name == playerArea)
                                {
                                    areaIndex = aIndex;
                                    if (areas[areaIndex].Name == World.GetZoneNameLabel(stashBlip.Position))
                                    {
                                        areas[areaIndex].GangPresance = 0;
                                    }
                                    break;
                                }
                                else { aIndex++; }
                            }
                        }
                        else
                        {
                            int aIndex = 0;
                            foreach (Area area in areas)
                            {
                                if (area.Name == playerArea)
                                {
                                    areaIndex = aIndex;
                                    break;
                                }
                                else { aIndex++; }
                            }
                        }
                    }
                    else
                    {
                        if (aDealer == null)
                        {
                            if(areas[areaIndex].GangPresance > 0)
                            {
                                Model model = 0;                              
                                List<Ped> aPeds = World.GetNearbyPeds(Game.Player.Character, 200).ToList();
                                if(aPeds.Count > 0)
                                {                                    
                                    foreach (Ped p in aPeds)
                                    {
                                        if (!p.IsInVehicle() && p.IsHuman && !p.IsPlayer && p.IsAlive && p.Position.DistanceTo(Game.Player.Character.Position) > 80 && World.GetZoneNameLabel(p.Position) == areas[areaIndex].Name)
                                        {
                                            if (customers.FindAll(r => r.Ped.Handle == p.Handle).Count == 0)
                                            {
                                                if (oldCustomers.FindAll(r => r.Handle == p.Handle).Count == 0)
                                                {
                                                    if (angryPeds.FindAll(r => r.Handle == p.Handle).Count == 0)
                                                    {
                                                        if (p.CurrentPedGroup != Game.Player.Character.CurrentPedGroup)
                                                        {
                                                            model = p.Model;                                                            
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if(model != 0)
                                    {
                                        Vector3 dStartPos = World.GetSafeCoordForPed(World.GetNextPositionOnSidewalk(Game.Player.Character.Position.Around(200)));
                                        for (int i = 0; i < 100; i++)
                                        {
                                            if (World.GetZoneNameLabel(dStartPos) != areas[areaIndex].Name)
                                            {
                                                dStartPos = World.GetSafeCoordForPed(World.GetNextPositionOnSidewalk(Game.Player.Character.Position.Around(200)));
                                            }
                                        }
                                        if (World.GetZoneNameLabel(dStartPos) == areas[areaIndex].Name)
                                        {
                                            Ped ped = World.CreatePed(model, dStartPos);
                                            if (ped != null)
                                            {
                                                aDealer = new AreaDealer(ped);
                                                if (DEBUG) { UI.Notify("Area Dealer Spawned using ~g~"+(PedHash)model); }
                                            }
                                            else
                                            {
                                                if (DEBUG) { UI.Notify("~r~There was a problem spawning the dealer!"); }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if(DEBUG) { UI.Notify("~r~No peds found to use as Dealer"); }
                                    }
                                    
                                }                                                          
                            }                            
                        } 
                        else
                        {                            
                            aDealer.DoState();
                            aDealer.CheckState();
                        }
                    }
                }
                // 300ms delay
                if (Game.GameTime > dayUpdateTimer + 350)
                {
                    if(aDealer != null)
                    {
                        if(aDealer.dPed.IsDead && !aDealer.dropDrugs)
                        {
                            aDealer.NewState = AreaDealer.dStates.NoState;
                            if (aDealer.cPed != null)
                            {
                                RemovePedFromWorld(aDealer.cPed, false);
                                aDealer.cPed = null;
                            }
                            aDealer.dPickup = World.CreatePickup(PickupType.MoneyMedBag, aDealer.dPed.Position, Vector3.Zero, new Model("prop_big_bag_01"), 0);
                            UI.ShowSubtitle("Dealer dropped a ~g~bag~w~, Go pick it up.", 4000);
                            if (aDealer.dPed.CurrentBlip == null) aDealer.dPed.AddBlip();
                            aDealer.dPed.CurrentBlip.Color = BlipColor.Green;                            
                            aDealer.dropDrugs = true;                            
                            if(areas[areaIndex].GangPresance > 0) { areas[areaIndex].GangPresance -= 1; }                            
                        }
                        if(aDealer.dropDrugs)
                        {
                            if(!aDealer.dPickup.ObjectExists())
                            {
                                int dWeed = rnd.Next(1, 50);
                                int dCrack = rnd.Next(1, 50);
                                int dCocaine = rnd.Next(1, 50);
                                PlayerInventory["Weed"] += dWeed;
                                PlayerInventory["Crack"] += dCrack;
                                PlayerInventory["Cocaine"] += dCocaine;
                                UpdateInventory();
                                UI.ShowSubtitle("You picked up " + dWeed + " ~g~Weed~w~, " + dCrack + " ~g~Crack~w~ and " + dCocaine + " ~g~Cocaine~w~.", 4000);
                                aDealer.dropDrugs = false;
                                aDealer.dPickup.Delete();
                                RemovePedFromWorld(aDealer.dPed, false);
                                aDealer = null;
                            }
                        }
                    }
                    dayUpdateTimer = Game.GameTime;
                    if (wepDealer != null && wepDealer.ped.IsAlive)
                    {
                        if (wepDealer.state == Dealer.States.Driving || wepDealer.state == Dealer.States.Fast)
                        {                                                     
                            if (wepDealer.vehicle.Position.DistanceTo(wepDealer.destination) < 80)
                            {                                
                                wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, 10f, (int)DrivingStyle.Normal);
                                wepDealer.state = Dealer.States.Ariving;
                            }
                        }
                        if (wepDealer.state == Dealer.States.Ariving)
                        {
                            if (wepDealer.vehicle.Position.DistanceTo(wepDealer.destination) < 20)
                            {
                                wepDealer.doneTutorial = false;
                                Vector3 pos = wepDealer.destination;
                                int NodeID = Function.Call<int>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID, pos.X, pos.Y, pos.Z, 1, 1, 1f, 0f);
                                OutputArgument outArgA = new OutputArgument();
                                Function.Call(Hash.GET_VEHICLE_NODE_POSITION, NodeID, outArgA);
                                Vector3 parkPos = outArgA.GetResult<Vector3>();
                                wepDealer.ped.Task.ParkVehicle(wepDealer.vehicle, parkPos, wepDealer.ped.CurrentVehicle.Heading);
                                wepDealer.destination = Vector3.Zero;
                                wepDealer.state = Dealer.States.Arrived;
                            }
                        }
                        if (wepDealer.state == Dealer.States.Arrived)
                        {
                            if (Game.Player.Character.IsInVehicle())
                            {
                                if (Game.Player.Character.CurrentVehicle.Handle == wepDealer.vehicle.Handle)
                                {
                                    if (!wepDealer.doneTutorial)
                                    {
                                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                        wepDealer.doneTutorial = true;
                                        displayHelpTimer = Game.GameTime;
                                        LsFunctions.DisplayHelpText("Press ~INPUT_CONTEXT~to interact or ~INPUT_CONTEXT_SECONDARY~to dismiss.");
                                    }
                                    if (Game.IsWaypointActive)
                                    {
                                        if (wepDealer.destination != World.GetNextPositionOnStreet(World.GetWaypointPosition()))
                                        {
                                            wepDealer.destination = World.GetNextPositionOnStreet(World.GetWaypointPosition());
                                            wepDealer.vehicle.IsInvincible = true;
                                            wepDealer.ped.Task.ClearAll();
                                            wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 1, 10f, (int)DrivingStyle.Normal);
                                            UI.ShowSubtitle("Ok, we heading to ~y~" +World.GetStreetName(wepDealer.destination));
                                            wepDealer.state = Dealer.States.Leaving;
                                        }
                                    }
                                }
                            }
                        }
                        if (wepDealer.state == Dealer.States.Leaving)
                        {
                            Wait(5000);
                            wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 1, 40f, (int)DrivingStyle.Normal);
                            wepDealer.state = Dealer.States.Driving;
                        }
                        if (wepDealer.state == Dealer.States.Faster)
                        {
                            wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, 50f, (int)DrivingStyle.AvoidTraffic);
                            wepDealer.state = Dealer.States.Fast;
                            UI.ShowSubtitle("Ok, putting my foot down.", 2000);
                        }
                        if (wepDealer.state == Dealer.States.Slower)
                        {
                            wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, 40f, (int)DrivingStyle.Normal);
                            wepDealer.state = Dealer.States.Driving;
                            UI.ShowSubtitle("Ok, Slowing it down.", 2000);
                        }
                    }
                    if(dealer1Angry && dealer1Debt < 170000)
                    {
                        dealer1Angry = false;
                    }
                    if (randomAttack == 1)
                    {
                        VehicleClass vehicleClass = VehicleClass.SUVs;
                        var vehs = World.GetNearbyVehicles(Game.Player.Character, 200);                        
                        VehicleHash vehicleHash = VehicleHash.Baller2;
                        if (vehs.ToList().Count > 0)
                        {
                            foreach (Vehicle v in vehs)
                            {
                                if (v.ClassType == vehicleClass)
                                {
                                    vehicleHash = (VehicleHash)v.Model.Hash;
                                    if(DEBUG) { UI.Notify("foundSUV"); }
                                }
                            }
                        }
                        
                        Vehicle vehicle = World.CreateVehicle((Model)vehicleHash, GenerateSpawnPos(Game.Player.Character.Position.Around(100), Nodetype.Road, false));
                        if (vehicle != null)
                        {
                            
                            vehicle.AddBlip();
                            vehicle.CreateRandomPedOnSeat(VehicleSeat.Driver);
                            vehicle.CreateRandomPedOnSeat(VehicleSeat.RightFront);
                            vehicle.CreateRandomPedOnSeat(VehicleSeat.RightRear);
                            vehicle.CreateRandomPedOnSeat(VehicleSeat.LeftRear);
                            Ped p1 = vehicle.GetPedOnSeat(VehicleSeat.Driver);
                            Ped p2 = vehicle.GetPedOnSeat(VehicleSeat.RightFront);
                            Ped p3 = vehicle.GetPedOnSeat(VehicleSeat.RightRear);
                            Ped p4 = vehicle.GetPedOnSeat(VehicleSeat.LeftRear);
                            randAttackers.Add(p1);
                            randAttackers.Add(p2);
                            randAttackers.Add(p3);
                            randAttackers.Add(p4);
                            randAttackVehs.Add(vehicle);
                            PedGroup attackers = new PedGroup();
                            attackers.Add(p1, true);
                            if (randAttackers.Count > 0)
                            {
                                foreach (Ped p in randAttackers)
                                {
                                    int EnemyRelationShipGroup = Function.Call<int>(Hash.GET_HASH_KEY, "HATES_PLAYER");
                                    Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, p, EnemyRelationShipGroup);
                                    p.Armor = 100;
                                    p.ShootRate = 75;
                                    p.Accuracy = 75;
                                    p.FiringPattern = FiringPattern.BurstFireDriveby;
                                    p.CanSwitchWeapons = true;
                                    int wepRnd = rnd.Next(0, 2);
                                    if (wepRnd == 0)
                                    {
                                        p.Weapons.Give(WeaponHash.CarbineRifle, 500, false, true);
                                        p.Weapons.Give(WeaponHash.MicroSMG, 500, true, true);
                                    }
                                    if (wepRnd == 1)
                                    {
                                        p.Weapons.Give(WeaponHash.PumpShotgun, 500, false, true);
                                        p.Weapons.Give(WeaponHash.Pistol, 500, true, true);
                                    }
                                    if (p != p1)
                                    {
                                        p1.CurrentPedGroup.Add(p, false);                                                                             
                                        p.Task.VehicleShootAtPed(Game.Player.Character);
                                    }
                                    else
                                    {
                                        p.Task.DriveTo(vehicle, Game.Player.Character.Position, 10, 30, (int)DrivingStyle.AvoidTrafficExtremely);                                        
                                        p.Task.VehicleChase(Game.Player.Character);
                                        randomAttack = 2;
                                    }
                                }
                            }
                        }                        
                    }
                    if (randomAttack == 2)
                    {
                        if (randAttackVehs.Count > 0)
                        {
                            foreach (Vehicle vehicle in randAttackVehs)
                            {
                                if (vehicle != null && vehicle.Exists())
                                {
                                    
                                    if (randAttackersChasing && vehicle.Position.DistanceTo(Game.Player.Character.Position) <= 15)
                                    {
                                        randAttackersChasing = false;
                                        vehicle.Driver.Task.VehicleShootAtPed(Game.Player.Character);
                                    }
                                    if (!randAttackersChasing && vehicle.Position.DistanceTo(Game.Player.Character.Position) > 15)
                                    {
                                        randAttackersChasing = true;
                                        vehicle.Driver.Task.VehicleChase(Game.Player.Character);

                                    }
                                }
                            }
                        }
                        if (randAttackers.Count > 0)
                        {
                            foreach (Ped p in randAttackers.ToList())
                            {
                                if (p != null && p.Exists())
                                {
                                    if (!p.IsInVehicle() && !p.CurrentBlip.Exists())
                                    {
                                        p.AddBlip();
                                    }
                                    if (p.IsDead)
                                    {
                                        if (areas[areaIndex].GangPresance > 0) { areas[areaIndex].GangPresance -= 1; }
                                        RemovePedFromWorld(p, true);
                                        randAttackers.Remove(p);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (randAttackVehs.Count > 0)
                            {
                                foreach (Vehicle veh in randAttackVehs.ToList())
                                {
                                    if (veh != null && veh.Exists())
                                    {
                                        entitiesCleanUp.Add(veh);
                                        veh.CurrentBlip.Remove();
                                        randAttackVehs.Remove(veh);
                                    }
                                }
                                randAttackVehs.Clear();
                            }
                            if (randAttackers.Count > 0)
                            {
                                foreach (Ped p in randAttackers.ToList())
                                {
                                    if (p != null && p.Exists())
                                    {
                                        entitiesCleanUp.Add(p);
                                        p.CurrentBlip.Remove();
                                        randAttackers.Remove(p);
                                    }
                                }
                                randAttackers.Clear();
                            }
                            randomAttack = 0;
                            if (DEBUG) { UI.Notify("rnd attackers are dead"); }
                        }
                    }                    
                    playerGroup = Game.Player.Character.CurrentPedGroup;                   
                    DateTimeUpdate();
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
                                            if (ammo < 100) { ammo = 100; }
                                            Game.Player.Character.Weapons.Give(weaponHash, ammo, true, true);
                                            Game.Player.Character.Weapons.Select(weaponHash, true);
                                            foreach (XElement c in element.Elements())
                                            {
                                                if (c.Name == "COMPONENT")
                                                {
                                                    Enum.TryParse(c.Value, out WeaponComponent wepcomp);
                                                    Game.Player.Character.Weapons.Current.SetComponent(wepcomp, true);
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
                            TotalDrugsCarried = 0;
                            foreach (KeyValuePair<string, int> entry in PlayerInventory)
                            {
                                TotalDrugsCarried += PlayerInventory[entry.Key];
                            }

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
                                            drug.Value = PlayerInventory["Cocaine"].ToString();
                                        }
                                    }
                                }
                            }
                            LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                        }
                    }
                    if (areas.Count > 0)
                    {
                        if (playerWanted)
                        {
                            areas[areaIndex].Heat += 2;                            
                        }
                        else
                        {
                            foreach(Area area in areas)
                            {
                                if(isDealing)
                                {
                                    if (area.Name != areas[areaIndex].Name && area.Heat > 0) { area.Heat -= 1; }
                                }
                                else
                                {
                                    if(area.Heat > 0) { area.Heat -= 1; }
                                }
                                                                                                
                            }
                        }
                    }
                    if (Game.Player.IsPlaying && notFined != true) { notFined = true; }
                    if (Game.Player.Character != null && !Game.Player.Character.IsAlive && !playerDead)
                    {
                        if (drugCarSpawned)
                        {
                            drugCar.InstallModKit();
                            drugCarSpawned = false;
                        }
                        playerDead = true;
                        if (isDealing) { isDealing = false; }
                        var killer = Game.Player.Character.GetKiller();
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
                                            SendTextMsg("I found the guy that got you, i sent you his location.");
                                            attacker = p;
                                            attacker.IsPersistent = true;                                            
                                            attacker.Money = Game.Player.Money;
                                            Game.Player.Money -= Game.Player.Money;
                                            if (angryPeds.Contains(attacker)) { angryPeds.Remove(attacker); }
                                            if (attacker.CurrentBlip.Exists()) { attacker.CurrentBlip.Remove(); }
                                            attacker.AddBlip();
                                            attacker.CurrentBlip.IsFriendly = false;
                                            attacker.CurrentBlip.Name = "Your Killer";
                                            WeaponHash weaponHash = WeaponHash.Unarmed;
                                            int ammo = 0;
                                            Game.Player.Character.Weapons.RemoveAll();
                                            attacker.Armor = 100;
                                            foreach (XElement element in playerWeapons)
                                            {
                                                if (element.Name == "weapon")
                                                {
                                                    attackerWeapons.Add(element);
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
                                                            attacker.Weapons.Give(weaponHash, ammo, true, true);
                                                        }
                                                    }
                                                }
                                            }
                                            
                                            playerWeapons.Clear();
                                            playerWeps.Clear();
                                            attacker.Weapons.Select(attacker.Weapons.BestWeapon);
                                            attacker.DropsWeaponsOnDeath = false;
                                            if (DEBUG)
                                            {
                                                UI.Notify("Killer has equiped " + attacker.Weapons.Current.Name + " & and has equiped Body Armour");
                                            }                                            
                                            foreach (KeyValuePair<string, int> entry in PlayerInventory)
                                            {
                                                if(!attackerInventory.ContainsKey(entry.Key))
                                                {
                                                    attackerInventory.Add(entry.Key, entry.Value);
                                                }                                                
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Game.Player.Character != null && !Game.Player.IsPlaying)
                    {
                        if (isDealing == true) { isDealing = false; }
                        if (notFined && playerWanted)
                        {
                            int drugCarDrugs = 0;
                            notFined = false;
                            playerWanted = false;
                            if (drugCarWanted && drugCar != null)
                            {
                                SendTextMsg("Storage Vehicle has been confiscated, and its contents destroyed");
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

                                
                                drugCarDrugs += drugCarWeed;
                                drugCarDrugs += drugCarCrack;
                                drugCarDrugs += drugCarCocain;                                
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
                            if (TotalDrugsCarried + drugCarDrugs > 0)
                            {
                                SendTextMsg("They took $" + Game.Player.Money + ", all your Drugs and Weapons.");
                                Game.Player.Character.Weapons.RemoveAll();
                                Game.Player.Money -= Game.Player.Money;
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
                                Game.Player.Character.Weapons.RemoveAll();
                                SendTextMsg("Someone robbed your $" + Game.Player.Money + ", all your Drugs and Weapons.");
                                Game.Player.Money -= Game.Player.Money;
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
                            pIndex++;
                            if (p == null)
                            {
                                angryPeds.RemoveAt(pIndex);
                                break;
                            }                                                 
                            if (p != null)
                            {
                                if (p.IsDead)
                                {                                    
                                    RemovePedFromWorld(p, false);
                                    angryPeds.Remove(p);
                                    if (areas[areaIndex].GangPresance > 0) { areas[areaIndex].GangPresance -= 1; }
                                    break;                                                                                
                                }
                                else
                                if (Game.Player.Character.Position.DistanceTo(p.Position) > 100)
                                {
                                    RemovePedFromWorld(p, false);
                                    angryPeds.Remove(p);
                                    break;
                                }
                                else
                                {
                                    if (!p.IsInCombatAgainst(Game.Player.Character) || !p.IsInMeleeCombat) { p.Task.FightAgainst(Game.Player.Character); }                                                                                                       
                                }                                
                            }                            
                        }
                    }
                    if (Game.Player.Character != null && Game.Player.Character.IsAlive && Game.Player.IsPlaying)
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
                        if (attacker != null)
                        {
                            if (attacker.IsDead)
                            {
                                RemovePedFromWorld(attacker, true);
                                if (!attackerPickupSpawned)
                                {
                                    attackerAttacking = false;
                                    attackerPickupSpawned = true;
                                    attackerPickup = World.CreatePickup(PickupType.MoneyMedBag, attacker.Position, Vector3.Zero, new Model("prop_big_bag_01"), 100);                                    
                                }
                                attacker = null;
                                attackerInventory.Clear();
                            }
                            else
                            {
                                if (!attacker.IsPersistent) { attacker.IsPersistent = true; }
                                float distance = attacker.Position.DistanceTo(Game.Player.Character.Position);                                
                                
                                if (!attackerAttacking)
                                {
                                    if (Game.GameTime > rayTimer + 1000)
                                    {
                                        rayTimer = Game.GameTime;
                                        //RaycastResult raycast = World.RaycastCapsule(attacker.Position + attacker.ForwardVector, Game.Player.Character.Position, 60, IntersectOptions.Everything);
                                        if (distance < 60 && Function.Call<bool>(Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY_IN_FRONT, attacker, Game.Player.Character))
                                        {
                                            attackerAttacking = true;
                                            attacker.Task.ClearAll();
                                            attacker.Task.FightAgainst(Game.Player.Character);
                                            
                                            int EnemyRelationShipGroup = Function.Call<int>(Hash.GET_HASH_KEY, "HATES_PLAYER");
                                            Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, attacker, EnemyRelationShipGroup);                                            
                                        }
                                        else
                                        {
                                            attacker.Task.ClearAll();
                                            attacker.Task.WanderAround();
                                            attackerAttacking = false;
                                        }
                                    }                                    
                                }
                            }
                        }
                        if (drugCarDestroyed == false && drugCar == null)
                        {
                            if (Game.Player.Character.Position.DistanceTo(vehPos) <= 200)
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
                                    if (drugCar.Driver == Game.Player.Character)
                                    {
                                        drugCar.CurrentBlip.Remove();
                                    }
                                }
                                if (Game.Player.Character.IsInVehicle())
                                {
                                    if (playerWanted && drugCar.Handle == Game.Player.Character.CurrentVehicle.Handle && drugCarWanted == false)
                                    {
                                        drugCarWanted = true;
                                    }
                                    if (drugCarSaved == true && drugCar.Driver == Game.Player.Character)
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
                                }
                                if (drugCarSaved == false && vehPos != drugCar.Position && !Game.Player.Character.IsInVehicle())
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
                                    foreach (XElement element in veh.Descendants())
                                    {
                                        if (element.Name == "Destroyed")
                                        {
                                            element.Value = drugCarDestroyed.ToString();
                                        }
                                    }
                                    veh.Save("scripts\\LSLife\\LSLife_Vehicles.xml");
                                }
                            }
                        }
                        if (!playerHasBag && (TotalDrugsCarried / 28) + playerWeps.Count + (Game.Player.Money/ 50000) > 15)
                        {
                            LsFunctions.PedBag(Game.Player.Character, true);
                            playerHasBag = true;
                            Function.Call(Hash.SET_PLAYER_SPRINT, Game.Player, false);
                        }
                        if (playerHasBag && (TotalDrugsCarried / 28) + playerWeps.Count + (Game.Player.Money / 50000) <= 15)
                        {
                            LsFunctions.PedBag(Game.Player.Character, false);
                            playerHasBag = false;
                            Function.Call(Hash.SET_PLAYER_SPRINT, Game.Player, true);
                        }                        
                        if (Game.Player.WantedLevel > 0 && playerWanted == false) { playerWanted = true; }
                        if (playerWanted && Game.Player.WantedLevel == 0) { playerWanted = false; }                       
                        if (drugCarDestroyed == false && drugCar != null && Game.Player.Character.Position.DistanceTo(drugCar.Position) > 200)
                        {
                            if (drugCar.Exists())
                            {
                                if (Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle != drugCar)
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
                                if (!Game.Player.Character.IsInVehicle())
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
                        if (weaponDealer1 != null)
                        {
                            if (weaponDealer1.IsDead)
                            {
                                RemovePedFromWorld(weaponDealer1, true);
                                weaponDealer1 = null;
                            }
                        }
                        if (Game.Player.WantedLevel > 0 && isDealing == true)
                        {
                            isDealing = false;
                        }
                        if (!isDealing)
                        {
                            if(!removedStuff)
                            {
                                removedStuff = true;
                                timerBarPool.Remove(heatBar);
                                if (pig != null && pig.Ped != null && pig.Ped.Exists())
                                {
                                    RemovePedFromWorld(pig.Ped, true);
                                    pig = null;
                                }
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
                                if (angryPeds.Count > 0)
                                {
                                    foreach (Ped p in angryPeds)
                                    {
                                        if (p != null && p.Exists())
                                        {
                                            RemovePedFromWorld(p, true);
                                        }
                                    }
                                    angryPeds.Clear();
                                }
                            }
                            if (ratChance != -100)
                            {
                                ratChance -= 1;
                            }
                        }
                        if (isDealing)
                        {                            
                            if(pig != null)
                            {
                                if(pig.Ped == null || !pig.Ped.Exists() || !pig.Ped.IsAlive || pig.Ped.Position.DistanceTo(Game.Player.Character.Position) > 150f)
                                {
                                    RemovePedFromWorld(pig.Ped, true);
                                    pig = null;
                                    if (DEBUG) { UI.Notify("Removed spawned Cop"); }
                                }
                                else
                                {
                                    pig.DoState();
                                    pig.CheckState();
                                }                                
                            }
                            if (pig == null && heatBar.Percentage > 0.25f && rnd.Next(0, 10) == 0)
                            {
                                Ped ped = World.CreatePed(PedHash.Cop01SMY, GenerateSpawnPos(Game.Player.Character.Position.Around(80f), Nodetype.AnyRoad, true));
                                if (ped != null)
                                {
                                    Function.Call(Hash.SET_PED_AS_COP, ped, true);
                                    if (DEBUG) { UI.Notify("Spawned Cop"); }
                                    pig = new Pig(ped);                                    
                                    if (areas[areaIndex].CopPresance < 10) { areas[areaIndex].CopPresance++; }                                    
                                }
                            }
                            int totalDrugs = TotalDrugsCarried + stashWeed + stashCrack + stashCocain + drugCarWeed + drugCarCrack + drugCarCocain;
                            if (dealer1Debt < 110000 && totalDrugs > 56 && Game.GameTime > jobOfferTimer + 20000 && jobs.ToList().Count == 0)
                            {
                                jobOfferTimer = Game.GameTime;
                                int r = rnd.Next(0, 100);
                                if (r > 70- LsFunctions.CurrentRepLvl(dealer1Rep))
                                {
                                    Vector3 spawnPos = GenerateSpawnPos(Game.Player.Character.Position.Around(275f), Nodetype.Offroad, true);                                    
                                    if (spawnPos == Vector3.Zero)
                                    {
                                        if(DEBUG) { UI.Notify("spawning new job, that one guy was found."); }
                                        spawnPos = GenerateSpawnPos(Game.Player.Character.Position.Around(200f), Nodetype.Offroad, true);
                                    }
                                    for(int j = 0; j < 100; j++)
                                    {
                                        if (Function.Call<bool>(Hash.IS_POINT_ON_ROAD, spawnPos.X, spawnPos.Y, spawnPos.Z))
                                        {
                                            spawnPos = GenerateSpawnPos(Game.Player.Character.Position.Around(200f), Nodetype.Offroad, true);
                                            spawnPos = World.GetNextPositionOnSidewalk(spawnPos);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }                                    
                                    var peds = World.GetNearbyPeds(Game.Player.Character.Position, 200f);
                                    var pedsList = peds.ToList();
                                    if (pedsList.Count > 0)
                                    {
                                        
                                        foreach (Ped p in peds)
                                        {
                                            if (!p.IsPlayer && p.IsHuman)
                                            {
                                                r = rnd.Next(0, 10);
                                                if (r > 3)
                                                {
                                                    Ped newPed = World.CreatePed(p.Model, spawnPos);
                                                    if (DEBUG) { UI.Notify("jobPos:" + spawnPos + " pedPos:" + newPed.Position); }
                                                    if (newPed != null)
                                                    {
                                                        List<string> pDrugs = new List<string>();
                                                        if (sellingWeed) { pDrugs.Add("Weed"); }
                                                        if (sellingCrack) { pDrugs.Add("Crack"); }
                                                        if (sellingCocaine) { pDrugs.Add("Cocaine"); }
                                                        Drug drug = null;
                                                        switch (pDrugs[rnd.Next(0,pDrugs.Count)])
                                                        {
                                                            case "Weed":
                                                                drug = areas[areaIndex].Drugs.Find(d => d.Name == "Weed");
                                                                break;
                                                            case "Crack":
                                                                drug = areas[areaIndex].Drugs.Find(d => d.Name == "Crack");
                                                                break;
                                                            case "Cocaine":
                                                                drug = areas[areaIndex].Drugs.Find(d => d.Name == "Cocaine");
                                                                break;
                                                        }
                                                        jobs.Add(new Customer(newPed, spawnPos, true, null, drug, (int)LsFunctions.CurrentRepLvl(dealer1Rep)));
                                                        if (jobs.Count > 0)
                                                        {
                                                            Customer c = jobs[0];
                                                            int price = (c.amount * c.Drug.PricePerG) - ((int)(c.amount / 3.5) * c.Drug.PricePerG);
                                                            SendTextMsg("Yo can you go drop ~g~" + c.amount + "~w~g of ~g~" + c.Drug.Name + " ~w~to my pal for $~g~" + price + "~w~?");
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
                            var distanceToPlayer = World.GetDistance(drugDealer1.Position, Game.Player.Character.Position);
                            if (drugDealer1.Position.DistanceTo(Game.Player.Character.Position) > 15f && drugDealer1.CurrentPedGroup != Game.Player.Character.CurrentPedGroup)
                            {
                                if (Game.GameTime > dealerTimer + 2000 && Game.Player.Character.Position != pedTarget)
                                {
                                    dealerTimer = Game.GameTime;
                                    pedTarget = Game.Player.Character.Position;
                                    drugDealer1.Task.GoTo(Game.Player.Character);
                                }
                            }
                            else
                            {
                                if (drugDealer1.CurrentPedGroup != Game.Player.Character.CurrentPedGroup)
                                {
                                    drugDealer1.Task.ClearAll();
                                    Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, drugDealer1.Handle, playerGroup);
                                }
                            }
                            if (drugDealer1.IsStopped && distanceToPlayer <= 3f && Game.Player.Character.IsStopped && Game.GameTime > displayHelpTimer + 1000)
                            {
                                {
                                    displayHelpTimer = Game.GameTime;

                                    var peds = World.GetNearbyPeds(Game.Player.Character, 10f);
                                    if (peds.Count() > 3)
                                    {
                                        //DealerTaskUI = new UIText("Its to busy here.", new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                                        LsFunctions.DisplayHelpText("Find some where quiter.");
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
                                        LsFunctions.DisplayHelpText("Press ~INPUT_CONTEXT~ to interact");
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
                                SendTextMsg("Dealer is dead!");
                                drugDealer1 = null;
                                readyToDeal = false;
                            }
                        }
                        if (weaponDealer1 != null && weaponDealer1.IsAlive)
                        {
                            var distanceToTarget = World.GetDistance(weaponDealer1.Position, pedTarget);
                            var distanceToPlayer = World.GetDistance(weaponDealer1.Position, Game.Player.Character.Position);
                            if (weaponDealer1.Position.DistanceTo(Game.Player.Character.Position) > 15f && weaponDealer1.CurrentPedGroup != Game.Player.Character.CurrentPedGroup)
                            {
                                if (Game.GameTime > dealerTimer + 2000 && Game.Player.Character.Position != pedTarget)
                                {
                                    dealerTimer = Game.GameTime;
                                    pedTarget = Game.Player.Character.Position;
                                    weaponDealer1.Task.GoTo(Game.Player.Character);
                                }
                            }
                            else
                            {
                                if (weaponDealer1.CurrentPedGroup != Game.Player.Character.CurrentPedGroup)
                                {
                                    weaponDealer1.Task.ClearAll();
                                    Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, weaponDealer1.Handle, playerGroup);
                                }
                            }
                            if (weaponDealer1.IsStopped && distanceToPlayer <= 3f && Game.Player.Character.IsStopped && Game.GameTime > displayHelpTimer + 1000)
                            {
                                {
                                    displayHelpTimer = Game.GameTime;

                                    var peds = World.GetNearbyPeds(Game.Player.Character, 10f);
                                    if (peds.Count() > 3)
                                    {
                                        //DealerTaskUI = new UIText("Its to busy here.", new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                                        LsFunctions.DisplayHelpText("Find some where quiter.");
                                        readyToDeal = false;
                                        if (LsMenuPool.IsAnyMenuOpen())
                                        {
                                            wepMenu.Visible = !wepMenu.Visible;
                                        }
                                    }
                                    else
                                    if (!readyToDeal && !LsMenuPool.IsAnyMenuOpen())
                                    {
                                        //DealerTaskUI = new UIText("This will do.", new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                                        LsFunctions.DisplayHelpText("Press ~INPUT_CONTEXT~ to interact");
                                        readyToDeal = true;
                                    }
                                }
                            }
                            
                            if (!weaponDealer1.IsStopped)
                            {
                                readyToDeal = false;
                            }
                           
                            if (weaponDealer1 != null && !weaponDealer1.IsAlive)
                            {
                                weaponDealer1.IsPersistent = false;
                                weaponDealer1.CurrentBlip.Remove();
                                weaponDealer1.MarkAsNoLongerNeeded();
                                SendTextMsg("Dealer is dead!");
                                weaponDealer1 = null;
                                readyToDeal = false;
                            }
                        }
                    }
                }
                // if we alive do this stuff each Tick.
                if (Game.Player.Character != null && Game.Player.Character.IsAlive && Game.Player.IsPlaying)
                {
                    if (drugCar != null)
                    {
                        if (drugCar.Exists())
                        {
                            Vector3 backPos = Vector3.Add(drugCar.Position, Vector3.Multiply(drugCar.ForwardVector * -1, (drugCar.Model.GetDimensions().Y / 2) + 0.5f));
                            if (drugCar.Driver != Game.Player.Character && Game.Player.Character.Position.DistanceTo(drugCar.Position) < 10)
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
                            if (!Game.Player.Character.IsInVehicle() && Game.Player.Character.Position.DistanceTo(backPos) <= 1f && drugCar.Model.IsCar)
                            {
                                bool trunkFound = false;
                                if (drugCar.HasBone("boot")) { trunkFound = true; }
                                if (trunkFound)
                                {
                                    if (!drugCar.IsDoorOpen(VehicleDoor.Trunk))
                                    {
                                        if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen() && Game.Player.Character.IsStopped)
                                        {
                                            displayHelpTimer = Game.GameTime;
                                            LsFunctions.DisplayHelpText("~INPUT_SPRINT~ to open trunk.");
                                        }
                                        if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && Game.Player.Character.IsStopped)
                                        {
                                            drugCar.OpenDoor(VehicleDoor.Trunk, false, false);
                                            if (PlayerInventory.Count > 0)
                                            {
                                                putWeedVeh.Maximum = PlayerInventory["Weed"];
                                                putCrackVeh.Maximum = PlayerInventory["Crack"];
                                                putCocainVeh.Maximum = PlayerInventory["Cocaine"];
                                            }
                                            putMoneyVeh.Maximum = Game.Player.Money;
                                            UpdateCarStash();
                                            vehStashMainMenu.Visible = !vehStashMainMenu.Visible;
                                        }
                                    }
                                    else
                                    {
                                        if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen() && Game.Player.Character.IsStopped)
                                        {
                                            displayHelpTimer = Game.GameTime;
                                            DisplayHelpText("~INPUT_SPRINT~ to close trunk.");
                                        }
                                        if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && Game.Player.Character.IsStopped)
                                        {
                                            
                                            drugCar.CloseDoor(VehicleDoor.Trunk, false);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!drugCar.IsDoorOpen(VehicleDoor.BackLeftDoor) && !LsMenuPool.IsAnyMenuOpen() && Game.Player.Character.IsStopped)
                                    {
                                        if (Game.GameTime > displayHelpTimer + 3000)
                                        {
                                            displayHelpTimer = Game.GameTime;
                                            LsFunctions.DisplayHelpText("~INPUT_SPRINT~ to back doors.");
                                        }
                                        if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && Game.Player.Character.IsStopped)
                                        {
                                            drugCar.OpenDoor(VehicleDoor.BackLeftDoor, false, false);
                                            drugCar.OpenDoor(VehicleDoor.BackRightDoor, false, false);

                                            vehStashMainMenu.Visible = !vehStashMainMenu.Visible;
                                        }
                                    }
                                    else
                                    {
                                        if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen() && Game.Player.Character.IsStopped)
                                        {
                                            displayHelpTimer = Game.GameTime;
                                            LsFunctions.DisplayHelpText("~INPUT_SPRINT~ to close back doors.");
                                        }
                                        if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen() && Game.Player.Character.IsStopped)
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

                    if (drugDealer1 != null && readyToDeal && !LsMenuPool.IsAnyMenuOpen())
                    {
                        if (Game.IsControlJustPressed(2, GTA.Control.Context))
                        {
                            if (LsMenuPool != null)
                            {
                                dealerMainMenu.Visible = !dealerMainMenu.Visible;
                                dealerMainMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)LsFunctions.CurrentRepLvl(dealer1Rep);
                                weedMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)LsFunctions.CurrentRepLvl(dealer1Rep);
                                crackMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)LsFunctions.CurrentRepLvl(dealer1Rep);
                                cocainMenu.Subtitle.Caption = "Rep:" + dealer1Rep + " Lvl:" + (int)LsFunctions.CurrentRepLvl(dealer1Rep);
                                bushWeed.Text = "Weed $" + (weed1Price - LsFunctions.Discount(weed1Price)) + "oz";
                                crack.Text = "Crack $" + (crack1Price - LsFunctions.Discount(crack1Price)) + "oz";
                                cocaine.Text = "Cocaine $" + (cocain1Price - LsFunctions.Discount(cocain1Price)) + "oz";
                            }
                        }
                        if (Game.IsControlJustPressed(2, GTA.Control.ContextSecondary) & chilling == false)
                        {
                            Wait(10);
                            drugDealer1.Task.StartScenario("WORLD_HUMAN_SMOKING_POT", drugDealer1.Position, drugDealer1.Heading);
                            Wait(10);
                            Game.Player.Character.Task.StartScenario("WORLD_HUMAN_SMOKING_POT", Game.Player.Character.Position, Game.Player.Character.Heading);
                            chilling = true;
                        }
                        if (chilling)
                        {
                            if (Game.IsControlJustPressed(2, Control.MoveLeftRight) || Game.IsControlJustPressed(2, Control.MoveUpDown))
                            {
                                chilling = false;
                                Game.Player.Character.Task.ClearAll();
                                drugDealer1.Task.ClearAll();
                            }
                        }
                    }
                    if (weaponDealer1 != null && readyToDeal && !LsMenuPool.IsAnyMenuOpen())
                    {
                        if (Game.IsControlJustPressed(2, GTA.Control.Context))
                        {
                            if (LsMenuPool != null)
                            {
                                wepMenu.Visible = !wepMenu.Visible;
                            }
                        }
                        if (Game.IsControlJustPressed(2, GTA.Control.ContextSecondary) & chilling == false)
                        {
                            Wait(10);
                            float dHeading = weaponDealer1.Heading;
                            float pHeading = Game.Player.Character.Heading;
                            weaponDealer1.Task.StartScenario("WORLD_HUMAN_SMOKING_POT", weaponDealer1.Position, dHeading);
                            weaponDealer1.Heading = dHeading;
                            Wait(10);
                            Game.Player.Character.Task.StartScenario("WORLD_HUMAN_SMOKING_POT", Game.Player.Character.Position, pHeading);
                            chilling = true;
                        }
                        if (chilling)
                        {
                            if (Game.IsControlJustPressed(2, Control.MoveLeftRight) || Game.IsControlJustPressed(2, Control.MoveUpDown))
                            {
                                chilling = false;
                                Game.Player.Character.Task.ClearAll();
                                weaponDealer1.Task.ClearAll();
                            }
                        }
                    }
                    //STREET DEALING                                  
                    if (isDealing == true)
                    {
                        if (ratChance <= 0f)
                        {
                            if (heatBar.Percentage != 0f) { heatBar.Percentage = 0f; }
                        }
                        else
                        {
                            Color color = Color.White;
                            if (heatBar.Percentage < 1) { heatBar.Percentage = ((float)ratChance / 100).Clamp(0, 1); }
                            if (heatBar.Percentage > 0.5f) { color = Color.Red; }
                            else
                            if (heatBar.Percentage > 0.25f) { color = Color.Yellow; }

                            if(color != heatBar.ForegroundColor) { heatBar.ForegroundColor = color; }
                            
                        }
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
                                    Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                    jobOffer = false;
                                    if (jobs[0].Ped != null)
                                    {
                                        SendTextMsg("Ok, ill let you know if we get anymore.");
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
                                    SendTextMsg("Cool, Sent you a ping.");
                                    if(PlayerInventory[jobs[0].Drug.Name] >= jobs[0].amount)
                                    {
                                        UI.ShowSubtitle("Deliver ~g~" + jobs[0].amount + "~w~g of ~g~" + jobs[0].Drug.Name + "~w~ to ~y~" + World.GetStreetName(jobs[0].Ped.Position) + ".", 10000);
                                    }
                                    else
                                    {
                                        UI.ShowSubtitle("You do not have enough ~g~" + jobs[0].Drug.Name + "~w~ go get ~g~" + jobs[0].amount + "~w~g and deliver it to ~y~" + World.GetStreetName(jobs[0].Ped.Position) + ".", 10000);
                                    }
                                    
                                }
                                if (Game.GameTime > jobAcceptTimer + 15000)
                                {
                                    SendTextMsg("Too slow man, got someone.");
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
                                        if (j.Ped.CurrentPedGroup != Game.Player.Character.CurrentPedGroup && j.Ped.Position.DistanceTo(Game.Player.Character.Position) <= 10f)
                                        {
                                            j.Ped.Task.ClearAll();
                                            Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, j.Ped.Handle, playerGroup);
                                            j.Ped.CurrentBlip.ShowRoute = false;
                                        }
                                        if (!LsMenuPool.IsAnyMenuOpen() && j.Ped.Position.DistanceTo(Game.Player.Character.Position) <= 2)
                                        {
                                            if (TotalDrugsCarried == 0)
                                            {
                                                SendTextMsg("You got nothing to sell.");
                                                lastTimeDealDone = Game.GameTime;
                                                RemovePedFromWorld(j.Ped, true);
                                                break;
                                            }
                                            else
                                            {
                                                if (Game.Player.Character.IsInVehicle() == true && j.Ped.IsInVehicle() == true)
                                                {
                                                    if (Game.GameTime > displayHelpTimer + 3000)
                                                    {
                                                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                                        displayHelpTimer = Game.GameTime;
                                                        DisplayHelpText("~INPUT_SPRINT~ to sell to this person.");
                                                    }
                                                    if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen())
                                                    {
                                                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                                        customerSellTo = j;
                                                        sell.Text = "Sell " + j.amount + "g of " + j.Drug.Name + "?";
                                                        customerMainMenu.Subtitle.Caption = "You have " + PlayerInventory[j.Drug.Name] + "g of " + j.Drug.Name;
                                                        customerMainMenu.Visible = !customerMainMenu.Visible;
                                                        customerMainMenu.CurrentSelection = 0;
                                                        break;
                                                    }
                                                }
                                                if (Game.Player.Character.IsInVehicle() == false)
                                                {
                                                    if (Game.GameTime > displayHelpTimer + 3000)
                                                    {
                                                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                                        displayHelpTimer = Game.GameTime;
                                                        DisplayHelpText("Press ~INPUT_SPRINT~ to sell to this person.");
                                                    }
                                                    if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen())
                                                    {
                                                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                                        customerSellTo = j;
                                                        sell.Text = "Got " + j.amount + "g of " + j.Drug.Name + "?";
                                                        customerMainMenu.Subtitle.Caption = "You have " + PlayerInventory[j.Drug.Name] + "g of " + j.Drug.Name;
                                                        customerMainMenu.Visible = !customerMainMenu.Visible;
                                                        customerMainMenu.CurrentSelection = 0;
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
                        
                        if (oldCustomers.ToList().Count > 0 && Game.GameTime > lastAngryTime + 10000)
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
                                    RemovePedFromWorld(ped, false);
                                    oldCustomers.RemoveAt(i);
                                    break;
                                }
                                float distanceToPlayer = ped.Position.DistanceTo(Game.Player.Character.Position);
                                if (ped != null && ped.IsAlive && distanceToPlayer < 30 && distanceToPlayer > 4)
                                {
                                    if (ped.Gender.Equals(Gender.Male) && areas[areaIndex].GangPresance > 0)
                                    {
                                        lastAngryTime = Game.GameTime;                                        
                                        int anger = rnd.Next(0, 100);
                                        if (anger >= (99 - areas[areaIndex].GangPresance))
                                        {                                            
                                            if (DEBUG)
                                            {
                                                UI.Notify("Ped angry:" + anger + ">=" + (99 - areas[areaIndex].GangPresance));
                                            }
                                            int EnemyRelationShipGroup = Function.Call<int>(Hash.GET_HASH_KEY, "HATES_PLAYER");
                                            Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, ped, EnemyRelationShipGroup);
                                            ped.CanSwitchWeapons = true;
                                            int n = rnd.Next(0, 5);
                                            if (n == 1)
                                            {
                                                ped.Weapons.Give(WeaponHash.SawnOffShotgun, 100, true, true);
                                            }
                                            if (n == 2)
                                            {
                                                ped.Weapons.Give(WeaponHash.Bat, 1, true, false);
                                            }
                                            if (n == 3)
                                            {
                                                ped.Weapons.Give(WeaponHash.Knife, 1, true, false);
                                            }
                                            if (n == 4)
                                            {
                                                ped.Weapons.Give(WeaponHash.MicroSMG, 100, true, true);
                                            }                                            
                                            ped.Weapons.Select(ped.Weapons.BestWeapon);
                                            ped.AddBlip();
                                            ped.Armor = 100;
                                            Function.Call(Hash.SET_PED_COMBAT_ABILITY, ped, 100);
                                            ped.IsPersistent = true;
                                            ped.BlockPermanentEvents = true;
                                            ped.AlwaysKeepTask = true;
                                            ped.CurrentBlip.IsFriendly = false;
                                            ped.Task.FightAgainstHatedTargets(100);
                                            ped.Task.FightAgainst(Game.Player.Character);
                                            oldCustomers.Remove(ped);
                                            
                                            angryPeds.Add(ped);
                                            if (playerHasBag && rnd.Next(0,10) == 0)
                                            {
                                                if (areas[areaIndex].GangPresance > 4 && randomAttack == 0)
                                                {
                                                    if (DEBUG) { UI.Notify("Robber Drive-by"); }
                                                    randomAttack = 1;
                                                }
                                            }                                                                                        
                                            break;
                                        }
                                    }                                                                                                            
                                }
                            }
                        }
                        //select ped for deal
                        
                        if (customers.Count < 5 && jobs.Count == 0)
                        {
                            if (Game.GameTime > lastTimePedSelected + 1000)
                            {
                                lastTimePedSelected = Game.GameTime;
                                int getPed = rnd.Next(0, 100);
                                if (getPed > 60)
                                {
                                    if(areas[areaIndex].NeedCustomer())
                                    {

                                        List<Ped> peds = new List<Ped>();
                                        foreach (Ped p in World.GetNearbyPeds(Game.Player.Character, 75f))
                                        {
                                            if (p.IsHuman && !p.IsPlayer && p.IsAlive)
                                            {
                                                if (customers.FindAll(r => r.Ped.Handle == p.Handle).Count == 0)
                                                {
                                                    if (oldCustomers.ToList().FindAll(r => r.Handle == p.Handle).Count == 0)
                                                    {
                                                        if (angryPeds.ToList().FindAll(r => r.Handle == p.Handle).Count == 0)
                                                        {
                                                            if (p.CurrentPedGroup != Game.Player.Character.CurrentPedGroup)
                                                            {
                                                                if(aDealer != null)
                                                                {
                                                                    if(p.Handle!=aDealer.dPed.Handle)
                                                                    {
                                                                        peds.Add(p);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    peds.Add(p);
                                                                }
                                                                
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (peds.Count > 0)
                                        {
                                            areas[areaIndex].GetCustomer(peds);
                                            Vector3 playerPos = Game.Player.Character.Position;
                                            foreach (Drug drug in areas[areaIndex].Drugs)
                                            {
                                                bool chooseCustomer = true;
                                                //check to see if we are selling the drug
                                                switch (drug.Name)
                                                {
                                                    case "Weed":
                                                        if (!sellingWeed)
                                                        {
                                                            if(areas[areaIndex].AreaType == AreaType.Normal)
                                                            {
                                                                if (areas[areaIndex].DemandNotMet == false)
                                                                {
                                                                    areas[areaIndex].DemandNotMet = true;
                                                                    areas[areaIndex].HighDemandTimer = Game.GameTime;
                                                                }                                                                    
                                                            }
                                                            chooseCustomer = false;
                                                            drug.SelectCustomer = false;
                                                            drug.Customer = null;
                                                        }
                                                        break;
                                                    case "Crack":
                                                        if (!sellingCrack)
                                                        {
                                                            if(areas[areaIndex].AreaType == AreaType.Poor)
                                                            {
                                                                if (areas[areaIndex].DemandNotMet == false)
                                                                {
                                                                    areas[areaIndex].DemandNotMet = true;
                                                                    areas[areaIndex].HighDemandTimer = Game.GameTime;
                                                                }
                                                            }                                                            
                                                            chooseCustomer = false;
                                                            drug.SelectCustomer = false;
                                                            drug.Customer = null;
                                                        }
                                                        break;
                                                    case "Cocaine":
                                                        if (!sellingCocaine)
                                                        {
                                                            if (areas[areaIndex].AreaType == AreaType.Rich)
                                                            {
                                                                if (areas[areaIndex].DemandNotMet == false)
                                                                {
                                                                    areas[areaIndex].DemandNotMet = true;
                                                                    areas[areaIndex].HighDemandTimer = Game.GameTime;
                                                                }
                                                            }                                                            
                                                            chooseCustomer = false;
                                                            drug.SelectCustomer = false;
                                                            drug.Customer = null;
                                                        }
                                                        break;
                                                }
                                                // check to see if the drug is in demand
                                                if (chooseCustomer && drug.SelectCustomer)
                                                {                                                   
                                                    Ped p = drug.Customer;
                                                    drug.Customer = null;
                                                    drug.SelectCustomer = false;
                                                    if(p != null)
                                                    {
                                                        if (p.IsInVehicle() && p.CurrentVehicle.Driver == p)
                                                        {
                                                            Vehicle vehicle = p.CurrentVehicle;
                                                            customers.Add(new Customer(p, playerPos, false, vehicle, drug, (int)LsFunctions.CurrentRepLvl(dealer1Rep)));
                                                        }
                                                        else
                                                        {
                                                            customers.Add(new Customer(p, playerPos, false, null, drug, (int)LsFunctions.CurrentRepLvl(dealer1Rep)));
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
                                            if (c.Ped.Position.DistanceTo(Game.Player.Character.Position) > 80f)
                                            {
                                                customers.Remove(c);
                                                RemovePedFromWorld(c.Ped, true);
                                                break;
                                            }
                                            if (c.Ped.CurrentPedGroup != Game.Player.Character.CurrentPedGroup && c.Ped.Position.DistanceTo(Game.Player.Character.Position) > 4f)
                                            {
                                                if (c.Ped.IsInVehicle())
                                                {
                                                    c.Ped.Task.PerformSequence(PerformTaskSeq(2, c.Ped.CurrentVehicle));
                                                    lastCustomerCheckTimer = Game.GameTime;
                                                }
                                                else
                                                {
                                                    Vector3 playerPos = Game.Player.Character.Position;
                                                    Vector3 playerForward = Game.Player.Character.ForwardVector;
                                                    Vector3 playerRight = Game.Player.Character.RightVector;                                                                                                                                 
                                                    if (Game.Player.Character.IsInVehicle())
                                                    {
                                                        MoveCustomer(c, playerPos + (playerRight * 2.5f));
                                                    }
                                                    else
                                                    {
                                                        MoveCustomer(c, playerPos + (playerForward * 2));
                                                    }
                                                    
                                                    lastCustomerCheckTimer = Game.GameTime;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            Customer nCustomer = null;
                            float nDistance = 3;
                            foreach (Customer c in customers)
                            {
                                if (c != null)
                                {
                                    if (c.Ped.Exists() == true && c.Ped.IsAlive && !c.Ped.IsPlayer)
                                    {
                                        float cDistance = c.Ped.Position.DistanceTo(Game.Player.Character.Position + Game.Player.Character.ForwardVector);                                        
                                        if (c.Ped.CurrentPedGroup != Game.Player.Character.CurrentPedGroup && cDistance <= 3f)
                                        {
                                            c.Ped.Task.ClearAll();
                                            Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, c.Ped.Handle, playerGroup);
                                            Function.Call(Hash._PLAY_AMBIENT_SPEECH1, c.Ped, "GENERIC_HI", "SPEECH_PARAMS_STANDARD");
                                        }
                                        if (cDistance < 2 && c.Ped.CurrentPedGroup == playerGroup && cDistance < nDistance)
                                        {
                                            nCustomer = c;
                                            nDistance = cDistance;
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
                            if(nCustomer != null && !LsMenuPool.IsAnyMenuOpen())
                            {
                                if (nCustomer.Ped.IsInRangeOf(Game.Player.Character.Position, 2f))
                                {
                                    if (TotalDrugsCarried == 0)
                                    {
                                        SendTextMsg("You got no drugs.");
                                        lastTimeDealDone = Game.GameTime;                                        
                                    }
                                    else
                                    {
                                        if (Game.Player.Character.IsInVehicle() == true && nCustomer.Ped.IsInVehicle() == true && nCustomer.Ped.CurrentVehicle.Speed < 1)
                                        {
                                            if (Game.GameTime > displayHelpTimer + 3000)
                                            {
                                                Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                                displayHelpTimer = Game.GameTime;
                                                DisplayHelpText("~INPUT_SPRINT~ to sell from car.");
                                            }
                                            if (Game.IsControlJustPressed(2, GTA.Control.Sprint))
                                            {
                                                Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                                customerSellTo = nCustomer;
                                                sell.Text = "Sell " + customerSellTo.amount + "g of " + customerSellTo.Drug.Name;
                                                customerMainMenu.Subtitle.Caption = "You have " + PlayerInventory[customerSellTo.Drug.Name] + "g of " + customerSellTo.Drug.Name;
                                                customerMainMenu.Visible = !customerMainMenu.Visible;
                                                customerMainMenu.CurrentSelection = 0;
                                            }
                                        }
                                        if (Game.Player.Character.IsInVehicle() == false && Game.Player.Character.IsStopped && !Game.Player.Character.IsInCombat)
                                        {
                                            if (Game.GameTime > displayHelpTimer + 3000)
                                            {
                                                Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                                displayHelpTimer = Game.GameTime;
                                                DisplayHelpText("Press ~INPUT_SPRINT~ to sell to this person.");
                                            }
                                            if (Game.IsControlJustPressed(2, GTA.Control.Sprint) && !LsMenuPool.IsAnyMenuOpen())
                                            {
                                                Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                                customerSellTo = nCustomer;
                                                sell.Text = "Sell " + customerSellTo.amount + "g of " + customerSellTo.Drug.Name;
                                                customerMainMenu.Subtitle.Caption = "You have " + PlayerInventory[customerSellTo.Drug.Name] + "g of " + customerSellTo.Drug.Name;
                                                customerMainMenu.Visible = !customerMainMenu.Visible;
                                                customerMainMenu.CurrentSelection = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (Game.GameTime > ratTimer + 1000)
                        {                            
                            timeDealing = Game.GameTime - timeStartedDealing;
                            thisStreet = World.GetStreetName(new Vector2(Game.Player.Character.Position.X, Game.Player.Character.Position.Y));
                            ratTimer = Game.GameTime;
                            rat = rnd.Next(0, 1000);
                            if (thisStreet == lastStreet && heatBar.Percentage > 0.5 && rat < ratChance + (timeDealing / 60000) && timeDealing > 10000)
                            {
                                SendTextMsg("Someone ratted on you.");
                                Game.Player.WantedLevel = 2;
                                isDealing = false;
                                ratChance = 0;
                                areas[areaIndex].Heat += 1000;                               
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
                                    }
                                }
                                else
                                {
                                    if (ratChance < 100)
                                    {
                                        ratChance += (timeDealing / 60000) + (areas[areaIndex].Heat/100) - (int)areas[areaIndex].Reputation + areas[areaIndex].CopPresance;

                                        if (ratChance > 0)
                                        {
                                            areas[areaIndex].Heat += 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (wepDealer != null)
                    {
                        
                        if (!Game.Player.Character.IsInVehicle())
                        {
                            if (wepDealer.vehicle.Position.DistanceTo(Game.Player.Character.Position) < 3)
                            {
                                if (Game.GameTime > displayHelpTimer + 3000)
                                {
                                    Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                    displayHelpTimer = Game.GameTime;
                                    DisplayHelpText("Press ~INPUT_VEH_EXIT~ to enter.");
                                }
                                if (Game.IsControlJustPressed(2, GTA.Control.VehicleExit))
                                {
                                    Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                                    Game.Player.Character.Task.EnterVehicle(wepDealer.vehicle, VehicleSeat.Passenger);
                                }
                            }
                        }
                        if (Game.Player.Character.IsInVehicle())
                        {
                            if (Game.Player.Character.CurrentVehicle.Handle == wepDealer.vehicle.Handle)
                            {                                                                                             
                                if (Game.IsControlJustPressed(2, GTA.Control.ContextSecondary))
                                {
                                    Game.Player.Character.Task.LeaveVehicle(Game.Player.Character.CurrentVehicle, true);
                                    Wait(5500);
                                    wepDealer.ped.CurrentVehicle.SoundHorn(15);
                                    Wait(15);
                                    wepDealer.ped.CurrentVehicle.SoundHorn(12);
                                    wepDealer.ped.IsPersistent = false;
                                    wepDealer.vehicle.IsPersistent = false;
                                    wepDealer.vehicle.CurrentBlip.Remove();
                                    Function.Call(Hash.TASK_VEHICLE_DRIVE_WANDER, wepDealer.ped.Handle, wepDealer.vehicle.Handle, 20f, (int)DrivingStyle.Normal);
                                    wepDealer = null;
                                }
                                if (Game.IsControlJustPressed(2, GTA.Control.Context))
                                {
                                    if (wepDealer.state == Dealer.States.Driving) { wepDealer.state = Dealer.States.Faster; }
                                    if (wepDealer.state == Dealer.States.Fast) { wepDealer.state = Dealer.States.Slower; }
                                }
                            }
                        }
                    }
                }
            }            
        }        

        void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == SetDrugCar && Game.Player.Character.IsAlive)
            {
                if (Game.Player.Character.IsInVehicle())
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
                    VehicleSave(Game.Player.Character.CurrentVehicle);
                    SendTextMsg("" + drugCar.DisplayName + " is now youre Stash Vehicle.");
                }
                if (!Game.Player.Character.IsInVehicle())
                {
                    SendTextMsg("Please ener a vehicle before trying to set a stash vehicle.");                                
                }
            }
            if (e.KeyCode == StopDealing && Game.Player.Character.IsAlive)
            {
                if (isDealing)
                {
                    isDealing = false;                    
                    SendTextMsg("Street dealing cancel");
                }
                else if (!isDealing)
                {
                    SendTextMsg("Street dealing is not in progress.");
                }
            }
            
            if (e.KeyCode == Keys.J && started == false)
            {
                if (DEBUG)
                {
                    Vector3 stashPos = new Vector3(-112.616f, -1479.229f, 30f);
                    string dName0 = areas[areaIndex].Drugs[0].Name;
                    float dDemand0 = areas[areaIndex].Drugs[0].Demand;
                    string dName1 = areas[areaIndex].Drugs[1].Name;
                    float dDemand1 = areas[areaIndex].Drugs[1].Demand;
                    string dName2 = areas[areaIndex].Drugs[2].Name;
                    float dDemand2 = areas[areaIndex].Drugs[2].Demand;
                    UI.Notify(dName0+": "+dDemand0 +" "+ dName1 + ": " + dDemand1 +" "+ dName2 + ": " + dDemand2);


                    //if(weedDoor == null)
                    //{
                    //    weedDoor = new Door(LoadWeedPlace());
                    //}
                    //else
                    //{
                    //    weedDoor.UseLock();                                               
                    //}


                    //if (!wepMenu.Visible) { wepMenu.Visible = true; }
                    //int currentInventorySize = (Game.Player.Money / 50000) + playerWeps.Count + (TotalDrugsCarried / 28);
                    //UI.Notify("Total inventory size " + currentInventorySize);
                    //float ground = World.GetGroundHeight(Game.Player.Character.Position);
                    ////UI.Notify("" + Function.Call<bool>(Hash.IS_POINT_ON_ROAD, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z));
                    //if (!attackerPickupSpawned)
                    //{
                    //    int drugs = 0;
                    //    attackerPickupSpawned = true;
                    //    attackerPickup = World.CreatePickup(PickupType.MoneyMedBag, Game.Player.Character.Position + (Game.Player.Character.ForwardVector * 2), Vector3.Zero, new Model("prop_big_bag_01"), 100);
                    //    foreach(XElement element in playerWeapons)
                    //    {
                    //        if (element.Name == "weapon")
                    //        {
                    //            attackerWeapons.Add(element);
                    //            UI.Notify("added " + element.Name);
                    //        }
                    //    }
                    //    foreach (KeyValuePair<string, int> entry in PlayerInventory)
                    //    {
                    //        attackerInventory.Add(entry.Key, entry.Value);
                    //        drugs += entry.Value;
                    //    }
                    //    UI.Notify("drugs = " + drugs);
                    //}
                }
            }
            if (e.KeyCode == MenuKey)
            {
                if (Game.Player != null && Game.Player.IsAlive)
                {
                    if (DEBUG)
                    {
                        UI.Notify("AreaLabel:" + World.GetZoneNameLabel(Game.Player.Character.Position) + " Type" + areas[areaIndex].AreaType);
                    }
                    SendTextMsg("Weed " + PlayerInventory["Weed"] + "g, Crack " + PlayerInventory["Crack"] + "g, Coke " + PlayerInventory["Cocaine"] + "g.");
                }
            }
        }           
        
        public static void callDealer(bool wepdealer)
        {
            if (wepdealer)
            {
                Vector3 dealer1pos;
                dealer1pos = World.GetNextPositionOnSidewalk(Game.Player.Character.Position.Around(60f));
                weaponDealer1 = World.CreatePed(PedHash.ArmBoss01GMM, GenerateSpawnPos(dealer1pos, Nodetype.AnyRoad, true));
                pedTarget = Game.Player.Character.Position;
                if(weaponDealer1 != null)
                {
                    weaponDealer1.AddBlip();
                    weaponDealer1.CurrentBlip.IsFriendly = true;
                    weaponDealer1.CanSwitchWeapons = true;
                    weaponDealer1.Weapons.Give(WeaponHash.Pistol, 9999, false, true);
                    weaponDealer1.Armor = 100;
                    Function.Call(Hash.SET_PED_COMBAT_ABILITY, weaponDealer1, 100);
                    readyToDeal = false;
                    Wait(100);
                    weaponDealer1.Task.GoTo(pedTarget);
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
                    cocaine.Maximum = dealer1Cocain;
                }
                else
                {
                    SendTextMsg("~r~Spawning Weapon Dealer Failed");
                }
                
            }
            else
            {
                Vector3 dealer1pos;
                dealer1pos = World.GetNextPositionOnSidewalk(Game.Player.Character.Position.Around(60f));
                drugDealer1 = World.CreatePed(PedHash.ArmBoss01GMM, GenerateSpawnPos(dealer1pos, Nodetype.AnyRoad, true));
                pedTarget = Game.Player.Character.Position;
                if(drugDealer1 != null)
                {
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
                    cocaine.Maximum = dealer1Cocain;
                }
                else
                {
                    SendTextMsg("~r~Spawning Drug Dealer Failed");
                }
            }            
        }

        void Housing()
        {
            if (isAtHome == false)
            {
                if (Game.Player.Character.Position.DistanceTo(houseTeleIn) < 30f)
                {
                    Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                    DisplayHelpText("isAthome = " + isAtHome + ", changing it to true");
                    isAtHome = true;
                }

                if (Game.Player.Character.Position.DistanceTo(houseTeleOut) < 10f)
                {
                    World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(-112.616f, -1479.229f, 36f), Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 1f), Color.Green);
                    if (Game.Player.Character.Position.DistanceTo(houseTeleOut) < 1f)
                    {
                        if(Game.GameTime > displayHelpTimer + 3000)
                        {
                            Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                            displayHelpTimer = Game.GameTime;
                            DisplayHelpText("Press ~INPUT_CONTEXT~ to enter Safehouse.");
                        }
                        if (Game.IsControlJustReleased(2, GTA.Control.Context))
                        {
                            Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                            Game.Player.Character.FreezePosition = true;
                            Game.FadeScreenOut(300);
                            Wait(300);
                            Game.Player.Character.Position = houseTeleIn;
                            Wait(300);
                            Game.Player.Character.FreezePosition = false;
                            Game.FadeScreenIn(300);
                            isAtHome = true;
                        }

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

                if (!LsMenuPool.IsAnyMenuOpen() && Game.Player.Character.Position.DistanceTo(houseTeleIn) < 1f)
                {
                    if (Game.GameTime > displayHelpTimer + 3000)
                    {
                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                        displayHelpTimer = Game.GameTime;
                        DisplayHelpText("Press ~INPUT_CONTEXT~ to exit Safehouse.");
                    }
                    if (Game.IsControlJustReleased(2, GTA.Control.Context))
                    {
                        Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                        Game.Player.Character.FreezePosition = true;
                        Game.FadeScreenOut(600);
                        Script.Wait(600);
                        Game.Player.Character.Position = houseTeleOut;
                        Script.Wait(300);
                        Game.Player.Character.FreezePosition = false;
                        Game.FadeScreenIn(300);
                        isAtHome = false;
                    }
                }
                
                if (!LsMenuPool.IsAnyMenuOpen() && Game.Player.Character.Position.DistanceTo(houseStash) < 1f )
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
                        stashMainMenu.Visible = !stashMainMenu.Visible;
                    }
                }
            }
        }        

        public enum DayGTA
        {
            Sunday = 0,
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6
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
                walkToVehicleDrive.AddTask.GoTo(vehicle.Position + ((vehicle.RightVector*2) * -1));
                walkToVehicleDrive.AddTask.EnterVehicle(vehicle, VehicleSeat.Driver);
                walkToVehicleDrive.AddTask.CruiseWithVehicle(vehicle, 20f, (int)DrivingStyle.SometimesOvertakeTraffic);
                //walkToVehicleDrive.AddTask.ClearAll();
                return walkToVehicleDrive;                
            }            
            if ( index == 2)
            {
                Vector3 playerPos = Game.Player.Character.Position;
                Vector3 playerForward = Game.Player.Character.ForwardVector;
                Vector3 playerRight = Game.Player.Character.RightVector;
                TaskSequence driveToPlayerAndExit = new TaskSequence();
                driveToPlayerAndExit.AddTask.ClearAllImmediately();
                driveToPlayerAndExit.AddTask.DriveTo(vehicle, World.GetNextPositionOnStreet(Game.Player.Character.Position), 10f, 20f, (int)DrivingStyle.SometimesOvertakeTraffic);
                if(Game.Player.Character.IsInVehicle())
                {
                    driveToPlayerAndExit.AddTask.GoTo(playerPos+(playerRight*3));
                }
                else
                {
                    driveToPlayerAndExit.AddTask.GoTo(playerPos+(playerForward*2));
                }
                
                return driveToPlayerAndExit;                
            }            
            else
            {
                return null;
            }
        }

        public static void MoveCustomer(Customer c, Vector3 target)
        {
            if(c.Target != target)
            {
                c.Target = target;                              
            }
            if(c.Target == target)
            {
                if (c.Ped.Position.DistanceTo(c.Target) > 20 || Function.Call<bool>(Hash.IS_POINT_ON_ROAD, c.Ped.Position.X, c.Ped.Position.Y, c.Ped.Position.Z))
                {
                    c.Ped.Task.RunTo(c.Target);
                }                
                else
                {
                    c.Ped.Task.GoTo(c.Target);                    
                }
            }            
        }
        

        
        #region Classes
        public class Door
        {
            private readonly bool DEBUG = true;
            public Prop Prop;
            private bool Locked = false;
            public Door(Prop prop)
            {
                this.Prop = prop;
                this.Prop.IsInvincible = true;
                this.UseLock();
            }
            public void UseLock()
            {
                
                if(this.Locked)
                {

                    this.Locked = false;                    
                    Wait(10);
                    Function.Call(Hash.REQUEST_SCRIPT_AUDIO_BANK, "BIG_SCORE_GOLD_CAGE_LOCK", 0);
                    Game.PlaySound("0x08053C9C", "BIG_SCORE_GOLD_CAGE_LOCK");
                    Function.Call(Hash._DOOR_CONTROL, this.Prop.Model.Hash, this.Prop.Position.X, this.Prop.Position.Y, this.Prop.Position.Z, false, 0f, 50f, 0f);
                    if (DEBUG) { UI.Notify(this.Prop.Model.Hash + " Door unlocked " + this.Prop.Position); }
                }
                else
                {
                    this.Locked = true;
                    Wait(10);                    
                    Game.PlaySound("Bar_Lower_And_Lock", "DLC_IND_ROLLERCOASTER_SOUNDS");
                    Function.Call(Hash._DOOR_CONTROL, this.Prop.Model.Hash, this.Prop.Position.X, this.Prop.Position.Y, this.Prop.Position.Z, true, 0f, 50f, 0f);
                    if (DEBUG) { UI.Notify(this.Prop.Model.Hash + " Door locked " + this.Prop.Position); }                    
                }
            }
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
        #endregion
        #region Stuff
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
                    foreach (Customer c in customers)
                    {
                        Ped p = c.Ped;
                        p.CurrentBlip.Remove();
                        p.Delete();
                    }
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
                        wepDealer.ped.Delete();
                    }
                }
            }
        }
        #endregion
    }
}
