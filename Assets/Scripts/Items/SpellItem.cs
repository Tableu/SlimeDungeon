using System.Collections.Generic;
using UnityEngine;

public class SpellItem : MonoBehaviour, IItem
{
    [SerializeField] private List<SpriteRenderer> renderers;
    [SerializeField] private float speed;
    [SerializeField] private float height;
    [SerializeField] private float rotateSpeed;
    private AttackData _data;
    private Vector3 _pos;
    private GameObject _model;

    public void Initialize(AttackData data)
    {
        _data = data;
        foreach (var renderer in renderers)
        {
            renderer.sprite = data.Icon;
        }
    }

    public void PickUp(PlayerController character)
    {
        character.UnlockAttack(_data);
        Destroy(gameObject);
    }
    
    private void Start()
    {
        _pos = transform.position;
    }

    private void Update()
    {
        float newY = Mathf.Sin(Time.time * speed)*height + _pos.y;
        transform.position = new Vector3(_pos.x, newY, _pos.z);
        transform.Rotate(Vector3.up, rotateSpeed);
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
