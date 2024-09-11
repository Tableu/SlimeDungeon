public interface IItem
{
    public bool CanPickup();
    public void PickUp(PlayerController character, InventoryController inventory, PartyController partyController);
    public void Highlight(bool enable);
}
