using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChickenFireballAttack : Attack
{
    private ChickenFireballAttackData _data;
    public override bool Begin()
    {
        if (CheckCooldown())
        {
            for (int i = 0; i < 360; i += 360/_data.FireballCount)
            {
                SpawnFireball(i);
            }
            Cooldown(Data.Cooldown);
            return true;
        }

        return false;
    }

    private void SpawnFireball(float angle)
    {
        GameObject fireball = SpawnProjectile(Transform);
        SetLayer(fireball);
        ChickenFireball script = fireball.GetComponent<ChickenFireball>();
        if (script == null)
            return;
            
        Transform.Rotate(0,angle,0);
        Vector3 launchAngle = Transform.forward;
        
        launchAngle = new Vector3(launchAngle.x*_data.LaunchAngle.x, _data.LaunchAngle.y, launchAngle.z*_data.LaunchAngle.x);
        script.Initialize(Data.Damage*CharacterStats.Damage, Data.Knockback, Data.HitStun, launchAngle*Data.Speed, Data.ElementType, CharacterStats.EnemyMask);
        Transform.Rotate(0,-angle,0);
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

    public ChickenFireballAttack(CharacterStats characterStats, ChickenFireballAttackData data, Transform transform) : base(characterStats, data, transform)
    {
        _data = data;
    }
}
