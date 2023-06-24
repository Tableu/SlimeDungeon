using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public abstract class Attack : ScriptableObject
{
    public abstract void Equip(Character character, PlayerInputActions inputActions = null);
    public abstract void Drop();
    public abstract void Begin(InputAction.CallbackContext callbackContext);
    public abstract void End();
}
