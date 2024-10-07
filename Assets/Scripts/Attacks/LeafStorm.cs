using UnityEngine;
using Type = Elements.Type;

public class LeafStorm : MonoBehaviour
{
    [SerializeField] private new SphereCollider collider;
    private float _damage;
    private float _attackStat;
    private float _duration;
    private float _damageTick;
    private Type _type;
    private float _startTime;
    
    public void Initialize(float damage, float attackStat, float duration, Type type)
    {
        _damage = damage;
        _attackStat = attackStat;
        _duration = duration;
        _type = type;
    }

    private void Start()
    {
        _startTime = Time.time;
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
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyDamage(other);
    }

    private void ApplyDamage(Collider enemy)
    {
        IDamageable damageable = enemy.attachedRigidbody != null ? 
            enemy.attachedRigidbody.gameObject.GetComponent<IDamageable>() 
            : enemy.GetComponent<IDamageable>();
        damageable?.TakeDamage(_damage, _attackStat, Vector3.zero, 0, _type);
    }
}
