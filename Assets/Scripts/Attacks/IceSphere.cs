using UnityEngine;

public class IceSphere : MonoBehaviour, BasicProjectile
{
    [SerializeField] private ParticleSystem iceSphere;
    [SerializeField] private ParticleSystem snowExplosion;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new Collider collider;
    [SerializeField] private GameObject orb;
    private float _damage;
    private float _attackStat;
    private float _hitStun;
    private Elements.Type _type;
    private bool _exploding;

    public void Initialize(float damage,float attackStat, float knockback, float hitStun, Vector3 force, Elements.Type type)
    {
        _damage = damage;
        _attackStat = attackStat;
        _hitStun = hitStun;
        _type = type;
        rigidbody.AddForce(force, ForceMode.Impulse);
        iceSphere.Play();
        snowExplosion.Stop();
    }

    private void FixedUpdate()
    {
        if (_exploding && !snowExplosion.IsAlive())
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        iceSphere.Stop();
        iceSphere.Clear();
        snowExplosion.Play();
        _exploding = true;
        Destroy(orb);
        rigidbody.velocity = Vector3.zero;
        collider.enabled = false;
        Collider[] enemies = Physics.OverlapSphere(transform.position, 2, LayerMask.GetMask("Enemy"));
        foreach (Collider enemy in enemies)
        {
            IDamageable damageable = enemy.attachedRigidbody != null ? 
                enemy.attachedRigidbody.gameObject.GetComponent<IDamageable>() 
                : enemy.GetComponent<IDamageable>();
            damageable?.TakeDamage(_damage, _attackStat,Vector3.zero, _hitStun, _type);
        }
    }
}
