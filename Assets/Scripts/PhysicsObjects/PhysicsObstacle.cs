using System;
using UnityEngine;
using Type = Elements.Type;

public interface IObstacle
{
    public void ApplyForce(Vector3 force, Vector3 contactPoint, float mass);
}
public class PhysicsObstacle : MonoBehaviour, IDamageable, IObstacle
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private float maxHealth;

    public float Health { get; private set; }

    public Action<int> OnDamage;
    public Action OnDeath;

    private void Start()
    {
        Health = maxHealth;
        ObstacleHealthBars.Instance.SpawnHealthBar(transform, this, Vector3.zero);
    }

    public void TakeDamage(float damage, float attackStat, Vector3 knockback, float hitStun, Type type)
    {
        rigidbody.AddForce(knockback, ForceMode.Impulse);
        Health -= damage;
        OnDamage?.Invoke((int)damage);
        if (Health < 0)
        {
            OnDeath?.Invoke();
            HandleDeath();
        }
    }

    protected virtual void HandleDeath()
    {
        Destroy(gameObject);
    }


    public void ApplyForce(Vector3 force, Vector3 contactPoint, float mass)
    {
        rigidbody.AddForceAtPosition(force*mass/rigidbody.mass, contactPoint, ForceMode.Impulse);
    }
}

