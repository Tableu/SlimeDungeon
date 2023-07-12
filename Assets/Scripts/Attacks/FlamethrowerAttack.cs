using Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using AnimationState = Controller.AnimationState;

[CreateAssetMenu(fileName = "Flamethrower Attack", menuName = "Attacks/Flamethrower Attack")]
public class FlamethrowerAttack : Attack
{
    [SerializeField] private GameObject flamethrowerPrefab;
    [SerializeField] private float offset;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float knockback;
    [SerializeField] private float hitStun;
    private GameObject _flamethrower;
    private float _oldSpeed;
    private PlayerInputActions _inputActions;
    public override void Equip(Character character)
    {
        base.character = character;
        _inputActions = character.playerInputActions;
        if (character.isPlayer && _inputActions != null)
        {
            _inputActions.Attack.Secondary.started += Begin;
            _inputActions.Attack.Secondary.canceled += End;
        }
    }

    public override void Drop()
    {
        if (character.isPlayer && _inputActions != null)
        {
            _inputActions.Attack.Secondary.started -= Begin;
            _inputActions.Attack.Secondary.canceled -= End;
        }

        character.currentAttack = null;
    }

    public override void Begin(InputAction.CallbackContext callbackContext)
    {
        character.currentAttack = this;
        Transform transform = character.transform;
        _flamethrower = Instantiate(flamethrowerPrefab, transform.position + offset*transform.forward, Quaternion.identity,transform);
        _flamethrower.transform.rotation = Quaternion.Euler(_flamethrower.transform.rotation.x, character.transform.rotation.eulerAngles.y-90, _flamethrower.transform.rotation.z);
        var script = _flamethrower.GetComponent<Flamethrower>();
        script.Initialize(damage*character.form.damageMultiplier, knockback, hitStun,transform.forward*speed*character.form.speedMultiplier, character.form.sizeMultiplier);
        _oldSpeed = character.Speed;
        character.Speed = 0.5f;
        character.disableRotation = true;
        _inputActions.Attack.Primary.Disable();
        _inputActions.Movement.Pressed.Disable();
        character.animator.SetFloat("Speed",0);
        OnSpellCast?.Invoke();
    }

    private void End(InputAction.CallbackContext context)
    {
        End();
    }

    public override void End()
    {
        character.currentAttack = null;
        character.disableRotation = false;
        _inputActions.Attack.Primary.Enable();
        _inputActions.Movement.Pressed.Enable();
        character.Speed = _oldSpeed;
        if (_inputActions.Movement.Direction.ReadValue<Vector2>() != Vector2.zero)
        {
            character.animator.SetFloat("Speed", 1);
        }
        _flamethrower.GetComponent<ParticleSystem>().Stop();
        _flamethrower.transform.SetParent(null, true);
    }

    internal override void PassMessage(AnimationState state)
    {
        
    }
}
