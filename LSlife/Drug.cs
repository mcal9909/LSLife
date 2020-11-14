using GTA;
using static LSlife.LSL;


namespace LSlife
{
    public class Drug
    {
        public string Name;
        public int PricePerG;
        public AreaType AreaType;
        public float Demand = 0;
        public bool Supplied = false;
        public float Delay = 15000;
        public int TimeOfLastCustomer = Game.GameTime;
        public bool SelectCustomer = false;
        public Ped Customer = null;

        public Drug(string _name, int _price, AreaType _area)
        {
            Name = _name;
            PricePerG = _price;
            AreaType = _area;
        }
    }
}
