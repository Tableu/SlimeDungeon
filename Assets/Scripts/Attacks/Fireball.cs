using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireball;
    [SerializeField] private GameObject explosion;
    [SerializeField] private new Rigidbody rigidbody;
    private float _damage;
    private float _knockback;
    private Vector3 _force;

    public void Initialize(float damage, float knockback, Vector3 force)
    {
        _damage = damage;
        _knockback = knockback;
        _force = force;
        rigidbody.AddForce(force, ForceMode.Impulse);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        IDamageable damage = other.gameObject.GetComponent<IDamageable>();
        if (damage != null)
        {
            damage.TakeDamage(_damage,_knockback*_force.normalized);
        }
        Debug.Log(other.gameObject.name);
        fireball.Stop();
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
