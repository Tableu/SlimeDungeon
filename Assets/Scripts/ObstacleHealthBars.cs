using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHealthBars : MonoBehaviour
{
    private static ObstacleHealthBars _instance;

    public static ObstacleHealthBars Instance => _instance;

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

    public void SpawnHealthBar(Transform enemy, PhysicsObstacle controller, Vector3 offset)
    {
        Vector3 pos = canvasCamera.WorldToScreenPoint(enemy.position);
        GameObject healthBar = Instantiate(healthBarPrefab, pos, Quaternion.identity, transform);
        ObstacleHealthBar script = healthBar.GetComponent<ObstacleHealthBar>();
        script.Initialize(controller, offset, canvasCamera);
    }
}
