using GTA;
using GTA.Math;
using GTA.Native;
using iFruitAddon2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static GTA.Script;
using static LSlife.Enums;
using static LSlife.LSL;

namespace LSlife
{
    static class LsFunctions
    {
        
        public const int pMaxDrugs = 7168;
        public static int pWeedOunce = 0;
        public static int pCrackOunce = 0;
        public static int pCocaineOunce = 0;

        public static List<DealerSpawnPostion> spawnPostions = new List<DealerSpawnPostion>();

        public static List<Drug> LsDrugs()
        {
            List<Drug> Drugs = new List<Drug>();
            if(hardMode)
            {
                Drugs.Add(new Drug("Weed", 10, AreaType.Normal));
                Drugs.Add(new Drug("Crack", 20, AreaType.Poor));
                Drugs.Add(new Drug("Cocaine", 100, AreaType.Rich));
            }
            else
            {
                Drugs.Add(new Drug("Weed", 50, AreaType.Normal));
                Drugs.Add(new Drug("Crack", 200, AreaType.Poor));
                Drugs.Add(new Drug("Cocaine", 400, AreaType.Rich));
            }
            return Drugs;
            
        }

        public static void LoadSpawnLocations()
        {
            XDocument XMLSpawnPositions;
            XMLSpawnPositions = XDocument.Load("scripts\\LSLife\\LSLife_Spawns.xml");
            foreach (XElement element in XMLSpawnPositions.Descendants())
            {
                if (element.Name == "pos")
                {
                    Vector3 position = Vector3.Zero;
                    float heading = 0f;
                    foreach (XElement ele in element.Descendants())
                    {
                        if (ele.Name == "x") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out position.X);
                        if (ele.Name == "y") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out position.Y);
                        if (ele.Name == "z") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out position.Z);
                        if (ele.Name == "h") float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out heading);
                    }
                    if (position != Vector3.Zero) spawnPostions.Add(new DealerSpawnPostion(position, heading));
                }
            }
        }


        public static int zeeDebtCap()
        {
            int initCap = 100000;
            if (hardMode) initCap = 1000;
            return initCap + (int)dealer1Rep;
        }

        public static void SaveDealer()
        {
            LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
            foreach (XElement element in LSLifeSave.Descendants())
            {
                if (element.Name == "dealer1")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weed")
                        {
                            e.Value = LSL.dealer1Weed.ToString();
                        }
                        if (e.Name == "crack")
                        {
                            e.Value = LSL.dealer1Crack.ToString();
                        }
                        if (e.Name == "cocain")
                        {
                            e.Value = LSL.dealer1Cocain.ToString();
                        }
                        if (e.Name == "debt")
                        {
                            e.Value = LSL.dealer1Debt.ToString();
                        }
                        if (e.Name == "money")
                        {
                            e.Value = LSL.dealer1Money.ToString();
                        }
                        if (e.Name == "rep")
                        {
                            e.Value = LSL.dealer1Rep.ToString();
                        }
                    }
                }
            }
            LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
        }

        public static bool IsPlayerArmoursMaxed()
        {

            if (Game.Player.Character.Armor >= 100 && playerArmours == 5)
            {
                return true;
            }
            else
                return false;
        }

        public static int DealerDelay()
        {
            return 60000 * (10 - areas[areaIndex].GangPresance);
        }

        public static void GiveArmourToPlayer()
        {
            if (Game.Player.Character.Armor < 100)
            {
                Game.Player.Character.Armor = 200;
            }
            else if (playerArmours < 5)
            {
                playerArmours += 1;
            }
        }

        public static int AmountOfPedsForDriveBy(int _gangStr)
        {
            if (_gangStr <= 1) return 1;
            else if (_gangStr <= 4) return 2;
            else if (_gangStr <= 7) return 3;
            else return 4;
        }


        public static int RandomInt(int start, int end)
        {
            Random r = new Random();
            return r.Next(start, end);
        }

        public static string OrderMenuDesc()
        {
            string mWeed =    "Weed:~g~" + NewOrder.weed + "~s~oz";
            string mCrack = "\nCrack:~g~" + NewOrder.crack + "~s~oz";
            string mCoke =  "\nCoke:~g~" + NewOrder.cocaine + "~s~oz";
            string mBill =  "\nBill:$" + IntToMoney(NewOrder.Bill);
            string mDebt =  "\nDebt:$" + IntToMoney(dealer1Debt);
            string mLimit = "\nCredit:$" + IntToMoney(zeeDebtCap());
            return mWeed + mCrack + mCoke + mBill + mDebt + mLimit;
        }



        public static void UpdateOrderMenuDesc()
        {
            string desc = OrderMenuDesc();
            PlaceOrder.Description = desc;
            bushWeed.Description = desc;
            crack.Description = desc;
            cocaine.Description = desc;
            WeedBuy.Description = desc;
            CrackBuy.Description = desc;
            CocainBuy.Description = desc;
            LsMenuPool.FormatDescriptions = true;
            LsMenuPool.RefreshIndex();
        }

        public static bool CanSee(Ped _ped, Ped _targetPed)
        {
            return Function.Call<bool>(Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY_IN_FRONT, _ped, _targetPed);
        }

        public static bool CanHearPlayer(Ped _ped)
        {
            return Function.Call<bool>(Hash.CAN_PED_HEAR_PLAYER, Game.Player.Character, _ped);
        }

        public static bool IsPolice(Ped p)
        {
            bool cop = false;
            foreach (LawEnforcePed lawEnforce in Enum.GetValues(typeof(LawEnforcePed)))
            {
                if (Function.Call<int>(Hash.GET_PED_TYPE, p.Handle) == (int)lawEnforce)
                {
                    cop = true;
                    break;
                }
            }
            return cop;
        }


        public static Vector3 RoadSideFromSpawn(Vector3 spawnPos, Vector3 directionPos)
        {
            Vector3 directionFromSpawn = directionPos - spawnPos;
            directionFromSpawn = directionFromSpawn.Normalized;
            Vector3 newSpawnPos = spawnPos + directionFromSpawn;
            Vector3 oldSpawnPos = newSpawnPos;
            Vector3 finalSpawnPos = Vector3.Zero;
            for (int i = 0; i < 16; i++)
            {
                newSpawnPos.Z = World.GetGroundHeight(newSpawnPos);
                if (Function.Call<bool>(Hash.IS_POINT_ON_ROAD, newSpawnPos.X, newSpawnPos.Y, newSpawnPos.Z) || newSpawnPos.DistanceTo(spawnPos) < newSpawnPos.DistanceTo(directionPos))
                {
                    oldSpawnPos = newSpawnPos;
                    newSpawnPos = newSpawnPos + (directionFromSpawn * 0.5f);
                }
                else
                {
                    finalSpawnPos = oldSpawnPos;
                    finalSpawnPos.Z = World.GetGroundHeight(finalSpawnPos);
                    return finalSpawnPos;
                }
            }
            return finalSpawnPos;
        }

        public static void SetImmuneToPlayer(Ped ped)
        {
            Function.Call(Hash.SET_ENTITY_CAN_BE_DAMAGED_BY_RELATIONSHIP_GROUP, ped, false, playerRelationship);
        }

        public static void NotImmuneToPlayer(Ped ped)
        {
            Function.Call(Hash.SET_ENTITY_CAN_BE_DAMAGED_BY_RELATIONSHIP_GROUP, ped, true, playerRelationship);
        }

        public static Vector2 World3DToScreen2d(Vector3 pos)
        {
            OutputArgument x2dp = new OutputArgument();
            OutputArgument y2dp = new OutputArgument();

            Function.Call(Hash._0x34E82F05DF2974F5, pos.X, pos.Y, pos.Z, x2dp, y2dp); //GET_SCREEN_COORD_FROM_WORLD_COORD
            return new Vector2(x2dp.GetResult<float>(), y2dp.GetResult<float>());
        }

        public static List<PedHash> LikleyCustomers = new List<PedHash>()
        {

        };



        public static float Lerp(float start, float end, float time)
        {
            return (1 - time) * start + time * end;
        }

        public static float GetRoadSpeed(Vector3 pos)
        {
            OutputArgument outArgA = new OutputArgument();
            OutputArgument outArgB = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_VEHICLE_NODE_PROPERTIES, pos.X, pos.Y, pos.Z, outArgA, outArgB))
            {
                int busy = outArgA.GetResult<int>();
                int flags = outArgB.GetResult<int>();

                float speed = 8;
                foreach (int flag in Enum.GetValues(typeof(PathnodeFlags)).Cast<PathnodeFlags>())
                {
                    if ((flag & flags) != 0)
                    {
                        switch ((PathnodeFlags)flag)
                        {
                            case PathnodeFlags.Two:
                                speed = 16;
                                break;
                            case PathnodeFlags.Freeway:
                                speed = 35;
                                break;
                            case PathnodeFlags.Slow:
                                speed = 9;
                                break;
                            case PathnodeFlags.Eight:
                                speed = 16;
                                break;
                            case PathnodeFlags.SlowTraffic:
                                speed = 14;
                                break;
                            case PathnodeFlags.FourWayIntersection:
                                speed = 11;
                                break;
                            case PathnodeFlags.Intersection:
                                speed = 11;
                                break;
                            default:
                                speed = 8;
                                break;
                        }
                    }
                }
                int NodeID = Function.Call<int>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_ID, pos.X, pos.Y, pos.Z, 1, 1, 200f, 200f);
                if (Function.Call<bool>(Hash._GET_IS_SLOW_ROAD_FLAG, NodeID))
                {
                    if (speed > 9)
                    {
                        speed *= 0.7f;
                    }
                }
                return speed;  // DisplayHelpTextThisFrame("Flags: " + d);
            }
            return 5;
        }

        public static void SetWorkerMenuHealth(Ped p)
        {
            WorkerMainMenu.Subtitle.Caption = "Health:" + p.Health + " Armour:" + p.Armor;
        }

        public static void SetRandomCombatStats(Ped p)
        {
            p.Armor = 0 + rnd.Next(0, 1 + areas[areaIndex].GangPresance * 10);
            p.ShootRate = 0 + rnd.Next(0, 1 + areas[areaIndex].GangPresance * 10);
            p.Accuracy = 0 + rnd.Next(0, 1 + areas[areaIndex].GangPresance * 10);
        }


        public static void DoGangGrowth()
        {
            foreach (Area area in areas)
            {
                int gChance = 20;
                
                if (area.Name == World.GetZoneNameLabel(HouseHandler.Houses[0].entrancePos))
                {
                    gChance *= 2;
                    area.GangPresance = 0;
                }
                Random gRnd = new Random();
                if (gRnd.Next(0, gChance) == 0 && area.GangPresance < 10 && !area.DemandMet())
                {
                    area.GangPresance += 1;                    
                }
            }
        }

        public static void SendTextMsg(string str)
        {
            Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
            UI.Notify(str);
        }



        public static int TotalDrugs(Dictionary<string, int> _drugs)
        {
            int totalDrugs = 0;
            foreach (KeyValuePair<string, int> entry in _drugs)
            {
                totalDrugs += _drugs[entry.Key];
            }
            totalDrugs += (pWeedOunce * 28) + (pCrackOunce * 28) + (pCocaineOunce * 28);
            return totalDrugs;
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

        public static void AddBlip(Entity _ent, BlipColor _col, string _name, bool _friend, bool _edge)
        {
            if (!_ent.CurrentBlip.Exists())
            {
                _ent.AddBlip();
            }
            Blip blip = _ent.CurrentBlip;
            blip.IsFriendly = _friend;
            blip.IsShortRange = _edge;
            blip.Color = _col;
            blip.Name = _name;
        }

        public static void RemovePedFromWorld(Ped ped, bool clearTask)
        {
            ped.CurrentBlip?.Remove();
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


        public static void PlayTheSound()
        {
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "LOCAL_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", 0);
        }

        static List<int> AllowedInteriors = new List<int>
        {
            118018, // Vanilla Unicorn
            271617, // AfterHours
            92674,  // TextileMill
            275201, // Diamond Casino
            233729  //Tequi-la-la.
        };


        public static bool IsInInterior(Entity _entity)
        {
            int interior = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, _entity);
            foreach (int inter in AllowedInteriors)
            {
                if (inter == interior)
                {
                    return false;
                }
            }
            if (interior != 0) return true;
            else return false;
        }

        public static bool IsUnderGround(Entity _entity)
        {
            Vector3 pos = _entity.Position;
            float eZ = pos.Z;
            float gZ = World.GetGroundHeight(pos);
            if (gZ > eZ) return true;
            else return false;
        }

        public static void SpawnDriveBy()
        {
            int amount = areas[areaIndex].GangPresance;
            if (!hardMode) amount = (int)(amount * 0.5);
            if (amount < 1) amount = 1;
            for (int i = 0; i < amount; i++)
            {
                driveByHandler.AddDriveBy(VehicleClass.SUVs, rnd.Next(0, AmountOfPedsForDriveBy(areas[areaIndex].GangPresance)), Game.Player.Character);
            }
        }

        public static void UpdateSave()
        {
            bool changed = false;
            foreach (XElement element in LSLifeSave.Descendants())
            {
                if (element.Name == "dealer1")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "debt" && e.Value != dealer1Debt.ToString())
                        {
                            e.Value = dealer1Debt.ToString();
                            changed = true;
                        }
                        if (e.Name == "weed" && e.Value != dealer1Weed.ToString())
                        {
                            e.Value = dealer1Weed.ToString();
                            changed = true;
                        }
                        if (e.Name == "crack" && e.Value != dealer1Crack.ToString())
                        {
                            e.Value = dealer1Crack.ToString();
                            changed = true;
                        }
                        if (e.Name == "cocain" && e.Value != dealer1Cocain.ToString())
                        {
                            e.Value = dealer1Cocain.ToString();
                            changed = true;
                        }
                        if (e.Name == "rep" && e.Value != dealer1Rep.ToString())
                        {
                            e.Value = dealer1Rep.ToString();
                            changed = true;
                        }
                        if (e.Name == "money" && e.Value != dealer1Money.ToString())
                        {
                            e.Value = dealer1Money.ToString();
                            changed = true;
                        }
                    }
                }
            }
            if (changed)
            {
                LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
                if (DEBUG) { UI.Notify("Save Changed"); }
            }
        }

        public static List<XElement> PedWeapons(Ped _ped)
        {
            List<XElement> weps = new List<XElement>();
            foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
            {
                bool hasPedGotWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, _ped, (int)weapon);
                if (hasPedGotWeapon)
                {
                    int ammunitionInWeapon = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, _ped, (int)weapon);
                    if (ammunitionInWeapon < 500) ammunitionInWeapon = 500;
                    var parentElement = new XElement("weapon");
                    var weaponNameElement = new XElement("HASH", weapon);
                    var weaponAmmoElement = new XElement("AMMO", ammunitionInWeapon);
                    parentElement.Add(weaponNameElement);
                    parentElement.Add(weaponAmmoElement);
                    foreach (WeaponComponent component in Enum.GetValues(typeof(WeaponComponent)))
                    {
                        bool hasWeaponGotComponent = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, _ped, (int)weapon, (int)component);
                        if (hasWeaponGotComponent)
                        {
                            var weaponComponentElement = new XElement("COMPONENT", component);
                            parentElement.Add(weaponComponentElement);
                        }
                    }
                    weps.Add(parentElement);
                }
            }
            return weps;
        }

        public static void GivePedWeapons(Ped _ped, List<XElement> _weapons)
        {
            if (_weapons.Count > 0)
            {
                WeaponHash cWeapon = _ped.Weapons.Current.Hash;

                foreach (XElement element in _weapons)
                {
                    bool hasWeapon = false;
                    WeaponHash weapon = WeaponHash.Unarmed;
                    foreach (XElement ele in element.Elements())
                    {
                        if (ele.Name == "HASH") Enum.TryParse(ele.Value, out weapon);
                        if (ele.Name == "AMMO")
                        {
                            Int32.TryParse(ele.Value, out int ammo);
                            if (ammo < 500) ammo = 500;
                            if (!_ped.Weapons.HasWeapon(weapon))
                            {
                                _ped?.Weapons.Give(weapon, ammo, true, true);
                            }
                            else { hasWeapon = true; }
                            _ped?.Weapons.Select(weapon);
                            _ped.Weapons.Current.Ammo += ammo;
                            if (DEBUG) UI.Notify("Gave ped" + weapon);
                        }
                        if (ele.Name == "COMPONENT" && !hasWeapon)
                        {
                            if (Enum.TryParse(ele.Value, out WeaponComponent wepcomp))
                            {
                                _ped?.Weapons.Current.SetComponent(wepcomp, true);
                            }
                        }
                    }
                }
                _ped.Weapons.Select(cWeapon);
            }
        }

        public static void UpdateInventory()
        {
            TotalDrugsCarried = 0;
            foreach (KeyValuePair<string, int> entry in PlayerInventory)
            {
                TotalDrugsCarried += PlayerInventory[entry.Key];
            }
            bool saveChanged = false;
            foreach (XElement element in LSLifeSave.Descendants())
            {
                if (element.Name == "playerInventory")
                {
                    foreach (XElement e in element.Elements())
                    {
                        if (e.Name == "weed")
                        {
                            if (e.Value != PlayerInventory["Weed"].ToString())
                            {
                                e.Value = PlayerInventory["Weed"].ToString();
                                saveChanged = true;
                            }
                        }
                        else
                        if (e.Name == "crack")
                        {
                            if (e.Value != PlayerInventory["Crack"].ToString())
                            {
                                e.Value = PlayerInventory["Crack"].ToString();
                                saveChanged = true;
                            }
                        }
                        else
                        if (e.Name == "cocain")
                        {
                            if (e.Value != PlayerInventory["Cocaine"].ToString())
                            {
                                e.Value = PlayerInventory["Cocaine"].ToString();
                                saveChanged = true;
                            }
                        }
                        else
                        if (e.Name == "weedOz")
                        {
                            if (e.Value != pWeedOunce.ToString())
                            {
                                e.Value = pWeedOunce.ToString();
                                saveChanged = true;
                            }
                        }
                        else
                        if (e.Name == "crackOz")
                        {
                            if (e.Value != pCrackOunce.ToString())
                            {
                                e.Value = pCrackOunce.ToString();
                                saveChanged = true;
                            }
                        }
                        else
                        if (e.Name == "cocaineOz")
                        {
                            if (e.Value != pCocaineOunce.ToString())
                            {
                                e.Value = pCocaineOunce.ToString();
                                saveChanged = true;
                            }
                        }
                        else
                        if (e.Name == "armour")
                        {
                            if (e.Value != playerArmours.ToString())
                            {
                                e.Value = playerArmours.ToString();
                                saveChanged = true;
                            }
                        }
                    }
                    break;
                }
            }
            if (saveChanged) { LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml"); }
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
                        int waitTime = 20;
                        if (waitTime - ((int)CurrentRepLvl(dealer1Rep) / 2) <= 5) waitTime = 5;
                        else waitTime -= ((int)CurrentRepLvl(dealer1Rep) / 2);
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
                        if (DEBUG) UI.Notify("" + dealer1ReloadHour);
                        string amPm = "AM";
                        if (timeTodeliver > 12)
                        {
                            timeTodeliver -= 12;
                            amPm = "PM";
                        }

                        SendTextMsg("Zee has ordered a reload, should come " + mDay.ToString() + " at " + timeTodeliver + ":00" + amPm);
                    }
                }
                int timeReduction = 0;
                if (CurrentRepLvl(dealer1Rep) > 36)
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
                    //UI.Notify("~r~Reload1");
                    dealer1ReloadOrder = false;
                    dealer1Reloaded = true;
                    reloadTimer = Game.GameTime;
                    if (dealer1Debt > 0)
                    {
                        float intrest = (float)0.05;
                        float intrestAmount = (float)dealer1Debt * intrest;
                        if (intrestAmount < 0) intrestAmount = 0;
                        dealer1Debt += (int)intrestAmount;
                        if (dealer1Debt < 0) dealer1Debt = 0;
                        SendTextMsg("Zee added $" + IntToMoney((int)intrestAmount) + " to your bill, its now $" + IntToMoney(dealer1Debt));
                    }
                    if (hardMode)
                    {
                        if (dealer1Money < 1000)
                        {
                            dealer1Money = 5000;
                        }
                    }
                    else
                    {
                        if (dealer1Money < 10000)
                        {
                            dealer1Money = 50000;
                        }
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
                    SendTextMsg("Zee Reloaded. He got " + "Weed:" + finalWeed + "oz, Crack:" + finalCrack + "oz, Cocaine:" + finalCocain + "oz.");
                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                    UpdateSave();
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
                        if (dealer1Debt < 0) dealer1Debt = 0;
                        SendTextMsg("Zee added $" + IntToMoney((int)intrestAmount) + " to your bill, its now $" + IntToMoney(dealer1Debt));

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
                    SendTextMsg("Zee Reloaded. He got " + "Weed:" + finalWeed + "oz, Crack:" + finalCrack + "oz, Cocaine:" + finalCocain + "oz.");
                    if (DEBUG) { UI.Notify("Weed:" + finalWeed + " Crack:" + finalCrack + " Cocaine:" + finalCocain); }
                    LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
                    UpdateSave();
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
                UpdateSave();
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
                        p.FreezePosition = true;
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

        public static Vector3 AdjustToGround(Vector3 _vector3)
        {
            Vector3 finalPos = _vector3;
            finalPos.Z = World.GetGroundHeight(finalPos);
            return finalPos;
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
                    UpdateDealerInventory();
                    zeeMenu.CurrentSelection = 0;

                }
            }
            // We need to close the phone at a moment.
            // We can close it as soon as the contact pick up calling _iFruit.Close().
            // Here, we will close the phone in 5 seconds (5000ms).
            iFruit.Close(1000);
        }

        private static void UpdateDealerInventory()
        {
            zeeMenu.Subtitle.Caption = "Rep:" + IntToMoney((int)dealer1Rep) + " Level:~g~" + (int)CurrentRepLvl(dealer1Rep);
            payDebt.Text = "Pay Debt $" + IntToMoney(dealer1Debt);
            bushWeed.Maximum = dealer1Weed;
            bushWeed.Value = 0;
            crack.Maximum = dealer1Crack;
            crack.Value = 0;
            cocaine.Maximum = dealer1Cocain;
            cocaine.Value = 0;
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

        public static void ChangeAdealerToPdealer()
        {
            if (aDealer != null && aDealer.dPed != null)
            {
                DealerHandler.AddDealer(new PlayerDealer(aDealer.dPed));
                aDealer = null;
                areas[areaIndex].KillDealer();
            }
        }

        public static void MakeStreetSale(Customer c, bool inCar)
        {
            List<Ped> police = new List<Ped>();
            Ped p = c.Ped;
            int price = c.amount * c.Drug.PricePerG;
            float divder = c.amount / 7f;
            if (divder >= 1)
            {
                price = (c.amount * c.Drug.PricePerG) - ((int)divder * c.Drug.PricePerG);
            }
            PedsCanSeeDeal = 0;
            //string dAmount = c.amount + "g";
            //if (c.amount >= 28) dAmount = GramsToOz(c.amount);
            //string pAmount = PlayerInventory[c.Drug.Name] + "g";
            //if (PlayerInventory[c.Drug.Name] >= 28) pAmount = GramsToOz(PlayerInventory[c.Drug.Name]);
            foreach (Ped ped in lPeds.GetPeds.Where(r => r.IsAlive))
            {
                if (IsPolice(ped))
                {
                    police.Add(ped);
                    ped.Task.LookAt(Game.Player.Character, 10000);
                }
                if (!ped.IsPlayer && customers.FindAll(s => s.Ped == ped).Count == 0 && !oldCustomers.Contains(ped) && CanSee(ped, Game.Player.Character))
                {
                    PedsCanSeeDeal++;
                }
            }
            if (inCar == false)
            {

                Game.Player.Character.Task.TurnTo(p);
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_HOWS_IT_GOING", "SPEECH_PARAMS_STANDARD", 0);
                Wait(1000);
                p.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                Game.Player.Character.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_shopkeeper", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                Wait(2000);
                Game.Player.Money += price;
                c.Ped.Money -= price;
                PlayerInventory[c.Drug.Name] -= c.amount;
                PlayTheSound();
                UI.Notify("Sold " + c.Drug.Name + " " + GramsToOz(c.amount) + ", you got " + GramsToOz(PlayerInventory[c.Drug.Name]) + " left.");
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_THANKS", "SPEECH_PARAMS_STANDARD", 0);
                oldCustomers.Add(p);
                if (c.Vehicle != null && c.Vehicle.Exists())
                {
                    c.Ped.Task.PerformSequence(PerformTaskSeq(1, c.Vehicle, Vector3.Zero));
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
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_HOWS_IT_GOING", "SPEECH_PARAMS_STANDARD", 0);
                Wait(1000);
                p.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8.0f, 3000, AnimationFlags.UpperBodyOnly);
                Wait(3000);
                Game.Player.Money += price;
                c.Ped.Money -= price;
                PlayerInventory[c.Drug.Name] -= c.amount;
                PlayTheSound();
                UI.Notify("Sold " + c.Drug.Name + " " + GramsToOz(c.amount) + ", you got " + GramsToOz(PlayerInventory[c.Drug.Name]) + " left.");
                Function.Call(Hash._PLAY_AMBIENT_SPEECH1, p, "GENERIC_THANKS", "SPEECH_PARAMS_STANDARD", 0);
                if (c.Vehicle != null && c.Vehicle.Exists())
                {
                    c.Ped.Task.PerformSequence(PerformTaskSeq(1, c.Vehicle, Vector3.Zero));
                }
                else
                {
                    c.Ped.Task.PerformSequence(PerformTaskSeq(0, null, Vector3.Zero));
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
                    break;
                }
            }
            bool tip = false;
            if (aDealer != null && aDealer.Detected == false)
            {
                double gangStr = areas[areaIndex].GangPresance * 1.5f;
                if (rnd.Next(0, 8 + (int)gangStr) == 0)
                {
                    UI.ShowSubtitle("Hey thought you might like to know, ~r~someone ~w~else selling over on ~y~" + World.GetStreetName(aDealer.dPed.Position) + "~w~.", 6000);


                    AddBlip(aDealer.dPed, BlipColor.Red, "Dealer", false, false);
                    aDealer.Detected = true;
                    tip = true;
                }
            }
            if (pigs.Count > 0 && !tip && !pigs[0].detected)
            {
                if (rnd.Next(0, 5 + areas[areaIndex].GangPresance) == 0)
                {
                    UI.ShowSubtitle("I hear ~r~Police ~w~are over on ~y~" + World.GetStreetName(pigs[0].Ped.Position) + "~w~. They heading this way.", 6000);
                    pigs[0].detected = true;
                }
            }
            LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");

            foreach (Ped pol in police)
            {
                if (DEBUG) UI.Notify("" + police.Count + " | " + Function.Call<int>(Hash.GET_PED_TYPE, pol.Handle) + " | " + IsPolice(pol));
                if (CanSee(pol, Game.Player.Character))
                {
                    //pol.AddBlip();
                    //pol.CurrentBlip.Color = BlipColor.Green;
                    //pol.CurrentBlip.Name = "Plice";
                    SendTextMsg("They spotted you dealing, your gona be hot round there now.");
                    areas[areaIndex].Heat += 1000;
                    Game.Player.WantedLevel = 1;
                    break;
                }
            }
            if (aDealer != null && aDealer.dPed != null && CanSee(aDealer.dPed, Game.Player.Character) && rnd.Next(0, 2) == 0 && driveByHandler.AmountOfDriveBy() == 0)
            {
                SpawnDriveBy();
            }
            customerSellTo = null;
        }


        public static void HandOverGoods(Ped _from, Ped _too)
        {
            List<Ped> police = new List<Ped>();
            PedsCanSeeDeal = 0;
            foreach (Ped ped in lPeds.GetPeds)
            {
                if (ped.IsAlive)
                {
                    if (IsPolice(ped))
                    {
                        police.Add(ped);
                        ped.Task.LookAt(Game.Player.Character, 10000);
                    }
                    if (!ped.IsPlayer && customers.FindAll(s => s.Ped == ped).Count == 0 && !oldCustomers.Contains(ped) && CanSee(ped, Game.Player.Character))
                    {
                        PedsCanSeeDeal += 1;
                    }
                }
            }
            
            if (!_from.IsInVehicle() || !_too.IsInVehicle())
            {            
                _from.Task.TurnTo(_too);
                _too.Task.TurnTo(_from);
                Wait(1000);
            }
            _too.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8.0f, 2000, AnimationFlags.UpperBodyOnly);
            _from.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_shopkeeper", 8.0f, 2000, AnimationFlags.UpperBodyOnly);
            Wait(2000);
            foreach (Ped pol in police)
            {
                if (DEBUG) UI.Notify("" + police.Count + " | " + Function.Call<int>(Hash.GET_PED_TYPE, pol.Handle) + " | " + IsPolice(pol));
                if (pol.Position.DistanceTo(playerPos) < 40 && CanSee(pol, Game.Player.Character))
                {
                    SendTextMsg("They spotted you dealing, your gona be hot round there now.");
                    areas[areaIndex].Heat += 1000;
                    Game.Player.WantedLevel = 1;
                    break;
                }
            }
            if (_from == player.Character || _too == player.Character)
            {
                player.Character.Task.ClearLookAt();
                player.Character.Task.ClearAll();
            }

            if (aDealer != null && aDealer.dPed != null && aDealer.dPed.Position.DistanceTo(playerPos) < 40 && CanSee(aDealer.dPed, Game.Player.Character) && rnd.Next(0, 2) == 0 && driveByHandler.AmountOfDriveBy() == 0)
            {
                DbugString("Spawning Driveby, player seen doing deal.");
                SpawnDriveBy();
            }
            lastTimeDealDone = Game.GameTime;
        }

        public static int MoneyToRep(int _money, bool _debt)
        {
            if (_debt) return (int)(_money / 100 * 0.5);
            else return (int)(_money / 100);
        }

        public static void Dealer1AddRep(int _rep)
        {
            if (_rep > 0)
            {
                dealer1Rep = dealer1Rep + _rep;
            }
        }

        public static string GramsToOz(int _grams)
        {
            if (_grams >= 28)
            {
                int oz = _grams / 28;
                int g = _grams - (oz * 28);
                return "~g~" + oz + "~s~oz, ~g~" + g + "~s~g";
            }
            else return "~g~" + _grams + "~s~g";

        }

        public static void DbugString(string _string)
        {
            if (DEBUG) UI.Notify(_string);
        }

        public static string IntToMoney(int _money)
        {
            var sb = new StringBuilder("~g~" + _money.ToString("#,##0") + "~s~");
            sb.Replace(",", "~s~,~g~");
            return sb.ToString();
        }

        public static bool zeeClose()
        {
            if (drugDealer1 != null && drugDealer1.Position.DistanceTo(playerPos) < 4)
                return true;
            if (weaponDealer1 != null && weaponDealer1.Position.DistanceTo(playerPos) < 4)
                return true;
            return false;
        }

        public static bool IsCustomerNear()
        {
            return customers.FindAll(r => r.Ped.Position.DistanceTo(playerPos) < 20).Count == 0 && jobs.FindAll(r => r.Ped.Position.DistanceTo(playerPos) < 20).Count == 0;
        }


        public static Dictionary<int, Tuple<int, int>> GetPedVariationData(Ped _ped)
        {
            List<int> compIds = new List<int> { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11 };
            Dictionary<int, Tuple<int, int>> pedData = new Dictionary<int, Tuple<int, int>>();
            foreach (int id in compIds)
            {
                int drawID = Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, _ped, id);
                int textureID = Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, _ped, id);
                Tuple<int, int> data = new Tuple<int, int>(drawID, textureID);
                pedData.Add(id, data);
            }
            if (pedData.Count > 0) return pedData;
            else return null;
        }

        public static void SetPedVariation(Ped _ped, Dictionary<int, Tuple<int, int>> _pData)
        {
            List<int> compIds = new List<int> { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11 };
            foreach (int id in compIds)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _ped, id, _pData[id].Item1, _pData[id].Item2, 0);
            }
        }
    }
}
