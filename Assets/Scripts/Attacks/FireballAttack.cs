using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballAttack : Attack
{
    public override bool Begin()
    {
        if (character.Mana >= data.ManaCost && !onCooldown)
        {
            Transform transform = character.transform;
            GameObject fireball = GameObject.Instantiate(data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            fireball.layer = character is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
            var script = fireball.GetComponent<Fireball>();
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

    public override void Performed()
    {
        return;
    }

    public override void End()
    {
        return;
    }
    
    public override void LinkInput(InputAction action)
    {
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

    public FireballAttack(Character character, AttackData data) : base(character, data)
    {
    }
}