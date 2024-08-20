using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class SpellItem : MonoBehaviour, IItem
{
    [SerializeField] private List<SpriteRenderer> renderers;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private Vector3 launchSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private bool isStoreItem;
    [SerializeField] private List<Outline> outlineScripts;

    private AttackData _data;
    private GameObject _model;

    public AttackData Data => _data;

    public void Initialize(AttackData data)
    {
        _data = data;
        foreach (var renderer in renderers)
        {
            renderer.sprite = data.Icon;
        }
        rigidbody.AddRelativeForce(launchSpeed, ForceMode.Impulse);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed);
    }

    public void PickUp(PlayerController character)
    {
        character.UnlockAttack(_data);
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
        foreach (Outline script in outlineScripts)
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
    [SerializeField] private AttackData testData;
    [ContextMenu("Test")]
    public void Test()
    {
        Initialize(testData);
    }
#endif
}
