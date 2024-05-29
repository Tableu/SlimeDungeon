using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlimeBallAttack : Attack
{
    private SlimeBallAttackData _data;

    public override bool Begin()
    {
        if (CheckManaCostAndCooldown())
        {
            Transform transform = CharacterInfo.Transform;
            GameObject slimeball = GameObject.Instantiate(_data.Prefab,
                    transform.position + new Vector3(CharacterInfo.SpellOffset.x*transform.forward.x, CharacterInfo.SpellOffset.y, CharacterInfo.SpellOffset.z*transform.forward.z), 
                    Quaternion.identity);
            SetLayer(slimeball);
            var script = slimeball.GetComponent<SlimeBall>();
            if (script == null)
                return false;
            script.Initialize(_data.DamagePerTick, _data.Knockback, _data.HitStun, 
                transform.forward * _data.Speed, _data.ElementType, _data.SlimeArea, _data.Slow, _data.Duration);
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
    
    public SlimeBallAttack(ICharacterInfo characterInfo, SlimeBallAttackData data) : base(characterInfo, data)
    {
        _data = data;
    }
}
