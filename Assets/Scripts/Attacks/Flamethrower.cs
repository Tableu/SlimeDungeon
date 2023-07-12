using System;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    private float _damage;
    private float _knockback;
    private Vector3 _force;
    private float _sizeMultiplier;

    public void Initialize(float damage, float knockback, Vector3 force, float sizeMultiplier)
    {
        _sizeMultiplier = sizeMultiplier;
        _damage = damage;
        _knockback = knockback;
        _force = force;
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
            damage.TakeDamage(_damage,_knockback*_force.normalized);
        }
    }
}
