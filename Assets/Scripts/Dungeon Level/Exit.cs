using FischlWorks_FogWar;
using UnityEngine;

public class Exit : MonoBehaviour, IItem
{
    [SerializeField] private new Collider collider;
    private LevelManager _levelManager;
    public void Initialize(RoomController controller, LevelManager levelManager)
    {
        _levelManager = levelManager;
        collider.enabled = false;
        controller.OnAllEnemiesDead += delegate { collider.enabled = true; };
    }

    public void PickUp(PlayerController character)
    {
        _levelManager.ExitLevel();
    }
}
