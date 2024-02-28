using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeafAttack : Attack
{
    public override bool Begin()
    {
        if (character.Mana >= data.ManaCost && !onCooldown)
        {
            Transform transform = character.transform;
            GameObject leaf = GameObject.Instantiate(data.Prefab,RandomPosition(transform)
                , Quaternion.identity);
            leaf.transform.rotation = Quaternion.Euler(0, Random.Range(-180,180),0);
            leaf.layer = character is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
            var script = leaf.GetComponent<Leaf>();
            if (script == null)
                return false;
            script.Initialize(data.Damage*character.DamageMultiplier, data.Knockback, data.HitStun,
                transform.forward * (data.Speed * character.SpeedMultiplier), character.SizeMultiplier, data.ElementType);
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

    private Vector3 RandomPosition(Transform transform)
    {
        Vector3 randomVector = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        return transform.position + new Vector3((character.SpellOffset.x+randomVector.x) * transform.forward.x, 
            character.SpellOffset.y+randomVector.y,
            (character.SpellOffset.z+randomVector.z) * transform.forward.z);
    }

    public LeafAttack(Character character, AttackData data) : base(character, data)
    {
        
    }
}
