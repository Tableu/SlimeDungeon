using Controller;
using UnityEngine;
using AnimationState = Controller.AnimationState;

public class FlamethrowerAttack : Attack
{
    private GameObject _flamethrower;
    private float _oldSpeed;

    public override void Begin()
    {
        base.Begin();
        Transform transform = character.transform;
        Collider col = AttackTargeting.SphereScan(transform, data.TargetingRange, character.enemyMask);
        if (col != null)
        {
            AttackTargeting.RotateTowards(transform, col.transform);
        }
        _flamethrower = GameObject.Instantiate(data.Prefab, transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity,transform);
        _flamethrower.transform.rotation = Quaternion.Euler(_flamethrower.transform.rotation.x, character.transform.rotation.eulerAngles.y-90, _flamethrower.transform.rotation.z);
        var script = _flamethrower.GetComponent<Flamethrower>();
        script.Initialize(data.Damage * character.damageMultiplier, data.Knockback, data.HitStun,
            transform.forward * data.Speed * character.speedMultiplier, character.sizeMultiplier, data.ElementType);
        if (character is PlayerController player)
        {
            //todo fix flamethrower attack
            //player.playerInputActions.Attack.Primary.Disable();
            player.PlayerInputActions.Movement.Pressed.Disable();
        }

        _oldSpeed = character.Speed;
        character.Speed = 0.5f;
        character.disableRotation = true;
        //character.animator.SetFloat("Speed",0);
    }

    public override void End()
    {
        base.End();
        character.disableRotation = false;
        if (character is PlayerController player)
        {
            //player.playerInputActions.Attack.Primary.Enable();
            player.PlayerInputActions.Movement.Pressed.Enable();
            if (player.PlayerInputActions.Movement.Direction.ReadValue<Vector2>() != Vector2.zero)
            {
                //character.animator.SetFloat("Speed", 1);
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
        
    }
}
