using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireball;
    [SerializeField] private GameObject explosion;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new SphereCollider collider;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private float _sizeMultiplier;
    private Elements.Type _type;

    public void Initialize(float damage, float knockback,float hitStun, Vector3 force, float sizeMultiplier, Elements.Type type)
    {
        _sizeMultiplier = sizeMultiplier;
        _hitStun = hitStun;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        rigidbody.AddForce(force, ForceMode.Impulse);
        var shape = fireball.shape;
        shape.radius *= sizeMultiplier;
        collider.radius *= sizeMultiplier;
        var main = fireball.main;
        main.startSpeed = new ParticleSystem.MinMaxCurve(main.startSpeed.constant*sizeMultiplier);
        main.startSize = new ParticleSystem.MinMaxCurve(main.startSize.constant*sizeMultiplier);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        IDamageable damage = other.gameObject.GetComponent<IDamageable>();
        if (damage != null)
        {
            damage.TakeDamage(_damage,_knockback*_force.normalized, _hitStun, _type);
        }
        fireball.Stop();
        GameObject g = Instantiate(explosion, transform.position, Quaternion.identity);
        g.layer = gameObject.layer;
        var explosionParticle = g.GetComponent<ParticleSystem>();
        var shape = explosionParticle.shape;
        shape.radius *= _sizeMultiplier;
        var main = explosionParticle.main;
        main.startSpeed = new ParticleSystem.MinMaxCurve(main.startSpeed.constant * _sizeMultiplier);
        main.startSize = new ParticleSystem.MinMaxCurve(main.startSize.constant * _sizeMultiplier);
        Destroy(gameObject);
    }
}
