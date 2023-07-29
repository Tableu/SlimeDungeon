using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Controller.Form;
using UnityEngine;
using UnityEngine.InputSystem;
using Type = Elements.Type;

public class PlayerController : Character
{
    [SerializeField] private GameObject model;
    [SerializeField] private PlayerData playerData;
    
    private Vector2 _direction;
    private bool _inKnockback = false;
    private Form _currentForm;
    private SavedForm _currentSavedForm;
    private List<SavedForm> _forms;
    private int _maxFormCount;
    private int _formIndex;
    private PlayerInputActions _playerInputActions;
    private List<AttackData> _unlockedAttacks;
    
    public override float Health
    {
        get => _currentForm.health;
        internal set => _currentForm.health = value;
    }
    public override float Speed => _currentForm.speed;
    public override Type ElementType => _currentForm.elementType;
    public override Vector3 SpellOffset => _currentForm.data.SpellOffset;
    public Form CurrentForm => _currentForm;
    public List<SavedForm> Forms => _forms;
    public PlayerInputActions PlayerInputActions => _playerInputActions;
    public int MaxFormCount => _maxFormCount;

    public override CharacterData CharacterData => playerData;
    public List<AttackData> UnlockedAttacks => _unlockedAttacks;

    public Action OnFormChange;
    public Action<SavedForm, int> OnFormAdd;
    public Action OnDeath;
    public Action<AttackData, int> OnAttackEquip;
    public Action<AttackData> OnAttackUnEquip;
    public Action<AttackData> OnAttackUnlocked;

    private void Awake()
    {
        attacks = new List<Attack>();
        _forms = new List<SavedForm>();
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
        _formIndex = 0;
        EquipForm(playerData.BaseForm);
        Armor = playerData.Armor;
        Mana = playerData.Mana;
        _maxFormCount = playerData.MaxFormCount;
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
        _playerInputActions.Other.Absorb.started += delegate(InputAction.CallbackContext context)
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
        _playerInputActions.Disable();
        _playerInputActions.Dispose();
    }
    
    public override void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
    {
        if (!_inKnockback)
        {
            base.TakeDamage(damage, knockback, hitStun, attackType);
            _currentSavedForm.Health = Health;
        }
    }

    protected override void HandleDeath()
    {
        if (_currentForm.data.GetType() == playerData.BaseForm.GetType())
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
        if (_forms.Count >= _maxFormCount)
        {
            if (_currentForm is not null)
            {
                _currentForm.Drop();
                Destroy(_currentForm);
            }
            if(_forms.Count > 0)
                _forms.RemoveAt(_formIndex);
            ChangeModel(formData);
            SavedForm savedForm = new SavedForm(formData);
            _forms.Insert(_formIndex, savedForm);
            _currentSavedForm = savedForm;
            _currentForm = formData.AttachScript(model);
            _currentForm.Equip(this);
            Health = _currentForm.health;
            OnFormChange?.Invoke();
            OnFormAdd?.Invoke(savedForm, _formIndex);
        }
        else
        {
            SavedForm savedForm = new SavedForm(formData);
            _forms.Add(savedForm);
            OnFormAdd?.Invoke(savedForm, _forms.Count-1);
        }
        
    }
    
    public void SwitchForms(int direction)
    {
        
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

    public override void Attack()
    {
        _currentForm.Attack();
    }

    private void ChangeModel(FormData data)
    {
        model.SetActive(false);
        Destroy(model);
        model = Instantiate(data.Model, transform);
        model.layer = gameObject.layer;
    }
}