using System.Collections;
using System.Linq;
using Controller;
using Controller.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem walkingSmokeParticleSystem;
    [SerializeField] private ParticleSystem switchFormParticleSystem;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private PartyController partyController;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private float itemPickupRange;

    private Vector2 _direction;
    private Vector2 _lastDirection;
    private bool _inKnockback = false;
    private bool _isMouseOverUI;
    private bool _basicAttackHeld;

    private Character _currentCharacter;
    private bool _disableRotation = false;
    private PlayerInputActions _playerInputActions;
    private IItem _highlightedItem;

    public Vector2 MaxVelocity => _currentCharacter.MaxVelocity;
    public IItem HighlightedItem => _highlightedItem;
    public CharacterStats Stats => _currentCharacter.Stats;
    
    public PlayerInputActions PlayerInputActions => _playerInputActions;
    #region Unity Event Functions

    public void Initialize()
    {
        _lastDirection = Vector2.zero;
        switchFormParticleSystem.Stop();
        
        if (_playerInputActions == null)
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
        }

        partyController.OnCharacterChanged += OnCharacterChanged;
    }
    
    private void Awake()
    {
        if (_playerInputActions == null)
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
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
        
        PlayerInputActions.Combat.PickUp.started += delegate(InputAction.CallbackContext context)
        {
            if (_highlightedItem != null)
            {
                _highlightedItem.PickUp(this, inventoryController, partyController);
                _highlightedItem.Highlight(false);
                _highlightedItem = null;
            }
        };
        
        PlayerInputActions.Combat.BasicAttack.started += delegate(InputAction.CallbackContext context)
        {
            if (_currentCharacter != null && !_isMouseOverUI)
            {
                _currentCharacter.CastBasicAttack();
                _basicAttackHeld = true;
            }
        };
        
        PlayerInputActions.Combat.BasicAttack.canceled += delegate(InputAction.CallbackContext context)
        {
            _basicAttackHeld = false;
            if(_currentCharacter != null)
                _currentCharacter.BasicAttack.End();
        };
        
        PlayerInputActions.Combat.Spell.started += delegate(InputAction.CallbackContext context)
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
        Vector2 mousePos = Input.mousePosition;
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
            
            rigidbody.AddForce(new Vector3(_direction.x*Stats.Speed, 0, _direction.y*Stats.Speed), ForceMode.Impulse);
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
        _playerInputActions.Combat.Enable();
        _playerInputActions.Movement.Enable();
        _disableRotation = false;
    }

    private void OnDisable()
    {
        _playerInputActions.Combat.Disable();
        _playerInputActions.Movement.Disable();
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
            _currentCharacter.ApplyDamage(damage, attackType);
            if (Stats.Health <= 0)
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

    #region Event Functions
    private void OnCharacterChanged(Character character)
    {
        _currentCharacter = character;
        switchFormParticleSystem.Play();
        model.SetActive(false);
        Destroy(model);
        model = Instantiate(character.Data.Model, transform);
        model.layer = gameObject.layer;
        _currentCharacter.Equip(model, _playerInputActions);
    }
    #endregion
}