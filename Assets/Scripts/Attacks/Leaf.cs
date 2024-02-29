using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;
    private List<GameObject> _previousCollisions = new List<GameObject>();

    public void Initialize(float damage, float knockback,float hitStun, Vector3 force, float sizeMultiplier, Elements.Type type)
    {
        _hitStun = hitStun;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        transform.localScale *= sizeMultiplier;
        rigidbody.AddForce(force, ForceMode.Impulse);
        _previousCollisions = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damage = other.gameObject.layer == LayerMask.NameToLayer("Player") ? 
            other.gameObject.GetComponentInParent<IDamageable>() : 
            other.gameObject.GetComponent<IDamageable>();
        if (damage != null && !_previousCollisions.Contains(other.gameObject))
        {
            _previousCollisions.Add(other.gameObject);
            damage.TakeDamage(_damage,_knockback*_force.normalized, _hitStun, _type);
        }
        if(LayerMask.NameToLayer("Walls") == other.gameObject.layer)
            Destroy(gameObject);
    }
}
