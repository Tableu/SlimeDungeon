using UnityEngine;

public class WaterBolt : MonoBehaviour
{
    [SerializeField] private ParticleSystem waterbolt;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new SphereCollider collider;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private float _sizeMultiplier = 1;
    private Elements.Type _type;
    private int _maxBounces;
    private int _bounceCount = 0;

    public void Initialize(float damage, float knockback,float hitStun, Vector3 force, float sizeMultiplier, Elements.Type type, int maxBounces)
    {
        _sizeMultiplier = sizeMultiplier;
        _hitStun = hitStun;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        _maxBounces = maxBounces;
        rigidbody.AddForce(force, ForceMode.Impulse);
        var shape = waterbolt.shape;
        shape.radius *= sizeMultiplier;
        collider.radius *= sizeMultiplier;
        var main = waterbolt.main;
        main.startSpeed = new ParticleSystem.MinMaxCurve(main.startSpeed.constant*sizeMultiplier);
        main.startSize = new ParticleSystem.MinMaxCurve(main.startSize.constant*sizeMultiplier);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        IDamageable damage = other.gameObject.GetComponent<IDamageable>();
        if (damage != null)
        {
            damage.TakeDamage(_damage,_knockback*_force.normalized, _hitStun, _type);
            Destroy(gameObject);
        }
        
        if (_bounceCount < _maxBounces)
        {
            _bounceCount++;
            rigidbody.velocity = Vector3.Reflect(_force, other.GetContact(0).normal);
            _force = rigidbody.velocity;
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log(rigidbody.velocity);
    }
}
