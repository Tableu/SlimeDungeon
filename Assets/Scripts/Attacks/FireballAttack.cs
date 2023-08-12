using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballAttack : Attack
{
    public override void Begin()
    {
        if (character.Mana >= data.ManaCost && character.CurrentAttack == null && !onCooldown)
        {
            base.Begin();
            character.Mana -= data.ManaCost;
            
            Transform transform = character.transform;
            GameObject fireball = GameObject.Instantiate(data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            fireball.layer = character is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
            var script = fireball.GetComponent<Fireball>();
            script.Initialize(data.Damage*character.damageMultiplier, data.Knockback, data.HitStun,
                transform.forward * (data.Speed * character.speedMultiplier), character.sizeMultiplier, data.ElementType);

            Cooldown(data.Cooldown);
        }
    }

    public override void End(InputAction.CallbackContext callbackContext)
    {
          End();
    }

    public override void End()
    {
        base.End();
        if (character is PlayerController player)
        {
            player.PlayerInputActions.Spells.Enable();
            player.PlayerInputActions.Movement.Enable();
        }
    }

    internal override void PassMessage(Controller.AnimationState state)
    {
        if (Controller.AnimationState.AttackEnded == state)
        {
            End();
        }
    }

    public override void CleanUp()
    {
        cancellationTokenSource?.Cancel();
    }

    public FireballAttack(Character character, AttackData data) : base(character, data)
    {
    }
}