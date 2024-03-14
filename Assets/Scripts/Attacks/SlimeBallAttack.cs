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
            Transform transform = character.transform;
            GameObject slimeball = GameObject.Instantiate(_data.Prefab,
                    transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), 
                    Quaternion.identity);
            SetLayer(slimeball);
            var script = slimeball.GetComponent<SlimeBall>();
            if (script == null)
                return false;
            script.Initialize(_data.DamagePerTick, _data.Knockback, _data.HitStun, 
                transform.forward * _data.Speed, _data.ElementType, _data.SlimeArea, _data.Slow, _data.Duration);
            Cooldown(_data.Cooldown);
            character.ApplyManaCost(_data.ManaCost);
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
    
    public SlimeBallAttack(Character character, SlimeBallAttackData data) : base(character, data)
    {
        _data = data;
    }
}
