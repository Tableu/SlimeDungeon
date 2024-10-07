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
            GameObject fireball = SpawnProjectile(Transform);
            SetLayer(fireball);
            BouncingFireball script = fireball.GetComponent<BouncingFireball>();
            if (script == null)
                return false;
            
            int maxBounces = 0;
            Vector3 launchAngle = Transform.forward;
            
            maxBounces = _data.MaxBounceCount;
            launchAngle = new Vector3(launchAngle.x*_data.LaunchAngle.x, _data.LaunchAngle.y, launchAngle.z*_data.LaunchAngle.x);
            
            script.Initialize(Data.Damage*CharacterStats.Attack, Data.Knockback, Data.HitStun, launchAngle*Data.Speed, maxBounces, 
                _data.ExplosionDamageRadius, Data.ElementType, CharacterStats.EnemyMask);
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

    public BouncingFireballAttack(CharacterStats characterStats, BouncingFireballAttackData data, Transform transform) : base(characterStats, data, transform)
    {
        _data = data;
    }
}
