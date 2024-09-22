using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaterBoltAttack : Attack
{
    public WaterBoltAttack(CharacterStats characterStats, AttackData data, Transform transform) : base(characterStats, data, transform)
    {
    }

    public override bool Begin()
    {
        if (CheckCooldown())
        {
            GameObject waterbolt = SpawnProjectile(Transform);
            SetLayer(waterbolt);
            var script = waterbolt.GetComponent<WaterBolt>();
            if (script == null)
                return false;
            script.Initialize(Data.Damage*CharacterStats.Damage, Data.Knockback, Data.HitStun,
                Transform.forward * Data.Speed, Data.ElementType,3);
            
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
}
