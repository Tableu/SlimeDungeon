using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    private static GlobalReferences _instance;

    public static GlobalReferences Instance => _instance;
    [SerializeField] private GameObject player;
    
    public PlayerInputActions PlayerInputActions
    {
        get;
        private set;
    }

    public GameObject Player => player;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        PlayerInputActions = new PlayerInputActions();
        PlayerInputActions.Enable();
    }
}
