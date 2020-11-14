using GTA;
using GTA.Math;
using GTA.Native;
using iFruitAddon2;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using static LSlife.Enums;
using static LSlife.LsFunctions;
using static LSlife.LsMenus;

namespace LSlife
{
    public class LSL : Script
    {

        #region Globals
        public static bool hardMode = false;
        public static bool DEBUG = true;
        public static bool UseSprites = true;
        public static bool DrawMarkers = true;
        public static bool started = false;
        public static bool EnableBlips;
        public static bool AreaDebug;
        public static ScriptSettings iniSettings;

        //UIStuff
        public static MenuPool LsMenuPool;


        public static PlayerDealerHandler DealerHandler;
        public static StashHouseHandler HouseHandler;
        public static PickupHandler pickupHandler = new PickupHandler();
        public static DriveByHandler driveByHandler = new DriveByHandler();
        ///EnemyHandler enemyHandler = new EnemyHandler();
        public static LsLPeds lPeds;
        public static int CustomerRel = World.AddRelationshipGroup("LSCUSTOMER");
        public static int playersTeam = World.AddRelationshipGroup("workers");
        public static int rival_nutral = World.AddRelationshipGroup("RIVAL_N");
        public static int rival_enemy = World.AddRelationshipGroup("RIVAL_E");
        public static int playerRelationship = Function.Call<int>(Hash.GET_HASH_KEY, "PLAYER");
        


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

        public PedDebugger PedDebugger = new PedDebugger();

        //Zee Menu
        public static UIMenu hireDealerMainMenu;
        public static UIMenuSliderItem hireDealerOfferAmount;
        public static UIMenuItem hireDealerMakeOffer;
        public static UIMenuItem hireDealerFollowButton;

        public static UIMenu WorkerMainMenu;
        public static UIMenuSliderItem wWeed;
        public static UIMenuSliderItem wCrack;
        public static UIMenuSliderItem wCocaine;
        public static UIMenuListItem wMultiplier;
        public static UIMenuItem wConfirm;
        public static UIMenuItem wCollect;
        public static UIMenuItem wFollow;
        public static UIMenuItem wArmour;
        public static UIMenuListItem wWeapons;

        public static UIMenu zeeMenu;
        public static UIMenu startDealing;
        public static UIMenuCheckboxItem sellWeed;
        public static UIMenuCheckboxItem sellCrack;
        public static UIMenuCheckboxItem sellCocaine;
        public static UIMenuItem StartDeal;


        public static UIMenu OrderDrugs;
        private UIMenuListItem OrderMultipliers;
        public static UIMenu weedMenu;
        public static UIMenuSliderItem bushWeed;
        public static UIMenu crackMenu;
        public static UIMenuSliderItem crack;
        public static UIMenu cocainMenu;
        public static UIMenuSliderItem cocaine;
        public static UIMenuItem WeedBuy;
        public static UIMenuItem CrackBuy;
        public static UIMenuItem CocainBuy;
        public static UIMenuItem PlaceOrder;

        public static UIMenuItem buyWeapons;
        public static UIMenuItem buyLift;
        public static UIMenuItem payDebt;

        //WEAPONS MENU
        public static UIMenu wepMenu;
        public static UIMenuListItem melee;
        public static List<dynamic> meleeWeps = new List<dynamic>() { { "Bat" }, { "Knife" }, { "Dagger" } };
        public static List<int> meleePrice = new List<int>() { { 5 }, { 100 }, { 2000 } };
        public static UIMenuListItem pistol;
        public static List<dynamic> pistolWeps = new List<dynamic>() { { "Pistol" }, { "Pistol50" }, { "HeavyPistol" }, { "Ammo" } };
        public static List<int> pistolPrice = new List<int>() { { 350 }, { 3750 }, { 4000 }, { 50 } };
        public static UIMenuListItem shotgun;
        public static List<dynamic> shotgunWeps = new List<dynamic>() { { "SawnOffShotgun" }, { "PumpShotgun" } };
        public static UIMenuListItem assaultRifle;
        public static List<dynamic> assaultRifleWeps = new List<dynamic>() { { "AssaultRifle" }, { "CarbineRifle" } };
        public static UIMenuListItem smg;
        public static List<dynamic> smgWeps = new List<dynamic>() { { "MicroSMG" }, { "SMG" } };
        public static UIMenuListItem mg;
        public static List<dynamic> mgWeps = new List<dynamic>() { { "MG" } };
        public static UIMenuListItem sniper;
        public static List<dynamic> sniperWeps = new List<dynamic>() { "SniperRifle" };
        public static UIMenuListItem heavy;
        public static List<dynamic> heavyWeps = new List<dynamic>() { "RPG" };
        public static UIMenuListItem thrown;
        public static List<dynamic> thrownWeps = new List<dynamic>() { "Grenade" };
        public static UIMenuItem armour;
        public static UIMenuItem dismiss;




        //public static UIMenuItem dismissDealer;



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

        public static UIMenuSliderItem putWeedOzStash;
        public static UIMenuSliderItem takeWeedOzStash;
        public static UIMenuSliderItem putCrackOzStash;
        public static UIMenuSliderItem takeCrackOzStash;
        public static UIMenuSliderItem putCocainOzStash;
        public static UIMenuSliderItem takeCocainOzStash;

        public static UIMenuSliderItem putMoneyStash;
        public static UIMenuSliderItem takeMoneyStash;
        public static UIMenuListItem putArmourStash;
        public static UIMenuListItem takeArmourStash;
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
        public static UIMenuSliderItem putWeedOzVeh;
        public static UIMenuSliderItem takeWeedOzVeh;
        public static UIMenuSliderItem putCrackOzVeh;
        public static UIMenuSliderItem takeCrackOzVeh;
        public static UIMenuSliderItem putCocainOzVeh;
        public static UIMenuSliderItem takeCocainOzVeh;
        public static UIMenuSliderItem putMoneyVeh;
        public static UIMenuSliderItem takeMoneyVeh;
        public static UIMenuListItem putArmourVeh;
        public static UIMenuListItem takeArmourVeh;
        public static UIMenuListItem putWeaponVeh;
        public static UIMenuListItem takeWeaponVeh;
        public static UIMenuListItem vehPutMultiplier;
        public static UIMenuListItem vehTakeMultiplier;
        public static readonly List<dynamic> multiplier = new List<dynamic>() { { 1 }, { 10 }, { 28 }, { 100 }, { 280 }, { 1000 }, { 2800 }, { 10000 }, { 100000 } };

        //Customer Menu
        public static UIMenu customerMainMenu;
        public static UIMenuItem sell;
        public static UIMenuItem turnAway;

        // TIMER BAR STUFF



        //Main menu and options 
        public static Keys MenuKey;
        public static Keys StopDealing;
        public static Keys SetDrugCar;

        //XML STUFF
        public static XDocument vehWeaponsXML;
        public static XDocument stashWeaponsXML;
        public static XDocument LSLifeSave;
        public static XDocument VehicleXml;




        public static float displayHelpTimer = Game.GameTime;

        public static int day;
        public static int hour;
        public static int SlowTickTime;


        //Ifruit stuff
        public static CustomiFruit iFruit;

        // Dealers
        //DealerIntro dealerIntro1;
        public static iFruitContact contactZee;


        public static DrugDealer ZeesDealers = null;
        public static DrugOrder NewOrder = new DrugOrder();

        public static Ped drugDealer1;
        public static Ped weaponDealer1;
        public static int dealerTimer;
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
        public static List<Pig> pigs = new List<Pig>();

        // Area Dealer / Gang stuff

        public static AreaDealer aDealer = null;

        public static StashVehicle pStashVehicle = null;


        public static Vector3 playerPos = Vector3.Zero;
        public static Player player = Game.Player;


        // WEED PLACE
        //int weedState = 0;
        //Door weedDoor;

        // Game.Player stuff
        public static bool removedStuff = true;

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
                {"BAYTRE"    , AreaType.Normal },
                {"GALLI"    , AreaType.Rich },
                {"OBSERV"    , AreaType.Normal },
                {"BRADT"    , AreaType.Normal },
                {"ZANCUDO"   , AreaType.Poor }
        };

        public enum jurisdictionType { LSPD, LSSD, BCSO, SAPR };
        public static Dictionary<string, jurisdictionType> jurisdiction = new Dictionary<string, jurisdictionType>()
            {
                { "ALAMO"    , jurisdictionType.BCSO },
                {"ALTA"      , jurisdictionType.LSPD },
                {"BHAMCA"    , jurisdictionType.LSSD },
                {"BANHAMC"   , jurisdictionType.LSSD },
                {"BANNING"   , jurisdictionType.LSPD },
                {"JAIL"      , jurisdictionType.LSSD },
                {"BRADP"     , jurisdictionType.BCSO },
                {"BURTON"    , jurisdictionType.LSPD },
                {"CALAFB"    , jurisdictionType.SAPR },
                {"CCREAK"    , jurisdictionType.SAPR },
                {"CHAMH"     , jurisdictionType.LSPD },
                {"CMSW"      , jurisdictionType.SAPR },
                {"CHU"       , jurisdictionType.LSSD },
                {"CYPRE"     , jurisdictionType.LSPD },
                {"DAVIS"     , jurisdictionType.LSPD },
                {"ZQ_UAR"    , jurisdictionType.LSSD },
                {"DELPE"     , jurisdictionType.LSPD },
                {"DELBE"     , jurisdictionType.LSPD },
                {"DOWNT"     , jurisdictionType.LSPD },
                {"DTVINE"    , jurisdictionType.LSPD },
                {"EAST_V"    , jurisdictionType.LSPD },
                {"EBURO"     , jurisdictionType.LSSD },
                {"ELGORL"    , jurisdictionType.BCSO },
                {"ELYSIAN"   , jurisdictionType.LSPD },
                {"ARMYB"     , jurisdictionType.BCSO },
                {"GALFISH"   , jurisdictionType.BCSO },
                {"DESRT"     , jurisdictionType.LSSD },
                {"GRAPES"    , jurisdictionType.BCSO },
                {"GREATC"    , jurisdictionType.LSSD },
                {"golf"      , jurisdictionType.LSPD },
                {"HARMO"     , jurisdictionType.BCSO },
                {"HAWICK"    , jurisdictionType.LSPD },
                {"HUMLAB"    , jurisdictionType.BCSO },
                {"LMESA"     , jurisdictionType.LSPD },
                {"LOSPUER"   , jurisdictionType.LSPD },
                {"LAGO"      , jurisdictionType.BCSO },
                {"LDAM"      , jurisdictionType.LSSD },
                {"LACT"      , jurisdictionType.LSSD },
                {"LEGSQU"    , jurisdictionType.LSPD },
                {"KOREAT"    , jurisdictionType.LSPD },
                {"AIRP"      , jurisdictionType.LSPD },
                {"STAD"      , jurisdictionType.LSPD },
                {"MIRR"      , jurisdictionType.LSPD },
                {"SKID"      , jurisdictionType.LSPD },
                {"MORN"      , jurisdictionType.LSPD },
                {"MTCHIL"    , jurisdictionType.SAPR },
                {"MTGORDO"   , jurisdictionType.SAPR },
                {"MTJOSE"    , jurisdictionType.BCSO },
                {"MURRI"     , jurisdictionType.LSPD },
                {"NOOSE"     , jurisdictionType.LSSD },
                {"NCHU"      , jurisdictionType.BCSO },
                {"PBLUFF"    , jurisdictionType.LSPD },
                {"OCEANA"    , jurisdictionType.LSPD }, //WATER
                {"PALETO"    , jurisdictionType.BCSO },
                {"PALCOV"    , jurisdictionType.BCSO },
                {"PALFOR"    , jurisdictionType.SAPR },
                {"PALMPOW"   , jurisdictionType.LSSD },
                {"PALHIGH"   , jurisdictionType.LSSD },
                {"PBOX"      , jurisdictionType.LSPD },
                {"ZP_ORT"    , jurisdictionType.LSPD },
                {"PROCOB"    , jurisdictionType.BCSO },
                {"DELSOL"    , jurisdictionType.LSPD },
                {"RANCHO"    , jurisdictionType.LSPD },
                {"CANNY"     , jurisdictionType.LSPD  },
                {"RTRAK"     , jurisdictionType.SAPR },
                {"MOVIE"     , jurisdictionType.LSPD },
                {"RICHM"     , jurisdictionType.LSPD },
                {"RGLEN"     , jurisdictionType.LSSD },
                {"ROCKF"     , jurisdictionType.LSPD },
                {"WINDF"     , jurisdictionType.LSSD },
                {"SanAnd"    , jurisdictionType.LSPD }, // ???
                {"SANCHIA"   , jurisdictionType.BCSO },
                {"SANDY"     , jurisdictionType.BCSO },
                {"SLAB"      , jurisdictionType.BCSO },
                {"STRAW"     , jurisdictionType.LSPD },
                {"TATAMO"    , jurisdictionType.LSSD },
                {"TERMINA"   , jurisdictionType.LSPD },
                {"TEXTI"     , jurisdictionType.LSPD },
                {"TONGVAH"   , jurisdictionType.LSPD },
                {"TONGVAV"   , jurisdictionType.LSSD },
                {"VESP"      , jurisdictionType.LSPD },
                {"BEACH"     , jurisdictionType.LSPD },
                {"VCANA"     , jurisdictionType.LSPD },
                {"VINE"      , jurisdictionType.LSPD },
                {"CHIL"      , jurisdictionType.SAPR },
                {"HORS"      , jurisdictionType.LSPD },
                {"WVINE"     , jurisdictionType.LSPD },
                {"BAYTRE"    , jurisdictionType.LSSD },
                {"GALLI"    , jurisdictionType.LSSD },
                {"OBSERV"    , jurisdictionType.LSPD },
                {"BRADT"    , jurisdictionType.BCSO },
                {"ZANCUDO"   , jurisdictionType.BCSO } };
        public static List<Area> areas = new List<Area>();
        public static int areaIndex = 0;
        public static string playerArea = "SanAnd";
        public static Random rnd = new Random();
        public static Dictionary<string, int> PlayerInventory = new Dictionary<string, int>();
        public static int TotalDrugsCarried = 0;
        public static int playerArmours = 0;
        public static bool playerWanted = false;


        public static bool notFined = true;
        public static bool playerDead = false;
        public static List<dynamic> playerWeps = new List<dynamic>();
        public static int newWepAmount = 0;
        public static List<XElement> playerWeapons = new List<XElement>();
        public static bool playerHasBag = false;
        public static bool canSprint = true;
        public static int randomAttackTimer;
        public static List<Entity> entitiesCleanUp = new List<Entity>();





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
        public static int attackCheckTime = 0;
        public static List<XElement> attackerWeapons = new List<XElement>();
        public int attackerWeed = 0;
        public int attackerCrack = 0;
        public int attackerCocaine = 0;
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
        

        //WEAPONS DEALER STUFF
        public static Dealer wepDealer;
        public static UIText myUIText;





        //DEBUG STUFF
        public static int PedsCanSeeDeal = 0;
        private bool showInventory;

        //int startTime = 0;




        public static bool PlayerComplied { get; private set; }
        #endregion

        public LSL()
        {
            Setup();
            Aborted += OnAbort;
            Tick += onTick;
            KeyDown += onKeyDown;

            Interval = 3;

            //blips            
        }



        void Setup()
        {
            player = Game.Player;
            player.Character.CurrentPedGroup.SeparationRange = 100;
            //player.Character.CurrentPedGroup.FormationType = FormationType.Circle1;
            iniSettings = ScriptSettings.Load("scripts\\LSlife.ini");
            DEBUG = iniSettings.GetValue<bool>("OPTIONS", "DEBUG", false);
            UseSprites = iniSettings.GetValue<bool>("OPTIONS", "SPRITES", true);
            hardMode = iniSettings.GetValue<bool>("OPTIONS", "HARD", false);
            DrawMarkers = iniSettings.GetValue<bool>("OPTIONS", "MARKERS", true);
            AreaDebug = iniSettings.GetValue<bool>("OPTIONS", "AREADEBUG", true);
            if (hardMode)
            {
                weed1Price = 180;
                crack1Price = 400;
                cocain1Price = 1300;
                
            }
            LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
            foreach (XElement element in LSLifeSave.Descendants())
            {
                if (element.Name == "playerInventory")
                {
                    int number;
                    bool foundArmour = false;
                    bool foundOz = false;
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weed")
                        {
                            Int32.TryParse(e.Value, out number);
                            PlayerInventory.Add("Weed", number);
                        }
                        if (e.Name == "weedOz")
                        {
                            Int32.TryParse(e.Value, out pWeedOunce);
                            foundOz = true;

                        }
                        if (e.Name == "crack")
                        {
                            Int32.TryParse(e.Value, out number);
                            PlayerInventory.Add("Crack", number);
                        }
                        if (e.Name == "crackOz")
                        {
                            Int32.TryParse(e.Value, out pCrackOunce);
                            foundOz = true;
                        }
                        if (e.Name == "cocain")
                        {
                            Int32.TryParse(e.Value, out number);
                            PlayerInventory.Add("Cocaine", number);
                        }
                        if (e.Name == "cocaineOz")
                        {
                            Int32.TryParse(e.Value, out pCocaineOunce);
                            foundOz = true;
                        }

                        if (e.Name == "armour")
                        {
                            int.TryParse(e.Value, out playerArmours);
                            foundArmour = true;
                        }
                    }
                    if (!foundArmour)
                    {
                        element.Add(new XElement("armour", playerArmours.ToString()));
                        LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                    }
                    if (!foundOz)
                    {
                        element.Add(new XElement("weedOz", pWeedOunce.ToString()));
                        element.Add(new XElement("crackOz", pCrackOunce.ToString()));
                        element.Add(new XElement("cocaineOz", pCocaineOunce.ToString()));
                        LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                    }
                }

                if (element.Name == "dealer1")
                {
                    bool foundRep = false;
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "rep")
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
                                if (dealer1Debt < 0) dealer1Debt = 0;
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

            VehicleXml = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");

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
                            //stashWeapons.Add(element);
                            //stashWeps.Add(e.Value);
                        }
                    }
                }
            }
            vehWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");

            int i = 0;
            foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
            {
                bool hasPedGotWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, player.Character, (int)weapon);

                if (hasPedGotWeapon)
                {
                    int ammunitionInWeapon = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, player.Character, (int)weapon);
                    var parentElement = new XElement("weapon");
                    parentElement.SetAttributeValue("id", i);
                    var weaponNameElement = new XElement("HASH", weapon);
                    if (DEBUG)
                    {
                        //UI.Notify("Added " + weapon + " to playerWeapons list.");
                    }

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
            }           // Get player weapons and create lists for menus
            //NATIVEUI STUFF
            LsMenuPool = new MenuPool();



            Color menuColor = Color.FromArgb(114, 204, 114);

            //ZEE MENU
            zeeMenu = new UIMenu("Zee", "What you want?");
            if (UseSprites) zeeMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
            LsMenuPool.Add(zeeMenu);
            startDealing = LsMenuPool.AddSubMenu(zeeMenu, "Dealing.");
            if (UseSprites) startDealing.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
            sellWeed = new UIMenuCheckboxItem("Sell Weed", true);
            sellCrack = new UIMenuCheckboxItem("Sell Crack", true);
            sellCocaine = new UIMenuCheckboxItem("Sell Cocaine", true);
            StartDeal = new UIMenuItem("Start Dealing.");
            startDealing.AddItem(sellWeed);
            startDealing.AddItem(sellCrack);
            startDealing.AddItem(sellCocaine);
            startDealing.AddItem(StartDeal);
            startDealing.OnItemSelect += OnstartDealSelect;

            OrderDrugs = LsMenuPool.AddSubMenu(zeeMenu, "Order Drugs.");
            if (UseSprites) OrderDrugs.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
            OrderDrugs.BannerSprite.Color = Color.Black;
            OrderDrugs.Title.DropShadow = true;
            OrderDrugs.Title.Outline = true;
            OrderDrugs.Title.Shadow = true;
            OrderDrugs.OnMenuOpen += OrderDrugs_OnMenuOpen;

            buyWeapons = new UIMenuItem("Buy Weapons.");
            zeeMenu.AddItem(buyWeapons);
            buyLift = new UIMenuItem("Ask for a Lift.");
            zeeMenu.AddItem(buyLift);
            payDebt = new UIMenuItem("Pay Debt");
            zeeMenu.AddItem(payDebt);
            zeeMenu.OnItemSelect += OnZeeMenuSelect;




            // Workers and shit menus

            hireDealerMainMenu = new UIMenu("Hire Dealer", "Select an option");
            if (UseSprites) hireDealerMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_dealer.png");
            LsMenuPool.Add(hireDealerMainMenu);
            hireDealerOfferAmount = new UIMenuSliderItem("Offer Amount");
            hireDealerOfferAmount.Maximum = player.Money;
            hireDealerOfferAmount.Multiplier = 1000;
            hireDealerMakeOffer = new UIMenuItem("Make Offer");
            hireDealerFollowButton = new UIMenuItem("Cancel Offer");
            hireDealerMainMenu.AddItem(hireDealerOfferAmount);
            hireDealerMainMenu.AddItem(hireDealerMakeOffer);
            hireDealerMainMenu.AddItem(hireDealerFollowButton);
            hireDealerOfferAmount.OnSliderChanged += HireDealerOfferAmount_OnSliderChanged;
            hireDealerMakeOffer.Activated += HireDealerOfferSelect;
            hireDealerFollowButton.Activated += HireDealerFollowSelect;

            WorkerMainMenu = new UIMenu("Worker", "Give stuff to your Dealer");
            if (UseSprites) WorkerMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_worker.png");
            LsMenuPool.Add(WorkerMainMenu);
            wWeed = new UIMenuSliderItem("Weed " + GramsToOz(0));
            wWeed.Maximum = PlayerInventory["Weed"];
            wCrack = new UIMenuSliderItem("Crack " + GramsToOz(0));
            wCrack.Maximum = PlayerInventory["Crack"];
            wCocaine = new UIMenuSliderItem("Coke " + GramsToOz(0));
            wCocaine.Maximum = PlayerInventory["Cocaine"];
            wMultiplier = new UIMenuListItem("Multiplier", multiplier, 0);
            wConfirm = new UIMenuItem("Confirm");
            wCollect = new UIMenuItem("Collect");
            wFollow = new UIMenuItem("Follow me");
            wArmour = new UIMenuItem("Give Armour");
            wWeapons = new UIMenuListItem("Give weapon", playerWeps, 0);
            WorkerMainMenu.AddItem(wWeed);
            WorkerMainMenu.AddItem(wCrack);
            WorkerMainMenu.AddItem(wCocaine);
            WorkerMainMenu.AddItem(wMultiplier);
            WorkerMainMenu.AddItem(wConfirm);
            WorkerMainMenu.AddItem(wCollect);
            WorkerMainMenu.AddItem(wFollow);
            WorkerMainMenu.AddItem(wArmour);
            WorkerMainMenu.AddItem(wWeapons);
            wWeed.OnSliderChanged += WWeed_OnSliderChanged;
            wCrack.OnSliderChanged += WCrack_OnSliderChanged;
            wCocaine.OnSliderChanged += WCocaine_OnSliderChanged;
            wMultiplier.OnListChanged += WMultiplier_OnListChanged;
            wConfirm.Activated += WConfirm_Activated;
            wCollect.Activated += WCollect_Activated;
            wFollow.Activated += WFollow_Activated;
            wArmour.Activated += WArmour_Activated;
            wWeapons.Activated += WWeapons_Activated;




            // WEP MENU
            int mWprice = meleePrice[0] - Discount(meleePrice[0]);
            int pWprice = pistolPrice[0] - Discount(pistolPrice[0]);
            wepMenu = new UIMenu("Wepons", "What you want?");
            if (UseSprites) wepMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_dealer.png");
            LsMenuPool.Add(wepMenu);
            melee = new UIMenuListItem("Melee $" + IntToMoney(mWprice), meleeWeps, 0);
            wepMenu.AddItem(melee);
            pistol = new UIMenuListItem("Pistol $" + IntToMoney(pWprice), pistolWeps, 0);
            wepMenu.AddItem(pistol);
            armour = new UIMenuItem("Armour $" + IntToMoney(250 - Discount(250)));
            wepMenu.AddItem(armour);
            dismiss = new UIMenuItem("Dismiss");
            wepMenu.AddItem(dismiss);
            wepMenu.OnItemSelect += OnWepMenuSelect;
            melee.OnListChanged += onListChange;
            pistol.OnListChanged += onListChange;


            //ORDER MENUS STUFF            
            OrderMultipliers = new UIMenuListItem("Multiplier", multiplier, 0);
            OrderMultipliers.OnListChanged += DMultiplier_OnListChanged;
            // WEED

            weedMenu = LsMenuPool.AddSubMenu(OrderDrugs, "Weed");
            if (UseSprites) weedMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
            bushWeed = new UIMenuSliderItem("Weed $" + IntToMoney(weed1Price - Discount(weed1Price)) + "oz");
            weedMenu.AddItem(bushWeed);
            bushWeed.Maximum = dealer1Weed;
            bushWeed.Multiplier = 1;
            weedMenu.AddItem(OrderMultipliers);
            WeedBuy = new UIMenuItem("Order: " + bushWeed.Value);
            weedMenu.AddItem(WeedBuy);
            bushWeed.OnSliderChanged += onSliderChange;
            weedMenu.OnItemSelect += onWeedMenuSelect;
            //CRACK 


            crackMenu = LsMenuPool.AddSubMenu(OrderDrugs, "Crack");
            if (UseSprites) crackMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
            crack = new UIMenuSliderItem("Crack $" + IntToMoney(crack1Price - Discount(crack1Price)) + "oz");
            crackMenu.AddItem(crack);
            crack.Maximum = dealer1Crack;
            crack.Multiplier = 1;
            crackMenu.AddItem(OrderMultipliers);
            CrackBuy = new UIMenuItem("Buy: " + crack.Value);
            crackMenu.AddItem(CrackBuy);
            crack.OnSliderChanged += onSliderChange;
            crackMenu.OnItemSelect += onCrackMenuSelect;
            //COCAIN


            cocainMenu = LsMenuPool.AddSubMenu(OrderDrugs, "Cocaine");
            if (UseSprites) cocainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
            cocaine = new UIMenuSliderItem("Cocaine $" + IntToMoney(cocain1Price - Discount(cocain1Price)) + "oz");
            cocainMenu.AddItem(cocaine);
            cocaine.Maximum = dealer1Cocain;
            cocaine.Multiplier = 1;
            cocainMenu.AddItem(OrderMultipliers);
            CocainBuy = new UIMenuItem("Buy: " + cocaine.Value);
            cocainMenu.AddItem(CocainBuy);
            cocaine.OnSliderChanged += onSliderChange;
            cocainMenu.OnItemSelect += onCocainMenuSelect;
            PlaceOrder = new UIMenuItem("Place Order $" + IntToMoney(0));
            OrderDrugs.AddItem(PlaceOrder);
            OrderDrugs.OnItemSelect += OnOrderDrugsMenuSelect;

            //STASH MENU
            stashMainMenu = new UIMenu("Safehouse Stash", "Select an option.");
            if (UseSprites) stashMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
            LsMenuPool.Add(stashMainMenu);

            stashTakeMenu = LsMenuPool.AddSubMenu(stashMainMenu, "Take");
            if (UseSprites) stashTakeMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");

            stashPutMenu = LsMenuPool.AddSubMenu(stashMainMenu, "Stash");
            if (UseSprites) stashPutMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
            //stash weed
            putWeedStash = new UIMenuSliderItem("Weed");
            stashPutMenu.AddItem(putWeedStash);
            putWeedStash.Maximum = PlayerInventory["Weed"];
            putWeedStash.Multiplier = 1;
            takeWeedStash = new UIMenuSliderItem("Weed");
            stashTakeMenu.AddItem(takeWeedStash);
            takeWeedStash.Maximum = 0;
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
            takeCrackStash.Maximum = 0;
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
            takeCocainStash.Maximum = 0;
            takeCocainStash.Multiplier = 1;
            putCocainStash.OnSliderChanged += onSliderChange;
            takeCocainStash.OnSliderChanged += onSliderChange;

            // OZZZ
            //stash weed
            putWeedOzStash = new UIMenuSliderItem("Weed Z's");
            stashPutMenu.AddItem(putWeedOzStash);
            putWeedOzStash.Maximum = pWeedOunce;
            putWeedOzStash.Multiplier = 1;
            putWeedOzStash.OnSliderChanged += onSliderChange;
            //stash crack
            putCrackOzStash = new UIMenuSliderItem("Crack Z's");
            stashPutMenu.AddItem(putCrackOzStash);
            putCrackOzStash.Maximum = pCrackOunce;
            putCrackOzStash.Multiplier = 1;
            putCrackStash.OnSliderChanged += onSliderChange;
            //stash COCAIN
            putCocainOzStash = new UIMenuSliderItem("Cocaine Oz");
            stashPutMenu.AddItem(putCocainOzStash);
            putCocainOzStash.Maximum = pCocaineOunce;
            putCocainStash.Multiplier = 1;
            putCocainStash.OnSliderChanged += onSliderChange;

            //stash MONEY
            putMoneyStash = new UIMenuSliderItem("Money $");
            stashPutMenu.AddItem(putMoneyStash);
            putMoneyStash.Maximum = player.Money;
            putMoneyStash.Multiplier = 100;
            takeMoneyStash = new UIMenuSliderItem("Money $");
            stashTakeMenu.AddItem(takeMoneyStash);
            takeMoneyStash.Maximum = 0;
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

            stashMainMenu.FormatDescriptions = true;
            string mWeed = "Weed. ";
            string mCrack = "Crack. ";
            string mCocaine = "Cocaine. ";
            string mGrams = "Grams of ";
            string mOunce = "Oz's of ";
            string mSell = "Ready to sell on the streets or give to a dealer.";
            string mStash = "Must be deposited in a Stash house before it can be sold.";
            takeWeedStash.Description = mGrams + mWeed + mSell;
            takeCrackStash.Description = mGrams + mCrack + mSell;
            takeCocainStash.Description = mGrams + mCocaine + mSell;
            putWeedStash.Description = mGrams + mWeed + mSell;
            putCrackStash.Description = mGrams + mCrack + mSell;
            putCocainStash.Description = mGrams + mCocaine + mSell;
            putWeedOzStash.Description = mOunce + mWeed + mStash;
            putCrackOzStash.Description = mOunce + mCrack + mStash;
            putCocainOzStash.Description = mOunce + mCocaine + mStash;


            List<dynamic> pArm = new List<dynamic>();
            for (int t = 0; t < playerArmours + 1; t++)
            {
                pArm.Add(t);
            }
            putArmourStash = new UIMenuListItem("Stash Armour", pArm, 0);
            stashPutMenu.AddItem(putArmourStash);


            List<dynamic> sArm = new List<dynamic>();
            for (int t = 0; t < 0 + 1; t++)
            {
                sArm.Add(t);
            }
            takeArmourStash = new UIMenuListItem("Take Armour", sArm, 0);
            stashTakeMenu.AddItem(takeArmourStash);


            //STASH WEAPONS
            if (playerWeapons.Count > 0)
            {
                putWeaponStash = new UIMenuListItem("Stash Weapon: ", playerWeps, 0);
                stashPutMenu.AddItem(putWeaponStash);
                putWeaponStash.OnListChanged += onListChange;
                takeWeaponStash = new UIMenuListItem("Take Weapon:", playerWeps, 0);
                stashTakeMenu.AddItem(takeWeaponStash);
                takeWeaponStash.OnListChanged += onListChange;
            }
            stashPutMenu.OnItemSelect += onStashMenuSelect;
            stashTakeMenu.OnItemSelect += onPlayerMenuSelect;

            ///VEHCILE MENUS
            vehStashMainMenu = new UIMenu("Vehicle Stash", "Select");

            if (UseSprites) vehStashMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
            LsMenuPool.Add(vehStashMainMenu);

            vehStashTakeMenu = LsMenuPool.AddSubMenu(vehStashMainMenu, "Take");
            if (UseSprites) vehStashTakeMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");

            vehStashPutMenu = LsMenuPool.AddSubMenu(vehStashMainMenu, "Stash");
            if (UseSprites) vehStashPutMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
            //stash weed
            putWeedVeh = new UIMenuSliderItem("Weed " + GramsToOz(PlayerInventory["Weed"]));
            vehStashPutMenu.AddItem(putWeedVeh);
            putWeedVeh.Maximum = PlayerInventory["Weed"];
            putWeedVeh.Multiplier = 1;
            takeWeedVeh = new UIMenuSliderItem("Weed " + "drugCarWeed" + "g");
            vehStashTakeMenu.AddItem(takeWeedVeh);
            takeWeedVeh.Maximum = 0;
            takeWeedVeh.Multiplier = 1;
            putWeedVeh.OnSliderChanged += onSliderChange;
            takeWeedVeh.OnSliderChanged += onSliderChange;
            //stash crack
            putCrackVeh = new UIMenuSliderItem("Crack " + GramsToOz(PlayerInventory["Crack"]));
            vehStashPutMenu.AddItem(putCrackVeh);
            putCrackVeh.Maximum = PlayerInventory["Crack"];
            putCrackVeh.Multiplier = 1;
            takeCrackVeh = new UIMenuSliderItem("Crack " + "drugCarCrack" + "g");
            vehStashTakeMenu.AddItem(takeCrackVeh);
            takeCrackVeh.Maximum = 0;
            takeCrackVeh.Multiplier = 1;
            putCrackVeh.OnSliderChanged += onSliderChange;
            takeCrackVeh.OnSliderChanged += onSliderChange;
            //stash COCAIN
            putCocainVeh = new UIMenuSliderItem("Cocaine " + GramsToOz(PlayerInventory["Cocaine"]));
            vehStashPutMenu.AddItem(putCocainVeh);
            putCocainVeh.Maximum = PlayerInventory["Cocaine"];
            putCocainVeh.Multiplier = 1;
            takeCocainVeh = new UIMenuSliderItem("Cocaine " + "drugCarCocain" + "g");
            vehStashTakeMenu.AddItem(takeCocainVeh);
            takeCocainVeh.Maximum = 0;
            takeCocainVeh.Multiplier = 1;
            putCocainVeh.OnSliderChanged += onSliderChange;
            takeCocainVeh.OnSliderChanged += onSliderChange;
            // OUNSES!!!
            //stash weed
            putWeedOzVeh = new UIMenuSliderItem("Weed Z's~g~" + pWeedOunce + "~s~oz");
            vehStashPutMenu.AddItem(putWeedOzVeh);
            putWeedOzVeh.Maximum = pWeedOunce;
            putWeedOzVeh.Multiplier = 1;
            takeWeedOzVeh = new UIMenuSliderItem("Weed Z's" + "drugCarWeed" + "oz");
            vehStashTakeMenu.AddItem(takeWeedOzVeh);
            takeWeedOzVeh.Maximum = 0;
            takeWeedOzVeh.Multiplier = 1;
            putWeedOzVeh.OnSliderChanged += onSliderChange;
            takeWeedOzVeh.OnSliderChanged += onSliderChange;
            //stash crack
            putCrackOzVeh = new UIMenuSliderItem("Crack Z's~g~" + pCrackOunce + "~s~oz");
            vehStashPutMenu.AddItem(putCrackOzVeh);
            putCrackOzVeh.Maximum = pCrackOunce;
            putCrackOzVeh.Multiplier = 1;
            takeCrackOzVeh = new UIMenuSliderItem("Crack Z's" + "drugCarCrack" + "oz");
            vehStashTakeMenu.AddItem(takeCrackOzVeh);
            takeCrackOzVeh.Maximum = 0;
            takeCrackOzVeh.Multiplier = 1;
            putCrackOzVeh.OnSliderChanged += onSliderChange;
            takeCrackOzVeh.OnSliderChanged += onSliderChange;
            //stash COCAIN
            putCocainOzVeh = new UIMenuSliderItem("Cocaine Z's~g~" + pCocaineOunce + "~s~oz");
            vehStashPutMenu.AddItem(putCocainOzVeh);
            putCocainOzVeh.Maximum = pCocaineOunce;
            putCocainOzVeh.Multiplier = 1;
            takeCocainOzVeh = new UIMenuSliderItem("Cocaine Z's" + "drugCarCocain" + "g");
            vehStashTakeMenu.AddItem(takeCocainOzVeh);
            takeCocainOzVeh.Maximum = 0;
            takeCocainOzVeh.Multiplier = 1;
            putCocainOzVeh.OnSliderChanged += onSliderChange;
            takeCocainOzVeh.OnSliderChanged += onSliderChange;




            //stash MONEY
            putMoneyVeh = new UIMenuSliderItem("Money $" + IntToMoney(player.Money));
            vehStashPutMenu.AddItem(putMoneyVeh);
            putMoneyVeh.Maximum = player.Money;
            putMoneyVeh.Multiplier = 100;
            takeMoneyVeh = new UIMenuSliderItem("Money $" + "drugCarMoney");
            vehStashTakeMenu.AddItem(takeMoneyVeh);
            takeMoneyVeh.Maximum = 0;
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
            takeWeaponVeh = new UIMenuListItem("Take Weapon :", new List<dynamic>() { "EROaR!" }, 0);
            vehStashTakeMenu.AddItem(takeWeaponVeh);
            takeWeaponVeh.OnListChanged += onListChange;




            putArmourVeh = new UIMenuListItem("Stash Armour", pArm, 0);
            vehStashPutMenu.AddItem(putArmourVeh);
            putArmourVeh.Activated += PutArmourVeh_Activated;

            List<dynamic> vArm = new List<dynamic>();
            for (int t = 0; t < 1; t++)
            {
                vArm.Add(t);
            }
            takeArmourVeh = new UIMenuListItem("Take Armour", vArm, 0);
            vehStashTakeMenu.AddItem(takeArmourVeh);
            takeArmourVeh.Activated += TakeArmourVeh_Activated;


            //VEHICLE WEAPONS
            if (playerWeapons.Count > 0)
            {
                putWeaponVeh = new UIMenuListItem("Stash Weapon: ", playerWeps, 0);
                vehStashPutMenu.AddItem(putWeaponVeh);
                putWeaponVeh.OnListChanged += onListChange;
            }
            vehStashPutMenu.OnItemSelect += onPutVehicleMenuSelect;
            vehStashTakeMenu.OnItemSelect += onTakeVehicleMenuSelect;


            //Customer Menu
            customerMainMenu = new UIMenu("Customer", "Select an option");
            if (UseSprites) customerMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_customer.png");
            LsMenuPool.Add(customerMainMenu);
            sell = new UIMenuItem("drug");
            customerMainMenu.AddItem(sell);
            turnAway = new UIMenuItem("cya");
            customerMainMenu.AddItem(turnAway);
            customerMainMenu.OnItemSelect += onCustomerMenuSelect;
            LsMenuPool.FormatDescriptions = true;
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

            DealerHandler = new PlayerDealerHandler();
            HouseHandler = new StashHouseHandler();

            LoadSpawnLocations();

            day = Function.Call<int>(Hash.GET_CLOCK_DAY_OF_WEEK);
            hour = Function.Call<int>(Hash.GET_CLOCK_HOURS);
            SlowTickTime = Game.GameTime;
            jobOfferTimer = Game.GameTime;
            randomAttackTimer = Game.GameTime;
            reloadTimer = Game.GameTime;
        }

        private void OrderDrugs_OnMenuOpen(UIMenu sender)
        {

            if (!NewOrder.Placed)
            {                                               
                PlaceOrder.Text = "Place Order $" + IntToMoney(NewOrder.Bill);
                UpdateOrderMenuDesc();
            }
        }

        private void DMultiplier_OnListChanged(UIMenuListItem sender, int newIndex)
        {
            int mlti = (int)sender.Items[sender.Index];
            bushWeed.Multiplier = mlti;
            crack.Multiplier = mlti;
            cocaine.Multiplier = mlti;
        }

        private void WMultiplier_OnListChanged(UIMenuListItem sender, int newIndex)
        {
            wWeed.Multiplier = multiplier[wMultiplier.Index];
            wCrack.Multiplier = multiplier[wMultiplier.Index];
            wCocaine.Multiplier = multiplier[wMultiplier.Index];
        }

        private void TakeArmourVeh_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            int amount = takeArmourVeh.Index;
            int max = 5;
            if (player.Character.Armor < 100) max = 6;
            if (amount > 0 && playerArmours + amount <= max)
            {
                if (player.Character.Armor < 100)
                {
                    player.Character.Armor = 100;
                    amount--;
                }
                playerArmours += amount;
                pStashVehicle.drugCarArmour -= amount;
                foreach (XElement ele in LSLifeSave.Descendants())
                {
                    if (ele.Name == "drugCarStash")
                    {
                        foreach (XElement el in ele.Elements())
                        {
                            if (el.Name == "armour")
                            {
                                el.Value = pStashVehicle.drugCarArmour.ToString();
                                break;
                            }
                        }
                    }
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
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                putArmourVeh.Items = new List<dynamic>();
                for (int t = 0; t < playerArmours + 1; t++)
                {
                    putArmourVeh.Items.Add(t);
                }
                takeArmourVeh.Items = new List<dynamic>();
                for (int t = 0; t < pStashVehicle.drugCarArmour + 1; t++)
                {
                    takeArmourVeh.Items.Add(t);
                }
            }
        }

        private void PutArmourVeh_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            int amount = putArmourVeh.Index;
            if (amount > 0 && playerArmours > 0)
            {

                playerArmours -= amount;
                pStashVehicle.drugCarArmour += amount;
                foreach (XElement ele in LSLifeSave.Descendants())
                {
                    if (ele.Name == "drugCarStash")
                    {
                        foreach (XElement el in ele.Elements())
                        {
                            if (el.Name == "armour")
                            {
                                el.Value = pStashVehicle.drugCarArmour.ToString();
                                break;
                            }
                        }
                    }
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
                    }
                }
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                putArmourVeh.Index = 0;
                putArmourVeh.Items = new List<dynamic>();
                for (int t = 0; t < playerArmours + 1; t++)
                {
                    putArmourVeh.Items.Add(t);
                }
                takeArmourVeh.Items = new List<dynamic>();
                for (int t = 0; t < pStashVehicle.drugCarArmour + 1; t++)
                {
                    takeArmourVeh.Items.Add(t);
                }

            }
        }


        void onTick(object sender, EventArgs e)
        {
            if (!Game.IsLoading)
            {
                if (firstRun)
                {
                    if (player.Character != null)
                    {

                        lPeds = new LsLPeds();
                        World.SetRelationshipBetweenGroups(Relationship.Pedestrians, playersTeam, CustomerRel);
                        World.SetRelationshipBetweenGroups(Relationship.Companion, playersTeam, playerRelationship);
                        World.SetRelationshipBetweenGroups(Relationship.Neutral, playersTeam, rival_nutral);
                        World.SetRelationshipBetweenGroups(Relationship.Hate, playersTeam, rival_enemy);
                        World.SetRelationshipBetweenGroups(Relationship.Pedestrians, playerRelationship, CustomerRel);
                        World.SetRelationshipBetweenGroups(Relationship.Companion, playerRelationship, playersTeam);
                        World.SetRelationshipBetweenGroups(Relationship.Neutral, playerRelationship, rival_nutral);
                        World.SetRelationshipBetweenGroups(Relationship.Hate, playerRelationship, rival_nutral);
                        World.SetRelationshipBetweenGroups(Relationship.Pedestrians, rival_nutral, CustomerRel);
                        World.SetRelationshipBetweenGroups(Relationship.Neutral, rival_nutral, playersTeam);
                        World.SetRelationshipBetweenGroups(Relationship.Neutral, rival_nutral, playerRelationship);
                        World.SetRelationshipBetweenGroups(Relationship.Respect, rival_nutral, rival_enemy);
                        World.SetRelationshipBetweenGroups(Relationship.Pedestrians, rival_enemy, CustomerRel);
                        World.SetRelationshipBetweenGroups(Relationship.Hate, rival_enemy, playersTeam);
                        World.SetRelationshipBetweenGroups(Relationship.Hate, rival_enemy, playerRelationship);
                        World.SetRelationshipBetweenGroups(Relationship.Respect, rival_enemy, rival_nutral);
                        World.SetRelationshipBetweenGroups(Relationship.Companion, CustomerRel, playersTeam);
                        World.SetRelationshipBetweenGroups(Relationship.Companion, CustomerRel, playerRelationship);
                        World.SetRelationshipBetweenGroups(Relationship.Companion, CustomerRel, rival_nutral);
                        World.SetRelationshipBetweenGroups(Relationship.Companion, CustomerRel, rival_enemy);
                        ratChance = -100;
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, player.Character, 9, 0, 0, 0);
                        playerHasBag = false;
                        canSprint = true;
                        playerArea = World.GetZoneNameLabel(playerPos);
                        areaTypes.TryGetValue(playerArea, out AreaType aType);
                        areas.Add(new Area(playerArea, LsDrugs(), aType));
                        areaIndex = 0;
                        firstRun = false;

                        //StashHouse = new StashHouse(new Vector3(488.5433f, -873.6772f, 24.34628f), Vector3.Zero, Vector3.Zero, false, 50000);

                    }
                }
                if (DEBUG)
                {
                    string debugString = "0";
                    if (Game.GameTime < areas[areaIndex].DemandBuff())
                    {
                        debugString = (areas[areaIndex].DemandBuff() - Game.GameTime).ToString();

                    }
                    //if() debugString += StashVehicle?.CurrentPosition;
                    myUIText = new UIText(debugString, new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                    myUIText.Draw();
                }
                //if (DEBUG)
                //{
                //string heat = "";
                //if (areas.Count > 0) heat += areas[areaIndex].Heat;

                //if (DealerHandler.dealers.Count > 0) pDealString += " pDstateN: " + DealerHandler.dealers[0].NewState + " pDstateC: " + DealerHandler.dealers[0].CurrentState;
                //if (pigs.Count > 0) pState += pigs[0].CurrentState + " nState:" + pigs[0].NewState;

                //     
                //string str = "";
                //string carSpeed = "";
                //if (Game.Player.Character.IsInVehicle()) carSpeed = Game.Player.Character.CurrentVehicle.Speed.ToString();
                ////string str = Function.Call<bool>(Hash.IS_PLAYER_CONTROL_ON).ToString();                    
                //Vector3 playerVel = playerPos + Game.Player.Character.Velocity;
                //str = "desSpeed: " + GetRoadSpeed(playerVel).ToString() + " curSpeed: " + carSpeed;
                //Vector3 nodePos = ClosetNodePos(playerVel);
                ////nodePos.Z = World.GetGroundHeight(nodePos);
                //float heading = 0f;
                //OutputArgument outArgA = new OutputArgument();
                //OutputArgument outArgB = new OutputArgument();
                //if (Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING, nodePos.X, nodePos.Y, nodePos.Z, outArgA, outArgB, 0, 1077936128, 0))
                //{
                //    heading = outArgB.GetResult<float>(); //getting heading                                               
                //}
                //double degres = (double)heading * (Math.PI / 180.0);
                //Vector3 nodeHeading = new Vector3((float)Math.Cos(degres), (float)Math.Sin(degres), 0);
                //Vector3 newSpawnPos = nodePos;
                //Vector3 headingInWorld = nodePos + nodeHeading;
                //Vector3 headingInWorldFlip = nodePos + (nodeHeading * -1);
                //newSpawnPos.Z += 0.5f;
                ////World.DrawMarker(MarkerType.DebugSphere, playerVel ,Vector3.Zero, Vector3.Zero, new Vector3(0.5f, 0.5f, 0.5f), Color.FromArgb(150, 150, 150, 255));
                //World.DrawMarker(MarkerType.DebugSphere, nodePos, Vector3.Zero, Vector3.Zero, new Vector3(0.5f, 0.5f, 0.5f), Color.FromArgb(150, 150, 255, 150));
                //World.DrawMarker(MarkerType.DebugSphere, headingInWorld, Vector3.Zero, Vector3.Zero, new Vector3(0.2f, 0.2f, 0.2f), Color.FromArgb(150, 255, 80, 80));
                //Vector3 oldSpawnPos = newSpawnPos;
                //Vector3 finalSpawnPos = oldSpawnPos;
                //for (int i = 0; i < 16; i++)
                //{
                //    if (Function.Call<bool>(Hash.IS_POINT_ON_ROAD, newSpawnPos.X, newSpawnPos.Y, newSpawnPos.Z))
                //    {
                //        oldSpawnPos = newSpawnPos;
                //        newSpawnPos = newSpawnPos + (nodeHeading * 0.5f);
                //    }
                //    else
                //    {
                //        finalSpawnPos = oldSpawnPos;
                //        finalSpawnPos.Z = World.GetGroundHeight(finalSpawnPos);
                //        break;
                //    }
                //}
                //World.DrawMarker(MarkerType.DebugSphere, finalSpawnPos, Vector3.Zero, Vector3.Zero, new Vector3(0.3f, 0.3f, 0.3f), Color.FromArgb(150, 150, 255, 255));
                //World.DrawMarker(MarkerType.DebugSphere, RoadSideFromSpawn(nodePos, playerPos), Vector3.Zero, Vector3.Zero, new Vector3(0.5f, 0.5f, 0.5f), Color.FromArgb(150, 80, 255, 255));


                //    //str = GetRoadFlags(Game.Player.Character.Position + (Game.Player.Character.Velocity));


                //}



                //string debugData = "Areas: "+areas.Count+" aName:" + area.Name + " aHeat:" + area.Heat + "% aPolice:" + area.CopPresance + " aGang:" + area.GangPresance+"/"+area.Dealers + " RChance:"+ratChance+ "% aDrug:"+area.Drugs[0].Demand/area.Dealers+"/"+area.Drugs[0].Delay / (area.Drugs[0].Demand/area.Dealers) + "ms DemandNotMet"+area.DemandNotMet;                                        
                if (Prison.Started && (int)Prison.CurrentState > 3)
                {
                    Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "restrictedareas");
                    Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "re_prison");
                }

                if (LsMenuPool != null)
                {
                    LsMenuPool.ProcessMenus();
                }
                iFruit.Update();

                if (Game.GameTime > oneSecDelay + 10001)
                {
                    oneSecDelay = Game.GameTime;
                    if (player.IsPlaying)
                    {

                        if (!dealer1Angry && dealer1Debt > zeeDebtCap() + (zeeDebtCap() * 0.5))
                        {
                            if (DEBUG) { UI.Notify("dealer1 angry"); }
                            dealer1Angry = true;
                        }
                        if (driveByHandler.AmountOfDriveBy() == 0)
                        {

                            if (isDealing && areas[areaIndex].GangPresance > 4 && timeDealing > 60000)
                            {
                                if (rnd.Next(0, 30) == 0)
                                {
                                    if (DEBUG) { UI.Notify("poor area drive-by"); }
                                    SpawnDriveBy();
                                }
                            }
                            if (dealer1Angry && driveByHandler.AmountOfDriveBy() == 0 && drugDealer1 == null)
                            {
                                if (rnd.Next(0, 10) == 0)
                                {
                                    if (DEBUG) { UI.Notify("dealer1 drive-by"); }
                                    SpawnDriveBy();
                                }
                            }
                        }
                        if (entitiesCleanUp.Count > 0)
                        {
                            foreach (Entity ent in entitiesCleanUp.ToList())
                            {
                                if (ent == null || !ent.Exists())
                                {
                                    entitiesCleanUp.Remove(ent);
                                }
                                else
                                if (ent.Exists())
                                {
                                    if (ent.CurrentBlip != null || ent.CurrentBlip.Exists()) { ent.CurrentBlip.Remove(); }
                                    if (ent == attacker)
                                    {
                                        entitiesCleanUp.Remove(ent);
                                        if (DEBUG) { UI.Notify("stoped delete attacker"); }
                                    }
                                    else
                                    {
                                        if (ent.Position.DistanceTo(playerPos) > 200 && !ent.IsOnScreen)
                                        {

                                            entitiesCleanUp.Remove(ent);
                                            if (DEBUG) { UI.Notify("Delete:" + ent); }
                                            ent.Delete();
                                        }
                                    }
                                }

                            }
                        }

                        if (areas.Count > 0)
                        {
                            DoGangGrowth();
                        }
                    }
                }

                #region 1000ms ticks
                if (Game.GameTime > areaCheckTimer + 999)
                {
                    newWepAmount = 0;
                    foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
                    {
                        if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, player.Character, (int)weapon))
                        {
                            newWepAmount++;
                            if (newWepAmount > playerWeapons.Count) break;
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
                            bool hasPedGotWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, player.Character, (int)weapon);
                            int ammunitionInWeapon = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, player.Character, (int)weapon);
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
                    }



                    int dealers = 0;
                    if (isDealing) { dealers += 1; }
                    if (aDealer != null && aDealer.Area == areas[areaIndex].Name) { dealers += 1; }
                    areas[areaIndex].Dealers = dealers;
                    areaCheckTimer = Game.GameTime;
                    string newArea = World.GetZoneNameLabel(playerPos);
                    if (aDealer != null && playerArea != aDealer.Area && playerPos.DistanceTo(aDealer.dPed.Position) > 300)
                    {
                        if (aDealer.cPed != null)
                        {
                            RemovePedFromWorld(aDealer.cPed, false);
                        }
                        RemovePedFromWorld(aDealer.dPed, false);
                        aDealer = null;
                    }
                    if (playerArea != newArea)
                    {
                        playerArea = newArea;
                        if (areas.FindAll(a => a.Name == playerArea).Count == 0)
                        {
                            if (areaTypes.TryGetValue(playerArea, out AreaType type))
                            {
                                areas.Add(new Area(playerArea, LsDrugs(), type));
                            }
                            else UI.Notify("~r~LsLife - Error getting area type for Area " + playerArea);
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
                        if (aDealer == null && areas[areaIndex].CanSpawnDealer())
                        {
                            DbugString("demandBuff 0");
                            if (areas[areaIndex].GangPresance > 0 && areas[areaIndex].Name != World.GetZoneNameLabel(HouseHandler.Houses[0].entrancePos))
                            {
                                List<Ped> aPeds = lPeds.GetPeds.FindAll(r => World.GetZoneNameLabel(r.Position) == areas[areaIndex].Name && r.Position.DistanceTo(playerPos) > 20);
                                if (aPeds.Count > 0)
                                {
                                    foreach (Ped p in aPeds)
                                    {
                                        if (!CheckIfPedUsed(p, false, false))
                                        {
                                            aDealer = new AreaDealer(p);
                                            if (DEBUG) { UI.Notify("Area Dealer Spawned using ~g~" + p.Model); }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {

                            DbugString("Cant spawn dealer dead :" + areas[areaIndex].DealerKilled);
                        }

                    }
                }
                #endregion


                #region 350ms ticks
                // 300ms delay
                if (Game.GameTime > SlowTickTime + 350)
                {
                    SlowTickTime = Game.GameTime;
                    if (Game.Player != player) player = Game.Player;
                    if (lPeds != null) lPeds.Ontick();
                    if (DealerHandler != null) DealerHandler?.OnSlowTick();
                    if (pickupHandler != null) pickupHandler?.OnSlowTick();
                    if (driveByHandler != null) driveByHandler.OnTick();
                    if (ZeesDealers != null)
                    {
                        ZeesDealers.OnSlowTick();
                        if (ZeesDealers.Kill) ZeesDealers = null;
                    }

                    if (aDealer != null)
                    {
                        if (aDealer.cPed != null)
                        {
                            if (aDealer.cPed.IsDead)
                            {
                                Ped dKiller = null;
                                if (aDealer.dPed.GetKiller() is Ped) dKiller = aDealer.dPed.GetKiller() as Ped;
                                if (dKiller != null && dKiller.CurrentPedGroup == player.Character.CurrentPedGroup)
                                {
                                    aDealer.NewState = DStates.AttackPlayer;
                                    if (driveByHandler.AmountOfDriveBy() == 0 && rnd.Next(0, 3) == 0)
                                    {
                                        SpawnDriveBy();
                                    }
                                }
                                RemovePedFromWorld(aDealer.cPed, false);
                                aDealer.cPed = null;
                            }
                        }
                        if (aDealer != null && aDealer.dPed.IsDead)
                        {
                            areas[areaIndex].KillDealer();

                            if (aDealer.dPed.WasKilledByTakedown && CanSee(aDealer.dPed, player.Character) && rnd.Next(0, 3) == 0)
                            {
                                if (driveByHandler.AmountOfDriveBy() == 0 && rnd.Next(0, 3) == 0)
                                {
                                    SpawnDriveBy();
                                }
                                if (DEBUG) { UI.Notify("Was seen killing dealer"); }
                            }
                            Ped dKiller = null;
                            if (aDealer.dPed.GetKiller() is Ped) dKiller = aDealer.dPed.GetKiller() as Ped;
                            if (dKiller != null && dKiller.CurrentPedGroup == player.Character.CurrentPedGroup)
                            {
                                pickupHandler.AddBag(new DroppedBag("A ~r~rival dealer~s~ dropped a ~g~bag.", rnd.Next(1, 50), rnd.Next(1, 50), rnd.Next(1, 50), rnd.Next(100, 5000), PedWeapons(aDealer.dPed), aDealer.dPed.Position));
                            }
                            if (aDealer.cPed != null)
                            {
                                RemovePedFromWorld(aDealer.cPed, true);
                                aDealer.cPed = null;
                            }
                            if (aDealer.dPed != null)
                            {
                                RemovePedFromWorld(aDealer.dPed, true);
                                aDealer.dPed = null;
                            }
                            aDealer = null;
                        }

                        if (aDealer != null && aDealer.dPed != null && aDealer.dPed.Exists() && aDealer.dPed.IsAlive)
                        {
                            aDealer.DoState();
                            aDealer.CheckState();
                        }
                    }

                    if (wepDealer != null && wepDealer.ped.IsAlive)
                    {
                        if (player.Character.CurrentVehicle != wepDealer.vehicle && wepDealer.ped.Position.DistanceTo(playerPos) < 10 || wepDealer.ped.Position.DistanceTo(wepDealer.destination) < 10)
                        {
                            Vector3 pos = wepDealer.vehicle.Position + wepDealer.vehicle.Velocity;
                            int NodeID = Function.Call<int>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID, pos.X, pos.Y, pos.Z, 1, 1, 1f, 0f);
                            OutputArgument outArgA = new OutputArgument();
                            Function.Call(Hash.GET_VEHICLE_NODE_POSITION, NodeID, outArgA);
                            Vector3 parkPos = outArgA.GetResult<Vector3>();
                            wepDealer.destination = parkPos;
                        }
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
                                wepDealer.ped.Task.DriveTo(wepDealer.vehicle, parkPos, 2f, 6f, (int)DrivingStyle.Normal);
                                wepDealer.destination = Vector3.Zero;
                                wepDealer.state = Dealer.States.Arrived;
                            }
                        }
                        if (wepDealer.state == Dealer.States.Arrived)
                        {
                            if (player.Character.IsInVehicle())
                            {
                                if (player.Character.CurrentVehicle.Handle == wepDealer.vehicle.Handle)
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
                                            UI.ShowSubtitle("Ok, we heading to ~y~" + World.GetStreetName(wepDealer.destination));
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
                            wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, 20f, (int)DrivingStyle.Normal);
                            wepDealer.state = Dealer.States.Driving;
                            UI.ShowSubtitle("Ok, Slowing it down.", 2000);
                        }
                    }
                    if (dealer1Angry && dealer1Debt < 170000)
                    {
                        dealer1Angry = false;
                    }

                    playerGroup = player.Character.CurrentPedGroup;
                    DateTimeUpdate();
                    if (attackerPickupSpawned)
                    {
                        //UI.Notify("Spawned not picked up");
                        if (!attackerPickup.ObjectExists())
                        {
                            //UI.Notify("Attackers bag picked UP");
                            attackerPickup.Delete();
                            attackerPickupSpawned = false;
                            WeaponHash weaponHash = WeaponHash.Unarmed;
                            int ammo = 0;
                            WeaponHash currentWep = player.Character.Weapons.Current.Hash;
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
                                            player.Character.Weapons.Give(weaponHash, ammo, true, true);
                                            player.Character.Weapons.Select(weaponHash, true);
                                            foreach (XElement c in element.Elements())
                                            {
                                                if (c.Name == "COMPONENT")
                                                {
                                                    Enum.TryParse(c.Value, out WeaponComponent wepcomp);
                                                    player.Character.Weapons.Current.SetComponent(wepcomp, true);
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
                            player.Character.Weapons.Select(currentWep);
                            PlayerInventory["Weed"] = attackerWeed;
                            PlayerInventory["Crack"] = attackerCrack;
                            PlayerInventory["Cocaine"] = attackerCocaine;
                            //UI.Notify("atacker had" + attackerInventory["Weed"] + "Weed, Player now has " + PlayerInventory["Weed"] + " Weed");
                            attackerWeed = 0;
                            attackerCrack = 0;
                            attackerCocaine = 0;
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
                            if (areas[areaIndex].Heat < 5000)
                            {
                                areas[areaIndex].Heat += player.WantedLevel;
                            }
                        }
                        else
                        {
                            foreach (Area area in areas)
                            {
                                if (area.Heat > 0)
                                {
                                    if (isDealing)
                                    {
                                        if (lastTimeDealDone + 10000 < Game.GameTime) { area.Heat -= 1; }
                                    }
                                    else
                                    {
                                        area.Heat -= 1;
                                    }
                                }
                            }
                        }
                    }
                    if (player.IsPlaying && notFined != true) { notFined = true; }


                    /// We Dead Stuff
                    if (player.Character != null && !player.Character.IsAlive && !playerDead)
                    {
                        playerDead = true;
                        if (isDealing) { isDealing = false; }
                        var killer = player.Character.GetKiller();
                        if (killer != null)
                        {
                            if (killer is Ped p)
                            {
                                if (p != null && p.IsHuman && !p.IsPlayer)
                                {
                                    if (!IsPolice(p) && attacker == null)
                                    {
                                        SendTextMsg("I found the guy that got you, i sent you his location.");
                                        attacker = p;
                                        attacker.IsPersistent = true;
                                        attacker.Money = player.Money;
                                        player.Money -= player.Money;
                                        if (oldCustomers.Contains(attacker)) { oldCustomers.Remove(attacker); }
                                        if (angryPeds.Contains(attacker)) { angryPeds.Remove(attacker); }
                                        if (driveByHandler.GetDriveBies.Count > 0)
                                        {
                                            foreach (DriveBy driveBy in driveByHandler.GetDriveBies)
                                            {
                                                if (driveBy.Peds.Count > 0 && driveBy.Peds.Contains(attacker))
                                                {
                                                    driveBy.Peds.Remove(attacker);
                                                    break;
                                                }
                                            }
                                        }

                                        if (aDealer?.dPed == attacker) { aDealer = null; }
                                        attacker.BlockPermanentEvents = true;
                                        attacker.IsPersistent = true;

                                        AddBlip(attacker, BlipColor.Red, "Robber", false, false);

                                        WeaponHash weaponHash = WeaponHash.Unarmed;
                                        int ammo = 0;
                                        player.Character.Weapons.RemoveAll();
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

                                        attackerWeed = PlayerInventory["Weed"];
                                        attackerCrack = PlayerInventory["Crack"];
                                        attackerCocaine = PlayerInventory["Cocaine"];
                                        foreach (var drug in PlayerInventory.Keys.ToList())
                                        {
                                            PlayerInventory[drug] = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // We busted

                    if (player.Character != null && !player.IsPlaying) /// We have been arested or killed Do stuff
                    {
                        if (isDealing == true) { isDealing = false; }
                        if (notFined && playerWanted)
                        {
                            notFined = false;
                            playerWanted = false;
                            int AmountOfDrugs = TotalDrugsCarried;
                            if (pStashVehicle != null && pStashVehicle.drugCarWanted)
                            {
                                if (pStashVehicle.TotalDrugs() > 0)
                                {
                                    AmountOfDrugs += pStashVehicle.TotalDrugs();
                                    pStashVehicle.ImpoundCar();
                                }
                            }
                            if (attacker == null)
                            {
                                if (AmountOfDrugs > 0)
                                {
                                    SendTextMsg("They took $" + IntToMoney(player.Money) + ", all your Drugs and Weapons.");
                                    player.Character.Weapons.RemoveAll();
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

                            //Prison.Started = true;
                        }
                        else
                        if (playerDead)
                        {
                            playerDead = false;
                            if (attacker == null)
                            {
                                if (TotalDrugsCarried > 0)
                                {
                                    player.Character.Weapons.RemoveAll();
                                    SendTextMsg("Police confisated $" + IntToMoney(player.Money) + ", all your Drugs and Weapons.");
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
                                if (playerPos.DistanceTo(p.Position) > 100)
                                {
                                    RemovePedFromWorld(p, false);
                                    angryPeds.Remove(p);
                                    break;
                                }
                                else
                                {
                                    if (!p.IsInCombatAgainst(player.Character) || !p.IsInMeleeCombat)
                                    {
                                        p.Task.FightAgainst(player.Character);
                                        if (p.IsPersistent == false) { p.IsPersistent = true; }
                                        p.BlockPermanentEvents = true;
                                        if (p.CurrentBlip.Exists())
                                        {
                                            p.CurrentBlip.Name = "Robber";
                                            p.CurrentBlip.IsFriendly = false;
                                        }
                                    }
                                }
                            }
                        }
                    }



                    // We Are Alive stuff
                    if (player.Character != null && player.Character.IsAlive && player.IsPlaying)
                    {
                        if (pigs.Count == 0)
                        {
                            if (PlayerComplied) PlayerComplied = false;
                            if (ratChance > 50f && rnd.Next(0, 100) == 0)
                            {
                                SpawnPigs();
                            }
                            if (ratChance > 95f && rnd.Next(0, 5) == 0)
                            {
                                SpawnPigs();
                            }
                        }
                        else HandlePigs();
                        if (pStashVehicle == null)
                        {
                            pStashVehicle = new StashVehicle();
                        }
                        else
                        {
                            pStashVehicle.OnDelayedTick();
                        }


                        if (Prison.Started)
                        {
                            if (Function.Call<bool>(Hash.IS_PLAYER_CONTROL_ON) && !Prison.Ready)
                            {
                                Function.Call(Hash.SET_PLAYER_CONTROL, player.Handle, false, 256);
                                UI.Notify("READY");
                                Prison.Ready = true;
                            }
                            Prison.PrisonTick();
                        }

                        if (attacker != null)
                        {
                            if (attacker.CurrentBlip == null || !attacker.CurrentBlip.Exists())
                            {
                                AddBlip(attacker, BlipColor.Red, "Robber", false, false);
                            }
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
                            }
                            else
                            {
                                if (!attacker.IsPersistent) { attacker.IsPersistent = true; }
                                float distance = attacker.Position.DistanceTo(playerPos);

                                if (!attackerAttacking)
                                {
                                    if (Game.GameTime > attackCheckTime + 1000)
                                    {
                                        attackCheckTime = Game.GameTime;
                                        //RaycastResult raycast = World.RaycastCapsule(attacker.Position + attacker.ForwardVector, Game.Player.Character.Position, 60, IntersectOptions.Everything);
                                        if (distance < 60 && CanSee(attacker, player.Character))
                                        {
                                            int EnemyRelationShipGroup = Function.Call<int>(Hash.GET_HASH_KEY, "HATES_PLAYER");
                                            Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, attacker, EnemyRelationShipGroup);
                                            attackerAttacking = true;
                                            attacker.Task.ClearAll();
                                            attacker.Task.FightAgainst(player.Character);
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

                        bool sprintChange = false;
                        if (!playerHasBag)
                        {
                            if (!canSprint)
                            {
                                canSprint = true;
                                sprintChange = true;
                            }
                            if ((TotalDrugsCarried / 28) + (playerWeps.Count - 1) + (player.Money / 50000) > 15)
                            {
                                PedBag(player.Character, true);
                                playerHasBag = true;
                            }
                        }
                        if (playerHasBag)
                        {
                            if (!canSprint)
                            {
                                if (player.Character.CurrentVehicle != null && player.Character.CurrentVehicle.Model.IsBicycle)
                                {
                                    canSprint = true;
                                    sprintChange = true;
                                }
                            }
                            else
                            {
                                if (player.Character.CurrentVehicle == null)
                                {
                                    canSprint = false;
                                    sprintChange = true;
                                }
                            }
                            if ((TotalDrugsCarried / 28) + (playerWeps.Count - 1) + (player.Money / 50000) <= 15)
                            {
                                PedBag(player.Character, false);
                                playerHasBag = false;
                            }
                        }
                        if (sprintChange)
                        {
                            if (canSprint)
                            {
                                Function.Call(Hash.SET_PLAYER_SPRINT, player, true);
                            }
                            else
                            {
                                Function.Call(Hash.SET_PLAYER_SPRINT, player, false);
                            }
                        }

                        if (player.WantedLevel > 0 && playerWanted == false) { playerWanted = true; }
                        if (playerWanted && player.WantedLevel == 0) { playerWanted = false; }

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
                        if (player.WantedLevel > 0 && isDealing == true)
                        {
                            isDealing = false;
                        }
                        if (!isDealing)
                        {
                            
                            if (!removedStuff)
                            {
                                removedStuff = true;

                                if (customers.Count > 0)
                                {
                                    foreach (Customer c in customers)
                                    {
                                        if (c != null)
                                        {
                                            foreach (Drug d in areas[areaIndex].Drugs)
                                            {
                                                if(d.Name == c.Drug.Name)
                                                {
                                                    d.Supplied = false;
                                                }
                                            }
                                            
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

                            int totalDrugs = TotalDrugsCarried + HouseHandler.TotalStashDrugs();
                            if (pStashVehicle != null) totalDrugs += pStashVehicle.TotalDrugs();
                            if (dealer1Debt < 110000 && totalDrugs > 56 && Game.GameTime > jobOfferTimer + 20000 && jobs.ToList().Count == 0)
                            {
                                jobOfferTimer = Game.GameTime;
                                int r = rnd.Next(0, 100);
                                if (r > 70 - (CurrentRepLvl(dealer1Rep) * 2))
                                {
                                    Vector3 spawnPos = GenerateSpawnPos(playerPos.Around(275f), Nodetype.Offroad, true);
                                    if (spawnPos == Vector3.Zero)
                                    {
                                        if (DEBUG) { UI.Notify("spawning new job, that one guy was found."); }
                                        spawnPos = GenerateSpawnPos(playerPos.Around(200f), Nodetype.Offroad, true);
                                    }
                                    for (int j = 0; j < 100; j++)
                                    {
                                        if (Function.Call<bool>(Hash.IS_POINT_ON_ROAD, spawnPos.X, spawnPos.Y, spawnPos.Z))
                                        {
                                            spawnPos = GenerateSpawnPos(playerPos.Around(200f), Nodetype.Offroad, true);
                                            spawnPos = World.GetNextPositionOnSidewalk(spawnPos);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    var peds = lPeds.GetPeds;
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
                                                        switch (pDrugs[rnd.Next(0, pDrugs.Count)])
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
                                                            float div = c.amount / 7;
                                                            int price = c.amount * c.Drug.PricePerG;
                                                            if (div >= 1)
                                                            {
                                                                price -= ((int)div * c.Drug.PricePerG);
                                                            }
                                                            SendTextMsg("Yo can you go drop ~g~" + GramsToOz(c.amount) + "~w~ of ~g~" + c.Drug.Name + " ~w~to my pal for $" + IntToMoney(price) + "?");
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

                        if (weaponDealer1 != null && weaponDealer1.IsAlive)
                        {
                            var distanceToPlayer = World.GetDistance(weaponDealer1.Position, playerPos);
                            if (distanceToPlayer > 15f && weaponDealer1.CurrentPedGroup != player.Character.CurrentPedGroup)
                            {
                                if (Game.GameTime > dealerTimer + 2000 && playerPos != pedTarget)
                                {
                                    dealerTimer = Game.GameTime;
                                    pedTarget = playerPos;
                                    weaponDealer1.Task.GoTo(player.Character);
                                }
                            }
                            else
                            {
                                if (weaponDealer1.CurrentPedGroup != player.Character.CurrentPedGroup)
                                {
                                    weaponDealer1.Task.ClearAll();
                                    Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, weaponDealer1.Handle, playerGroup);
                                }
                            }
                            if (!jobOffer && weaponDealer1.IsStopped && distanceToPlayer <= 3f && player.Character.IsStopped && IsCustomerNear())
                            {

                                var peds = lPeds.GetPeds.FindAll(r => r.Position.DistanceTo(playerPos) < 10f);
                                if (peds.Count() - PedsInPlayersGroup() > 3)
                                {
                                    //DealerTaskUI = new UIText("Its to busy here.", new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);
                                    if (Game.GameTime > displayHelpTimer + 5000)
                                    {
                                        displayHelpTimer = Game.GameTime;
                                        LsFunctions.DisplayHelpText("Find some where quiter.");
                                    }
                                    readyToDeal = false;
                                    if (wepMenu.Visible)
                                    {
                                        wepMenu.Visible = !wepMenu.Visible;
                                    }
                                }
                                else
                                if (!readyToDeal && !LsMenuPool.IsAnyMenuOpen())
                                {
                                    if (Game.GameTime > displayHelpTimer + 5000)
                                    {

                                        displayHelpTimer = Game.GameTime;
                                        LsFunctions.DisplayHelpText("Press ~INPUT_CONTEXT~ to interact");
                                    }
                                    //DealerTaskUI = new UIText("This will do.", new Point(10, 10), 0.4f, Color.WhiteSmoke, 0, false);

                                    readyToDeal = true;
                                }
                            }
                            else readyToDeal = false;

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
                #endregion


                #region everyTick
                // if we alive do this stuff each Tick.

                if (player.Character != null && player.Character.IsAlive && player.IsPlaying)
                {
                    //enemyHandler.Ontick();
                    if (DEBUG) PedDebugger.OnTick();
                    if (DealerHandler != null) DealerHandler.OnTick();
                    if (HouseHandler != null) HouseHandler.Ontick();
                    if (ZeesDealers != null)
                    {
                        ZeesDealers.OnTick();
                    }
                    if (showInventory)
                    {
                        InventoryGui();
                        if(AreaDebug) areaInfoUI();
                        player.GetTargetedEntity();
                    }


                    playerPos = player.Character.Position;

                    if (pigs.Count > 0 && pigs[0].AskPlayerToComply)
                    {
                        if (Game.GameTime > displayHelpTimer + 3000 && !LsMenuPool.IsAnyMenuOpen())
                        {
                            Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
                            displayHelpTimer = Game.GameTime;
                            DisplayHelpText("~INPUT_CONTEXT~ to comply.");
                        }
                        if (Game.IsControlJustPressed(2, GTA.Control.Context))
                        {
                            PlayerComplied = true;
                            TaskSequence giveUP = new TaskSequence();
                            if (player.Character.CurrentVehicle != null) giveUP.AddTask.LeaveVehicle();
                            giveUP.AddTask.TurnTo((pigs[0].Ped.ForwardVector) + playerPos, 1000);
                            giveUP.AddTask.HandsUp(20000);
                            player.Character.Task.PerformSequence(giveUP);
                        }
                    }

                    if (!LsMenuPool.IsAnyMenuOpen())
                    {
                        HandleAreaDealers();
                        if (weaponDealer1 != null && readyToDeal)
                        {
                            if (Game.IsControlJustPressed(2, GTA.Control.Context))
                            {
                                if (LsMenuPool != null && !LsMenuPool.IsAnyMenuOpen())
                                {
                                    wepMenu.Visible = !wepMenu.Visible;
                                }
                            }
                        }
                    }

                    if (pStashVehicle != null) pStashVehicle.OnTick();

                    if (Game.GameTime > ratTimer + 1000)
                    {

                        if (isDealing) timeDealing = Game.GameTime - timeStartedDealing;
                        else timeDealing = 0;
                        thisStreet = World.GetStreetName(new Vector2(playerPos.X, playerPos.Y));
                        ratTimer = Game.GameTime;
                        //rat = rnd.Next(0, 1000);
                        //if (thisStreet == lastStreet && heatBar.Percentage > 0.5 && rat < ratChance + (timeDealing / 60000) && timeDealing > 10000)
                        //{
                        //    SendTextMsg("Someone ratted on you.");
                        //    //Game.Player.WantedLevel = 2;
                        //    //isDealing = false;
                        //    ratChance = 0;
                        //    areas[areaIndex].Heat += 1000;                               
                        //}
                        if (thisStreet != lastStreet)
                        {
                            lastStreet = thisStreet;
                            ratChance -= 10;
                            if (ratChance < -100)
                            { ratChance = -100; }
                            if (isDealing) timeStartedDealing = Game.GameTime;
                        }
                        else
                        {
                            if (Game.GameTime > lastTimeDealDone + 10000)
                            {
                                if (ratChance > -100 && player.WantedLevel == 0)
                                {
                                    if (isDealing) ratChance += timeDealing / 180000;
                                    else ratChance--;
                                }
                            }
                            else
                            {
                                if (ratChance < 100)
                                {
                                    if (ratChance + (timeDealing / 60000) + (areas[areaIndex].Heat / 100) - (int)areas[areaIndex].Reputation + areas[areaIndex].CopPresance > 100)
                                    {
                                        ratChance = 100;
                                    }
                                    else
                                    {
                                        ratChance += (timeDealing / 60000) + (areas[areaIndex].Heat / 100) - (int)areas[areaIndex].Reputation + areas[areaIndex].CopPresance;
                                    }
                                    areas[areaIndex].Heat += PedsCanSeeDeal;
                                }
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
                                    if (PlayerInventory[jobs[0].Drug.Name] >= jobs[0].amount)
                                    {
                                        UI.ShowSubtitle("Deliver " + GramsToOz(jobs[0].amount) + " of ~g~" + jobs[0].Drug.Name + "~w~ to ~y~" + World.GetStreetName(jobs[0].Ped.Position) + ".", 10000);
                                    }
                                    else
                                    {
                                        UI.ShowSubtitle("You do not have enough ~g~" + jobs[0].Drug.Name + "~w~ go get ~g~" + GramsToOz(jobs[0].amount) + "~w~ and deliver it to ~y~" + World.GetStreetName(jobs[0].Ped.Position) + ".", 10000);
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
                                        if (j.Ped.CurrentPedGroup != player.Character.CurrentPedGroup && j.Ped.Position.DistanceTo(playerPos) <= 10f)
                                        {
                                            j.Ped.Task.ClearAll();
                                            Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, j.Ped.Handle, playerGroup);
                                            j.Ped.CurrentBlip.ShowRoute = false;
                                        }
                                        if (!LsMenuPool.IsAnyMenuOpen() && j.Ped.Position.DistanceTo(playerPos) <= 2)
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
                                                if (player.Character.IsInVehicle() == true && j.Ped.IsInVehicle() == true && player.Character.CurrentVehicle.Speed < 1)
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
                                                        sell.Text = "Sell " + GramsToOz(j.amount) + " of " + j.Drug.Name + "?";
                                                        customerMainMenu.Subtitle.Caption = "You have " + GramsToOz(PlayerInventory[j.Drug.Name]) + " of " + j.Drug.Name;
                                                        customerMainMenu.Visible = !customerMainMenu.Visible;
                                                        customerMainMenu.CurrentSelection = 0;
                                                        break;
                                                    }
                                                }
                                                if (player.Character.IsInVehicle() == false && player.Character.IsStopped && !player.Character.IsInCombat)
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
                                                        sell.Text = "Sell " + GramsToOz(j.amount) + " of " + j.Drug.Name + "?";
                                                        customerMainMenu.Subtitle.Caption = "You have " + GramsToOz(PlayerInventory[j.Drug.Name]) + " of " + j.Drug.Name;
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
                                float distanceToPlayer = ped.Position.DistanceTo(playerPos);
                                if (ped != null && ped.IsAlive && distanceToPlayer < 30 && distanceToPlayer > 4)
                                {
                                    if (ped.Gender.Equals(Gender.Male) && areas[areaIndex].GangPresance > 0)
                                    {
                                        lastAngryTime = Game.GameTime;
                                        int anger = rnd.Next(0, 100);
                                        int percent = 97 - areas[areaIndex].GangPresance;
                                        if (playerHasBag) percent -= 5;
                                        if (anger >= (percent - areas[areaIndex].GangPresance))
                                        {
                                            if (DEBUG) { UI.Notify("Ped angry:" + anger + ">=" + (99 - areas[areaIndex].GangPresance)); }
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

                                            AddBlip(ped, BlipColor.Red, "Robber", false, false);

                                            ped.Armor = 100;
                                            Function.Call(Hash.SET_PED_COMBAT_ABILITY, ped, 100);
                                            ped.IsPersistent = true;
                                            ped.BlockPermanentEvents = true;
                                            ped.AlwaysKeepTask = true;
                                            ped.CurrentBlip.IsFriendly = false;
                                            ped.Task.FightAgainstHatedTargets(100);
                                            ped.Task.FightAgainst(player.Character);
                                            oldCustomers.Remove(ped);
                                            angryPeds.Add(ped);
                                            if (rnd.Next(0, 10) == 0 && areas[areaIndex].GangPresance > 3)
                                            {
                                                if (driveByHandler.AmountOfDriveBy() == 0)
                                                {
                                                    SpawnDriveBy();
                                                }
                                                if (DEBUG) { UI.Notify("Robber Drive-by"); }

                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //select ped for deal

                        if (customers.Count < 10 && jobs.Count == 0)
                        {
                            if (Game.GameTime > lastTimePedSelected + 1000)
                            {
                                lastTimePedSelected = Game.GameTime;
                                int getPed = rnd.Next(0, 100);
                                if (getPed > 60)
                                {
                                    if (areas[areaIndex].NeedCustomer())
                                    {

                                        List<Ped> peds = new List<Ped>();
                                        foreach (Ped p in lPeds.GetPeds)
                                        {
                                            if (!CheckIfPedUsed(p, true, true)) peds.Add(p);
                                        }
                                        if (peds.Count > 0)
                                        {
                                            areas[areaIndex].GetCustomer(peds);
                                            foreach (Drug drug in areas[areaIndex].Drugs)
                                            {
                                                bool chooseCustomer = true;
                                                //check to see if we are selling the drug
                                                switch (drug.Name)
                                                {
                                                    case "Weed":
                                                        if (!sellingWeed)
                                                        {
                                                            drug.Supplied = false;
                                                            chooseCustomer = false;
                                                            drug.SelectCustomer = false;
                                                            drug.Customer = null;
                                                        }
                                                        else
                                                        {
                                                            //drug.Supplied = true;
                                                        }
                                                        break;
                                                    case "Crack":
                                                        if (!sellingCrack)
                                                        {
                                                            drug.Supplied = false;
                                                            chooseCustomer = false;
                                                            drug.SelectCustomer = false;
                                                            drug.Customer = null;
                                                        }
                                                        else
                                                        {
                                                            //drug.Supplied = true;
                                                        }
                                                        break;
                                                    case "Cocaine":
                                                        if (!sellingCocaine)
                                                        {
                                                            drug.Supplied = false;
                                                            chooseCustomer = false;
                                                            drug.SelectCustomer = false;
                                                            drug.Customer = null;
                                                        }
                                                        else
                                                        {
                                                            //drug.Supplied = true;
                                                        }
                                                        break;
                                                }
                                                // check to see if the drug is in demand
                                                if (chooseCustomer && drug.SelectCustomer)
                                                {
                                                    Ped p = drug.Customer;
                                                    drug.Customer = null;
                                                    drug.SelectCustomer = false;
                                                    if (p != null)
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
                                foreach (Customer c in customers.ToList())
                                {
                                    if (c != null)
                                    {
                                        if (c.Ped.Exists() && c.Ped.IsAlive && !c.Ped.IsPlayer)
                                        {
                                            if (c.Ped.Position.DistanceTo(playerPos) > 80f)
                                            {                                                
                                                customers.Remove(c);
                                                RemovePedFromWorld(c.Ped, true);
                                                break;
                                            }
                                            if (c.Ped.CurrentPedGroup != player.Character.CurrentPedGroup)
                                            {
                                                if (c.Ped.Position.DistanceTo(playerPos) < 3f)
                                                {
                                                    c.Ped.Task.ClearAll();
                                                    Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, c.Ped.Handle, playerGroup);
                                                    Function.Call(Hash._PLAY_AMBIENT_SPEECH1, c.Ped, "GENERIC_HI", "SPEECH_PARAMS_STANDARD");
                                                }
                                                else
                                                {
                                                    if (c.Ped.IsInVehicle())
                                                    {
                                                        if (c.Target != World.GetNextPositionOnStreet(playerPos))
                                                        {
                                                            c.Target = World.GetNextPositionOnStreet(playerPos);
                                                            c.Ped.Task.PerformSequence(PerformTaskSeq(2, c.Ped.CurrentVehicle, c.Target));
                                                        }

                                                        lastCustomerCheckTimer = Game.GameTime;
                                                    }
                                                    else
                                                    {

                                                        Vector3 playerForward = player.Character.ForwardVector;
                                                        Vector3 playerRight = player.Character.RightVector;
                                                        if (player.Character.IsInVehicle())
                                                        {
                                                            MoveCustomer(c, playerPos + (playerRight * 2));
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
                                        else
                                        {
                                            customers.Remove(c);
                                            RemovePedFromWorld(c.Ped, true);
                                        }
                                    }
                                }
                            }
                            Customer nCustomer = null;                                                       
                            if (player.Character.IsInVehicle())
                            {
                                nCustomer = FindClosestCustomerInVehicle();
                            }
                            else
                            {
                                nCustomer = FindClosestCustomer();
                            }
                            if (nCustomer != null && !LsMenuPool.IsAnyMenuOpen())
                            {
                                if (player.Character.IsInVehicle() == true && nCustomer.Ped.IsInVehicle() == true && player.Character.CurrentVehicle.Speed < 1)
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
                                        sell.Text = "Sell " + GramsToOz(customerSellTo.amount) + " of " + customerSellTo.Drug.Name + "?";
                                        customerMainMenu.Subtitle.Caption = "You have " + GramsToOz(PlayerInventory[customerSellTo.Drug.Name]) + " of " + customerSellTo.Drug.Name;
                                        customerMainMenu.Visible = !customerMainMenu.Visible;
                                        customerMainMenu.CurrentSelection = 0;
                                    }
                                }
                                if (player.Character.IsInVehicle() == false && player.Character.IsStopped && !player.Character.IsInCombat)
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
                                        sell.Text = "Sell " + GramsToOz(customerSellTo.amount) + " of " + customerSellTo.Drug.Name + "?";
                                        customerMainMenu.Subtitle.Caption = "You have " + GramsToOz(PlayerInventory[customerSellTo.Drug.Name]) + " of " + customerSellTo.Drug.Name;
                                        customerMainMenu.Visible = !customerMainMenu.Visible;
                                        customerMainMenu.CurrentSelection = 0;
                                    }
                                }
                            }
                        }

                    }
                    if (wepDealer != null)
                    {

                        if (!player.Character.IsInVehicle())
                        {
                            if (wepDealer.vehicle.Position.DistanceTo(playerPos) < 3)
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
                                    player.Character.Task.EnterVehicle(wepDealer.vehicle, VehicleSeat.Passenger);
                                }
                            }
                        }
                        else
                        if (player.Character.IsInVehicle())
                        {
                            if (player.Character.CurrentVehicle.Handle == wepDealer.vehicle.Handle)
                            {
                                if (Game.IsControlJustPressed(2, GTA.Control.ContextSecondary))
                                {
                                    player.Character.Task.LeaveVehicle(player.Character.CurrentVehicle, true);
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
                #endregion
            }
        }

        private Customer FindClosestCustomer()
        {
            float nDistance = 100f;
            Customer cCustomer = null;
            foreach (Customer c in customers.Where(r => r.Ped.CurrentPedGroup == playerGroup))
            {
                float cDistance = c.Ped.Position.DistanceTo(playerPos+player.Character.ForwardVector);
                if (cDistance < 3 && cDistance < nDistance)
                {
                    nDistance = cDistance;
                    cCustomer = c;
                }
            }
            return cCustomer;
        }

        private Customer FindClosestCustomerInVehicle()
        {
            float nDistance = 100f;
            Customer cCustomer = null;
            foreach(Customer c in customers.Where(r => r.Ped.CurrentPedGroup == playerGroup && r.Ped.CurrentVehicle == player.Character.CurrentVehicle))
            {
                float cDistance = c.Ped.Position.DistanceTo(playerPos);
                if(cDistance < nDistance)
                {
                    nDistance = cDistance;
                    cCustomer = c;
                }
            }
            return cCustomer;
        }

        void InventoryGui()
        {

            Vector2 tPos = World3DToScreen2d(new Vector3(player.Character.GetBoneCoord(Bone.FB_Brow_Centre_000).X, player.Character.GetBoneCoord(Bone.FB_Brow_Centre_000).Y, player.Character.GetBoneCoord(Bone.FB_Brow_Centre_000).Z + 0.5f));

            //Size size = Game.ScreenResolution;
            tPos.X = (1280 * tPos.X) - 30;
            tPos.Y = (720 * tPos.Y) + 60;
            Point UIpos = new Point(Game.ScreenResolution.Width - 20, (int)(Game.ScreenResolution.Height * 0.1));                                    
            UIResText UIWeedtxt = new UIResText("-Weed-", UIpos, 0.4f, Color.WhiteSmoke, GTA.Font.Pricedown, UIResText.Alignment.Right);
            UIWeedtxt.DropShadow = true;
            UIWeedtxt.Shadow = true;
            UIWeedtxt.Outline = true;
            UIResText UIWeed = new UIResText("", new Point(UIpos.X, UIpos.Y + 20), 0.50f, Color.WhiteSmoke, GTA.Font.Pricedown, UIResText.Alignment.Right);
            //UICont.Items.Add(UIWeed);
            UIWeed.Enabled = true;
            UIWeed.DropShadow = true;
            UIWeed.Shadow = true;
            UIWeed.Outline = true;
            UIResText UIweed = new UIResText("", new Point(UIpos.X, UIpos.Y + 45), 0.5f, Color.WhiteSmoke, GTA.Font.Pricedown, UIResText.Alignment.Right);
            //UICont.Items.Add(UIweed);
            UIweed.Enabled = true;
            UIweed.DropShadow = true;
            UIweed.Shadow = true;
            UIweed.Outline = true;


            UIResText UICracktxt = new UIResText("-Crack-", new Point(UIpos.X, UIpos.Y + 80), 0.4f, Color.WhiteSmoke, GTA.Font.Pricedown, UIResText.Alignment.Right);
            UICracktxt.DropShadow = true;
            UICracktxt.Shadow = true;
            UICracktxt.Outline = true;
            UIResText UICrack = new UIResText("", new Point(UIpos.X, UIpos.Y + 80 + 20), 0.5f, Color.WhiteSmoke, GTA.Font.Pricedown, UIResText.Alignment.Right);
            //UICont.Items.Add(UICrack);
            UICrack.Enabled = true;
            UICrack.DropShadow = true;
            UICrack.Shadow = true;
            UICrack.Outline = true;
            UIResText UIcrack = new UIResText("", new Point(UIpos.X, UIpos.Y + 100 + 25), 0.5f, Color.WhiteSmoke, GTA.Font.Pricedown, UIResText.Alignment.Right);
            //UICont.Items.Add(UIcrack);
            UIcrack.Enabled = true;
            UIcrack.DropShadow = true;
            UIcrack.Shadow = true;
            UIcrack.Outline = true;

            UIResText UICocainetxt = new UIResText("-Coke-", new Point(UIpos.X, UIpos.Y + 160), 0.4f, Color.WhiteSmoke, GTA.Font.Pricedown, UIResText.Alignment.Right);
            UICocainetxt.DropShadow = true;
            UICocainetxt.Shadow = true;
            UICocainetxt.Outline = true;
            UIResText UICocaine = new UIResText("", new Point(UIpos.X, UIpos.Y + 160 + 20), 0.5f, Color.WhiteSmoke, GTA.Font.Pricedown, UIResText.Alignment.Right);
            //UICont.Items.Add(UICocaine);
            UICocaine.Enabled = true;
            UICocaine.DropShadow = true;
            UICocaine.Shadow = true;
            UICocaine.Outline = true;
            UIResText UIcocaine = new UIResText("", new Point(UIpos.X, UIpos.Y + 160 + 20 + 25), 0.5f, Color.WhiteSmoke, GTA.Font.Pricedown, UIResText.Alignment.Right);
            //UICont.Items.Add(UIcocaine);
            UIcocaine.Enabled = true;
            UIcocaine.DropShadow = true;
            UIcocaine.Shadow = true;
            UIcocaine.Outline = true;

            UIWeed.Caption = "~r~" + pWeedOunce + "~s~z";
            UIweed.Caption = "" + GramsToOz(PlayerInventory["Weed"]);

            UICrack.Caption = "~r~" + pCrackOunce + "~s~z";
            UIcrack.Caption = "" + GramsToOz(PlayerInventory["Crack"]);

            UICocaine.Caption = "~r~" + pCocaineOunce + "~s~z";
            UIcocaine.Caption = "" + GramsToOz(PlayerInventory["Cocaine"]);
            //UICont.Draw();
            UIWeedtxt.Draw();
            UIWeed.Draw();
            UIweed.Draw();
            UICracktxt.Draw();
            UICrack.Draw();
            UIcrack.Draw();
            UICocainetxt.Draw();
            UICocaine.Draw();
            UIcocaine.Draw();
            UI.ShowHudComponentThisFrame(HudComponent.Cash);
        }

        void areaInfoUI()
        {
            Point point = new Point(10, 10);
            Size size = new Size(80, 174);
            UIContainer cont = new UIContainer(point, size, Color.FromArgb(100, Color.Black));
            UIText aHeader = new UIText("--AREA--", new Point(2, 2), 0.2f);
            UIText name = new UIText("name:" + areas[areaIndex].Name, new Point(2, 12), 0.2f);
            UIText type = new UIText("type:" + areas[areaIndex].AreaType, new Point(2, 22), 0.2f);
            UIText cop = new UIText("cStr:" + areas[areaIndex].CopPresance, new Point(2, 32), 0.2f);
            UIText gang = new UIText("gStr:" + areas[areaIndex].GangPresance, new Point(2, 42), 0.2f);
            UIText heat = new UIText("heat:" + areas[areaIndex].Heat, new Point(2, 52), 0.2f);
            UIText dealers = new UIText("nDealer:" + areas[areaIndex].Dealers, new Point(2, 62), 0.2f);
            UIText demandMet = new UIText("dMet:" + areas[areaIndex].DemandMet(), new Point(2, 72), 0.2f);

            UIText drug0H = new UIText("--"+areas[areaIndex].Drugs[0].Name+"--", new Point(2, 82), 0.2f);
            UIText drug0D = new UIText("Dem:"+areas[areaIndex].Drugs[0].Demand, new Point(2, 92), 0.2f);
            UIText drug0S = new UIText("Sup:"+ areas[areaIndex].Drugs[0].Supplied, new Point(2, 102), 0.2f);

            UIText drug1H = new UIText("--" + areas[areaIndex].Drugs[1].Name + "--", new Point(2, 112), 0.2f);
            UIText drug1D = new UIText("Dem:" + areas[areaIndex].Drugs[1].Demand, new Point(2, 122), 0.2f);
            UIText drug1S = new UIText("Sup:" + areas[areaIndex].Drugs[1].Supplied, new Point(2, 132), 0.2f);

            UIText drug2H = new UIText("--" + areas[areaIndex].Drugs[2].Name + "--", new Point(2, 142), 0.2f);
            UIText drug2D = new UIText("Dem:" + areas[areaIndex].Drugs[2].Demand, new Point(2, 152), 0.2f);
            UIText drug2S = new UIText("Sup:" + areas[areaIndex].Drugs[2].Supplied, new Point(2, 162), 0.2f);

            cont.Items.Add(aHeader);
            cont.Items.Add(name);
            cont.Items.Add(type);
            cont.Items.Add(cop);
            cont.Items.Add(gang);
            cont.Items.Add(heat);
            cont.Items.Add(dealers);
            cont.Items.Add(demandMet);

            cont.Items.Add(drug0H);
            cont.Items.Add(drug0D);
            cont.Items.Add(drug0S);

            cont.Items.Add(drug1H);
            cont.Items.Add(drug1D);
            cont.Items.Add(drug1S);

            cont.Items.Add(drug2H);
            cont.Items.Add(drug2D);
            cont.Items.Add(drug2S);

            cont.Draw();
        }





        private static void HandlePigs()
        {
            if (pigs.Count > 0)
            {
                foreach (Pig pig in pigs.ToList())
                {
                    if (pig.Ped != null && pig.Ped.Exists())
                    {
                        if (!pig.Ped.IsAlive || pig.Ped.Position.DistanceTo(playerPos) > 1050f || ratChance == -100)
                        {
                            pig.DeletePig();
                            if (DEBUG) { UI.Notify("Removed spawned Cop"); }
                        }
                        pig.DoState();
                        pig.CheckState();
                    }
                    else
                    {
                        pigs.Remove(pig);
                    }
                }
            }
        }

        private static void SpawnPigs()
        {

            Vehicle pigCar = World.CreateVehicle(GetPoliceVehType(), GenerateSpawnPos(playerPos.Around(200), Nodetype.Road, false));
            if (pigCar != null)
            {
                if (DEBUG)
                {
                    UI.Notify("COP SPAWNED");
                    pigCar.AddBlip();
                    pigCar.CurrentBlip.Name = "Police Car";
                }
                Function.Call(Hash.SET_VEHICLE_SIREN, pigCar.Handle, true);
                pigs.Add(new Pig(pigCar.CreatePedOnSeat(VehicleSeat.Driver, GetPolicePedType()), true));
                pigs.Add(new Pig(pigCar.CreatePedOnSeat(VehicleSeat.Passenger, GetPolicePedType()), false));
            }
            if (pigs.Count > 0)
            {
                foreach (Pig p in pigs)
                {
                    if (p.Ped != null)
                    {
                        Function.Call(Hash.SET_PED_AS_COP, p.Ped, true);
                        if (areas[areaIndex].CopPresance < 10) { areas[areaIndex].CopPresance++; }
                    }
                }
            }
        }

        void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == SetDrugCar && player.Character.IsAlive)
            {
                if (player.Character.IsInVehicle())
                {
                    if (pStashVehicle != null && pStashVehicle.drugCar != null)
                    {
                        SendTextMsg("" + pStashVehicle.drugCar.DisplayName + " is no longer a stash Vehicle.");
                        pStashVehicle.RemoveOldCar();

                        pStashVehicle = null;
                    }
                    else
                    {
                        if (player.Character.CurrentVehicle.Model.IsCar)
                        {
                            pStashVehicle = new StashVehicle();
                            pStashVehicle.VehicleSave(player.Character.CurrentVehicle);
                            if (pStashVehicle.drugCar != null)
                            {
                                SendTextMsg("" + pStashVehicle.drugCar.DisplayName + " is now youre Stash Vehicle.");
                            }
                        }
                        else
                        {
                            SendTextMsg("You can not use this vehicle as a stash.");
                        }
                    }
                }
                else
                {
                    SendTextMsg("Please ener a vehicle before trying to set a stash vehicle.");
                }
            }
            if (e.KeyCode == StopDealing && player.Character.IsAlive)
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
                //Game.Player.Character.Task.PlayAnimation("anim@heists@keypad@", "enter", 8.0f, 3000, AnimationFlags.None);
                //Function.Call(Hash.REQUEST_ANIM_DICT, "anim@heists@keypad");
                //while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, "anim@heists@keypad"))
                //{
                //    Wait(100);
                //}
                //Function.Call(Hash.TASK_PLAY_ANIM, Game.Player.Character, "anim@heists@keypad", "enter", 8.0f, 1.0f, -1, 49, .1f, 0, 0, 0);



                if (DEBUG)
                {
                    if (player.Character.IsInVehicle())
                    {
                        PlayTheSound();
                        Vector3 vPos = player.Character.CurrentVehicle.Position;
                        float vHead = player.Character.CurrentVehicle.Heading;
                        XDocument spawnDox = XDocument.Load("scripts\\LSLife\\LSLife_Spawns.xml");
                        XElement ePos = new XElement("pos");
                        ePos.Add(new XElement("x", vPos.X));
                        ePos.Add(new XElement("y", vPos.Y));
                        ePos.Add(new XElement("z", vPos.Z));
                        ePos.Add(new XElement("h", vHead));
                        spawnDox.Root.Add(ePos);
                        spawnDox.Save("scripts\\LSLife\\LSLife_Spawns.xml");
                        UI.Notify("position added to xml");
                    }
                    else
                    {
                        UI.Notify("you must be in a vehicle to add a position");
                    }
                    //UI.Notify("Robber Drive-by");
                    //if (driveByHandler.AmountOfDriveBy() == 0) driveByHandler.AddDriveBy(VehicleClass.SUVs, rnd.Next(0, 4), player.Character);
                    //GenerateDealerSpawn();
                    //GetPedVariationData(Game.Player.Character);

                    //int interior = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, player.Character);

                    //Vector3 stashPos = new Vector3(-112.616f, -1479.229f, 30f);
                    //string dName0 = areas[areaIndex].Drugs[0].Name;
                    //float dDemand0 = areas[areaIndex].Drugs[0].Demand;
                    //string dName1 = areas[areaIndex].Drugs[1].Name;
                    //float dDemand1 = areas[areaIndex].Drugs[1].Demand;
                    //string dName2 = areas[areaIndex].Drugs[2].Name;
                    //float dDemand2 = areas[areaIndex].Drugs[2].Demand;
                    ////UI.Notify(dName0+": "+dDemand0 +" "+ dName1 + ": " + dDemand1 +" "+ dName2 + ": " + dDemand2);
                    //UI.Notify("intHash:" + interior + " isNotallowed:"+IsInInterior(player.Character));


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
                if (DEBUG)
                {
                    //if (!enemyHandler.Started) enemyHandler.Start();
                    //else enemyHandler.Stop();
                    if (!player.Character.IsInVehicle())
                    {
                        //Prison.Started = true;
                    }
                    //else UI.Notify("Please Exit Vehicle before testing this feature");

                }
            }
            if (e.KeyCode == MenuKey)
            {

                if (player != null && player.IsAlive)
                {
                    if (DEBUG)
                    {

                        int oz = TotalDrugsCarried / 28;
                        int g = TotalDrugsCarried - (oz * 28);
                        string s = oz + "oz " + g + "g";
                        //UI.Notify("AreaLabel:" + World.GetZoneNameLabel(playerPos) + " Type" + areas[areaIndex].AreaType);
                        if (player.Character != null)
                        {
                            string toPaste = playerPos.X + "f, " + playerPos.Y + "f, " + playerPos.Z + "f";
                            Thread thread = new Thread(() => Clipboard.SetText(toPaste));
                            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                            thread.Start();
                            thread.Join();
                            thread.Abort();
                            //UI.Notify(Game.Player.Character.CurrentVehicle.Position.ToString());
                        }
                    }
                    showInventory = !showInventory;
                    Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
                    //SendTextMsg("Weed " + GramsToOz(PlayerInventory["Weed"]) + "~n~Crack " + GramsToOz(PlayerInventory["Crack"]) + "~n~Coke " + GramsToOz(PlayerInventory["Cocaine"]));
                }
            }
        }




        public static void callDealer(bool wepdealer)
        {
            if (wepdealer)
            {
                Vector3 dealer1pos;
                dealer1pos = World.GetNextPositionOnSidewalk(GenerateSpawnPos(playerPos.Around(60f), Nodetype.AnyRoad, true));
                weaponDealer1 = World.CreatePed(PedHash.ArmBoss01GMM, GenerateSpawnPos(dealer1pos, Nodetype.AnyRoad, true));
                while (weaponDealer1 == null)
                {
                    Wait(0);
                }
                pedTarget = playerPos;
                if (weaponDealer1 != null)
                {
                    weaponDealer1.RelationshipGroup = playersTeam;

                    AddBlip(weaponDealer1, BlipColor.Blue, "Zee's Dealer", true, true);

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
                if (ZeesDealers == null && NewOrder != null)
                {                      
                    ZeesDealers = new DrugDealer(NewOrder);
                    if (ZeesDealers != null)
                    {
                        PlaceOrder.Text = "Cancel Order";
                    }
                }
                else
                {
                    SendTextMsg("I'll let them know your not coming.");
                    ZeesDealers.Dismiss();
                    if (NewOrder != null)
                    {
                        NewOrder.PutBackDrugs();
                        NewOrder.ResetOrder();
                    }                    
                    LsMenuPool.CloseAllMenus();
                }
            }
        }




        public static TaskSequence PerformTaskSeq(int index, Vehicle vehicle, Vector3 target)
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
                walkToVehicleDrive.AddTask.GoTo(vehicle.Position + ((vehicle.RightVector * 2) * -1));
                walkToVehicleDrive.AddTask.EnterVehicle(vehicle, VehicleSeat.Driver);
                walkToVehicleDrive.AddTask.CruiseWithVehicle(vehicle, 20f, (int)DrivingStyle.SometimesOvertakeTraffic);
                //walkToVehicleDrive.AddTask.ClearAll();
                return walkToVehicleDrive;
            }
            if (index == 2)
            {

                Vector3 playerForward = player.Character.ForwardVector;
                Vector3 playerRight = player.Character.RightVector;
                TaskSequence driveToPlayerAndExit = new TaskSequence();
                driveToPlayerAndExit.AddTask.ClearAllImmediately();
                driveToPlayerAndExit.AddTask.DriveTo(vehicle, target, 10f, 20f, (int)DrivingStyle.SometimesOvertakeTraffic);
                if (player.Character.IsInVehicle())
                {
                    driveToPlayerAndExit.AddTask.GoTo(playerPos + (playerRight * 3));
                }
                else
                {
                    driveToPlayerAndExit.AddTask.GoTo(playerPos + (playerForward * 2));
                }

                return driveToPlayerAndExit;
            }
            else
            {
                return null;
            }
        }

        public static bool CheckIfPedUsed(Ped _ped, bool _allowPolice, bool _allowVehicle)
        {
            if (!_ped.Exists()) return true;
            if (_ped.IsPlayer) return true;
            if (!_allowVehicle && _ped.IsInVehicle()) return true;
            if (_ped.Alpha == 0) return true;
            if (!_ped.IsHuman) return true;
            if (_ped.IsDead) return true;
            if (!_ped.IsAlive) return true;
            if (IsInInterior(_ped)) return true;
            if (IsUnderGround(_ped)) return true;
            if (_ped.IsInAir) return true;
            if (!_ped.IsInVehicle() && _ped.Position.Z > World.GetGroundHeight(_ped.Position + (_ped.ForwardVector * 2)) + 2f) return true;
            if (player.Character.IsInVehicle() && _ped.IsInVehicle() && _ped.CurrentVehicle == player.Character.CurrentVehicle) return true;
            if (Game.Player.Character.CurrentPedGroup == _ped.CurrentPedGroup) return true;
            if (drugDealer1 != null && drugDealer1 == _ped) return true;
            if (weaponDealer1 != null && weaponDealer1 == _ped) return true;
            if (wepDealer != null && wepDealer.ped == _ped) return true;
            if (customers.Count > 0 && customers.FindAll(r => r.Ped == _ped).Count != 0) return true;
            if (oldCustomers.Count > 0 && oldCustomers.FindAll(r => r == _ped).Count != 0) return true;
            if (angryPeds.Count > 0 && angryPeds.FindAll(r => r == _ped).Count != 0) return true;
            if (pigs.Count > 0 && pigs.FindAll(r => r.Ped == _ped).Count != 0) return true;
            if (jobs.Count > 0 && jobs.FindAll(r => r.Ped == _ped).Count != 0) return true;
            if (aDealer != null)
            {
                if (aDealer.dPed != null && aDealer.dPed == _ped) return true;
                if (aDealer.cPed != null && aDealer.cPed == _ped) return true;
                if (aDealer.LastCustomer == _ped.Handle) return true;
            }
            if (ZeesDealers != null)
            {
                if (ZeesDealers.lieut == _ped || ZeesDealers.goon == _ped) return true;
            }
            if (attacker != null && _ped == attacker) return true;
            if (DealerHandler.dealers.Count > 0)
            {
                foreach (PlayerDealer d in DealerHandler.dealers)
                {
                    if (d != null)
                    {
                        if (d.Ped != null && d.Ped == _ped) return true;
                        if (d.cPed != null && d.cPed == _ped) return true;
                        if (d.lPed == _ped) return true;
                    }
                }
            }
            if (driveByHandler.GetDriveBies.Count > 0)
            {
                foreach (DriveBy d in driveByHandler.GetDriveBies)
                {
                    if (d.Peds.Contains(_ped)) return true;
                }
            }
            if (!Function.Call<bool>(Hash.IS_ENTITY_VISIBLE, _ped)) return true;
            int type = Function.Call<int>(Hash.GET_PED_TYPE, _ped.Handle);
            if (type == (int)PedType.PED_TYPE_PROSTITUTE || type == (int)PedType.PED_TYPE_MISSION) return true;
            if (!_allowPolice)
            {
                if (IsPolice(_ped)) return true;
            }
            return false;
        }

        public void HandleAreaDealers()
        {
            if (aDealer != null)
            {
                if (aDealer.NewState == DStates.TalkToPlayer && aDealer.CurrentState == DStates.TalkToPlayer && player.Character.IsStopped && aDealer.dPed.Position.DistanceTo(playerPos) < 2)
                {
                    aDealer.PromtPlayerToTalk();
                }
            }
        }

        public static void MoveCustomer(Customer c, Vector3 target)
        {
            if (c.Target != target)
            {
                c.Target = target;
            }
            if (c.Target == target)
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

        static float MaxSpeed()
        {
            return 0;
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

                if (this.Locked)
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
                while (!Function.Call<bool>(Hash._GET_IS_SLOW_ROAD_FLAG, NodeID))
                {
                    NodeID = Function.Call<int>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID, desiredPos.X, desiredPos.Y, desiredPos.Z, NodeNumber, type, 200f, 200f);
                    NodeNumber++;
                }
            }

            Function.Call(Hash.GET_VEHICLE_NODE_POSITION, NodeID, outArgA);
            finalpos = outArgA.GetResult<Vector3>();

            if (sidewalk) finalpos = World.GetNextPositionOnSidewalk(finalpos);
            return finalpos;

        }

        public Vector3 ClosetNodePos(Vector3 pos)
        {
            Vector3 finalPos = Vector3.Zero;
            int NodeID = Function.Call<int>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID, pos.X, pos.Y, pos.Z, 1, 1, 200f, 200f);
            OutputArgument outArgA = new OutputArgument();
            Function.Call(Hash.GET_VEHICLE_NODE_POSITION, NodeID, outArgA);
            finalPos = outArgA.GetResult<Vector3>();
            return finalPos;
        }

        int PedsInPlayersGroup()
        {
            return player.Character.CurrentPedGroup.ToList(false).Count;
        }

        public enum PathnodeFlags
        {
            Slow = 1,
            Two = 2,
            Intersection = 4,
            Eight = 8, SlowTraffic = 12, ThirtyTwo = 32, Freeway = 64, FourWayIntersection = 128, BigIntersectionLeft = 512
        }
        public static List<PathnodeFlags> GetRoadFlags(Vector3 pos)
        {
            List<PathnodeFlags> pathnodesflags = new List<PathnodeFlags>();
            OutputArgument outArgA = new OutputArgument();
            OutputArgument outArgB = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_VEHICLE_NODE_PROPERTIES, pos.X, pos.Y, pos.Z, outArgA, outArgB))
            {
                int busy = outArgA.GetResult<int>();
                int flags = outArgB.GetResult<int>();

                foreach (int flag in Enum.GetValues(typeof(PathnodeFlags)).Cast<PathnodeFlags>())
                {
                    //if ((flag & flags) != 0) d += " " + flag;
                    if ((flag & flags) != 0) pathnodesflags.Add((PathnodeFlags)flag);
                }
            }
            return pathnodesflags;
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

        private void OnAbort(object sender, EventArgs e)
        {
            var A_0 = true;
            if (A_0)
            {
                
                if (HouseHandler?.Houses.Count > 0)
                {
                    foreach (StashHouse house in HouseHandler.Houses)
                    {
                        house?.RemoveBlip();
                    }
                }
                if (drugDealer1 != null)
                {
                    drugDealer1.CurrentBlip?.Remove();
                    drugDealer1.Delete();
                }
                if (weaponDealer1 != null)
                {
                    weaponDealer1.CurrentBlip?.Remove();
                    weaponDealer1.Delete();
                }
                if (customers.Count >= 1)
                {
                    foreach (Customer c in customers.ToList())
                    {
                        if (c != null)
                        {
                            Ped p = c.Ped;
                            p.CurrentBlip?.Remove();
                            p.Delete();
                        }
                    }
                }
                if (pStashVehicle != null && pStashVehicle.drugCar != null)
                {
                    if (pStashVehicle.drugCar != null)
                    {
                        if (pStashVehicle.drugCar.Exists())
                        {
                            pStashVehicle.drugCar.CurrentBlip.Remove();
                            pStashVehicle.drugCar.Delete();
                        }
                    }
                    pStashVehicle.drugCarBlip?.Remove();
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
                if (aDealer != null)
                {
                    if (aDealer.dPed != null)
                    {
                        aDealer.dPed.CurrentBlip?.Remove();
                        aDealer.dPed.Delete();
                    }
                    if (aDealer.cPed != null)
                    {
                        aDealer.cPed.CurrentBlip?.Remove();
                        aDealer.cPed.Delete();
                    }
                }
                if (DealerHandler.dealers.Count > 0)
                {
                    foreach (PlayerDealer pd in DealerHandler.dealers)
                    {
                        if (pd.Ped != null)
                        {
                            NotImmuneToPlayer(pd.Ped);
                            pd.cPed?.CurrentBlip?.Remove();
                            pd.cPed?.Delete();
                            pd.Ped?.CurrentBlip?.Remove();
                            pd.Ped.Delete();
                        }
                        else pd.RemoveLocationBlip();
                    }
                }
                World.RemoveRelationshipGroup(playersTeam);
                World.RemoveRelationshipGroup(rival_nutral);
                World.RemoveRelationshipGroup(rival_enemy);
                World.RemoveRelationshipGroup(CustomerRel);

                driveByHandler.CleanDriveBys();
                if (pigs.Count > 0)
                {
                    foreach (Pig p in pigs)
                    {
                        p.Ped?.CurrentBlip?.Remove();
                        p.Ped?.Delete();
                        p.Vehicle?.CurrentBlip?.Remove();
                        p.Vehicle.Delete();
                    }
                }
                pickupHandler.RemoveBags();
            }
        }
        #endregion
    }
}
