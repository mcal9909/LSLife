namespace LSlife.Inventory
{
    class ItemSlot
    {
        int SlotNumber = 0;
        private bool empty = true;
        private ItemObject item = null;
        private int itemAmount;

        public ItemSlot(ItemObject _item, int _amount)
        {
            item = _item;
            itemAmount = _amount;
            empty = false;
        }

        public bool Empty { get => empty; set => empty = value; }
        public ItemObject Item { get => item; set => item = value; }
        public int ItemAmount { get => itemAmount; set => itemAmount = value; }
    }
}
