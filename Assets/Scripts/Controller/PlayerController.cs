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
    [SerializeField] private AttackDataDictionary attackDictionary;
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem walkingSmokeParticleSystem;
    [SerializeField] private ParticleSystem switchFormParticleSystem;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private PartyController partyController;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private float itemPickupRange;

    private Vector2 _direction;
    private Vector2 _lastDirection;
    private bool _inKnockback = false;
    private bool _isMouseOverUI;
    private bool _basicAttackHeld;
    private List<AttackData> _unlockedAttacks = new List<AttackData>();
    private bool _loaded = false;

    private Character _currentCharacter;
    private bool _disableRotation = false;
    private PlayerInputActions _playerInputActions;
    private IItem _highlightedItem;

    public Vector2 MaxVelocity => _currentCharacter.MaxVelocity;
    public IItem HighlightedItem => _highlightedItem;
    public float Health
    {
        get => _currentCharacter.Health;
        private set => _currentCharacter.Health = value;
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
    public PlayerInputActions PlayerInputActions => _playerInputActions;
    public string id { get; } = "PlayerController";

    public Action<AttackData, bool> OnAttackUnlocked;
    public Action<AttackData> OnAttackRemoved;
    #region Unity Event Functions
    private void Awake()
    {
        Speed = new ModifiableStat(1);
        
        _lastDirection = Vector2.zero;
        switchFormParticleSystem.Stop();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        
        partyController.OnCharacterChanged += OnCharacterChanged;
        partyController.Initialize(_playerInputActions);
        EnemyMask = LayerMask.GetMask("Enemy");

        if (!_loaded)
        {
            _unlockedAttacks = new List<AttackData>();
            foreach (AttackData attackData in
                _currentCharacter.Data.StartingSpells) //Must be initialized in awake to properly trigger events
                UnlockAttack(attackData);
        }
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
            if (_highlightedItem != null)
            {
                _highlightedItem.PickUp(this);
                _highlightedItem.Highlight(false);
                _highlightedItem = null;
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
        
        PlayerInputActions.Other.Spell.started += delegate(InputAction.CallbackContext context)
        {
            if (_currentCharacter != null && !_isMouseOverUI)
            {
                _currentCharacter.CastSpell();
            }
        };
    }

    //Code for rotating the player to follow the mouse
    private void Update()
    {
        _isMouseOverUI = EventSystem.current.IsPointerOverGameObject();
        if (_disableRotation || _isMouseOverUI) return;
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
        
        //Check for nearby items
        Collider[] itemColliders = Physics.OverlapSphere(transform.position, itemPickupRange, LayerMask.GetMask("Items"));
        var itemsOrderedByProximity = itemColliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
            .ToArray();
        if (itemsOrderedByProximity.Length == 0)
        {
            _highlightedItem?.Highlight(false);
            _highlightedItem = null;
            return;
        }

        var col = itemsOrderedByProximity[0];
        IItem item = col.GetComponent<IItem>() ?? col.GetComponentInParent<IItem>();
        if (item != null && item.CanPickup())
        {
            if (_highlightedItem == item)
                return;
            _highlightedItem?.Highlight(false);
            item.Highlight(true);
            _highlightedItem = item;
        }
    }

    private void FixedUpdate()
    {
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

    private void OnEnable()
    {
        _playerInputActions.Enable();
        _disableRotation = false;
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
        _disableRotation = true;
    }

    private void OnDestroy()
    {
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
        if (partyController.IsPartyAllFainted())
        {
            levelManager.HandlePlayerDeath();
            Destroy(gameObject);
        }
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

    public void UnlockAttack(AttackData attackData)
    {
        if (!_unlockedAttacks.Contains(attackData))
        {
            _unlockedAttacks.Add(attackData);
            OnAttackUnlocked?.Invoke(attackData, false);
        }
    }

    public void RemoveAttack(AttackData attackData)
    {
        _unlockedAttacks.Remove(attackData);
        OnAttackRemoved?.Invoke(attackData);
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
        _currentCharacter.Equip(model, _playerInputActions);
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
        _loaded = true;
        var saveData = state.ToObject<SaveData>();
        _unlockedAttacks = new List<AttackData>();
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