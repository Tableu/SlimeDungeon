using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballAttack : Attack
{
    public override void Begin(InputAction.CallbackContext callbackContext)
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
            if (character.isPlayer && character.playerInputActions != null)
            {
                character.playerInputActions.Attack.Disable();
                character.playerInputActions.Movement.Disable();
            }

            OnSpellCast?.Invoke();
        }
    }

    public override void End()
    {
        if (character.isPlayer && character.playerInputActions != null)
        {
            character.playerInputActions.Attack.Enable();
            character.playerInputActions.Movement.Enable();
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
        if (character.isPlayer && character.playerInputActions != null)
        {
            character.playerInputActions.Attack.Primary.started -= Begin;
        }

        character.attacks.Remove(this);
        character.currentAttack = null;
    }

    public FireballAttack(Character character, AttackData data) : base(character, data)
    {
        if (character.isPlayer && character.playerInputActions != null)
        {
            character.playerInputActions.Attack.Primary.started += Begin;
        }
    }
}