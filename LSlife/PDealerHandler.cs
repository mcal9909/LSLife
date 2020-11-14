using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using static LSlife.Enums;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class PlayerDealerHandler
    {
        public List<PlayerDealer> dealers { get; set; } = new List<PlayerDealer>();
        int gameHour = Function.Call<int>(Hash.GET_CLOCK_HOURS);
        public bool needsSave = false;
        List<int> allocatedIDs = new List<int>();
        public PlayerDealer currentDealer;
        PedGroup Group = new PedGroup();
        public List<Vector3> HealLocations = new List<Vector3>
        {
            new Vector3(342.7698f, -1397.948f, 32.50927f),
            new Vector3(298.8214f, -584.4205f, 43.26083f),
            new Vector3(355.9612f, -596.3049f, 28.77687f),
            new Vector3(-448.425f, -340.619f, 34.50178f),
            new Vector3(1839.235f, 3673.061f, 34.27673f)
        };
        public HealPositionTypes HealPositionType;


        public PlayerDealerHandler()
        {
            LoadDealers();
        }

        public void OnTick()
        {
            if (dealers.Count > 0)
            {
                foreach (PlayerDealer dealer in dealers)
                {
                    if (dealer == null)
                    {
                        dealers.Remove(dealer);
                        break;
                    }
                    else
                    {
                        dealer.OnTick();
                    }
                }
                if (needsSave)
                {
                    SaveDealers();
                    needsSave = false;
                }
            }
        }

        public void OnSlowTick()
        {
            if (dealers.Count > 0)
            {
                int currentHour = Function.Call<int>(Hash.GET_CLOCK_HOURS);
                int hoursPassed = 0;
                if (gameHour != currentHour)
                {
                    if (currentHour > gameHour) { hoursPassed = currentHour - gameHour; }
                    else
                    {
                        int toMid = 24 - gameHour;
                        hoursPassed = toMid + currentHour;
                    }
                    gameHour = currentHour;
                    if (DEBUG) UI.Notify("DealerSim Hours Passed " + hoursPassed);
                }

                foreach (PlayerDealer dealer in dealers.ToList())
                {
                    if (dealer == null) dealers.Remove(dealer);
                    else
                    {
                        if (dealer.CurrentState != PdStates.Dead && dealer.CurrentState != PdStates.FollowPlayer && dealer.CurrentState != PdStates.FollowingPlayer)
                        {
                            if (hoursPassed > 0)
                            {
                                dealer.SimulateSales(hoursPassed);
                                needsSave = true;
                            }
                        }
                        if (dealer.CurrentState == PdStates.Dead)
                        {
                            DeleteDealer(dealer.ID);
                            dealers.Remove(dealer);
                            break;
                        }
                        else if (dealer != null) dealer?.OnSlowTick();
                    }
                }
            }
        }

        void LoadDealers()
        {
            XDocument dealerDoc;
            if (!File.Exists("scripts\\LSLife\\LSLife_Dealers.xml"))
            {
                dealerDoc = new XDocument(new XElement("dealers"));
                dealerDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
            }
            else
            {
                dealerDoc = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
            }
            if (dealerDoc.Descendants().ToList().Count > 0)
            {
                List<int> usedIDs = new List<int>();
                foreach (XElement element in dealerDoc.Descendants())
                {
                    if (element.Name == "dealer")
                    {
                        foreach (XElement ele in element.Elements())
                        {
                            int id;
                            bool found = true;
                            if (ele.Name == "id")
                            {
                                int.TryParse(ele.Value, out id);
                                if (usedIDs.Count > 0)
                                {
                                    while (found)
                                    {
                                        if (usedIDs.FindAll(d => d == id).Count == 0) found = false;
                                        else id++;
                                    }
                                }
                                else
                                {
                                    found = false;
                                }
                                if (found == false)
                                {
                                    usedIDs.Add(id);
                                    ele.Value = id.ToString();
                                }
                            }
                        }
                    }
                }
                dealerDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
                foreach (XElement ele in dealerDoc.Descendants())
                {
                    if (ele.Name == "dealer")
                    {
                        int id = 0;
                        PedHash pedHash = PedHash.Acult01AMM;
                        Vector3 pos = Vector3.Zero;
                        int weed = 0;
                        int crack = 0;
                        int cocaine = 0;
                        int money = 0;
                        int armour = -1;
                        List<XElement> tWeapons = new List<XElement>();
                        Dictionary<int, Tuple<int, int>> pData = new Dictionary<int, Tuple<int, int>>();
                        foreach (XElement e in ele.Elements())
                        {
                            if (e.Name == "id") int.TryParse(e.Value, out id);
                            if (e.Name == "hash") Enum.TryParse<PedHash>(e.Value, out pedHash);
                            if (e.Name == "posX")
                            {
                                float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out pos.X);
                                if (pos.X == 0) float.TryParse(e.Value, out pos.X);
                            }
                            if (e.Name == "posY")
                            {
                                float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out pos.Y);
                                if (pos.Y == 0) float.TryParse(e.Value, out pos.Y);
                            }
                            if (e.Name == "posZ")
                            {
                                float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out pos.Z);
                                if (pos.Z == 0) float.TryParse(e.Value, out pos.Z);
                            }
                            if (e.Name == "weed") int.TryParse(e.Value, out weed);
                            if (e.Name == "crack") int.TryParse(e.Value, out crack);
                            if (e.Name == "cocaine") int.TryParse(e.Value, out cocaine);
                            if (e.Name == "money") int.TryParse(e.Value, out money);
                            if (e.Name == "armour") int.TryParse(e.Value, out armour);
                            if (e.Name == "weapon") tWeapons.Add(e);
                            if (e.Name == "pedData")
                            {
                                int headD = 0;
                                int headT = 0;
                                int beardD = 0;
                                int beardT = 0;
                                int hairD = 0;
                                int hairT = 0;
                                int torsoD = 0;
                                int torsoT = 0;
                                int legsD = 0;
                                int legsT = 0;
                                int handsD = 0;
                                int handsT = 0;
                                int footD = 0;
                                int footT = 0;
                                int acces1D = 0;
                                int acces1T = 0;
                                int acces2D = 0;
                                int acces2T = 0;
                                int decalD = 0;
                                int decalT = 0;
                                int torsoAuxD = 0;
                                int torsoAuxT = 0;

                                foreach (XElement pD in e.Elements())
                                {
                                    if (pD.Name == "headD") int.TryParse(pD.Value, out headD);
                                    if (pD.Name == "headT") int.TryParse(pD.Value, out headT);

                                    if (pD.Name == "beardD") int.TryParse(pD.Value, out beardD);
                                    if (pD.Name == "beardT") int.TryParse(pD.Value, out beardT);

                                    if (pD.Name == "hairD") int.TryParse(pD.Value, out hairD);
                                    if (pD.Name == "hairT") int.TryParse(pD.Value, out hairT);

                                    if (pD.Name == "torsoD") int.TryParse(pD.Value, out torsoD);
                                    if (pD.Name == "torsoT") int.TryParse(pD.Value, out torsoT);

                                    if (pD.Name == "legsD") int.TryParse(pD.Value, out legsD);
                                    if (pD.Name == "legsT") int.TryParse(pD.Value, out legsT);

                                    if (pD.Name == "handsD") int.TryParse(pD.Value, out handsD);
                                    if (pD.Name == "handsT") int.TryParse(pD.Value, out handsT);

                                    if (pD.Name == "footD") int.TryParse(pD.Value, out footD);
                                    if (pD.Name == "footT") int.TryParse(pD.Value, out footT);

                                    if (pD.Name == "acces1D") int.TryParse(pD.Value, out acces1D);
                                    if (pD.Name == "acces1T") int.TryParse(pD.Value, out acces1T);

                                    if (pD.Name == "acces2D") int.TryParse(pD.Value, out acces2D);
                                    if (pD.Name == "acces2T") int.TryParse(pD.Value, out acces2T);

                                    if (pD.Name == "decalD") int.TryParse(pD.Value, out decalD);
                                    if (pD.Name == "decalT") int.TryParse(pD.Value, out decalT);

                                    if (pD.Name == "torsoAuxD") int.TryParse(pD.Value, out torsoAuxD);
                                    if (pD.Name == "torsoAuxT") int.TryParse(pD.Value, out torsoAuxT);
                                }

                                pData.Add(0, new Tuple<int, int>(headD, headT));
                                pData.Add(1, new Tuple<int, int>(beardD, beardT));
                                pData.Add(2, new Tuple<int, int>(hairD, hairT));
                                pData.Add(3, new Tuple<int, int>(torsoD, torsoT));
                                pData.Add(4, new Tuple<int, int>(legsD, legsT));
                                pData.Add(5, new Tuple<int, int>(handsD, handsT));
                                pData.Add(6, new Tuple<int, int>(footD, footT));
                                pData.Add(8, new Tuple<int, int>(acces1D, acces1T));
                                pData.Add(9, new Tuple<int, int>(acces2D, acces2T));
                                pData.Add(10, new Tuple<int, int>(decalD, decalT));
                                pData.Add(11, new Tuple<int, int>(torsoAuxD, torsoAuxT));
                            }
                        }

                        if (pos != Vector3.Zero)
                        {
                            allocatedIDs.Add(id);
                            if (DEBUG) UI.Notify("Added " + id + " to allocated ID's, Size of pool =" + allocatedIDs.Count);
                            if (dealers.FindAll(d => d.area == World.GetZoneNameLabel(pos)).Count == 0)
                            {
                                AddDealer(new PlayerDealer(id, pedHash, PdStates.Normal, pos, weed, crack, cocaine, money, armour, tWeapons, pData));
                            }
                            else
                            {
                                AddDealer(new PlayerDealer(id, pedHash, PdStates.FollowPlayer, pos, weed, crack, cocaine, money, armour, tWeapons, pData));
                            }
                            if (DEBUG) UI.Notify("Loaded dealer" + id);
                        }
                    }
                }
            }
        }

        void SaveDealers()
        {
            XDocument dealerDoc1 = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
            foreach (PlayerDealer pdealer in dealers)
            {
                SaveDealer(pdealer, dealerDoc1);
            }
            dealerDoc1.Save("scripts\\LSLife\\LSLife_Dealers.xml");
        }

        public Vector3 GetCloseHealPosition()
        {
            Vector3 house = Vector3.Zero;
            if (HouseHandler.Houses.Count > 0)
            {
                List<StashHouse> cHouses = HouseHandler.Houses.Where(h => h.entrancePos.DistanceTo(playerPos) < 40).ToList();
                if (cHouses != null && cHouses.Count > 0)
                {
                    StashHouse cHouse = cHouses[0];
                    foreach (StashHouse h in cHouses)
                    {
                        if (cHouse.entrancePos.DistanceTo(playerPos) < h.entrancePos.DistanceTo(playerPos))
                        {
                            cHouse = h;
                        }
                    }
                    house = cHouse.entrancePos;
                }
            }

            Vector3 hospital = Vector3.Zero;
            if (DealerHandler.HealLocations.Count > 0)
            {
                List<Vector3> cHospitals = DealerHandler.HealLocations.Where(h => h.DistanceTo(playerPos) < 50).ToList();
                if (cHospitals != null && cHospitals.Count > 0)
                {
                    Vector3 cHospital = cHospitals[0];
                    foreach (Vector3 h in cHospitals)
                    {
                        if (h.DistanceTo(playerPos) < cHospital.DistanceTo(playerPos))
                        {
                            cHospital = h;
                        }
                    }
                    hospital = cHospital;
                }
            }

            if (house != Vector3.Zero && hospital != Vector3.Zero)
            {
                if (house.DistanceTo(playerPos) < hospital.DistanceTo(playerPos))
                {
                    HealPositionType = HealPositionTypes.House;
                    return house;
                }
                else
                {
                    HealPositionType = HealPositionTypes.Hospital;
                    return hospital;
                }
            }
            else if (house != Vector3.Zero)
            {
                HealPositionType = HealPositionTypes.House;
                return house;
            }
            else if (hospital != Vector3.Zero)
            {
                HealPositionType = HealPositionTypes.Hospital;
                return hospital;
            }
            else
            {
                HealPositionType = HealPositionTypes.None;
                return Vector3.Zero;
            }
        }


        public int GetNewID()
        {
            int id = 0;
            if (allocatedIDs.FindAll(i => i == id).Count != 0)
            {
                id++;
                while (allocatedIDs.FindAll(i => i == id).Count != 0)
                {
                    id++;
                }
            }
            if (DEBUG) UI.Notify("New ID " + id);
            allocatedIDs.Add(id);
            return id;
        }

        public void SaveDealer(PlayerDealer _dealer, XDocument _doc)
        {

            foreach (XElement element in _doc.Descendants())
            {
                if (element.Name == "id" && element.Value == _dealer.ID.ToString())
                {
                    bool foundArmour = false;
                    bool foundPedData = false;
                    foreach (XElement ele in element.Parent.Elements())
                    {
                        if (ele.Name == "posX") ele.Value = _dealer.Pos.X.ToString();
                        if (ele.Name == "posY") ele.Value = _dealer.Pos.Y.ToString();
                        if (ele.Name == "posZ") ele.Value = _dealer.Pos.Z.ToString();
                        if (ele.Name == "weed") ele.Value = _dealer.Drugs["Weed"].ToString();
                        if (ele.Name == "crack") ele.Value = _dealer.Drugs["Crack"].ToString();
                        if (ele.Name == "cocaine") ele.Value = _dealer.Drugs["Cocaine"].ToString();
                        if (ele.Name == "money") ele.Value = _dealer.Money.ToString();
                        if (ele.Name == "armour")
                        {
                            foundArmour = true;
                            ele.Value = _dealer.dArmour.ToString();
                        }
                        if (ele.Name == "pedData") { foundPedData = true; }
                    }
                    if (!foundArmour)
                    {
                        XElement armourEle = new XElement("armour", _dealer.dArmour);
                        element.Parent.Add(armourEle);
                    }
                    if (!foundPedData && _dealer.Ped != null)
                    {
                        var pedData = new XElement("pedData");
                        _dealer.pData = GetPedVariationData(_dealer.Ped);
                        var headD = new XElement("headD", _dealer.pData[0].Item1.ToString());
                        var headT = new XElement("headT", _dealer.pData[0].Item2.ToString());
                        pedData.Add(headD);
                        pedData.Add(headT);
                        var beardD = new XElement("beardD", _dealer.pData[1].Item1.ToString());
                        var beardT = new XElement("beardT", _dealer.pData[1].Item2.ToString());
                        pedData.Add(beardD);
                        pedData.Add(beardT);
                        var hairD = new XElement("hairD", _dealer.pData[2].Item1.ToString());
                        var hairT = new XElement("hairT", _dealer.pData[2].Item2.ToString());
                        pedData.Add(hairD);
                        pedData.Add(hairT);
                        var torsoD = new XElement("torsoD", _dealer.pData[3].Item1.ToString());
                        var torsoT = new XElement("torsoT", _dealer.pData[3].Item2.ToString());
                        pedData.Add(torsoD);
                        pedData.Add(torsoT);
                        var legsD = new XElement("legsD", _dealer.pData[4].Item1.ToString());
                        var legsT = new XElement("legsT", _dealer.pData[4].Item2.ToString());
                        pedData.Add(legsD);
                        pedData.Add(legsT);
                        var handsD = new XElement("handsD", _dealer.pData[5].Item1.ToString());
                        var handsT = new XElement("handsT", _dealer.pData[5].Item2.ToString());
                        pedData.Add(handsD);
                        pedData.Add(handsT);
                        var footD = new XElement("footD", _dealer.pData[6].Item1.ToString());
                        var footT = new XElement("footT", _dealer.pData[6].Item2.ToString());
                        pedData.Add(footD);
                        pedData.Add(footT);
                        var acces1D = new XElement("acces1D", _dealer.pData[8].Item1.ToString());
                        var acces1T = new XElement("acces1T", _dealer.pData[8].Item2.ToString());
                        pedData.Add(acces1D);
                        pedData.Add(acces1T);
                        var acces2D = new XElement("acces2D", _dealer.pData[9].Item1.ToString());
                        var acces2T = new XElement("acces2T", _dealer.pData[9].Item2.ToString());
                        pedData.Add(acces2D);
                        pedData.Add(acces2T);
                        var decalD = new XElement("decalD", _dealer.pData[10].Item1.ToString());
                        var decalT = new XElement("decalT", _dealer.pData[10].Item2.ToString());
                        pedData.Add(decalD);
                        pedData.Add(decalT);
                        var torsoAuxD = new XElement("torsoAuxD", _dealer.pData[11].Item1.ToString());
                        var torsoAuxT = new XElement("torsoAuxT", _dealer.pData[11].Item2.ToString());
                        pedData.Add(torsoAuxD);
                        pedData.Add(torsoAuxT);
                        element.Parent.Add(pedData);
                    }
                    break;
                }
            }
        }

        void DeleteDealer(int _id)
        {
            XDocument dealerDoc = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
            if (dealerDoc.Descendants().ToList().Count > 0 && dealers.Count > 0)
            {
                foreach (PlayerDealer dealer in dealers.ToList())
                {
                    if (dealer.ID == _id) { dealers.Remove(dealer); }
                    XElement dealerEle = null;
                    foreach (XElement ele in dealerDoc.Descendants()) //Find the right dealer in the XML
                    {
                        if (ele.Name == "dealer")
                        {
                            foreach (XElement e in ele.Elements())
                            {
                                if (e.Value == _id.ToString())
                                {
                                    dealerEle = e.Parent;
                                    break;
                                }
                            }
                        }
                    }
                    if (dealerEle != null)
                    {
                        dealerEle.Remove();
                        //UI.Notify("ped deleetd");
                        break;
                    }
                }
                dealerDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
            }
        }

        public void AddDealer(PlayerDealer dealer)
        {
            dealers.Add(dealer);
        }

        //public PlayerDealer currentDealer()
        //{
        //    PlayerDealer nearestDealer = dealers[0];
        //    foreach(PlayerDealer dealer in dealers)
        //    {
        //        if (dealer.Pos.DistanceTo(playerPos) < nearestDealer.Pos.DistanceTo(playerPos)) nearestDealer = dealer;                
        //    }
        //    return nearestDealer;
        //}
    }
}
