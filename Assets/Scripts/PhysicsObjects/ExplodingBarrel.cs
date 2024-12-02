
using System;
using UnityEngine;

public class ExplodingBarrel : PhysicsObstacle
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionDamageRadius;
    [SerializeField] private float damage;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, explosionDamageRadius);
    }

    protected override void HandleDeath()
    {
        GameObject g = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        g.layer = gameObject.layer;
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, explosionDamageRadius,LayerMask.GetMask("Enemy","Player"));
        foreach (Collider col in enemyColliders)
        {
            IDamageable dmg = col.attachedRigidbody != null ? 
                col.attachedRigidbody.gameObject.GetComponent<IDamageable>() 
                : col.GetComponent<IDamageable>();
            IObstacle obstacle = col.attachedRigidbody != null ? 
                col.attachedRigidbody.gameObject.GetComponent<IObstacle>() 
                : col.GetComponent<IObstacle>();
            obstacle?.ApplyForce((col.transform.position-transform.position).normalized, col.ClosestPointOnBounds(transform.position), 1);
            dmg?.TakeDamage(damage, 2,Vector3.zero, 0, Elements.Type.None);
        }
        base.HandleDeath();
    }
}
