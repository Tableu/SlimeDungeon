using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    private float _damage;
    private float _knockback;
    private Vector3 _force;
    private float _sizeMultiplier;
    private float _hitStun;
    private Elements.Type _type;

    public void Initialize(float damage, float knockback, float hitStun, Vector3 force, float sizeMultiplier, Elements.Type type)
    {
        _sizeMultiplier = sizeMultiplier;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _hitStun = hitStun;
        _type = type;
    }
    private void Update()
    {
        if (!particleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        IDamageable damage = other.GetComponent<IDamageable>();
        if (damage != null)
        {
            damage.TakeDamage(_damage,_knockback*_force.normalized, _hitStun, _type);
        }
    }
}