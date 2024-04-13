using UnityEngine;

public class EnemyHealthBars : MonoBehaviour
{
    private static EnemyHealthBars _instance;

    public static EnemyHealthBars Instance => _instance;

    [SerializeField] private GameObject enemyStatBars;
    
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

    public void SpawnHealthBar(Transform enemy, EnemyController controller)
    {
        GameObject statbars = Instantiate(enemyStatBars, enemy.position, enemyStatBars.transform.rotation, transform);
        EnemyStatBar script = statbars.GetComponent<EnemyStatBar>();
        script.Initialize(controller);
    }
}
