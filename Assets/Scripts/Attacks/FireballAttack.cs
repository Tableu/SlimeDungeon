using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballAttack : Attack
{
    public override void Begin()
    {
        if (character.Mana >= data.ManaCost && character.currentAttack == null)
        {
            character.Mana -= data.ManaCost;
            character.currentAttack = this;
            Transform transform = character.transform;
            GameObject fireball = GameObject.Instantiate(data.Prefab,
                transform.position + data.Offset * transform.forward, Quaternion.identity);
            fireball.layer = character is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
            var script = fireball.GetComponent<Fireball>();
            if (character is PlayerController player && player.form != null)
            {
                script.Initialize(data.Damage * player.form.damageMultiplier, data.Knockback, data.HitStun,
                    transform.forward * data.Speed * player.form.speedMultiplier, player.form.sizeMultiplier, data.ElementType);
                player.playerInputActions.Spells.Disable();
                player.playerInputActions.Movement.Disable();
            }
            else
            {
                script.Initialize(data.Damage, data.Knockback, data.HitStun,
                    transform.forward * data.Speed, 1, data.ElementType);
            }

            character.animator.SetTrigger("Attack");
            OnSpellCast?.Invoke();
        }
    }

    public override void End(InputAction.CallbackContext callbackContext)
    {
        
    }

    public override void End()
    {
        if (character is PlayerController player)
        {
            player.playerInputActions.Spells.Enable();
            player.playerInputActions.Movement.Enable();
        }

        character.currentAttack = null;
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
        character.attacks.Remove(this);
        character.currentAttack = null;
    }

    public FireballAttack(Character character, AttackData data) : base(character, data)
    {
    }
}