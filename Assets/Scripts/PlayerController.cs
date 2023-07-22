using System;
using System.Collections;
using System.Linq;
using Controller;
using Controller.Form;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Type = Elements.Type;

public class PlayerController : Character
{
    private Vector2 _direction;
    private bool _inKnockback = false;
    
    [SerializeField] private Slider manaBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject model;
    
    internal Form form;
    internal PlayerInputActions playerInputActions;

    private PlayerData _playerData;

    private new void Start()
    {
        base.Start();
        _playerData = characterData as PlayerData;
        playerInputActions = GlobalReferences.Instance.PlayerInputActions;
        int i = 0;
        foreach (InputAction action in playerInputActions.Spells.Get())
        {
            if (attacks.Count <= i)
                break;
            action.started += attacks[i].Begin;
            action.canceled += attacks[i].End;
            i++;
        }
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
        playerInputActions.Other.Absorb.started += delegate(InputAction.CallbackContext context)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("Absorbables"));
            var orderedByProximity = colliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
                .ToArray();
            foreach (var col in orderedByProximity)
            {
                var absorbable = col.GetComponent<IAbsorbable>();
                if (absorbable != null)
                {
                    absorbable.Absorb(this);
                    break;
                }
            }
        };
        manaBar.maxValue = characterData.Mana;
        healthBar.maxValue = characterData.Health;
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        manaBar.value = Mana;
        healthBar.value = Health;
        _direction = playerInputActions.Movement.Direction.ReadValue<Vector2>();
        
        if (_direction != Vector2.zero && !_inKnockback)
        {
            if (!disableRotation)
            {
                float rotation = (float) (Math.Atan2(_direction.x, _direction.y) * (180 / Mathf.PI));
                transform.rotation = Quaternion.Euler(transform.rotation.x, rotation, transform.rotation.z);
            }

            rigidbody.AddForce(new Vector3(_direction.x*Speed, 0, _direction.y*Speed), ForceMode.Impulse);
            if (Mathf.Abs(rigidbody.velocity.x) > _playerData.MaxVelocity.x)
            {
                rigidbody.velocity = new Vector3(Mathf.Sign(rigidbody.velocity.x) * _playerData.MaxVelocity.x, 0,
                    rigidbody.velocity.z);
            }

            if (Mathf.Abs(rigidbody.velocity.z) > _playerData.MaxVelocity.y)
            {
                rigidbody.velocity =
                    new Vector3(rigidbody.velocity.x, 0, Mathf.Sign(rigidbody.velocity.z) * _playerData.MaxVelocity.y);
            }
        }
    }

    private void OnDestroy()
    {
        int i = 0;
        foreach (InputAction action in playerInputActions.Spells.Get())
        {
            if (attacks.Count <= i)
                break;
            action.started -= attacks[i].Begin;
            action.canceled -= attacks[i].End;
            i++;
        }
        healthBar.value = Health;
        playerInputActions.Disable();
        playerInputActions.Dispose();
    }
    
    public override void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
    {
        if(!_inKnockback)
            base.TakeDamage(damage,knockback, hitStun,attackType);
    }
    
    protected override IEnumerator ApplyKnockback(Vector3 knockback, float hitStun)
    {
        _inKnockback = true;
        playerInputActions.Disable();
        animator.applyRootMotion = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(knockback, ForceMode.Impulse);
        yield return new WaitForSeconds(hitStun);
        playerInputActions.Enable();
        _inKnockback = false;
        animator.applyRootMotion = true;
    }

    public void EquipForm(FormData formData)
    {
        DropForm();
        form = formData.AttachScript(gameObject);
        form.Equip(this);
    }

    public void DropForm()
    {
        if (form is not null)
        {
            form.Drop();
            Destroy(form);
        }
        form = null;
        elementType = Type.None;
    }

    public void ChangeModel(FormData data)
    {
        DestroyImmediate(model);
        model = Instantiate(data.Model, transform);
        animator.avatar = data.Avatar;
        animator.runtimeAnimatorController = data.AnimatorController;
        model.layer = gameObject.layer;
    }
}