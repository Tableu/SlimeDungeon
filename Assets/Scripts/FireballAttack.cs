using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Fireball Attack", menuName = "Attacks/Fireball Attack")]
public class FireballAttack : Controller.Attack
{
    public float Offset;
    public float Speed;
    [SerializeField] private GameObject fireballPrefab;
    private Controller.Character _character;
    private PlayerInputActions _inputActions;

    public override void Equip(Controller.Character character, PlayerInputActions inputActions = null)
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
        _character.currentAttack = this;
        Transform transform = _character.transform;
        GameObject fireball = Instantiate(fireballPrefab, transform.position + Offset*transform.forward, Quaternion.identity);
        Rigidbody r = fireball.GetComponent<Rigidbody>();
        if (r != null)
        {
            r.AddForce(transform.forward*Speed, ForceMode.Impulse);
        }
        _character.animator.SetTrigger("Attack");
        _inputActions.Attack.Primary.Disable();
        _inputActions.Movement.Direction.Disable();
    }

    public override void End()
    {
        _inputActions.Attack.Primary.Enable();
        _inputActions.Movement.Direction.Enable();
        _character.currentAttack = null;
    }

    internal override void PassMessage(Controller.AnimationState state)
    {
        if (Controller.AnimationState.AnimationAttackEnded == state)
        {
            End();
        }
    }
}