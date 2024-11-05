using UnityEngine;
using Type = Elements.Type;

public interface IObstacle
{
    public void ApplyForce(Vector3 force, float mass);
}
public class PhysicsObstacle : MonoBehaviour, IDamageable, IObstacle
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private float maxHealth;
    private float _health;

    private void Start()
    {
        _health = maxHealth;
    }

    public void TakeDamage(float damage, float attackStat, Vector3 knockback, float hitStun, Type type)
    {
        rigidbody.AddForce(knockback, ForceMode.Impulse);
        _health -= damage;
        if(_health < 0)
            Destroy(gameObject);
    }


    public void ApplyForce(Vector3 force, float mass)
    {
        rigidbody.AddForce(force*mass/rigidbody.mass, ForceMode.Impulse);
    }
}

