using Controller;
using UnityEngine;
using AnimationState = Controller.AnimationState;

public class BouncingFireballAttack : Attack
{
    public override bool Begin()
    {
        if (character.Mana >= data.ManaCost && character.CurrentAttack == null && !onCooldown)
        {
            OnBegin?.Invoke(this);
            Transform transform = character.transform;
            GameObject fireball = GameObject.Instantiate(data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            fireball.layer = character is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
            BouncingFireball script = fireball.GetComponent<BouncingFireball>();
            if (script == null)
                return false;
            
            int maxBounces = 0;
            Vector3 launchAngle = transform.forward;
            if (data is BouncingFireballAttackData fireballData)
            {
                maxBounces = fireballData.MaxBounceCount;
                launchAngle = new Vector3(launchAngle.x*fireballData.LaunchAngle.x, fireballData.LaunchAngle.y, launchAngle.z*fireballData.LaunchAngle.x);
            }
            script.Initialize(data.Damage, data.Knockback, data.HitStun, launchAngle*data.Speed, maxBounces, data.ElementType);
            Cooldown(data.Cooldown);
            return true;
        }

        return false;
    }

    public override void End()
    {
        OnEnd?.Invoke(this);
    }

    internal override void PassMessage(AnimationState state)
    {
        if (AnimationState.AttackEnded == state)
        {
            End();
        }
    }
    
    public BouncingFireballAttack(Character character, AttackData data) : base(character, data)
    {
    }
}
