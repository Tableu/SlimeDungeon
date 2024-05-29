using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaterWaveAttack : Attack
{
    public override bool Begin()
    {
        if (CheckManaCostAndCooldown())
        {
            Transform transform = CharacterInfo.Transform;
            GameObject wave = GameObject.Instantiate(Data.Prefab,
                transform.position + 
                new Vector3(CharacterInfo.SpellOffset.x*transform.forward.x, CharacterInfo.SpellOffset.y, CharacterInfo.SpellOffset.z*transform.forward.z), 
                Quaternion.identity);
            wave.transform.rotation = Quaternion.Euler(
                Data.Prefab.transform.rotation.eulerAngles.x, 
                Data.Prefab.transform.rotation.eulerAngles.y+transform.rotation.eulerAngles.y, 
                Data.Prefab.transform.rotation.eulerAngles.z);
            wave.layer = LayerMask.NameToLayer("TriggerColliderAttacks");
            var script = wave.GetComponent<WaterWave>();
            if (script == null)
                return false;
            script.Initialize(Data.Damage, Data.Knockback, Data.HitStun,
                transform.forward*Data.Speed, Data.ElementType, CharacterInfo.EnemyMask);
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
    
    public WaterWaveAttack(ICharacterInfo characterInfo, AttackData data) : base(characterInfo, data)
    {
    }
}
