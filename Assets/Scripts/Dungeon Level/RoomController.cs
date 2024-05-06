using System;
using System.Collections.Generic;
using Controller.Form;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomController : MonoBehaviour
{
    public Action OnAllEnemiesDead;
    private Vector2 _center;
    private Vector2 _size;
    private float _tileSize;
    private List<Transform> _waypoints;
    private int _enemyCount = 0;
    private List<EnemyController> _enemies;
    public void Initialize(Vector2 center, Vector2 size, float tileSize)
    {
        _center = center;
        _size = size;
        _tileSize = tileSize;
        _waypoints = new List<Transform>();
        GameObject waypoints = new GameObject("Waypoints");
        waypoints.transform.parent = transform;
        Vector2 waypoint = _center + (size/2 - new Vector2(1+tileSize,1+tileSize));
        _waypoints.Add(new GameObject("Waypoint 1").transform);
        _waypoints[0].SetParent(waypoints.transform);
        _waypoints[0].transform.position = new Vector3(waypoint.x, 0, waypoint.y);
        
        waypoint = _center - (size/2 - new Vector2(5,5));
        _waypoints.Add(new GameObject("Waypoint 2").transform);
        _waypoints[1].SetParent(waypoints.transform);
        _waypoints[1].transform.position = new Vector3(waypoint.x, 0, waypoint.y);

        _enemies = new List<EnemyController>();
    }
    public void SpawnEnemies(List<GameObject> enemies)
    {
        if (_waypoints == null)
        {
            Debug.Log("Enemy spawn failed - waypoints were not created");
            return;
        }

        GameObject enemyParent = new GameObject("Enemies")
        {
            transform =
            {
                parent = transform,
                localPosition = Vector3.zero
            },
            layer = LayerMask.NameToLayer("Enemy")
        };
        
        foreach (GameObject enemy in enemies)
        {
            GameObject enemyInstance = Instantiate(enemy, enemyParent.transform, false);
            
            float boundX = (_size.x / 2) - 1;
            float boundY = (_size.y / 2) - 1;
            
            float x = _center.x + Random.Range(-1*boundX, boundX);
            float y = _center.y + Random.Range(-1*boundY, boundY);
            enemyInstance.transform.position = new Vector3(x, 0, y);
            EnemyController controller = enemyInstance.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.SetWaypoints(_waypoints);
                controller.OnDeath += OnEnemyDeath;
                _enemies.Add(controller);
                _enemyCount++;
            }
        }
    }

    public void SpawnCapturedCharacter(FormData data)
    {
        GameObject capturedParent = new GameObject("Captured Characters")
        {
            transform =
            {
                parent = transform,
                localPosition = Vector3.zero
            },
            layer = LayerMask.NameToLayer("Items")
        };
        
        GameObject characterInstance = Instantiate(data.CapturedForm, capturedParent.transform, false);
        float boundX = (_size.x / 2) - 1 - _tileSize*2;
        float boundY = (_size.y / 2) - 1 - _tileSize*2;
        float x = _center.x + Random.Range(-1*boundX, boundX);
        float y = _center.y + Random.Range(-1*boundY, boundY);
        characterInstance.transform.position = new Vector3(x, 0, y);
        CapturedCharacter script = characterInstance.GetComponent<CapturedCharacter>();
        if (script != null)
        {
            script.Initialize(this, data);
        }
    }

    public void SpawnTreasureChest(GameObject chest)
    {
        GameObject chestParent = new GameObject("Treasure Chests")
        {
            transform =
            {
                parent = transform,
                localPosition = Vector3.zero
            },
            layer = LayerMask.NameToLayer("Items")
        };
        
        GameObject chestInstance = Instantiate(chest, chestParent.transform, false);
        float boundX = (_size.x / 2) - 1 - _tileSize*2;
        float boundY = (_size.y / 2) - 1 - _tileSize*2;
        float x = _center.x + Random.Range(-1*boundX, boundX);
        float y = _center.y + Random.Range(-1*boundY, boundY);
        chestInstance.transform.position = new Vector3(x, 0, y);
    }

    private void OnEnemyDeath()
    {
        _enemyCount--;
        if (_enemyCount < 1)
        {
            OnAllEnemiesDead?.Invoke();
        }
    }

    private void OnDestroy()
    {
        foreach (EnemyController controller in _enemies)
        {
            if (controller != null)
            {
                controller.OnDeath -= OnEnemyDeath;
            }
        }
    }
}
