using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class BouncingFireballAttack : Attack
{
    public override bool Begin()
    {
        if (CheckManaCostAndCooldown())
        {
            Transform transform = character.transform;
            GameObject fireball = GameObject.Instantiate(data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            SetLayer(fireball);
            BouncingFireball script = fireball.GetComponent<BouncingFireball>();
            if (script == null)
                return false;
            
            int maxBounces = 0;
            Vector3 launchAngle = transform.forward;
            if (data is BouncingFireballAttackData fireballData)
            {
                maxBounces = fireballData.MaxBounceCount;
                launchAngle = new Vector3(launchAngle.x*fireballData.LaunchAngle.x, fireballData.LaunchAngle.y, launchAngle.z*fireballData.LaunchAngle.x);
            }
            script.Initialize(data.Damage, data.Knockback, data.HitStun, launchAngle*data.Speed, maxBounces, data.ElementType);
            Cooldown(data.Cooldown);
            character.ApplyManaCost(data.ManaCost);
            return true;
        }

        return false;
    }

    public override void End()
    {
        return;
    }
    
    public override void LinkInput(InputAction action)
    {
        UnlinkInput();
        inputAction = action;
        action.started += Begin;
    }

    public override void UnlinkInput()
    {
        if (inputAction != null)
        {
            inputAction.started -= Begin;
        }
    }

    public BouncingFireballAttack(Character character, AttackData data) : base(character, data)
    {
    }
}
