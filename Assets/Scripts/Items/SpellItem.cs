using System.Collections.Generic;
using UnityEngine;

public class SpellItem : Item
{
    [SerializeField] private List<SpriteRenderer> renderers;
    private AttackData _data;
    public void Initialize(AttackData data)
    {
        _data = data;
        foreach (var renderer in renderers)
        {
            renderer.sprite = data.Icon;
        }
    }

    public override void PickUp(PlayerController character)
    {
        character.UnlockAttack(_data);
        Destroy(gameObject);
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
