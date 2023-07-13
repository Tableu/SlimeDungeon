using System;
using System.Collections;
using Controller;
using Controller.Form;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Character
{
    public Vector2 MaxVelocity;
    private Vector2 _direction;
    private bool _inKnockback = false;
    
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Material originalMaterial;

    private new void Start()
    {
        base.Start();
        playerInputActions = GlobalReferences.Instance.PlayerInputActions;
        playerInputActions.Movement.Pressed.canceled += delegate(InputAction.CallbackContext context)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }
        };
        playerInputActions.Movement.Pressed.started += delegate(InputAction.CallbackContext context)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", characterData.Speed);
            }
        };
    }

    private void FixedUpdate()
    {
        _direction = playerInputActions.Movement.Direction.ReadValue<Vector2>();
        
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
        playerInputActions.Disable();
        playerInputActions.Dispose();
    }
    
    public override void TakeDamage(float damage, Vector3 knockback, float hitStun)
    {
        if(!_inKnockback)
            base.TakeDamage(damage,knockback, hitStun);
    }
    
    protected override IEnumerator ApplyKnockback(Vector3 knockback, float hitStun)
    {
        _inKnockback = true;
        playerInputActions.Disable();
        animator.applyRootMotion = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(knockback, ForceMode.Impulse);
        yield return new WaitForSeconds(characterData.HitStun);
        playerInputActions.Enable();
        _inKnockback = false;
        animator.applyRootMotion = true;
    }

    public void ChangeForms(Material material)
    {
        meshRenderer.material = material;
    }

    public void ResetForm()
    {
        meshRenderer.material = originalMaterial;
    }
    
    #if UNITY_EDITOR
    [SerializeField] private FireFormData data;
    [ContextMenu("Equip Fire Form")]
    public void EquipFireForm()
    {
        form = data.AttachScript(gameObject);
        form.Equip(this);
    }
    [ContextMenu("Remove Fire Form")]
    public void RemoveFireForm()
    {
        if (form is not null)
        {
            form.Drop();
            Destroy(form);
        }

        form = null;
    }
    #endif
}