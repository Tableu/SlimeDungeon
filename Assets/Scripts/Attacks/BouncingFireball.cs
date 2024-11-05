using UnityEngine;

public class BouncingFireball : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private ParticleSystem fireball;
    [SerializeField] private GameObject explosion;
    private float _damage;
    private float _attackStat;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;
    private int _maxBounces;
    private int bounceCount = 0;
    private float lastCollisionTime;
    private LayerMask _enemyMask;
    private float _explosionDamageRadius;

    public void Initialize(float damage, float attackStat, float knockback, float hitStun, Vector3 force, int maxBounces, float explosionDamageRadius, Elements.Type type, LayerMask enemyMask)
    {
        _hitStun = hitStun;
        _attackStat = attackStat;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        _maxBounces = maxBounces;
        rigidbody.AddForce(force, ForceMode.VelocityChange);
        _enemyMask = enemyMask;
        _explosionDamageRadius = explosionDamageRadius;
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
        if (_enemyMask == (_enemyMask | (1 << other.gameObject.layer)))
        {
            SpawnExplosion();
            Destroy(gameObject);
            return;
        }

        if (Time.time - lastCollisionTime < 0.1)
            return;

        int mask = LayerMask.GetMask("Floor", "Walls", "Obstacles");
        if (mask == (mask | (1 << other.gameObject.layer)))
        {
            if (bounceCount >= _maxBounces)
            {
                SpawnExplosion();
                Destroy(gameObject);
            }
            bounceCount++;
            lastCollisionTime = Time.time;
        }
    }

    private void SpawnExplosion()
    {
        fireball.Stop();
        GameObject g = Instantiate(explosion, transform.position, Quaternion.identity);
        g.layer = gameObject.layer;
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, _explosionDamageRadius,_enemyMask);
        foreach (Collider col in enemyColliders)
        {
            IDamageable dmg = col.attachedRigidbody != null ? 
                col.attachedRigidbody.gameObject.GetComponent<IDamageable>() 
                : col.GetComponent<IDamageable>();
            IObstacle obstacle = col.attachedRigidbody != null ? 
                col.attachedRigidbody.gameObject.GetComponent<IObstacle>() 
                : col.GetComponent<IObstacle>();
            obstacle?.ApplyForce(_force.normalized, rigidbody.mass);
            dmg?.TakeDamage(_damage, _attackStat,_knockback*_force.normalized, _hitStun, _type);
            
        }
    }
}
