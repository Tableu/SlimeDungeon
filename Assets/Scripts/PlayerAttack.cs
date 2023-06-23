using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float Offset;
    public float Speed;
    [SerializeField] private GameObject fireballPrefab;
    private PlayerInputActions _playerInputActions;
    // Start is called before the first frame update
    void Start()
    {
        _playerInputActions = GlobalReferences.Instance.PlayerInputActions;   
        _playerInputActions.Attack.Primary.started += delegate(InputAction.CallbackContext context)
        {
            GameObject fireball = Instantiate(fireballPrefab, transform.position + Offset*transform.forward, Quaternion.identity);
            Rigidbody r = fireball.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddForce(transform.forward*Speed, ForceMode.Impulse);
            }
        };
    }
}
