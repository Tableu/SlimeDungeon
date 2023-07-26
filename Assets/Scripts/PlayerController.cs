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
    public override Vector3 SpellOffset => form.data.SpellOffset;

    private Vector2 _direction;
    private bool _inKnockback = false;
    
    [SerializeField] private Slider manaBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image formIcon;
    [SerializeField] private GameObject model;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private List<Icon> spellIcons;
    
    internal Form form;
    internal PlayerInputActions playerInputActions;
    internal override CharacterData CharacterData => playerData;

    private new void Start()
    {
        Armor = playerData.Armor;
        Mana = playerData.Mana;
        attacks = new List<Attack>();
        foreach (AttackData attackData in playerData.Attacks)
        {
            attackData.EquipAttack(this);
        }

        var i = 0;
        foreach (Attack attack in attacks)
        {
            attack.OnCooldown += spellIcons[i].OnCooldown;
            spellIcons[i].SetIcon(playerData.Attacks[i].Icon);
            i++;
        }

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        EquipForm(playerData.BaseForm);
        i = 0;
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
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("FormItems"));
            var orderedByProximity = colliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
                .ToArray();
            foreach (var col in orderedByProximity)
            {
                var absorbable = col.GetComponent<FormItem>();
                if (absorbable != null)
                {
                    absorbable.PickUp(this);
                    break;
                }
            }
        };
        manaBar.maxValue = playerData.Mana;
        healthBar.maxValue = form.health;
        
        if (manaBar.transform is RectTransform rt) 
            rt.sizeDelta = new Vector2(playerData.Mana * 2, rt.sizeDelta.y);
    }

    /*private void Update()
    {
        var mousePos = Mouse.current.position.ReadValue();
        var ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hitData, 1000, LayerMask.GetMask("Walls","Default")))
        {
            var diff = hitData.point - transform.position;
            var target = new Vector3(diff.x, transform.position.y, diff.z);
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, target, Mathf.Infinity, 0.0f));
        }
    }*/

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
            if (Mathf.Abs(rigidbody.velocity.x) > playerData.MaxVelocity.x)
            {
                rigidbody.velocity = new Vector3(Mathf.Sign(rigidbody.velocity.x) * playerData.MaxVelocity.x, 0,
                    rigidbody.velocity.z);
            }

            if (Mathf.Abs(rigidbody.velocity.z) > playerData.MaxVelocity.y)
            {
                rigidbody.velocity =
                    new Vector3(rigidbody.velocity.x, 0, Mathf.Sign(rigidbody.velocity.z) * playerData.MaxVelocity.y);
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
        i = 0;
        foreach (Attack attack in attacks)
        {
            attack.OnCooldown -= spellIcons[i].OnCooldown;
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
        if (form.data.GetType() == playerData.BaseForm.GetType())
        {
            Destroy(gameObject);
        }
        else
        {
            EquipForm(playerData.BaseForm);
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
        formIcon.sprite = formData.Icon;
        Health = form.health;
        healthBar.maxValue = form.health;
        healthBar.value = Health;
        if (healthBar.transform is RectTransform rt) 
            rt.sizeDelta = new Vector2(form.health * 2, rt.sizeDelta.y);
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