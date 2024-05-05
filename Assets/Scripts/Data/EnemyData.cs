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
    [SerializeField] private int coinsOnDeath;
    [SerializeField] private Vector2 idleTimeRange = new Vector2(2,3);
    [SerializeField] private bool moveWhileAttacking;

    [HeaderAttribute("Collision Damage")] 
    [SerializeField] private bool enableCollisionDamage;
    [SerializeField] private CollisionData collisonData;
    
    public float AttackRange => attackRange;
    public float StoppingDistance => stoppingDistance;
    public float AggroRange => aggroRange;
    public float DeAggroRange => deAggroRange;
    public int DetectTick => detectTick;
    public bool EnableCollisionDamage => enableCollisionDamage;
    public CollisionData CollisionData => collisonData;
    public int CoinsOnDeath => coinsOnDeath;
    public Vector2 IdleTimeRange => idleTimeRange;
    public bool MoveWhileAttacking => moveWhileAttacking;
}

[Serializable]
public struct CollisionData
{
    public float Damage;
    public float HitStun;
    public float Knockback;
}