using UnityEngine;

public class CaptureOrb : MonoBehaviour, BasicProjectile
{
    [SerializeField] private new Rigidbody rigidbody;
    private float _hitStun;
    
    public void Initialize(float damage, float knockback, float hitStun, Vector3 force, Elements.Type type)
    {
        _hitStun = hitStun;
        rigidbody.AddForce(force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        ICapturable capturable = other.gameObject.GetComponent<ICapturable>();
        if (capturable != null)
        {
            capturable.AttemptCapture(_hitStun);
        }
        Destroy(gameObject);
    }
}