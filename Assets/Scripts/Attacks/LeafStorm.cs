using UnityEngine;
using Type = Elements.Type;

public class LeafStorm : MonoBehaviour
{
    [SerializeField] private new SphereCollider collider;
    private float _damage;
    private float _duration;
    private float _damageTick;
    private Type _type;
    private float _startTime;
    
    public void Initialize(float damage, float duration, Type type)
    {
        _damage = damage;
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
        ApplyDamage(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyDamage(other.gameObject);
    }

    private void ApplyDamage(GameObject enemy)
    {
        IDamageable damageable = enemy.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage, Vector3.zero, 0, _type);
        }
    }
}
