using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballAttack : Attack
{
    public override bool Begin()
    {
        if (character.Mana >= data.ManaCost && character.CurrentAttack == null && !onCooldown)
        {
            OnBegin?.Invoke(this);
            character.Mana -= data.ManaCost;
            
            Transform transform = character.transform;
            GameObject fireball = GameObject.Instantiate(data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            fireball.layer = character is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
            var script = fireball.GetComponent<Fireball>();
            if (script == null)
                return false;
            script.Initialize(data.Damage*character.damageMultiplier, data.Knockback, data.HitStun,
                transform.forward * (data.Speed * character.speedMultiplier), character.sizeMultiplier, data.ElementType);

            Cooldown(data.Cooldown);
            return true;
        }
        return false;
    }

    public override void End(InputAction.CallbackContext callbackContext)
    {
        End();
    }

    public override void End()
    {
        OnEnd?.Invoke(this);
    }

    internal override void PassMessage(Controller.AnimationState state)
    {
        if (Controller.AnimationState.AttackEnded == state)
        {
            End();
        }
    }

    public FireballAttack(Character character, AttackData data) : base(character, data)
    {
    }
}