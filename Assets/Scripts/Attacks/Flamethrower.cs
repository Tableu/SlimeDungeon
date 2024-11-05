using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    private float _damage;
    private float _attackStat;
    private float _knockback;
    private Vector3 _force;
    private float _hitStun;
    private Elements.Type _type;

    public void Initialize(float damage, float attackStat, float knockback, float hitStun, Vector3 force, Elements.Type type)
    {
        _attackStat = attackStat;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _hitStun = hitStun;
        _type = type;
        if (gameObject.layer == LayerMask.NameToLayer("PlayerAttacks"))
        {
            var particleCollision = particleSystem.collision;
            particleCollision.collidesWith = LayerMask.GetMask("Enemy", "Walls", "Obstacles");
        }
        else if (gameObject.layer == LayerMask.NameToLayer("EnemyAttacks"))
        {
            var particleCollision = particleSystem.collision;
            particleCollision.collidesWith = LayerMask.GetMask("Player", "Walls", "Obstacles");
        }
    }
    private void FixedUpdate()
    {
        if (!particleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        IDamageable damage = other.GetComponent<IDamageable>();
        damage?.TakeDamage(_damage,_attackStat,_knockback*_force.normalized, _hitStun, _type);
        IObstacle obstacle = other.GetComponent<IObstacle>();
        obstacle?.ApplyForce(_force.normalized, 5);
    }
}