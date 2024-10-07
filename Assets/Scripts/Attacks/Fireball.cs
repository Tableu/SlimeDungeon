using UnityEngine;

public class Fireball : MonoBehaviour, BasicProjectile
{
    [SerializeField] private ParticleSystem fireball;
    [SerializeField] private GameObject explosion;
    [SerializeField] private new Rigidbody rigidbody;
    private float _damage;
    private float _attackStat;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;

    public void Initialize(float damage, float attackStat, float knockback,float hitStun, Vector3 force, Elements.Type type)
    {
        _hitStun = hitStun;
        _attackStat = attackStat;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        rigidbody.AddForce(force, ForceMode.Impulse);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            IDamageable damage = other.attachedRigidbody.gameObject.GetComponent<IDamageable>();
            if (damage != null)
            {
                damage.TakeDamage(_damage, _attackStat,_knockback * _force.normalized, _hitStun, _type);
            }
        }

        fireball.Stop();
        GameObject g = Instantiate(explosion, transform.position, Quaternion.identity);
        g.layer = gameObject.layer;
        Destroy(gameObject);
    }
}
