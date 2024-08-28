public interface IItem
{
    public bool CanPickup();
    public void PickUp(PlayerController character, InventoryController inventory);
    public void Highlight(bool enable);
}
