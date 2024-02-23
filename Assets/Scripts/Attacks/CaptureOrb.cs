using UnityEngine;

public class CaptureOrb : MonoBehaviour
{
    [SerializeField] private ParticleSystem orb;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new SphereCollider collider;
    private float _hitStun;
    private Vector3 _force;
    
    public void Initialize(float hitStun, Vector3 force)
    {
        _hitStun = hitStun;
        _force = force;
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