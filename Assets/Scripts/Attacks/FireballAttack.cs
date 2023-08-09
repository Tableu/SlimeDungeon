using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballAttack : Attack
{
    public override void Begin()
    {
        if (character.Mana >= data.ManaCost && character.CurrentAttack == null && !cooldownActive)
        {
            base.Begin();
            character.Mana -= data.ManaCost;
            
            Transform transform = character.transform;
            Collider col = AttackTargeting.SphereScan(transform, data.TargetingRange, character.enemyMask);
            if (col != null)
            {
                AttackTargeting.RotateTowards(transform, col.transform);
            }

            GameObject fireball = GameObject.Instantiate(data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            fireball.layer = character is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
            var script = fireball.GetComponent<Fireball>();
            if (character is PlayerController player && player.FormManager.CurrentForm != null)
            {
                script.Initialize(data.Damage * player.FormManager.CurrentForm.DamageMultiplier, data.Knockback, data.HitStun,
                    transform.forward * data.Speed * player.FormManager.CurrentForm.SpeedMultiplier, player.FormManager.CurrentForm.SizeMultiplier, data.ElementType);
                player.PlayerInputActions.Spells.Disable();
                player.PlayerInputActions.Movement.Disable();
            }
            else
            {
                script.Initialize(data.Damage, data.Knockback, data.HitStun,
                    transform.forward * data.Speed, 1, data.ElementType);
            }
            
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