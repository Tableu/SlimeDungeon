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
        get => _formManager.CurrentForm.Health;
        internal set => _formManager.CurrentForm.Health = value;
    }
    public override float Speed => _formManager.CurrentForm.Speed;
    public override Type ElementType => _formManager.CurrentForm.ElementType;
    public override Vector3 SpellOffset => _formManager.CurrentForm.Data.SpellOffset;
    public PlayerInputActions PlayerInputActions => _playerInputActions;
    public override CharacterData CharacterData => playerData;
    public List<AttackData> UnlockedAttacks => _unlockedAttacks;
    public FormManager FormManager => _formManager;
    
    public Action OnFormFaint;
    public Action OnDamage;
    public Action<AttackData, int> OnAttackEquip;
    public Action<AttackData> OnAttackUnEquip;
    public Action<AttackData> OnAttackUnlocked;
    #region Unity Event Functions
    private void Awake()
    {
        attacks = new List<Attack>();
        _unlockedAttacks = new List<AttackData>();
        
        foreach (AttackData attackData in playerData.Attacks)
        {
            attacks.Add(attackData.EquipAttack(this));
            _unlockedAttacks.Add(attackData);
        }
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _formManager = new FormManager(this, model);
        _formManager.OnFormChange += OnFormChange;
        Mana = playerData.Mana;
    }
    
    private new void Start()
    {
        var i = 0;
        foreach (InputAction action in _playerInputActions.Spells.Get())
        {
            if (attacks.Count <= i)
                break;
            LinkInput(action, attacks[i]);
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
    private void Update()
    {
        if (!disableRotation)
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hitData, 1000, LayerMask.GetMask("Walls", "Default")))
            {
                var diff = hitData.point - transform.position;
                var target = new Vector3(diff.x, transform.position.y, diff.z);
                transform.rotation =
                    Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, target, Mathf.Infinity, 0.0f));
                transform.forward = new Vector3(target.x, 0, target.z).normalized;
            }
        }
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        _direction = _playerInputActions.Movement.Direction.ReadValue<Vector2>();
        
        if (_direction != Vector2.zero && !_inKnockback)
        {
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
            UnlinkInput(action, attacks[i]);
            i++;
        }
        _formManager.OnFormChange -= OnFormChange;
        _playerInputActions.Disable();
        _playerInputActions.Dispose();
    }
    #endregion
    #region Base Class Overrides
    public override void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
    {
        if (!_inKnockback)
        {
            float typeMultiplier = GlobalReferences.Instance.TypeManager.GetTypeMultiplier(ElementType, attackType);
            Health -= damage*typeMultiplier;
            if (Health <= 0)
            {
                HandleDeath();
                return;
            }
            StartCoroutine(ApplyKnockback(knockback, hitStun));
            OnDamage?.Invoke();
        }
    }

    protected override void HandleDeath()
    {
        if (_formManager.CurrentForm.Data.GetType() == playerData.StartForm.GetType())
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
    
    protected override void OnAttackBegin(Attack attack)
    {
        currentAttack = attack;
    }

    #endregion
    public void EquipAttack(AttackData attackData, int index)
    {
        OnAttackUnEquip?.Invoke(attacks[index].Data);
        var inputs = _playerInputActions.Spells.Get();
        UnlinkInput(inputs.actions[index], attacks[index]);
        attacks[index].CleanUp();
        currentAttack = null;
        attacks.RemoveAt(index);
        attacks.Insert(index, attackData.EquipAttack(this));
        
        OnAttackEquip?.Invoke(attackData, index);
        LinkInput(inputs.actions[index], attacks[index]);
    }

    public void UnlockAttack(AttackData attackData)
    {
        _unlockedAttacks.Add(attackData);
        OnAttackUnlocked?.Invoke(attackData);
    }

    public void LinkInput(InputAction action, Attack attack)
    {
        action.started += attack.Begin;
        action.canceled += attack.End;
        attack.OnBegin += OnAttackBegin;
        attack.OnEnd += OnAttackEnd;
    }

    public void UnlinkInput(InputAction action, Attack attack)
    {
        action.started -= attack.Begin;
        action.canceled -= attack.End;
        attack.OnBegin -= OnAttackBegin;
        attack.OnEnd -= OnAttackEnd;
    }
    
    #region Event Functions
    private void OnFormChange()
    {
        Health = _formManager.CurrentForm.Health;
    }
    #endregion
}