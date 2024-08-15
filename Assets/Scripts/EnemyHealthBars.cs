using UnityEngine;

public class EnemyHealthBars : MonoBehaviour
{
    private static EnemyHealthBars _instance;

    public static EnemyHealthBars Instance => _instance;

    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Camera canvasCamera;
    
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

    public void SpawnHealthBar(Transform enemy, EnemyController controller, Vector3 offset)
    {
        Vector3 pos = canvasCamera.WorldToScreenPoint(enemy.position);
        GameObject healthBar = Instantiate(healthBarPrefab, pos, Quaternion.identity, transform);
        EnemyHealthBar script = healthBar.GetComponent<EnemyHealthBar>();
        script.Initialize(controller, offset, canvasCamera);
    }
}
