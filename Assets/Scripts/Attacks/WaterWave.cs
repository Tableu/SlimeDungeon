using System.Collections.Generic;
using UnityEngine;

public class WaterWave : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    private float _damage;
    private float _attackStat;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;
    private LayerMask _enemyMask;
    private List<IDamageable> previousCollisions = new List<IDamageable>();
    
    public void Initialize(float damage, float attackStat, float knockback, float hitStun, Vector3 force, Elements.Type type, LayerMask enemyMask)
    {
        _attackStat = attackStat;
        _hitStun = hitStun;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        rigidbody.AddForce(force, ForceMode.Impulse);
        _enemyMask = enemyMask;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            IDamageable damage = other.attachedRigidbody.gameObject.GetComponent<IDamageable>();
            if (damage != null && (_enemyMask & (1 << other.gameObject.layer)) != 0
            && !previousCollisions.Contains(damage))
            {
                damage.TakeDamage(_damage, _attackStat,_knockback * _force.normalized, _hitStun, _type);
                previousCollisions.Add(damage);
            }
        }

        if ((LayerMask.GetMask("PlayerAttacks", "EnemyAttacks") & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(other.gameObject);
        }
        if ((LayerMask.GetMask("Walls", "Obstacles") & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(gameObject);
        }
    }
}
