using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeafAttack : Attack
{
    public override bool Begin()
    {
        if (CheckManaCostAndCooldown())
        {
            Transform transform = CharacterInfo.Transform;
            GameObject leaf = GameObject.Instantiate(Data.Prefab,RandomPosition(transform),Quaternion.identity);
            leaf.transform.rotation = Quaternion.Euler(0, Random.Range(-180,180),0);
            SetLayer(leaf);
            var script = leaf.GetComponent<Leaf>();
            if (script == null)
                return false;
            script.Initialize(Data.Damage, Data.Knockback, Data.HitStun,
                transform.forward * Data.Speed, Data.ElementType);
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
        return transform.position + new Vector3((CharacterInfo.SpellOffset.x+randomVector.x) * transform.forward.x, 
            CharacterInfo.SpellOffset.y+randomVector.y,
            (CharacterInfo.SpellOffset.z+randomVector.z) * transform.forward.z);
    }

    public LeafAttack(ICharacterInfo characterInfo, AttackData data) : base(characterInfo, data)
    {
        
    }
}
