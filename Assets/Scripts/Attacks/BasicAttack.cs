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
        if (CheckManaCostAndCooldown())
        {
            Transform transform = character.transform;
            GameObject projectile = GameObject.Instantiate(Data.Prefab,
                transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, 
                    character.SpellOffset.z*transform.forward.z), Quaternion.identity);
            SetLayer(projectile);
            var script = projectile.GetComponent<T>();
            if (script == null)
                return false;
            script.Initialize(Data.Damage, Data.Knockback, Data.HitStun,
                transform.forward * Data.Speed, Data.ElementType);
            Cooldown(Data.Cooldown);
            character.ApplyManaCost(Data.ManaCost);
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
    
    public BasicAttack(Character character, AttackData data) : base(character, data)
    {
    }
}