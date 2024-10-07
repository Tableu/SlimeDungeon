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
        Vector3 offset = CharacterStats.SpellOffset + Data.SpawnOffset;
        _flamethrower = GameObject.Instantiate(Data.Prefab, Transform.position + new Vector3(offset.x*Transform.forward.x, 
            offset.y, offset.z*Transform.forward.z), Quaternion.identity,Transform);
        _flamethrower.transform.rotation = Quaternion.Euler(_flamethrower.transform.rotation.x, Transform.rotation.eulerAngles.y-90, _flamethrower.transform.rotation.z);
        SetLayer(_flamethrower);
        var script = _flamethrower.GetComponent<Flamethrower>();
        script.Initialize(Data.Damage*CharacterStats.Attack, Data.Knockback, Data.HitStun,
            Transform.forward * Data.Speed, Data.ElementType);

        CharacterStats.Speed.MultiplicativeModifer -= 0.5f;
        isActive = true;
        ApplyManaCost(Time.fixedDeltaTime);
        return true;
    }

    public override void End()
    {
        if (!isActive)
            return; 
        
        CharacterStats.Speed.MultiplicativeModifer += 0.5f;
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
        End();
    }

    private async void ApplyManaCost(float updateInterval)
    {
        manaCostCancellationTokenSource = new CancellationTokenSource();
        await Task.Run(() =>
        {
            while (isActive)
            {
                try
                {
                    Task.Delay(TimeSpan.FromSeconds(updateInterval)).Wait(manaCostCancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                { }
            }
        });
    }

    public FlamethrowerAttack(CharacterStats characterStats, AttackData data, Transform transform) : base(characterStats, data, transform)
    {
    }
}
