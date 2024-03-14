using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireball;
    [SerializeField] private GameObject explosion;
    [SerializeField] private new Rigidbody rigidbody;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;

    public void Initialize(float damage, float knockback,float hitStun, Vector3 force, Elements.Type type)
    {
        _hitStun = hitStun;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        rigidbody.AddForce(force, ForceMode.Impulse);
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
        Destroy(gameObject);
    }
}
