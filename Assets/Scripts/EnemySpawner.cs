using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Vector2 _center;
    private Vector2 _size;
    private List<Transform> _waypoints;
    public void Initialize(Vector2 center, Vector2 size)
    {
        _center = center;
        _size = size;
        _waypoints = new List<Transform>();
        Vector2 waypoint = _center + (size/2 - new Vector2(1,1));
        _waypoints.Add(new GameObject().transform);
        _waypoints[0].SetParent(transform);
        _waypoints[0].transform.position = new Vector3(waypoint.x, 0, waypoint.y);
        
        waypoint = _center - (size/2 - new Vector2(1,1));
        _waypoints.Add(new GameObject().transform);
        _waypoints[1].SetParent(transform);
        _waypoints[1].transform.position = new Vector3(waypoint.x, 0, waypoint.y);
    }
    public void SpawnEnemies(List<GameObject> enemies)
    {
        if (_waypoints == null)
        {
            Debug.Log("Enemy spawn failed - waypoints were not created");
            return;
        }
        foreach (GameObject enemy in enemies)
        {
            GameObject enemyInstance = Instantiate(enemy, transform, false);
            float x = _center.x + Random.Range(0f, _size.x / 2);
            float y = _center.y + Random.Range(0f, _size.y / 2);
            enemyInstance.transform.position = new Vector3(x, 0, y);
            EnemyController controller = enemyInstance.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.SetWaypoints(_waypoints);
            }
        }
    }
}
