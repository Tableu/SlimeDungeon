using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using Type = Elements.Type;

public class PlayerController : Character
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject model;
    
    private Vector2 _direction;
    private bool _inKnockback = false;
    
    private PlayerInputActions _playerInputActions;
    private List<AttackData> _unlockedAttacks;
    private FormManager _formManager;
    
    public override float Health
    {
        get => _formManager.CurrentForm.health;
        internal set => _formManager.CurrentForm.health = value;
    }
    public override float Speed => _formManager.CurrentForm.speed;
    public override Type ElementType => _formManager.CurrentForm.elementType;
    public override Vector3 SpellOffset => _formManager.CurrentForm.data.SpellOffset;
    public PlayerInputActions PlayerInputActions => _playerInputActions;
    public override CharacterData CharacterData => playerData;
    public List<AttackData> UnlockedAttacks => _unlockedAttacks;

    public FormManager FormManager => _formManager;
    
    public Action OnDeath;
    public Action OnFormFaint;
    public Action OnDamage;
    public Action<AttackData, int> OnAttackEquip;
    public Action<AttackData> OnAttackUnEquip;
    public Action<AttackData> OnAttackUnlocked;

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
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _formManager = new FormManager(this, model);
        _formManager.OnFormChange += OnFormChange;
        Armor = playerData.Armor;
        Mana = playerData.Mana;
    }

    private new void Start()
    {
        var i = 0;
        foreach (InputAction action in _playerInputActions.Spells.Get())
        {
            if (attacks.Count <= i)
                break;
            action.started += attacks[i].Begin;
            action.canceled += attacks[i].End;
            i++;
        }
        _playerInputActions.Other.PickUp.started += delegate(InputAction.CallbackContext context)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("Items"));
            var orderedByProximity = colliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
                .ToArray();
            foreach (var col in orderedByProximity)
            {
                var item = col.GetComponent<Item>();
                if (item != null)
                {
                    item.PickUp(this);
                    break;
                }
            }
        };
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
        OnDeath?.Invoke();
        int i = 0;
        foreach (InputAction action in _playerInputActions.Spells.Get())
        {
            if (attacks.Count <= i)
                break;
            action.started -= attacks[i].Begin;
            action.canceled -= attacks[i].End;
            i++;
        }
        _formManager.OnFormChange -= OnFormChange;
        _playerInputActions.Disable();
        _playerInputActions.Dispose();
    }
    
    public override void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
    {
        if (!_inKnockback)
        {
            base.TakeDamage(damage, knockback, hitStun, attackType);
            OnDamage?.Invoke();
        }
    }

    protected override void HandleDeath()
    {
        if (_formManager.CurrentForm.data.GetType() == playerData.BaseForm.GetType())
        {
            Destroy(gameObject);
        }
        else
        {
            OnFormFaint?.Invoke();
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

    public void UnlockAttack(AttackData attackData)
    {
        _unlockedAttacks.Add(attackData);
        OnAttackUnlocked?.Invoke(attackData);
    }

    private void OnFormChange()
    {
        Health = _formManager.CurrentForm.health;
    }

    public override void Attack()
    {
        _formManager.CurrentForm.Attack();
    }
}