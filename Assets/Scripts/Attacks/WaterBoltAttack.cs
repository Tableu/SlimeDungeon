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
        if (character.Mana >= data.ManaCost && character.CurrentAttack == null && !cooldownActive)
        {
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
            var script = fireball.GetComponent<WaterBolt>();
            
            if (character is PlayerController player && player.FormManager.CurrentForm != null)
            {
                player.PlayerInputActions.Spells.Disable();
                player.PlayerInputActions.Movement.Disable();
                script.Initialize(data.Damage * player.FormManager.CurrentForm.DamageMultiplier, data.Knockback, data.HitStun,
                    transform.forward * data.Speed * player.FormManager.CurrentForm.SpeedMultiplier, player.FormManager.CurrentForm.SizeMultiplier, data.ElementType,3);
            }
            else
            {
                script.Initialize(data.Damage, data.Knockback, data.HitStun,
                    transform.forward * data.Speed, 1, data.ElementType,3);
            }
            
            OnBegin?.Invoke(this);
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
        OnEnd?.Invoke(this);
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
        OnEnd?.Invoke(this);
    }
}
