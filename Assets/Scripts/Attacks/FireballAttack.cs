using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballAttack : Attack
{
    public override void Begin()
    {
        if (character.Mana > data.ManaCost && character.currentAttack == null)
        {
            character.Mana -= data.ManaCost;
            character.currentAttack = this;
            Transform transform = character.transform;
            GameObject fireball = GameObject.Instantiate(data.Prefab,
                transform.position + data.Offset * transform.forward, Quaternion.identity);
            var script = fireball.GetComponent<Fireball>();
            if (character.form != null)
            {
                script.Initialize(data.Damage * character.form.damageMultiplier, data.Knockback, data.HitStun,
                    transform.forward * data.Speed * character.form.speedMultiplier, character.form.sizeMultiplier, data.ElementType);
            }
            else
            {
                script.Initialize(data.Damage, data.Knockback, data.HitStun,
                    transform.forward * data.Speed, 1, data.ElementType);
            }

            character.animator.SetTrigger("Attack");
            if (character is PlayerController player)
            {
                player.playerInputActions.Attack.Disable();
                player.playerInputActions.Movement.Disable();
            }

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
            player.playerInputActions.Attack.Enable();
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