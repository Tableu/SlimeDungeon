using Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using AnimationState = Controller.AnimationState;

public class WaterBoltAttack : Attack
{
    public WaterBoltAttack(Character character, AttackData data) : base(character, data)
    {
    }

    public override void Begin()
    {
        if (character.Mana >= data.ManaCost && character.currentAttack == null && !cooldownActive)
        {
            character.Mana -= data.ManaCost;
            character.currentAttack = this;
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
            var script = fireball.GetComponent<WaterBolt>();
            
            if (character is PlayerController player && player.Form != null)
            {
                player.PlayerInputActions.Spells.Disable();
                player.PlayerInputActions.Movement.Disable();
                script.Initialize(data.Damage * player.Form.damageMultiplier, data.Knockback, data.HitStun,
                    transform.forward * data.Speed * player.Form.speedMultiplier, player.Form.sizeMultiplier, data.ElementType,3);
            }
            else
            {
                script.Initialize(data.Damage, data.Knockback, data.HitStun,
                    transform.forward * data.Speed, 1, data.ElementType,3);
            }
            character.Attack();
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
            player.PlayerInputActions.Spells.Enable();
            player.PlayerInputActions.Movement.Enable();
        }

        character.currentAttack = null;
    }

    internal override void PassMessage(AnimationState state)
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
}
