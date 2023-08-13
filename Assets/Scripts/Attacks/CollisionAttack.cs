using Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using AnimationState = Controller.AnimationState;

public class CollisionAttack : Attack
{
    private AttackData _data;
    public CollisionAttack(Character character, AttackData data) : base(character, data)
    {
        character.CollisionEnter += CollisionEnter;
        _data = data;
    }

    private void CollisionEnter(Collision other)
    {
        if (character.enemyMask == (character.enemyMask | (1 << other.gameObject.layer)))
        {
            IDamageable health = other.gameObject.GetComponent<IDamageable>();
            health.TakeDamage(_data.Damage,(other.transform.position - character.transform.position).normalized*_data.Knockback, 
                _data.HitStun, _data.ElementType);
        }
    }

    public override bool Begin()
    {
        return true;
    }

    public override void End()
    {
    }

    internal override void PassMessage(AnimationState state)
    {
    }

    public override void CleanUp()
    {
        character.CollisionEnter -= CollisionEnter;
    }
}
