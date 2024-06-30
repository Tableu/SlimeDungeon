using UnityEngine;

public class BouncingFireball : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private ParticleSystem fireball;
    [SerializeField] private GameObject explosion;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;
    private int _maxBounces;
    private int bounceCount = 0;
    private float lastCollisionTime;

    public void Initialize(float damage, float knockback, float hitStun, Vector3 force, int maxBounces, Elements.Type type)
    {
        _hitStun = hitStun;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        _maxBounces = maxBounces;
        rigidbody.AddForce(force, ForceMode.Impulse);
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
            return;
        }
        //todo should set radius in the attack data class
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, 2.5f, LayerMask.GetMask("Enemy"));
        foreach (Collider col in enemyColliders)
        {
            IDamageable dmg = col.attachedRigidbody != null ? 
                col.attachedRigidbody.gameObject.GetComponent<IDamageable>() 
                : col.GetComponent<IDamageable>();
            dmg?.TakeDamage(_damage, _knockback*_force.normalized, _hitStun, _type);
        }
        SpawnExplosion();
        Destroy(gameObject);
    }

    private void SpawnExplosion()
    {
        fireball.Stop();
        GameObject g = Instantiate(explosion, transform.position, Quaternion.identity);
        g.layer = gameObject.layer;
        
    }
}
