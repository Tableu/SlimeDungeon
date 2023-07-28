using System;
using Controller;
using UnityEngine;
using Type = Elements.Type;

[Serializable]
public abstract class AttackData : ScriptableObject
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float knockback;
    [SerializeField] private float manaCost;
    [SerializeField] private float hitStun;
    [SerializeField] private float cooldown;
    [SerializeField] private float targetingRange;
    [SerializeField] private Type elementType;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite icon;
    
    public float Speed => speed;
    public float Damage => damage;
    public float Knockback => knockback;
    public float HitStun => hitStun;
    public float ManaCost => manaCost;
    public float Cooldown => cooldown;
    public GameObject Prefab => prefab;
    public Type ElementType => elementType;
    public float TargetingRange => targetingRange;
    public Sprite Icon => icon;
        
    public abstract Attack EquipAttack(Character character, int index);
}
