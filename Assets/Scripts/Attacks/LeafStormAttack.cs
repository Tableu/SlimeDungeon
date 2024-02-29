using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeafStormAttack : Attack
{
    public override bool Begin()
    {
        if (CheckManaCostAndCooldown())
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hitData, 1000, LayerMask.GetMask("Floor")))
            {
                GameObject leafStorm = GameObject.Instantiate(data.Prefab, hitData.point, Quaternion.identity);
                SetLayer(leafStorm);
                LeafStorm script = leafStorm.GetComponent<LeafStorm>();
                if (script == null)
                    return false;

                float duration = 5;
                if (data is LeafStormAttackData attackData)
                {
                    duration = attackData.Duration;
                }
                script.Initialize(data.Damage, duration, data.ElementType);
                Cooldown(data.Cooldown);
                character.ApplyManaCost(data.ManaCost);
                return true;
            }
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
    
    public LeafStormAttack(Character character, AttackData data) : base(character, data)
    {
    }
}
