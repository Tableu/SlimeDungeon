using System;
using System.Threading;
using System.Threading.Tasks;
using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlamethrowerAttack : Attack
{
    private GameObject _flamethrower;
    private bool isActive;
    private CancellationTokenSource manaCostCancellationTokenSource;

    public override bool Begin()
    {
        if (isActive || character.Mana < ((FlamethrowerAttackData)data).InitialManaCost)
            return false;
        Transform transform = character.transform;
        _flamethrower = GameObject.Instantiate(data.Prefab, transform.position + new Vector3(character.SpellOffset.x*transform.forward.x, character.SpellOffset.y, character.SpellOffset.z*transform.forward.z), Quaternion.identity,transform);
        _flamethrower.transform.rotation = Quaternion.Euler(_flamethrower.transform.rotation.x, character.transform.rotation.eulerAngles.y-90, _flamethrower.transform.rotation.z);
        SetLayer(_flamethrower);
        var script = _flamethrower.GetComponent<Flamethrower>();
        script.Initialize(data.Damage * character.DamageMultiplier, data.Knockback, data.HitStun,
            transform.forward * data.Speed * character.SpeedMultiplier, data.ElementType);

        character.Speed.MultiplicativeModifer -= 0.5f;
        isActive = true;
        ApplyManaCost(Time.fixedDeltaTime);
        return true;
    }

    public override void End()
    {
        character.Speed.MultiplicativeModifer += 0.5f;
        
        _flamethrower.GetComponent<ParticleSystem>().Stop();
        _flamethrower.transform.SetParent(null, true);
        isActive = false;
    }
    
    public override void LinkInput(InputAction action)
    {
        UnlinkInput();
        inputAction = action;
        action.started += Begin;
        action.canceled += End;
    }

    public override void UnlinkInput()
    {
        if (inputAction != null)
        {
            inputAction.started -= Begin;
            inputAction.canceled -= End;
        }
    }

    public override void CleanUp()
    {
        base.CleanUp();
        manaCostCancellationTokenSource?.Cancel();
    }

    private async void ApplyManaCost(float updateInterval)
    {
        manaCostCancellationTokenSource = new CancellationTokenSource();
        character.ApplyManaCost(data.ManaCost);
        await Task.Run(() =>
        {
            while (isActive)
            {
                Task.Delay(TimeSpan.FromSeconds(updateInterval)).Wait(manaCostCancellationTokenSource.Token);
                character.ApplyManaCost(data.ManaCost);
            }
        });
    }

    public FlamethrowerAttack(Character character, AttackData data) : base(character, data)
    {
    }
}
