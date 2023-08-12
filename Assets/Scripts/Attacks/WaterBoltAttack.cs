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
        if (character.Mana >= data.ManaCost && character.CurrentAttack == null && !onCooldown)
        {
            character.Mana -= data.ManaCost;
            Transform transform = character.transform;
            GameObject fireball = GameObject.Instantiate(data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            fireball.layer = character is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
            var script = fireball.GetComponent<WaterBolt>();
            
            script.Initialize(data.Damage * character.damageMultiplier, data.Knockback, data.HitStun,
                transform.forward * (data.Speed * character.speedMultiplier), character.sizeMultiplier, data.ElementType,3);

            OnBegin?.Invoke(this);
            Cooldown(data.Cooldown);
        }
    }
    
    public override void End(InputAction.CallbackContext callbackContext)
    {
        
    }

    public override void End()
    {
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
