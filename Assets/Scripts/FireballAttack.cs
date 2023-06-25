using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Fireball Attack", menuName = "Attacks/Fireball Attack")]
public class FireballAttack : Attack
{
    public float Offset;
    public float Speed;
    [SerializeField] private GameObject fireballPrefab;
    private Character _character;
    private PlayerInputActions _inputActions;

    public override void Equip(Character character, PlayerInputActions inputActions = null)
    {
        _character = character;
        _inputActions = inputActions;
        if (inputActions != null)
        {
            inputActions.Attack.Primary.started += Begin;
        }
    }
    public override void Drop()
    {
        if (_inputActions != null)
        {
            _inputActions.Attack.Primary.started -= Begin;
        }
    }

    public override void Begin(InputAction.CallbackContext callbackContext)
    {
        Transform transform = _character.transform;
        GameObject fireball = Instantiate(fireballPrefab, transform.position + Offset*transform.forward, Quaternion.identity);
        Rigidbody r = fireball.GetComponent<Rigidbody>();
        if (r != null)
        {
            r.AddForce(transform.forward*Speed, ForceMode.Impulse);
        }
        _character.Animator.SetTrigger("Attack");
    }

    public override void End()
    {
        
    }
}