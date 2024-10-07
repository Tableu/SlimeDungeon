using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public interface BasicProjectile
{
    public void Initialize(float damage, float attackStat, float knockback,float hitStun, Vector3 force, Elements.Type type);
}

public class BasicAttack<T> : Attack where T : BasicProjectile
{
    public override bool Begin()
    {
        if (CheckCooldown())
        {
            GameObject projectile = SpawnProjectile(Transform);
            SetLayer(projectile);
            var script = projectile.GetComponent<T>();
            if (script == null)
                return false;
            script.Initialize(Data.Damage, CharacterStats.Attack, Data.Knockback, Data.HitStun,
                Transform.forward * Data.Speed, Data.ElementType);
            Cooldown(Data.Cooldown);
            return true;
        }
        return false;
    }

    public override void End()
    {
        return;
    }

    public override void LinkInput(InputAction action)
    {
        UnlinkInput();
        inputAction = action;
        action.started += Begin;
    }

    public override void UnlinkInput()
    {
        if (inputAction != null)
        {
            inputAction.started -= Begin;
        }
    }
    
    public BasicAttack(CharacterStats characterStats, AttackData data, Transform transform) : base(characterStats, data, transform)
    {
    }
}