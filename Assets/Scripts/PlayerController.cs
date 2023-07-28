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
    [SerializeField] private Slider manaBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject model;
    [SerializeField] private PlayerData playerData;
    
    private Vector2 _direction;
    private bool _inKnockback = false;
    private Form _form;
    private PlayerInputActions _playerInputActions;
    private List<AttackData> _unlockedAttacks;
    
    public override float Health
    {
        get => _form.health;
        internal set => _form.health = value;
    }
    public override float Speed => _form.speed;
    public override Type ElementType => _form.elementType;
    public override Vector3 SpellOffset => _form.data.SpellOffset;
    public Form Form => _form;
    public PlayerInputActions PlayerInputActions => _playerInputActions;

    public override CharacterData CharacterData => playerData;
    public List<AttackData> UnlockedAttacks => _unlockedAttacks;

    public Action OnFormChange;
    public Action<AttackData, int> OnAttackEquip;
    public Action<AttackData> OnAttackUnEquip;

    private void Awake()
    {
        attacks = new List<Attack>();
        _unlockedAttacks = new List<AttackData>();
        var i = 0;
        foreach (AttackData attackData in playerData.Attacks)
        {
            attackData.EquipAttack(this, i);
            _unlockedAttacks.Add(attackData);
            i++;
        }
    }

    private new void Start()
    {
        Armor = playerData.Armor;
        Mana = playerData.Mana;
        
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        EquipForm(playerData.BaseForm);
        var i = 0;
        foreach (InputAction action in _playerInputActions.Spells.Get())
        {
            if (attacks.Count <= i)
                break;
            action.started += attacks[i].Begin;
            action.canceled += attacks[i].End;
            i++;
        }
        _playerInputActions.Other.Absorb.started += delegate(InputAction.CallbackContext context)
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
        healthBar.maxValue = _form.health;
        
        if (manaBar.transform is RectTransform rt) 
            rt.sizeDelta = new Vector2(playerData.Mana * 2, rt.sizeDelta.y);
    }

    //Code for rotating the player to follow the mouse
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
        _direction = _playerInputActions.Movement.Direction.ReadValue<Vector2>();
        
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
        foreach (InputAction action in _playerInputActions.Spells.Get())
        {
            if (attacks.Count <= i)
                break;
            action.started -= attacks[i].Begin;
            action.canceled -= attacks[i].End;
            i++;
        }
        healthBar.value = Health;
        _playerInputActions.Disable();
        _playerInputActions.Dispose();
    }
    
    public override void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
    {
        if(!_inKnockback)
            base.TakeDamage(damage,knockback, hitStun,attackType);
    }

    protected override void OnDeath()
    {
        if (_form.data.GetType() == playerData.BaseForm.GetType())
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
        _playerInputActions.Disable();
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(knockback, ForceMode.Impulse);
        yield return new WaitForSeconds(hitStun);
        _playerInputActions.Enable();
        _inKnockback = false;
    }

    public void EquipForm(FormData formData)
    {
        if (_form is not null)
        {
            _form.Drop();
            Destroy(_form);
        }
        ChangeModel(formData);
        _form = formData.AttachScript(model);
        _form.Equip(this);
        OnFormChange?.Invoke();
        Health = _form.health;
        healthBar.maxValue = _form.health;
        healthBar.value = Health;
        if (healthBar.transform is RectTransform rt) 
            rt.sizeDelta = new Vector2(_form.health * 2, rt.sizeDelta.y);
    }

    public void EquipAttack(AttackData attackData, int index)
    {
        OnAttackUnEquip?.Invoke(attacks[index].Data);
        var inputs = _playerInputActions.Spells.Get();
        inputs.actions[index].started -= attacks[index].Begin;
        inputs.actions[index].canceled -= attacks[index].End;
        attacks[index].CleanUp();
        
        attackData.EquipAttack(this, index);
        
        OnAttackEquip?.Invoke(attackData, index);
        inputs.actions[index].started += attacks[index].Begin;
        inputs.actions[index].canceled += attacks[index].End;
    }

    public override void Attack()
    {
        _form.Attack();
    }

    private void ChangeModel(FormData data)
    {
        model.SetActive(false);
        Destroy(model);
        model = Instantiate(data.Model, transform);
        model.layer = gameObject.layer;
    }
}