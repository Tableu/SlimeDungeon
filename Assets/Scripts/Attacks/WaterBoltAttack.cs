using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaterBoltAttack : Attack
{
    public WaterBoltAttack(ICharacterInfo characterInfo, AttackData data) : base(characterInfo, data)
    {
    }

    public override bool Begin()
    {
        if (CheckManaCostAndCooldown())
        {
            Transform transform = CharacterInfo.Transform;
            GameObject waterbolt = SpawnProjectile(transform);
            SetLayer(waterbolt);
            var script = waterbolt.GetComponent<WaterBolt>();
            if (script == null)
                return false;
            script.Initialize(Data.Damage, Data.Knockback, Data.HitStun,
                transform.forward * Data.Speed, Data.ElementType,3);
            
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
