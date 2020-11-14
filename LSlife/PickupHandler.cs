using System.Collections.Generic;
using System.Linq;
using static LSlife.LsFunctions;
using static LSlife.LSL;

namespace LSlife
{
    public class PickupHandler
    {
        List<DroppedBag> Bags = new List<DroppedBag>();

        public PickupHandler()
        {

        }

        public void OnSlowTick()
        {
            if (Bags != null && Bags.Count > 0)
            {
                foreach (DroppedBag bag in Bags.ToList())
                {
                    if (bag != null)
                    {
                        if (bag.CheckIfPickedUp())
                        {
                            bag.AddStuffToPlayer();
                            bag.RemovePickup();
                            RemoveBag(bag);
                            break;
                        }
                        if (bag.CanDespawn())
                        {
                            bag.RemovePickup();
                            RemoveBag(bag);
                            if (DEBUG) SendTextMsg("A bag was removed for being too old.");
                        }
                    }
                    else
                    {
                        DbugString("Bag was null");
                        RemoveBag(bag);
                    }
                }
            }
        }

        public void AddBag(DroppedBag _bag)
        {
            Bags.Add(_bag);
        }

        void RemoveBag(DroppedBag _bag)
        {
            Bags.Remove(_bag);
        }

        public void RemoveBags()
        {
            if (Bags != null && Bags.Count > 0)
            {
                foreach (DroppedBag bag in Bags.ToList())
                {
                    if (bag != null)
                    {
                        bag.RemovePickup();
                        RemoveBag(bag);
                    }

                }
            }
        }
    }
}
