using System.Collections.Generic;
using System.Linq;
using cakeslice;
using UnityEngine;

public class HatItem : MonoBehaviour, IItem
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private Vector3 launchSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private bool isStoreItem;

    private List<Outline> _outlineScripts;
    private EquipmentData _data;
    private GameObject _model;
    private Chatbox _chatBox;

    public void Initialize(EquipmentData data)
    {
        _data = data;
        Instantiate(data.Model, transform);
        rigidbody.AddRelativeForce(launchSpeed, ForceMode.Impulse);
    }

    private void Start()
    {
        if (isStoreItem)
        {
            _chatBox = ChatBoxManager.Instance.SpawnChatBox(transform);
            _chatBox.SetMessage("<sprite name=\"UI_117\"> " + _data.Cost.ToString());
        }

        _outlineScripts = GetComponentsInChildren<Outline>().ToList();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed);
    }

    private void OnDestroy()
    {
        if(_chatBox != null)
            Destroy(_chatBox.gameObject);
    }

    public void PickUp(PlayerController character, InventoryController inventory)
    {
        if (isStoreItem)
            ResourceManager.Instance.Coins.Remove(_data.Cost);
        inventory.Add(_data.Name, _data, InventoryController.ItemType.Hats);
        Destroy(gameObject);
    }

    public bool CanPickup()
    {
        if (isStoreItem)
        {
            return ResourceManager.Instance.Coins.Value >= _data.Cost;
        }

        return true;
    }

    public void Highlight(bool enable)
    {
        foreach (Outline script in _outlineScripts)
        {
            if(script != null)
                script.enabled = enable;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        }
    }

#if UNITY_EDITOR
    [SerializeField] private EquipmentData testData;
    [ContextMenu("Test")]
    public void Test()
    {
        Initialize(testData);
    }
#endif
}

