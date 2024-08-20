public interface IItem
{
    public bool CanPickup();
    public void PickUp(PlayerController character);
    public void Highlight(bool enable);
}
