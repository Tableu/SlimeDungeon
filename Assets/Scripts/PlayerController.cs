using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Controller.Form;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Type = Elements.Type;

public class PlayerController : Character
{
    public override float Health
    {
        get => form.health;
        internal set => form.health = value;
    }
    public override float Speed => form.speed;
    public override Type ElementType => form.elementType;
    
    private Vector2 _direction;
    private bool _inKnockback = false;
    
    [SerializeField] private Slider manaBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject model;
    
    internal Form form;
    internal PlayerInputActions playerInputActions;
    //todo merge playerdata and enemy data parameters into characterdata
    private PlayerData _playerData;

    private new void Start()
    {
        Armor = characterData.Armor;
        Mana = characterData.Mana;
        attacks = new List<Attack>();
        foreach (AttackData attackData in characterData.Attacks)
        {
            attackData.EquipAttack(this);
        }
        _playerData = characterData as PlayerData;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        EquipForm(_playerData.BaseForm);
        int i = 0;
        foreach (InputAction action in playerInputActions.Spells.Get())
        {
            if (attacks.Count <= i)
                break;
            action.started += attacks[i].Begin;
            action.canceled += attacks[i].End;
            i++;
        }
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
        healthBar.maxValue = form.health;
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

    protected override void OnDeath()
    {
        if (form.data.GetType() == _playerData.BaseForm.GetType())
        {
            Destroy(gameObject);
        }
        else
        {
            EquipForm(_playerData.BaseForm);
        }
    }

    protected override IEnumerator ApplyKnockback(Vector3 knockback, float hitStun)
    {
        _inKnockback = true;
        playerInputActions.Disable();
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(knockback, ForceMode.Impulse);
        yield return new WaitForSeconds(hitStun);
        playerInputActions.Enable();
        _inKnockback = false;
    }

    public void EquipForm(FormData formData)
    {
        if (form is not null)
        {
            form.Drop();
            Destroy(form);
        }
        ChangeModel(formData);
        form = formData.AttachScript(model);
        form.Equip(this);
        Health = form.health;
        healthBar.maxValue = form.health;
        healthBar.value = Health;
    }

    public override void Attack()
    {
        form.Attack();
    }

    private void ChangeModel(FormData data)
    {
        model.SetActive(false);
        Destroy(model);
        model = Instantiate(data.Model, transform);
        model.layer = gameObject.layer;
    }
}