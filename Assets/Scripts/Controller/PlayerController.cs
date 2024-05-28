using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Controller.Form;
using Newtonsoft.Json.Linq;
using Systems.Modifiers;
using Systems.Save;
using UnityEngine;
using UnityEngine.InputSystem;
using Type = Elements.Type;

public class PlayerController : Character, ISavable
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private AttackDataDictionary attackDictionary;
    [SerializeField] private FormDataDictionary formDictionary;
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem walkingSmokeParticleSystem;
    [SerializeField] private ParticleSystem switchFormParticleSystem;
    
    private Vector2 _direction;
    private Vector2 _lastDirection;
    private bool _inKnockback = false;
    private PlayerInputActions _playerInputActions;
    private List<AttackData> _unlockedAttacks = new List<AttackData>();
    private List<Form> _initialForms = new List<Form>();
    private List<Form> _forms = new List<Form>();
    private int _maxFormCount;
    private Form _currentForm;
    private int _currentFormIndex = 0;

    public Vector2 MaxVelocity => _currentForm.MaxVelocity;
    public override float Health
    {
        get => _currentForm.Health;
        protected set => _currentForm.Health = value;
    }
    public override Type ElementType => _currentForm.ElementType;
    public override Vector3 SpellOffset => _currentForm.Data.SpellOffset;
    public PlayerInputActions PlayerInputActions => _playerInputActions;
    public override CharacterData CharacterData => playerData;
    public List<AttackData> UnlockedAttacks => _unlockedAttacks;
    public string id { get; } = "PlayerController";
    public Form CurrentForm => _currentForm; 
    public List<Form> Forms => _forms;
    public int MaxFormCount => _maxFormCount;
    
    public Action OnFormChange;
    public Action<Form, int> OnFormAdd;
    public Action<Form> OnFormRemove;
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

        _forms = new List<Form>();
        _maxFormCount = playerData.MaxFormCount;
        
        PlayerInputActions.Other.SwitchForms.started += SwitchForms;
        OnDamage += OnDamage;

        Speed = new ModifiableStat(playerData.Speed);

        string initialForm = PlayerPrefs.GetString("Initial Form");
        
        _lastDirection = Vector2.zero;
        switchFormParticleSystem.Stop();
        if (_initialForms.Count == 0)
        {
            _initialForms.Add(new Form(formDictionary.Dictionary[initialForm]));
        }
        
        InitializeForms();
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
                IItem item = col.GetComponent<IItem>();
                if (item != null)
                {
                    item.PickUp(this);
                    break;
                }

                CapturedCharacter character = col.gameObject.GetComponentInParent<CapturedCharacter>();
                if (character != null)
                {
                    Form oldForm = AddForm(character.Form);
                    if (oldForm != null)
                    {
                        character.SwitchCharacter(oldForm);
                    }
                    else
                    {
                        Destroy(character.gameObject);
                    }
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
        
        if (_playerInputActions.Other.BasicAttack.IsPressed() && _currentForm != null)
        {
            _currentForm.CastBasicAttack();
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
        FormFainted();
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
        if (index < 0 || index >= attacks.Count)
            return;
        bool attackInSlot = attacks[index] != null;
        if (attackInSlot && attacks[index].OnCooldown)
            return;
        if (attackInSlot)
        {
            UnEquipAttack(index);
        }
        var inputs = _playerInputActions.Spells.Get();
        attacks[index] = attackData.CreateInstance(this);
        OnAttackEquipped?.Invoke(attacks[index], index);
        attacks[index].LinkInput(inputs.actions[index]);
    }

    public void UnEquipAttack(int index)
    {
        if (index < 0 || index >= attacks.Count)
            return;
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
    private void FormChanged()
    {
        Speed.UpdateBaseValue(_currentForm.Speed);
        switchFormParticleSystem.Play();
    }

    private void FormAdded(Form form, int index)
    {
        foreach (AttackData attackData in form.Data.Spells)
        {
            UnlockAttack(attackData);
        }
    }

    private void FormRemoved(Form form)
    {
        foreach (AttackData attackData in form.Data.Spells)
        {
            int index = attacks.FindIndex(attack => attack?.Data == attackData);
            if (index != -1)
            {
                UnEquipAttack(index);
            }
            RemoveAttack(attackData);
        }
    }
    #endregion
    #region Form Methods
    private void InitializeForms()
    {
        _forms.Clear();
        foreach (Form form in _initialForms)
        {
            _forms.Add(form);
        }

        int i = 0;
        for (i = 0; i < _forms.Count; i++)
        {
            if (_forms[i].Health > 0)
            {
                ChangeForm(_forms[i], _forms.Count - 1);
                break;
            }
        }

        OnFormAdd?.Invoke(_forms[i], _forms.Count-1);
        FormAdded(_forms[i], _forms.Count-1);
    }
    private Form AddForm(Form form)
    {
        Form oldForm = null;
        if (_forms.Count >= _maxFormCount)
        {
            if (_forms.Count > 0)
            {
                oldForm = _currentForm;
                OnFormRemove?.Invoke(_currentForm);
                FormRemoved(_currentForm);
            }
            ChangeForm(form, _currentFormIndex);
            _forms[_currentFormIndex] = form;
            OnFormAdd?.Invoke(form, _currentFormIndex);
            FormAdded(form, _currentFormIndex);
        }
        else
        {
            _forms.Add(form);
            OnFormAdd?.Invoke(form, _forms.Count-1);
            FormAdded(form, _forms.Count-1);
        }

        return oldForm;
    }
    
    private void SwitchForms(InputAction.CallbackContext context)
    {
        int oldIndex = _currentFormIndex;
        int diff = (int)context.ReadValue<float>();
        int formIndex = _currentFormIndex+diff;
        if (formIndex >= _forms.Count)
        {
            formIndex = 0;
        }

        if (formIndex < 0)
        {
            formIndex = _forms.Count - 1;
        }
        
        while (formIndex != oldIndex)
        {
            if (_forms[formIndex].Health > 0)
            {
                ChangeForm(_forms[formIndex],formIndex);
                FormChanged();
                return;
            }

            formIndex += diff;
            if (formIndex >= _forms.Count)
            {
                formIndex = 0;
            }

            if (formIndex < 0)
            {
                formIndex = _forms.Count - 1;
            }
        }
    }

    private void HealForm(float amount)
    {
        if (Math.Abs(_currentForm.Health - _currentForm.Data.Health) > Mathf.Epsilon)
        {
            _currentForm.Health += amount;
        }
        else
        {
            foreach (var formInstance in _forms)
            {
                if (Math.Abs(_currentForm.Health - _currentForm.Data.Health) > Mathf.Epsilon)
                {
                    formInstance.Health += amount;
                }
            }
        }
    }

    private void HealForms(float amount)
    {
        foreach (var form in _forms)
        {
            form.Health += amount;
        }
    }
    
    private void FormFainted()
    {
        int index = 0;
        foreach (Form form in _forms)
        {
            if (form.Health > 0)
            {
                ChangeForm(form, index);
                return;
            }

            index++;
        }
        Destroy(gameObject);
    }
    
    private void ChangeForm(Form newForm, int newIndex)
    {
        if(_currentForm is not null)
            _currentForm.Drop();
        _currentForm = newForm;
        _currentFormIndex = newIndex;
        ChangeModel(newForm.Data);
        _currentForm.Equip(model, this);
        OnFormChange?.Invoke();
        FormChanged();
    }
    private void ChangeModel(FormData data)
    {
        model.SetActive(false);
        Destroy(model);
        model = Instantiate(data.Model, transform);
        model.layer = gameObject.layer;
    }
    #endregion
    #region Save Methods
    
    public object SaveState()
    {
        List<Form.SaveData> formSaveData = new List<Form.SaveData>();
        foreach (Form form in _forms)
        {
            formSaveData.Add(new Form.SaveData(formDictionary.Dictionary.First(i => i.Value == form.Data).Key, form.Health));
        }
        
        return new SaveData()
        {
            UnlockedAttacks = _unlockedAttacks.Select(x => attackDictionary.Dictionary.First(i => i.Value == x).Key).ToList(),
            Forms = formSaveData
        };
    }

    public void LoadState(JObject state)
    {
        var saveData = state.ToObject<SaveData>();
        _unlockedAttacks.Clear();
        foreach (string attack in saveData.UnlockedAttacks)
        {
            _unlockedAttacks.Add(attackDictionary.Dictionary[attack]);
        }
        
        _initialForms.Clear();
        foreach (Form.SaveData formData in saveData.Forms)
        {
            _initialForms.Add(new Form(formDictionary.Dictionary[formData.Form], formData.Health));
        }

        InitializeForms();
    }
    
    [Serializable]
    public struct SaveData
    {
        public List<string> UnlockedAttacks;
        public List<Form.SaveData> Forms;
    }
    #endregion
}