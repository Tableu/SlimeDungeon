using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    private static GlobalReferences _instance;

    public static GlobalReferences Instance => _instance;
    [SerializeField] private GameObject player;
    [SerializeField] private Canvas canvas;
    [SerializeField] private ElementTypeManager typeManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameObject enemyHealthbars;

    public GameObject Player => player;
    public Canvas Canvas => canvas;
    public ElementTypeManager TypeManager => typeManager;
    public LevelManager LevelManager => levelManager;
    public GameObject EnemyHealthbars => enemyHealthbars;

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
    }
}
