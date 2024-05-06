using UnityEngine;

public class Chest : MonoBehaviour, IItem
{
    [SerializeField] private GameObject chestLid;
    [SerializeField] private Vector3 openedRotation;

    private bool _opened;

    public void PickUp(PlayerController character)
    {
        if (!_opened)
        {
            ResourceManager.Instance.Coins.Add(100);
            chestLid.transform.rotation = Quaternion.Euler(openedRotation.x,openedRotation.y,openedRotation.z);
            _opened = true;
            gameObject.layer = LayerMask.NameToLayer("Obstacles");
        }
    }
}
