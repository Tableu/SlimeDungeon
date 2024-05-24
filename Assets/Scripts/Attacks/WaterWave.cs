using UnityEngine;

public class WaterWave : MonoBehaviour
{
    [SerializeField] private ParticleSystem waterwave;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new Collider collider;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;
    private LayerMask _enemyMask;
    
    public void Initialize(float damage, float knockback, float hitStun, Vector3 force, Elements.Type type, LayerMask enemyMask)
    {
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
        IDamageable damage = other.gameObject.GetComponent<IDamageable>();
        if (damage != null && (_enemyMask & (1 << other.gameObject.layer)) != 0)
        {
            damage.TakeDamage(_damage,_knockback*_force.normalized, _hitStun, _type);
        }

        if ((LayerMask.GetMask("PlayerAttacks", "EnemyAttacks") & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(other.gameObject);
        }
        if ((LayerMask.GetMask("Walls") & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(gameObject);
        }
    }
}
