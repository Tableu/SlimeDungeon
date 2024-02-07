using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Controller.Form;
using Systems.Modifiers;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Type = Elements.Type;

public class PlayerController : Character
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem walkingSmokeParticleSystem;
    
    private Vector2 _direction;
    private Vector2 _lastDirection;
    private bool _inKnockback = false;
    
    private PlayerInputActions _playerInputActions;
    private List<AttackData> _unlockedAttacks;
    private FormManager _formManager;

    public Vector2 MaxVelocity => _formManager.CurrentForm.MaxVelocity;
    public override float Health
    {
        get => _formManager.CurrentForm.Health;
        protected set => _formManager.CurrentForm.Health = value;
    }
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
        Mana = playerData.Mana;
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
        Speed = new ModifiableStat(_formManager.CurrentForm.Speed);
        
        _lastDirection = Vector2.zero;
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
        
        _playerInputActions.Movement.Pressed.started += delegate(InputAction.CallbackContext context)
        {
            walkingSmokeParticleSystem.Play();
        };
        
        _playerInputActions.Movement.Pressed.canceled += delegate(InputAction.CallbackContext context)
        {
            walkingSmokeParticleSystem.Stop();
        };
        
        _playerInputActions.Other.Absorb.started += delegate(InputAction.CallbackContext context)
        {
            Collider[] enemyColliders = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("Enemy"));
            var enemiesOrderedByProximity = enemyColliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
                .ToArray();
            foreach (var col in enemiesOrderedByProximity)
            {
                var enemy = col.GetComponent<EnemyController>();
                if (enemy != null && enemy.CharacterData is EnemyData data && enemy.CurrentState == EnemyControllerState.Stunned)
                {
                    if (data.FormData.CaptureDifficulty * enemy.Health < Random.Range(21,25))
                    {
                        FormManager.AddForm(new Form(data.FormData, this));
                        enemy.TakeDamage(1000, Vector3.zero, 0, Type.None);
                    }
                    break;
                }
            }
        };
        
        _playerInputActions.Other.PickUp.started += delegate(InputAction.CallbackContext context)
        {
            Collider[] itemColliders = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("Items"));
            var itemsOrderedByProximity = itemColliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
                .ToArray();
            foreach (var col in itemsOrderedByProximity)
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
        if (_inKnockback)
            return;
        _direction = _playerInputActions.Movement.Direction.ReadValue<Vector2>();

        if (_direction != Vector2.zero)
        {
            if (_lastDirection != _direction)
            {
                rigidbody.velocity = Vector3.zero;
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

            _lastDirection = _direction;
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
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
        base.OnAttackBegin(attack);
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
        Speed.UpdateBaseValue(_formManager.CurrentForm.Speed);
    }
    #endregion
}