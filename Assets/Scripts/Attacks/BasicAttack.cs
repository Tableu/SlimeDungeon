using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public interface BasicProjectile
{
    public void Initialize(float damage, float knockback,float hitStun, Vector3 force, Elements.Type type);
}

public class BasicAttack<T> : Attack where T : BasicProjectile
{
    public override bool Begin()
    {
        if (CheckCooldown())
        {
            Transform transform = CharacterInfo.Transform;
            GameObject projectile = SpawnProjectile(transform);
            SetLayer(projectile);
            var script = projectile.GetComponent<T>();
            if (script == null)
                return false;
            script.Initialize(Data.Damage*CharacterInfo.Damage, Data.Knockback, Data.HitStun,
                transform.forward * Data.Speed, Data.ElementType);
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
    
    public BasicAttack(ICharacterInfo characterInfo, AttackData data) : base(characterInfo, data)
    {
    }
}