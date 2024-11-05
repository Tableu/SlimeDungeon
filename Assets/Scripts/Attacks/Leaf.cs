using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    private float _damage;
    private float _attackStat;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;
    private List<GameObject> _previousCollisions = new List<GameObject>();

    public void Initialize(float damage,float attackStat, float knockback,float hitStun, Vector3 force, Elements.Type type)
    {
        _hitStun = hitStun;
        _attackStat = attackStat;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        rigidbody.AddForce(force, ForceMode.VelocityChange);
        _previousCollisions = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damage = other.attachedRigidbody != null ? 
            other.attachedRigidbody.gameObject.GetComponent<IDamageable>() 
            : other.GetComponent<IDamageable>();
        if (damage != null && !_previousCollisions.Contains(other.gameObject))
        {
            _previousCollisions.Add(other.gameObject);
            damage.TakeDamage(_damage,_attackStat,_knockback*_force.normalized, _hitStun, _type);
        }

        int mask = LayerMask.GetMask("Walls", "Obstacles");
        if(mask == (mask | (1 << other.gameObject.layer)))
            Destroy(gameObject);
    }
}
