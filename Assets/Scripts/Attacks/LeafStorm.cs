using System;
using System.Collections.Generic;
using UnityEngine;
using Type = Elements.Type;

public class LeafStorm : MonoBehaviour
{
    [SerializeField] private new SphereCollider collider;
    private float _damage;
    private float _attackStat;
    private float _duration;
    private Type _type;
    private float _startTime;
    private Dictionary<int, int> _damageTicks;
    private int _tick;
    
    public void Initialize(float damage, float attackStat, float duration, Type type, int tick)
    {
        _damage = damage;
        _attackStat = attackStat;
        _duration = duration;
        _type = type;
        _tick = tick;
    }

    private void Start()
    {
        _startTime = Time.time;
        _damageTicks = new Dictionary<int, int>();
    }

    private void FixedUpdate()
    {
        if (Time.time - _startTime >= _duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyDamage(other);
        _damageTicks.TryAdd(other.gameObject.GetInstanceID(), 0);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_damageTicks.ContainsKey(other.gameObject.GetInstanceID()))
            return;
        _damageTicks[other.gameObject.GetInstanceID()]++;
        if (_damageTicks[other.gameObject.GetInstanceID()] > _tick)
        {
            _damageTicks[other.gameObject.GetInstanceID()] = 0;
            ApplyDamage(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _damageTicks.Remove(other.gameObject.GetInstanceID());
    }

    private void ApplyDamage(Collider enemy)
    {
        IDamageable damageable = enemy.attachedRigidbody != null ? 
            enemy.attachedRigidbody.gameObject.GetComponent<IDamageable>() 
            : enemy.GetComponent<IDamageable>();
        damageable?.TakeDamage(_damage, _attackStat, Vector3.zero, 0, _type);
    }
}
