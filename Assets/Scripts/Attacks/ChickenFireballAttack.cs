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
        Transform transform = CharacterInfo.Transform;
        GameObject fireball = SpawnProjectile(transform);
        SetLayer(fireball);
        ChickenFireball script = fireball.GetComponent<ChickenFireball>();
        if (script == null)
            return;
            
        transform.Rotate(0,angle,0);
        Vector3 launchAngle = transform.forward;
        
        launchAngle = new Vector3(launchAngle.x*_data.LaunchAngle.x, _data.LaunchAngle.y, launchAngle.z*_data.LaunchAngle.x);
        script.Initialize(Data.Damage, Data.Knockback, Data.HitStun, launchAngle*Data.Speed, Data.ElementType, CharacterInfo.EnemyMask);
        transform.Rotate(0,-angle,0);
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

    public ChickenFireballAttack(ICharacterInfo characterInfo, ChickenFireballAttackData data) : base(characterInfo, data)
    {
        _data = data;
    }
}
