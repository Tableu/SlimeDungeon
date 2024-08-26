using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class BouncingFireballAttack : Attack
{
    private BouncingFireballAttackData _data;
    public override bool Begin()
    {
        if (CheckCooldown())
        {
            Transform transform = CharacterInfo.Transform;
            GameObject fireball = SpawnProjectile(transform);
            SetLayer(fireball);
            BouncingFireball script = fireball.GetComponent<BouncingFireball>();
            if (script == null)
                return false;
            
            int maxBounces = 0;
            Vector3 launchAngle = transform.forward;
            
            maxBounces = _data.MaxBounceCount;
            launchAngle = new Vector3(launchAngle.x*_data.LaunchAngle.x, _data.LaunchAngle.y, launchAngle.z*_data.LaunchAngle.x);
            
            script.Initialize(Data.Damage, Data.Knockback, Data.HitStun, launchAngle*Data.Speed, maxBounces, 
                _data.ExplosionDamageRadius, Data.ElementType, CharacterInfo.EnemyMask);
            Cooldown(Data.Cooldown);
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

    public BouncingFireballAttack(ICharacterInfo characterInfo, BouncingFireballAttackData data) : base(characterInfo, data)
    {
        _data = data;
    }
}
