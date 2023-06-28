using System;
using System.Collections;
using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Character
{
    public Vector2 MaxVelocity;
    private PlayerInputActions _playerInputActions;
    private Vector2 _direction;
    private bool _inKnockback = false;
    
    [SerializeField] private Attack fireballAttack;
    [SerializeField] private Attack flamethrowerAttack;

    private new void Start()
    {
        base.Start();
        _playerInputActions = GlobalReferences.Instance.PlayerInputActions;
        _playerInputActions.Movement.Pressed.canceled += delegate(InputAction.CallbackContext context)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }
        };
        _playerInputActions.Movement.Pressed.started += delegate(InputAction.CallbackContext context)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", characterData.Speed);
            }
        };
        fireballAttack.Equip(this, _playerInputActions);
        flamethrowerAttack.Equip(this, _playerInputActions);
    }

    private void FixedUpdate()
    {
        _direction = _playerInputActions.Movement.Direction.ReadValue<Vector2>();
        
        if (_direction != Vector2.zero && !_inKnockback)
        {
            if (!disableRotation)
            {
                float rotation = (float) (Math.Atan2(_direction.x, _direction.y) * (180 / Mathf.PI));
                transform.rotation = Quaternion.Euler(transform.rotation.x, rotation, transform.rotation.z);
            }

            rigidbody.AddForce(new Vector3(_direction.x*Speed, 0, _direction.y*Speed), ForceMode.Impulse);
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

    private void OnDestroy()
    {
        fireballAttack.Drop();
        flamethrowerAttack.Drop();
    }
    
    public override void TakeDamage(float damage, Vector3 knockback)
    {
        if(!_inKnockback)
            base.TakeDamage(damage,knockback);
    }
    
    protected override IEnumerator ApplyKnockback(Vector3 knockback)
    {
        _inKnockback = true;
        _playerInputActions.Disable();
        animator.applyRootMotion = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(knockback, ForceMode.Impulse);
        yield return new WaitForSeconds(characterData.HitStun);
        _playerInputActions.Enable();
        _inKnockback = false;
        animator.applyRootMotion = true;
    }
}