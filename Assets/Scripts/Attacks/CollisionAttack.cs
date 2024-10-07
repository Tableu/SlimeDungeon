using UnityEngine;

public class CollisionAttack : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IDamageable health = other.gameObject.GetComponent<IDamageable>();
            health.TakeDamage(enemyData.MeleeAttackData.Damage, enemyData.Attack,
                (other.transform.position - transform.position).normalized*enemyData.MeleeAttackData.Knockback, 
                enemyData.MeleeAttackData.HitStun, enemyData.ElementType);
        }
    }
}
