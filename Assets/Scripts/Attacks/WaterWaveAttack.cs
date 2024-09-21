using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaterWaveAttack : Attack
{
    public override bool Begin()
    {
        if (CheckCooldown())
        {
            GameObject wave = SpawnProjectile(Transform);
            wave.transform.rotation = Quaternion.Euler(
                Data.Prefab.transform.rotation.eulerAngles.x, 
                Data.Prefab.transform.rotation.eulerAngles.y+Transform.rotation.eulerAngles.y, 
                Data.Prefab.transform.rotation.eulerAngles.z);
            wave.layer = LayerMask.NameToLayer("TriggerColliderAttacks");
            var script = wave.GetComponent<WaterWave>();
            if (script == null)
                return false;
            script.Initialize(Data.Damage*CharacterInfo.Damage, Data.Knockback, Data.HitStun,
                Transform.forward*Data.Speed, Data.ElementType, CharacterInfo.EnemyMask);
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
    
    public WaterWaveAttack(ICharacterInfo characterInfo, AttackData data, Transform transform) : base(characterInfo, data, transform)
    {
    }
}
