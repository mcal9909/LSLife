using GTA;
using GTA.Math;


namespace LSlife
{
    class CustomerPed : DefaultPed
    {

        private Vehicle Vehicle;
        public Drug Drug;
        public int amount;
        bool walkToPlayer;
        enum CustomerStates { None, Waiting, WalkTo, Buying, WalktAway };
        CustomerStates cState = CustomerStates.None;

        public CustomerPed(PedType type, Ped ped, Vector3 target, bool working, int areaIndex, Vehicle _vehicle, Drug _drug, int _drugAmount, bool _walk) : base(type, ped, target)
        {
            Vehicle = _vehicle;
            Drug = _drug;
            amount = _drugAmount;
            walkToPlayer = _walk;
        }

        public new void Tick()
        {
            switch (cState)
            {
                case CustomerStates.None:
                    if (walkToPlayer)
                    {
                        MovePed();
                        cState = CustomerStates.WalkTo;
                    }
                    else
                    {
                        Wait();
                        cState = CustomerStates.Waiting;
                    }
                    break;
                case CustomerStates.Waiting:
                    if (Game.Player.Character.Position.DistanceTo(Ped.Position) < 10)
                    {
                        MovePed();
                        cState = CustomerStates.WalkTo;
                    }
                    break;
                case CustomerStates.WalkTo:
                    if (Game.Player.Character.Position != target)
                    {
                        target = Game.Player.Character.Position;
                        MovePed();
                    }
                    if (Game.Player.Character.Position.DistanceTo(Ped.Position) < 3)
                    {

                    }
                    break;

            }
        }

        void BuyDrugs()
        {
            //MakeStreetSale(this, false);
        }
    }
}
