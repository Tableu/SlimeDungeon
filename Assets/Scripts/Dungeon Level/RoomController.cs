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
    private List<Transform> _waypoints;
    private int _enemyCount = 0;
    private List<EnemyController> _enemies;
    public void Initialize(Vector2 center, Vector2 size)
    {
        _center = center;
        _size = size;
        _waypoints = new List<Transform>();
        GameObject waypoints = new GameObject("Waypoints");
        waypoints.transform.parent = transform;
        Vector2 waypoint = _center + (size/2 - new Vector2(5,5));
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

            float x = _center.x + Random.Range(0f, (_size.x / 2) - 1);
            float y = _center.y + Random.Range(0f, (_size.y / 2) - 1);
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
        float x = _center.x + Random.Range(0f, (_size.x / 2) - 1 - 4);
        float y = _center.y + Random.Range(0f, (_size.y / 2) - 1 - 4);
        characterInstance.transform.position = new Vector3(x, 0, y);
        CapturedCharacter script = characterInstance.GetComponent<CapturedCharacter>();
        if (script != null)
        {
            script.Initialize(this, data);
        }
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
