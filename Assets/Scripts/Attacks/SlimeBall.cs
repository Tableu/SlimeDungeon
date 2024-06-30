using UnityEngine;

public class SlimeBall : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    private float _damage;
    private float _knockback;
    private float _hitstun;
    private float _slow;
    private float _duration;
    private GameObject _slimeArea;
    private Vector3 _force;
    private Elements.Type _type;

    public void Initialize(float damage, float knockback, float hitStun, Vector3 force, Elements.Type type, GameObject slimeArea, float slow, float duration)
    {
        _damage = damage;
        _knockback = knockback;
        _hitstun = hitStun;
        _force = force;
        _type = type;
        _slimeArea = slimeArea;
        _slow = slow;
        _duration = duration;
        rigidbody.AddForce(force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            IDamageable damage = other.attachedRigidbody.gameObject.GetComponent<IDamageable>();
            damage?.TakeDamage(_damage, _knockback * _force.normalized, _hitstun, _type);
        }

        Vector3 position = new Vector3(transform.position.x, 0, transform.position.z);
        GameObject g = Instantiate(_slimeArea, position, Quaternion.identity);
        SlimeArea script = g.GetComponent<SlimeArea>();
        if(script != null)
            script.Initialize(_damage, _duration, _slow, _type);
        g.layer = gameObject.layer;
        Destroy(gameObject);
    }
    
}
