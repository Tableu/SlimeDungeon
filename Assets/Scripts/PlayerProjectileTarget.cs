using UnityEngine;

public class PlayerProjectileTarget : MonoBehaviour
{
    private GameObject _player;
    private Rigidbody _rigidbody;
    private void Start()
    {
        _player = GlobalReferences.Instance.Player;
        _rigidbody = GlobalReferences.Instance.Player.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_player != null && _rigidbody != null)
        {
            transform.position = Vector3.Lerp(transform.position,
                GlobalReferences.Instance.Player.transform.position +
                new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z) * 0.5f, Time.fixedDeltaTime * 2f);
        }
    }
}
