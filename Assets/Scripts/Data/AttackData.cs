using System;
using Controller;
using UnityEngine;

[Serializable]
public abstract class AttackData : ScriptableObject
{
    [SerializeField] private float offset;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float knockback;
    [SerializeField] private float hitStun;
    [SerializeField] private GameObject prefab;

    public float Offset => offset;
    public float Speed => speed;
    public float Damage => damage;
    public float Knockback => knockback;
    public float HitStun => hitStun;
    public GameObject Prefab => prefab;
        
    public abstract Attack EquipAttack(Character character);
}
