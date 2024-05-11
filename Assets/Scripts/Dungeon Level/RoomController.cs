using System;
using System.Collections.Generic;
using Controller.Form;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomController : MonoBehaviour
{
    public Action OnAllEnemiesDead;
    private RectInt _bounds;
    private float _tileSize;
    private List<Transform> _waypoints;
    private int _enemyCount = 0;
    private List<EnemyController> _enemies;
    private Grid2D<Generator2D.CellType> _roomGrid;

    public void Initialize(RectInt bounds,  float tileSize, Grid2D<Generator2D.CellType> roomGrid)
    {
        _bounds = bounds;
        _tileSize = tileSize;
        _waypoints = new List<Transform>();
        GameObject waypoints = new GameObject("Waypoints");
        waypoints.transform.parent = transform;
        Vector2 waypoint = (bounds.max - new Vector2(2,2))*tileSize;
        _waypoints.Add(new GameObject("Waypoint 1").transform);
        _waypoints[0].SetParent(waypoints.transform);
        _waypoints[0].transform.position = new Vector3(waypoint.x, 0, waypoint.y);
        
        waypoint = (bounds.min + new Vector2(1,1))*tileSize;
        _waypoints.Add(new GameObject("Waypoint 2").transform);
        _waypoints[1].SetParent(waypoints.transform);
        _waypoints[1].transform.position = new Vector3(waypoint.x, 0, waypoint.y);

        _enemies = new List<EnemyController>();
        _roomGrid = roomGrid;
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
            
            Vector2Int randPos = GetRandomPosition();
            _roomGrid[randPos] = Generator2D.CellType.Enemy;

            enemyInstance.transform.position = new Vector3(
                (_bounds.x+randPos.x)*_tileSize + _tileSize/2, 0, 
                (_bounds.y+randPos.y)*_tileSize + _tileSize/2);
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
        Vector2Int randPos = GetRandomPosition();
        _roomGrid[randPos] = Generator2D.CellType.Character;
        
        characterInstance.transform.position = new Vector3(
            (_bounds.x+randPos.x)*_tileSize + _tileSize/2, 0, 
            (_bounds.y+randPos.y)*_tileSize + _tileSize/2);
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
        Vector2Int randPos = GetRandomPosition();
        _roomGrid[randPos] = Generator2D.CellType.Chest;

        chestInstance.transform.position = new Vector3(
            (_bounds.x+randPos.x)*_tileSize + _tileSize/2, 0, 
            (_bounds.y+randPos.y)*_tileSize + _tileSize/2);
    }

    public void SpawnExit(GameObject exit, LevelManager levelManager)
    {
        GameObject exitInstance = Instantiate(exit, transform, false);
        Vector2Int randPos = GetRandomPosition();
        _roomGrid[randPos] = Generator2D.CellType.Exit;

        exitInstance.transform.position = new Vector3(
            (_bounds.x+randPos.x)*_tileSize + _tileSize/2, 0, 
            (_bounds.y+randPos.y)*_tileSize + _tileSize/2);

        Exit script = exitInstance.GetComponent<Exit>();
        if (script != null)
        {
            script.Initialize(this, levelManager);
        }
    }

    private Vector2Int GetRandomPosition()
    {
        Vector2Int randPos = new Vector2Int(
            Random.Range(2, _bounds.size.x - 2), 
            Random.Range(2, _bounds.size.y - 2));
        int i = 0;
        while (_roomGrid[randPos] != Generator2D.CellType.Room && i < 5)
        {
            i++;
            randPos = new Vector2Int(
                Random.Range(2, _bounds.size.x - 2), 
                Random.Range(2, _bounds.size.y - 2));
        }

        return randPos;
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
