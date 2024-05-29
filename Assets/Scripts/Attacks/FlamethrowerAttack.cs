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
        if (isActive)
            return false;
        Transform transform = CharacterInfo.Transform;
        _flamethrower = GameObject.Instantiate(Data.Prefab, transform.position + new Vector3(CharacterInfo.SpellOffset.x*transform.forward.x, CharacterInfo.SpellOffset.y, CharacterInfo.SpellOffset.z*transform.forward.z), Quaternion.identity,transform);
        _flamethrower.transform.rotation = Quaternion.Euler(_flamethrower.transform.rotation.x, CharacterInfo.Transform.rotation.eulerAngles.y-90, _flamethrower.transform.rotation.z);
        SetLayer(_flamethrower);
        var script = _flamethrower.GetComponent<Flamethrower>();
        script.Initialize(Data.Damage, Data.Knockback, Data.HitStun,
            transform.forward * Data.Speed, Data.ElementType);

        CharacterInfo.Speed.MultiplicativeModifer -= 0.5f;
        isActive = true;
        ApplyManaCost(Time.fixedDeltaTime);
        return true;
    }

    public override void End()
    {
        if (!isActive)
            return; 
        
        CharacterInfo.Speed.MultiplicativeModifer += 0.5f;
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
        await Task.Run(() =>
        {
            while (isActive)
            {
                Task.Delay(TimeSpan.FromSeconds(updateInterval)).Wait(manaCostCancellationTokenSource.Token);
            }
        });
        End();
    }

    public FlamethrowerAttack(ICharacterInfo characterInfo, AttackData data) : base(characterInfo, data)
    {
    }
}
