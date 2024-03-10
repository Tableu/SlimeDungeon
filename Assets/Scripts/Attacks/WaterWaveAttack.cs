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
            Transform transform = character.transform;
            GameObject wave = GameObject.Instantiate(data.Prefab,
                transform.position + 
                new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), 
                Quaternion.identity);
            wave.transform.rotation = Quaternion.Euler(
                data.Prefab.transform.rotation.eulerAngles.x, 
                data.Prefab.transform.rotation.eulerAngles.y+transform.rotation.eulerAngles.y, 
                data.Prefab.transform.rotation.eulerAngles.z);
            SetLayer(wave);
            var script = wave.GetComponent<WaterWave>();
            if (script == null)
                return false;
            script.Initialize(data.Damage, data.Knockback, data.HitStun,
                transform.forward*data.Speed, character.ElementType);
            Cooldown(data.Cooldown);
            character.ApplyManaCost(data.ManaCost);
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
    
    public WaterWaveAttack(Character character, AttackData data) : base(character, data)
    {
    }
}
