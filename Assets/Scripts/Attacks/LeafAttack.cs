using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeafAttack : Attack
{
    private LeafAttackData _data;
    public override bool Begin()
    {
        if (CheckCooldown())
        {
            GameObject leaf = GameObject.Instantiate(Data.Prefab,RandomPosition(Transform),Quaternion.identity);
            leaf.transform.rotation = Quaternion.Euler(0, Random.Range(-180,180),0);
            SetLayer(leaf);
            var script = leaf.GetComponent<Leaf>();
            if (script == null)
                return false;
            Quaternion angleAxis = Quaternion.AngleAxis(Random.Range(-_data.RandomAngle,_data.RandomAngle), Vector3.up);
            script.Initialize(Data.Damage,CharacterStats.Attack, Data.Knockback, Data.HitStun,
                angleAxis*Transform.forward * Data.Speed, Data.ElementType);
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

    private Vector3 RandomPosition(Transform transform)
    {
        Vector3 randomVector = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        Vector3 offset = CharacterStats.SpellOffset + Data.SpawnOffset;
        return transform.position + new Vector3((offset.x+randomVector.x) * transform.forward.x, 
            offset.y+randomVector.y,
            (offset.z+randomVector.z) * transform.forward.z);
    }

    public LeafAttack(CharacterStats characterStats, LeafAttackData data, Transform transform) : base(characterStats, data, transform)
    {
        _data = data;
    }
}
