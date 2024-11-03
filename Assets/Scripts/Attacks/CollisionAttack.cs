using UnityEngine;

public class CollisionAttack : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private SphereCollider collider;

    private void FixedUpdate()
    {
        RaycastHit[] hits = Physics.SphereCastAll(collider.transform.position, collider.radius, 
            Vector3.forward, 0, LayerMask.GetMask("Player"));
        foreach (RaycastHit hit in hits)
        {
            if (hit.rigidbody != null && hit.rigidbody.gameObject != null)
            {
                IDamageable health = hit.rigidbody.gameObject.GetComponent<IDamageable>();
                health.TakeDamage(enemyData.MeleeAttackData.Damage, enemyData.Attack,
                    (hit.rigidbody.transform.position - transform.position).normalized*enemyData.MeleeAttackData.Knockback, 
                    enemyData.MeleeAttackData.HitStun, enemyData.ElementType);
            }
        }
    }
}
