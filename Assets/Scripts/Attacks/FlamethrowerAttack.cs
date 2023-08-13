using Controller;
using UnityEngine;
using AnimationState = Controller.AnimationState;

public class FlamethrowerAttack : Attack
{
    private GameObject _flamethrower;
    private float _oldSpeed;

    public override bool Begin()
    {
        OnBegin?.Invoke(this);
        Transform transform = character.transform;
        _flamethrower = GameObject.Instantiate(data.Prefab, transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity,transform);
        _flamethrower.transform.rotation = Quaternion.Euler(_flamethrower.transform.rotation.x, character.transform.rotation.eulerAngles.y-90, _flamethrower.transform.rotation.z);
        var script = _flamethrower.GetComponent<Flamethrower>();
        script.Initialize(data.Damage * character.damageMultiplier, data.Knockback, data.HitStun,
            transform.forward * data.Speed * character.speedMultiplier, character.sizeMultiplier, data.ElementType);

        _oldSpeed = character.Speed;
        character.Speed = 0.5f;
        character.disableRotation = true;
        //character.animator.SetFloat("Speed",0);
        return true;
    }

    public override void End()
    {
        OnEnd?.Invoke(this);
        character.disableRotation = false;
        if (character is PlayerController player)
        {
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
