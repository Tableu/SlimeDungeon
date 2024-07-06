using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Controller.Player;
using Newtonsoft.Json.Linq;
using Systems.Modifiers;
using Systems.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Type = Elements.Type;

public class PlayerController : MonoBehaviour, ICharacterInfo, ISavable, IDamageable
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private AttackDataDictionary attackDictionary;
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem walkingSmokeParticleSystem;
    [SerializeField] private ParticleSystem switchFormParticleSystem;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private PartyController partyController; 

    private List<Attack> _attacks;
    private Vector2 _direction;
    private Vector2 _lastDirection;
    private bool _inKnockback = false;
    private bool _isMouseOverUI;
    private bool _basicAttackHeld;
    private List<AttackData> _unlockedAttacks = new List<AttackData>();

    private Character _currentCharacter;
    private bool disableRotation = false;
    private PlayerInputActions _playerInputActions;

    public Vector2 MaxVelocity => _currentCharacter.MaxVelocity;
    public float Health
    {
        get => _currentCharacter.Health;
        private set => _currentCharacter.Health = value;
    }
    public float Mana
    {
        get;
        private set;
    }
    public ModifiableStat Speed
    {
        get;
        private set;
    }
    public LayerMask EnemyMask
    {
        get;
        private set;
    }
    
    public Transform Transform => transform;
    public Type ElementType => _currentCharacter.ElementType;
    public Vector3 SpellOffset => _currentCharacter.Data.SpellOffset;
    public List<AttackData> UnlockedAttacks => _unlockedAttacks;
    public PlayerData PlayerData => playerData;
    public PlayerInputActions PlayerInputActions => _playerInputActions;
    public string id { get; } = "PlayerController";

    public Action<Attack, int> OnAttackEquipped;
    public Action<Attack, int> OnAttackUnEquipped;
    public Action<AttackData> OnAttackUnlocked;
    public Action<AttackData> OnAttackRemoved;
    #region Unity Event Functions
    private void Awake()
    {
        Mana = playerData.Mana;
        Speed = new ModifiableStat(1);
        _attacks = new List<Attack>(new Attack[playerData.MaxSpellCount]);
        _unlockedAttacks = new List<AttackData>();
        
        _lastDirection = Vector2.zero;
        switchFormParticleSystem.Stop();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        
        partyController.OnCharacterChanged += OnCharacterChanged;
        partyController.OnPartyMemberAdded += OnPartyMemberAdded;
        partyController.Initialize(_playerInputActions);
        EnemyMask = LayerMask.GetMask("Enemy");
    }

    private void Start()
    {
        PlayerInputActions.Movement.Pressed.started += delegate(InputAction.CallbackContext context)
        {
            walkingSmokeParticleSystem.Play();
        };
        
        PlayerInputActions.Movement.Pressed.canceled += delegate(InputAction.CallbackContext context)
        {
            walkingSmokeParticleSystem.Stop();
        };
        
        PlayerInputActions.Other.PickUp.started += delegate(InputAction.CallbackContext context)
        {
            Collider[] itemColliders = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("Items"));
            var itemsOrderedByProximity = itemColliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
                .ToArray();
            if (itemsOrderedByProximity.Length == 0)
                return;
            var col = itemsOrderedByProximity[0];
            IItem item = col.GetComponent<IItem>();
            if (item != null)
            {
                item.PickUp(this);
            }
        };
        
        PlayerInputActions.Other.BasicAttack.started += delegate(InputAction.CallbackContext context)
        {
            if (_currentCharacter != null && !_isMouseOverUI)
            {
                _currentCharacter.CastBasicAttack();
                _basicAttackHeld = true;
            }
        };
        PlayerInputActions.Other.BasicAttack.canceled += delegate(InputAction.CallbackContext context)
        {
            _basicAttackHeld = false;
            _currentCharacter.BasicAttack.End();
        };
    }

    //Code for rotating the player to follow the mouse
    private void Update()
    {
        _isMouseOverUI = EventSystem.current.IsPointerOverGameObject();
        if (disableRotation) return;
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

    private void FixedUpdate()
    {
        Mana += playerData.ManaRegen;
        if (Mana > playerData.Mana)
        {
            Mana = playerData.Mana;
        }
        if (_inKnockback)
            return;
        _direction = PlayerInputActions.Movement.Direction.ReadValue<Vector2>();

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
        
        if (_currentCharacter != null && _basicAttackHeld)
        {
            _currentCharacter.CastBasicAttack();
        }
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
        disableRotation = true;
    }

    private void OnDestroy()
    {
        foreach (Attack attack in _attacks)
        {
            if (attack != null)
            {
                attack.UnlinkInput();
            }
        }
        PlayerInputActions.Disable();
        PlayerInputActions.Dispose();
    }
    #endregion
    public void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
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
            StartCoroutine(HandleKnockback(knockback, hitStun));
        }
    }

    private void HandleDeath()
    {
        partyController.CharacterFainted();
    }

    private IEnumerator HandleKnockback(Vector3 knockback, float hitStun)
    {
        _inKnockback = true;
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(knockback, ForceMode.Impulse);
        if (hitStun > 0)
        {
            PlayerInputActions.Disable();
            yield return new WaitForSeconds(hitStun);
            PlayerInputActions.Enable();
        }

        _inKnockback = false;
    }

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
        if (index < 0 || index >= _attacks.Count)
            return;
        bool attackInSlot = _attacks[index] != null;
        if (attackInSlot && _attacks[index].OnCooldown)
            return;
        if (attackInSlot)
        {
            UnEquipAttack(index);
        }
        var inputs = PlayerInputActions.Spells.Get();
        _attacks[index] = attackData.CreateInstance(this);
        OnAttackEquipped?.Invoke(_attacks[index], index);
        _attacks[index].LinkInput(inputs.actions[index]);
    }

    public void UnEquipAttack(int index)
    {
        if (index < 0 || index >= _attacks.Count)
            return;
        OnAttackUnEquipped?.Invoke(_attacks[index], index);
        _attacks[index].UnlinkInput();
        _attacks[index].CleanUp();
    }

    public void UnlockAttack(AttackData attackData)
    {
        if (!_unlockedAttacks.Contains(attackData))
        {
            _unlockedAttacks.Add(attackData);
            OnAttackUnlocked?.Invoke(attackData);
        }
    }

    public void RemoveAttack(AttackData attackData)
    {
        _unlockedAttacks.Remove(attackData);
        OnAttackRemoved?.Invoke(attackData);
    }

    public void ApplyManaCost(float manaCost)
    {
        Mana -= manaCost;
    }

    #region Event Functions
    private void OnCharacterChanged(Character character)
    {
        _currentCharacter = character;
        Speed.UpdateBaseValue(character.Speed);
        switchFormParticleSystem.Play();
        model.SetActive(false);
        Destroy(model);
        model = Instantiate(character.Data.Model, transform);
        model.layer = gameObject.layer;
        _currentCharacter.Equip(model, this);
    }

    private void OnPartyMemberAdded(Character character, int index)
    {
        foreach (AttackData attackData in character.Data.Spells)
        {
            UnlockAttack(attackData);
        }
    }
    #endregion
    
    #region Save Methods
    
    public object SaveState()
    {
        return new SaveData()
        {
            UnlockedAttacks = _unlockedAttacks.Select(x => attackDictionary.Dictionary.First(i => i.Value == x).Key).ToList()
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
    }
    
    [Serializable]
    public struct SaveData
    {
        public List<string> UnlockedAttacks;
    }
    #endregion
}