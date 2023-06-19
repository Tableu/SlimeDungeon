using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public Vector2 MaxVelocity;
    private PlayerInputActions _playerInputActions;
    private Vector2 _direction;

    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private Animator animator;
    private void Start()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Movement.Pressed.canceled += delegate(InputAction.CallbackContext context)
        {
            animator.SetFloat("Speed",0);
        };
        _playerInputActions.Movement.Direction.started += delegate(InputAction.CallbackContext context)
        {
            animator.SetFloat("Speed", 1);
        };
    }

    private void FixedUpdate()
    {
        _direction = _playerInputActions.Movement.Direction.ReadValue<Vector2>();
        
        if (_direction != Vector2.zero)
        {
            float rotation = (float)(Math.Atan2(_direction.x, _direction.y)*(180/Mathf.PI));
            transform.rotation = Quaternion.Euler(transform.rotation.x, rotation, transform.rotation.z);
            rigidbody.AddForce(new Vector3(_direction.x, 0, _direction.y), ForceMode.Impulse);
            if (Mathf.Abs(rigidbody.velocity.x) > MaxVelocity.x)
            {
                rigidbody.velocity = new Vector3(Mathf.Sign(rigidbody.velocity.x) * MaxVelocity.x, 0,
                    rigidbody.velocity.z);
            }

            if (Mathf.Abs(rigidbody.velocity.z) > MaxVelocity.y)
            {
                rigidbody.velocity =
                    new Vector3(rigidbody.velocity.x, 0, Mathf.Sign(rigidbody.velocity.z) * MaxVelocity.y);
            }
        }
    }
}
