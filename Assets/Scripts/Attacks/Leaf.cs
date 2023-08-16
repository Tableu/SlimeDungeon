using System;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    private float _damage;
    private float _knockback;
    private float _hitStun;
    private Vector3 _force;
    private Elements.Type _type;

    public void Initialize(float damage, float knockback,float hitStun, Vector3 force, float sizeMultiplier, Elements.Type type)
    {
        _hitStun = hitStun;
        _damage = damage;
        _knockback = knockback;
        _force = force;
        _type = type;
        transform.localScale *= sizeMultiplier;
        rigidbody.AddForce(force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damage = other.gameObject.GetComponent<IDamageable>();
        if (damage != null)
        {
            damage.TakeDamage(_damage,_knockback*_force.normalized, _hitStun, _type);
        }
        if(LayerMask.NameToLayer("Walls") == other.gameObject.layer)
            Destroy(gameObject);
    }
}
