using UnityEngine;

public class LevelExit : MonoBehaviour, IItem
{
    [SerializeField] private new Collider collider;
    private LevelManager _levelManager;
    private RoomController _roomController;
    public void Initialize(RoomController controller, LevelManager levelManager)
    {
        _levelManager = levelManager;
        collider.enabled = false;
        _roomController = controller;
        controller.OnAllEnemiesDead += delegate { collider.enabled = true; };
        if (controller.AllEnemiesDead)
        {
            collider.enabled = true;
        }
    }

    public void PickUp(PlayerController character, InventoryController inventory)
    {
        _levelManager.ExitLevel();
    }

    public bool CanPickup()
    {
        return _roomController.AllEnemiesDead;
    }

    public void Highlight(bool enable)
    {
        
    }
}
