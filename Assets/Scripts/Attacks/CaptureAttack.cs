using Controller;
using UnityEngine;
using AnimationState = Controller.AnimationState;

public class CaptureAttack : Attack
{
    public override bool Begin()
    {
        if (character.Mana >= data.ManaCost && character.CurrentAttack == null && !onCooldown)
        {
            OnBegin?.Invoke(this);
            Transform transform = character.transform;
            GameObject orb = GameObject.Instantiate(data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            orb.layer = character is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
            CaptureOrb script = orb.GetComponent<CaptureOrb>();
            if (script == null)
                return false;
            script.Initialize(data.HitStun, transform.forward * data.Speed);
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
    
    public CaptureAttack(Character character, AttackData data) : base(character, data)
    {
    }
}