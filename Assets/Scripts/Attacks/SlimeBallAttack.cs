using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlimeBallAttack : Attack
{
    private SlimeBallAttackData _data;

    public override bool Begin()
    {
        if (CheckCooldown())
        {
            GameObject slimeball = SpawnProjectile(Transform);
            SetLayer(slimeball);
            var script = slimeball.GetComponent<SlimeBall>();
            if (script == null)
                return false;
            script.Initialize(_data.DamagePerTick*CharacterInfo.Damage, _data.Knockback, _data.HitStun, 
                Transform.forward * _data.Speed, _data.ElementType, _data.SlimeArea, _data.Slow, _data.Duration);
            Cooldown(_data.Cooldown);
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
    
    public SlimeBallAttack(ICharacterInfo characterInfo, SlimeBallAttackData data, Transform transform) : base(characterInfo, data, transform)
    {
        _data = data;
    }
}
