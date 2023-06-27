using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireball;
    [SerializeField] private GameObject explosion;
    public float Damage;

    private void OnCollisionEnter(Collision other)
    {
        IDamageable damage = other.gameObject.GetComponent<IDamageable>();
        if (damage != null)
        {
            damage.TakeDamage(Damage);
        }
        Debug.Log(other.gameObject.name);
        fireball.Stop();
        GameObject death = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
