using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Fireball Attack", menuName = "Attacks/Fireball Attack")]
public class FireballAttack : Controller.Attack
{
    [SerializeField] private float offset;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float knockback;
    [SerializeField] private GameObject fireballPrefab;
    private PlayerInputActions _inputActions;
    public override void Equip(Controller.Character character)
    {
        this.character = character;
        _inputActions = character.playerInputActions;
        if (character.isPlayer && _inputActions != null)
        {
            _inputActions.Attack.Primary.started += Begin;
        }
    }
    public override void Drop()
    {
        if (character.isPlayer && _inputActions != null)
        {
            _inputActions.Attack.Primary.started -= Begin;
        }
    }

    public override void Begin(InputAction.CallbackContext callbackContext)
    {
        character.currentAttack = this;
        Transform transform = character.transform;
        GameObject fireball = Instantiate(fireballPrefab, transform.position + offset*transform.forward, Quaternion.identity);
        var script = fireball.GetComponent<Fireball>();
        script.Initialize(damage*character.form.damageMultiplier, knockback,transform.forward*speed*character.form.speedMultiplier, character.form.sizeMultiplier);
        character.animator.SetTrigger("Attack");
        if (character.isPlayer && _inputActions != null)
        {
            _inputActions.Attack.Disable();
            _inputActions.Movement.Disable();
        }
        OnSpellCast?.Invoke();
    }

    public override void End()
    {
        if (character.isPlayer && _inputActions != null)
        {
            _inputActions.Attack.Enable();
            _inputActions.Movement.Enable();
        }

        character.currentAttack = null;
    }

    internal override void PassMessage(Controller.AnimationState state)
    {
        if (Controller.AnimationState.AttackEnded == state)
        {
            End();
        }
    }
}