using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeafStormAttack : Attack
{
    private LeafStormAttackData _data;
    public override bool Begin()
    {
        if (CheckCooldown())
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hitData, 1000, LayerMask.GetMask("Floor")))
            {
                GameObject leafStorm = GameObject.Instantiate(Data.Prefab, hitData.point, Quaternion.identity);
                SetLayer(leafStorm);
                LeafStorm script = leafStorm.GetComponent<LeafStorm>();
                if (script == null)
                    return false;

                script.Initialize(Data.Damage,CharacterStats.Attack, _data.Duration, Data.ElementType, _data.Tick);
                Cooldown(Data.Cooldown);
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
    
    public LeafStormAttack(CharacterStats characterStats, LeafStormAttackData data, Transform transform) : base(characterStats, data, transform)
    {
        _data = data;
    }
}
