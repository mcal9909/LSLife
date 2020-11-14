using GTA;
using GTA.Math;
using LSlife.Inventory;
using System;

namespace LSlife
{
    class DefaultPed
    {
        public enum PedType { Default, NewCustomer, OldCustomer }
        public PedType type;
        public Ped Ped;
        public Vector3 target;
        public bool working;
        int areaIndex;
        InventoryObject pInventory = new InventoryObject();

        public DefaultPed(PedType type, Ped ped, Vector3 target)
        {
            this.type = type;
            Ped = ped ?? throw new ArgumentNullException(nameof(ped));
            this.target = target;
            //this.working = working;
            //this.areaIndex = areaIndex;

        }

        public void MovePed()
        {
            Ped.Task.GoTo(target);
            pInventory.AddItem(new Cocaine("Coke", true, ""), 420);

        }

        public void Wait()
        {
            Ped.Task.PlayAnimation("friends@", "pickupwait", 8f, -1, AnimationFlags.Loop);
        }

        public void Tick()
        {

        }
    }
}
