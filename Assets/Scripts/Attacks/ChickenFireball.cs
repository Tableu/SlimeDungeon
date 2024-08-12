using UnityEngine;

public class ChickenFireball : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private ParticleSystem fireball;
    [SerializeField] private GameObject explosion;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;
    private int bounceCount = 0;
    private float lastCollisionTime;
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

    private void OnDrawGizmosSelected()
    {
        #if UNITY_EDITOR
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 2.5f);
        #endif 
    }

    private void OnCollisionEnter(Collision other)
    {
        int mask = LayerMask.GetMask("Walls", "Obstacles");
        if (mask == (mask | (1 << other.gameObject.layer)))
        {
            SpawnExplosion();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_enemyMask == (_enemyMask | (1 << other.gameObject.layer)))
        {
            IDamageable dmg = other.attachedRigidbody != null ? 
                other.attachedRigidbody.gameObject.GetComponent<IDamageable>() 
                : other.GetComponent<IDamageable>();
            dmg?.TakeDamage(_damage, _knockback*_force.normalized, _hitStun, _type);
            SpawnExplosion();
            Destroy(gameObject);
        }
    }

    private void SpawnExplosion()
    {
        fireball.Stop();
        GameObject g = Instantiate(explosion, transform.position, Quaternion.identity);
        g.layer = gameObject.layer;
    }
}