using Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using AnimationState = Controller.AnimationState;

[CreateAssetMenu(fileName = "Flamethrower Attack", menuName = "Attacks/Flamethrower Attack")]
public class FlamethrowerAttack : Attack
{
    [SerializeField] private GameObject flamethrowerPrefab;
    public float Offset;
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
        _flamethrower = Instantiate(flamethrowerPrefab, transform.position + Offset*transform.forward, Quaternion.identity,transform);
        _flamethrower.transform.rotation = Quaternion.Euler(_flamethrower.transform.rotation.x, character.transform.rotation.eulerAngles.y-90, _flamethrower.transform.rotation.z);
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
        Debug.Log("end");
        character.currentAttack = null;
        character.disableRotation = false;
        _inputActions.Attack.Primary.Enable();
        _inputActions.Movement.Pressed.Enable();
        character.Speed = _oldSpeed;
        Destroy(_flamethrower);
    }

    internal override void PassMessage(AnimationState state)
    {
        
    }
}
