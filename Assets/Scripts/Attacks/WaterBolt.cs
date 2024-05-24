using System;
using UnityEngine;

public class WaterBolt : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;
    private int _maxBounces;
    private int _bounceCount = 0;
    private float _lastBounceTimestamp;

    public void Initialize(float damage, float knockback,float hitStun, Vector3 force, Elements.Type type, int maxBounces)
    {
        _hitStun = hitStun;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        _maxBounces = maxBounces;
        rigidbody.AddForce(force, ForceMode.Impulse);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        IDamageable damage = other.gameObject.GetComponent<IDamageable>();
        if (damage != null)
        {
            damage.TakeDamage(_damage,_knockback*_force.normalized, _hitStun, _type);
            Destroy(gameObject);
        }

        if (Math.Abs(Time.time - _lastBounceTimestamp) < 0.001)
        {
            return;
        }
        
        if (_bounceCount < _maxBounces)
        {
            _bounceCount++;
            rigidbody.velocity = Vector3.Reflect(_force, other.GetContact(0).normal);
            _force = rigidbody.velocity;
            _lastBounceTimestamp = Time.time;
        }
        else
        {
            Destroy(gameObject);
        }

        
    }
}
