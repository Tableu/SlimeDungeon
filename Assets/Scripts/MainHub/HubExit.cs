using UnityEngine;

public class HubExit : MonoBehaviour, IItem
{
    [SerializeField] private HubManager hubManager;
    
    public void PickUp(PlayerController character, InventoryController inventory, PartyController partyController)
    {
        hubManager.LoadDungeonLevel();
    }

    public bool CanPickup()
    {
        return true;
    }

    public void Highlight(bool enable)
    {
        
    }
}
