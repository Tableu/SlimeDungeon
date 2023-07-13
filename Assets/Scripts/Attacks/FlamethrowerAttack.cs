using Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using AnimationState = Controller.AnimationState;

public class FlamethrowerAttack : Attack
{
    private GameObject _flamethrower;
    private float _oldSpeed;

    public override void Begin(InputAction.CallbackContext callbackContext)
    {
        character.currentAttack = this;
        Transform transform = character.transform;
        _flamethrower = GameObject.Instantiate(data.Prefab, transform.position + data.Offset*transform.forward, Quaternion.identity,transform);
        _flamethrower.transform.rotation = Quaternion.Euler(_flamethrower.transform.rotation.x, character.transform.rotation.eulerAngles.y-90, _flamethrower.transform.rotation.z);
        var script = _flamethrower.GetComponent<Flamethrower>();
        script.Initialize(data.Damage*character.form.damageMultiplier, data.Knockback, data.HitStun,transform.forward*data.Speed*character.form.speedMultiplier, character.form.sizeMultiplier);
        _oldSpeed = character.Speed;
        character.Speed = 0.5f;
        character.disableRotation = true;
        character.playerInputActions.Attack.Primary.Disable();
        character.playerInputActions.Movement.Pressed.Disable();
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
        character.playerInputActions.Attack.Primary.Enable();
        character.playerInputActions.Movement.Pressed.Enable();
        character.Speed = _oldSpeed;
        if (character.playerInputActions.Movement.Direction.ReadValue<Vector2>() != Vector2.zero)
        {
            character.animator.SetFloat("Speed", 1);
        }
        _flamethrower.GetComponent<ParticleSystem>().Stop();
        _flamethrower.transform.SetParent(null, true);
    }

    internal override void PassMessage(AnimationState state)
    {
        
    }

    public FlamethrowerAttack(Character character, AttackData data) : base(character, data)
    {
        if (character.isPlayer && character.playerInputActions != null)
        {
            character.playerInputActions.Attack.Secondary.started += Begin;
            character.playerInputActions.Attack.Secondary.canceled += End;
        }
    }

    public override void CleanUp()
    {
        if (character.isPlayer && character.playerInputActions != null)
        {
            character.playerInputActions.Attack.Secondary.started -= Begin;
            character.playerInputActions.Attack.Secondary.canceled -= End;
        }
        character.attacks.Remove(this);
        character.currentAttack = null;
    }
}
