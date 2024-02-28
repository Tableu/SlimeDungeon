using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Controller.Form;
using Systems.Modifiers;
using UnityEngine;
using UnityEngine.InputSystem;
using Type = Elements.Type;

public class PlayerController : Character
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem walkingSmokeParticleSystem;
    [SerializeField] private ParticleSystem switchFormParticleSystem;
    
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
    public Action<Attack, int> OnAttackEquipped;
    public Action<Attack, int> OnAttackUnEquipped;
    public Action<AttackData> OnAttackUnlocked;
    public Action<AttackData> OnAttackRemoved;
    #region Unity Event Functions
    private void Awake()
    {
        Mana = playerData.Mana;
        attacks = new List<Attack>(new Attack[playerData.MaxSpellCount]);
        _unlockedAttacks = new List<AttackData>();
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _formManager = new FormManager(this, model);
        _formManager.OnFormChange += OnFormChange;
        _formManager.OnFormAdd += OnFormAdd;
        _formManager.OnFormRemoved += OnFormRemoved;
        
        Speed = new ModifiableStat(playerData.Speed);
        foreach (AttackData attackData in playerData.Attacks)
        {
            _unlockedAttacks.Add(attackData);
        }
        _lastDirection = Vector2.zero;
        _formManager.InitializeForm();
        switchFormParticleSystem.Stop();
    }
    
    private new void Start()
    {
        _playerInputActions.Movement.Pressed.started += delegate(InputAction.CallbackContext context)
        {
            walkingSmokeParticleSystem.Play();
        };
        
        _playerInputActions.Movement.Pressed.canceled += delegate(InputAction.CallbackContext context)
        {
            walkingSmokeParticleSystem.Stop();
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
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hitData, 1000, LayerMask.GetMask("Walls", "Default", "Floor")))
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
        
        if (_playerInputActions.Other.BasicAttack.IsPressed() && _formManager.CurrentForm != null && _formManager.CurrentForm.BasicAttack != null)
        {
            _formManager.CurrentForm.BasicAttack.Begin();
        }
    }

    private void OnDestroy()
    {
        OnDeath?.Invoke();

        foreach (Attack attack in attacks)
        {
            if (attack != null)
            {
                attack.UnlinkInput();
            }
        }
        _formManager.OnFormChange -= OnFormChange;
        _formManager.OnFormAdd -= OnFormAdd;
        _formManager.OnFormRemoved -= OnFormRemoved;
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
            StartCoroutine(HandleKnockback(knockback, hitStun, typeMultiplier));
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

    protected override IEnumerator HandleKnockback(Vector3 knockback, float hitStun, float typeMultiplier)
    {
        _inKnockback = true;
        _playerInputActions.Disable();
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(knockback, ForceMode.Impulse);
        yield return new WaitForSeconds(hitStun);
        _playerInputActions.Enable();
        _inKnockback = false;
    }
    #endregion

    public void InitializeAttacks()
    {
        var i = 0;
        foreach (AttackData attackData in _unlockedAttacks)
        {
            EquipAttack(attackData,i);
            if (i >= playerData.MaxSpellCount)
                break;
            i++;
        }
    }
    
    public void EquipAttack(AttackData attackData, int index)
    {
        var inputs = _playerInputActions.Spells.Get();
        if (attacks[index] != null)
        {
            attacks[index].UnlinkInput();
            attacks[index].CleanUp();
        }
        attacks[index] = attackData.CreateInstance(this);
        OnAttackEquipped?.Invoke(attacks[index], index);
        
        attacks[index].LinkInput(inputs.actions[index]);
    }

    public void UnEquipAttack(int index)
    {
        OnAttackUnEquipped?.Invoke(attacks[index], index);
        attacks[index].UnlinkInput();
        attacks[index].CleanUp();
    }

    public void UnlockAttack(AttackData attackData)
    {
        _unlockedAttacks.Add(attackData);
        OnAttackUnlocked?.Invoke(attackData);
    }

    public void RemoveAttack(AttackData attackData)
    {
        _unlockedAttacks.Remove(attackData);
        OnAttackRemoved?.Invoke(attackData);
    }

    #region Event Functions
    private void OnFormChange()
    {
        Health = _formManager.CurrentForm.Health;
        Speed.UpdateBaseValue(_formManager.CurrentForm.Speed);
        switchFormParticleSystem.Play();
    }

    private void OnFormAdd(Form form, int index)
    {
        foreach (AttackData attackData in form.Data.Spells)
        {
            UnlockAttack(attackData);
        }
    }

    private void OnFormRemoved(Form form)
    {
        foreach (AttackData attackData in form.Data.Spells)
        {
            int index = attacks.FindIndex(attack => attack.Data == attackData);
            if (index != -1)
            {
                UnEquipAttack(index);
            }
            RemoveAttack(attackData);
        }
    }
    #endregion
}