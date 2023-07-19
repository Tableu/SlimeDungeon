using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    private static GlobalReferences _instance;

    public static GlobalReferences Instance => _instance;
    [SerializeField] private GameObject player;
    [SerializeField] private Canvas canvas;
    [SerializeField] private ElementTypeManager typeManager;
    
    public PlayerInputActions PlayerInputActions
    {
        get;
        private set;
    }

    public GameObject Player => player;
    public Canvas Canvas => canvas;
    public ElementTypeManager TypeManager => typeManager;

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
