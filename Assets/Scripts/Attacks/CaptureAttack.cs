using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaptureAttack : Attack
{
    public override bool Begin()
    {
        if (CheckManaCostAndCooldown())
        {
            Transform transform = character.transform;
            GameObject orb = GameObject.Instantiate(data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            SetLayer(orb);
            CaptureOrb script = orb.GetComponent<CaptureOrb>();
            if (script == null)
                return false;
            script.Initialize(data.HitStun, transform.forward * data.Speed);
            Cooldown(data.Cooldown);
            character.ApplyManaCost(data.ManaCost);
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

    public CaptureAttack(Character character, AttackData data) : base(character, data)
    {
    }
}