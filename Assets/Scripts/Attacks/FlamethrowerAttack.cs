using Controller;
using UnityEngine;
using AnimationState = Controller.AnimationState;

public class FlamethrowerAttack : Attack
{
    private GameObject _flamethrower;
    private float _oldSpeed;

    public override void Begin()
    {
        character.currentAttack = this;
        Transform transform = character.transform;
        _flamethrower = GameObject.Instantiate(data.Prefab, transform.position + data.Offset*transform.forward, Quaternion.identity,transform);
        _flamethrower.transform.rotation = Quaternion.Euler(_flamethrower.transform.rotation.x, character.transform.rotation.eulerAngles.y-90, _flamethrower.transform.rotation.z);
        var script = _flamethrower.GetComponent<Flamethrower>();
        if (character is PlayerController player)
        {
            //player.playerInputActions.Attack.Primary.Disable();
            player.playerInputActions.Movement.Pressed.Disable();
            script.Initialize(data.Damage * player.form.damageMultiplier, data.Knockback, data.HitStun,
                transform.forward * data.Speed * player.form.speedMultiplier, player.form.sizeMultiplier, data.ElementType);
        }
        else
        {
            script.Initialize(data.Damage, data.Knockback, data.HitStun,
                transform.forward * data.Speed, 1, data.ElementType);
        }

        _oldSpeed = character.Speed;
        character.Speed = 0.5f;
        character.disableRotation = true;
        character.animator.SetFloat("Speed",0);
        OnSpellCast?.Invoke();
    }

    public override void End()
    {
        character.currentAttack = null;
        character.disableRotation = false;
        if (character is PlayerController player)
        {
            //player.playerInputActions.Attack.Primary.Enable();
            player.playerInputActions.Movement.Pressed.Enable();
            if (player.playerInputActions.Movement.Direction.ReadValue<Vector2>() != Vector2.zero)
            {
                character.animator.SetFloat("Speed", 1);
            }
        }
        character.Speed = _oldSpeed;
        
        _flamethrower.GetComponent<ParticleSystem>().Stop();
        _flamethrower.transform.SetParent(null, true);
    }

    internal override void PassMessage(AnimationState state)
    {
        
    }

    public FlamethrowerAttack(Character character, AttackData data) : base(character, data)
    {
    }

    public override void CleanUp()
    {
        character.attacks.Remove(this);
        character.currentAttack = null;
    }
}
