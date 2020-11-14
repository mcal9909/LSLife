namespace LSlife.Inventory
{
    class Cocaine : ItemObject
    {
        public Cocaine(string _name, bool _stack, string _desc) : base(_name, _stack, _desc)
        {

        }
        void Innit(string _name, bool _stack, string _desc)
        {
            name = _name;
            stackable = _stack;
            description = _desc;

        }
    }
}
