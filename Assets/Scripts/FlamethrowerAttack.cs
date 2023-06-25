using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using AnimationState = Controller.AnimationState;

[CreateAssetMenu(fileName = "Flamethrower Attack", menuName = "Attacks/Flamethrower Attack")]
public class FlamethrowerAttack : Attack
{
    [SerializeField] private GameObject flamethrowerPrefab;
    public float Offset;
    private GameObject _flamethrower;
    public override void Equip(Character character, PlayerInputActions inputActions = null)
    {
        _character = character;
        _inputActions = inputActions;
        if (inputActions != null)
        {
            inputActions.Attack.Secondary.started += Begin;
            inputActions.Attack.Secondary.canceled += delegate(InputAction.CallbackContext context) { End(); };
        }
    }

    public override void Drop()
    {
        if (_inputActions != null)
        {
            _inputActions.Attack.Secondary.started -= Begin;
        }
    }

    public override void Begin(InputAction.CallbackContext callbackContext)
    {
        _character.currentAttack = this;
        Transform transform = _character.transform;
        _flamethrower = Instantiate(flamethrowerPrefab, transform.position + Offset*transform.forward, Quaternion.identity);
        _flamethrower.transform.rotation = Quaternion.Euler(_flamethrower.transform.rotation.x, _character.transform.rotation.eulerAngles.y-90, _flamethrower.transform.rotation.z);
        
        _inputActions.Attack.Primary.Disable();
        _inputActions.Movement.Direction.Disable();
    }

    public override void End()
    {
        Debug.Log("end");
        _character.currentAttack = null;
        _inputActions.Attack.Primary.Enable();
        _inputActions.Movement.Direction.Enable();
        Destroy(_flamethrower);
    }

    internal override void PassMessage(AnimationState state)
    {
        
    }
}
