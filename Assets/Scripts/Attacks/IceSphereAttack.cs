using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class IceSphereAttack : Attack
{
    public override bool Begin()
    {
        if (CheckManaCostAndCooldown())
        {
            Transform transform = character.transform;
            GameObject iceSphere = GameObject.Instantiate(Data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, 
                    character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            SetLayer(iceSphere);
            var script = iceSphere.GetComponent<IceSphere>();
            if (script == null)
                return false;
            script.Initialize(Data.Damage, Data.HitStun, 
                transform.forward * Data.Speed, Data.ElementType);
            Cooldown(Data.Cooldown);
            character.ApplyManaCost(Data.ManaCost);
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
    
    public IceSphereAttack(Character character, AttackData data) : base(character, data)
    {
    }
}
