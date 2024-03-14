using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class BouncingFireballAttack : Attack
{
    private BouncingFireballAttackData _data;
    public override bool Begin()
    {
        if (CheckManaCostAndCooldown())
        {
            Transform transform = character.transform;
            GameObject fireball = GameObject.Instantiate(Data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            SetLayer(fireball);
            BouncingFireball script = fireball.GetComponent<BouncingFireball>();
            if (script == null)
                return false;
            
            int maxBounces = 0;
            Vector3 launchAngle = transform.forward;
            
            maxBounces = _data.MaxBounceCount;
            launchAngle = new Vector3(launchAngle.x*_data.LaunchAngle.x, _data.LaunchAngle.y, launchAngle.z*_data.LaunchAngle.x);
            
            script.Initialize(Data.Damage, Data.Knockback, Data.HitStun, launchAngle*Data.Speed, maxBounces, Data.ElementType);
            Cooldown(Data.Cooldown);
            character.ApplyManaCost(Data.ManaCost);
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

    public BouncingFireballAttack(Character character, BouncingFireballAttackData data) : base(character, data)
    {
        _data = data;
    }
}
