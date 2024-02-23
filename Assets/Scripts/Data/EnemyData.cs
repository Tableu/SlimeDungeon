using System;
using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Data/Enemy Data")]
public class EnemyData : CharacterData
{
    [HeaderAttribute("Enemy")] 
    [SerializeField] private float attackRange;
    [SerializeField] private float aggroRange;
    [SerializeField] private float deAggroRange;
    [SerializeField] private int detectTick;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private FormData formData;

    [HeaderAttribute("Collision Damage")] 
    [SerializeField] private bool enableCollisionDamage;
    [SerializeField] private CollisionData collisonData;
    
    public float AttackRange => attackRange;
    public float StoppingDistance => stoppingDistance;
    public float AggroRange => aggroRange;
    public float DeAggroRange => deAggroRange;
    public int DetectTick => detectTick;
    public FormData FormData => formData;
    public bool EnableCollisionDamage => enableCollisionDamage;
    public CollisionData CollisionData => collisonData;
}

[Serializable]
public struct CollisionData
{
    public float Damage;
    public float HitStun;
    public float Knockback;
}