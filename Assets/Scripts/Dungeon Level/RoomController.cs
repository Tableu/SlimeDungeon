using System;
using System.Collections.Generic;
using System.Linq;
using Controller.Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomController : MonoBehaviour
{
    [SerializeField] private RoomCamera roomCamera;
    [SerializeField] private RoomDoors roomDoors;
    
    private RectInt _bounds;
    private float _tileSize;
    private List<Transform> _waypoints;
    private int _enemyCount = 0;
    private List<EnemyController> _enemies;
    private RoomData _roomData;
    
    public Action OnAllEnemiesDead;
    public bool AllEnemiesDead => _enemyCount < 1;

    public void Initialize(RectInt bounds,  float tileSize, List<Door> doors, 
        List<RoomDecorationData> roomLayoutDatas, RandomRoomTypeData randomRoomTypeDatas, Transform colliderTransform)
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
        roomCamera.Initialize(bounds, tileSize);
        roomDoors.Initialize(this, doors, bounds, tileSize);
        int i = 0;
        RoomDecorationData roomDecorationData;
        do
        {
            roomDecorationData = roomLayoutDatas[Random.Range(0, roomLayoutDatas.Count)];
            if (_bounds.width < roomDecorationData.MaxSize && _bounds.width >= roomDecorationData.MinSize ||
                _bounds.height < roomDecorationData.MaxSize && _bounds.width >= roomDecorationData.MinSize)
            {
                break;
            }
            i++;
        } while (i < 20);

        List<RoomDecorationData.DecorationSpot> decorationPositions = roomDecorationData.PlaceRoomLayout(colliderTransform, bounds, tileSize, 
            doors.Select(o=>o.transform.position).ToList(),this);
        _roomData = randomRoomTypeDatas.GetRandomElement();
        _roomData.DecorateRoom(decorationPositions);
    }
    public void SpawnEnemies()
    {
        if (_waypoints == null)
        {
            Debug.Log("Enemy spawn failed - waypoints were not created");
            return;
        }

        List<GameObject> enemies = _roomData.RandomEnemyGroups.GetRandomGroup();

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
            GameObject enemyInstance = Instantiate(enemy, enemyParent.transform.position + GetRandomPosition(), Quaternion.identity, enemyParent.transform);
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

    public void SpawnCapturedCharacter(CharacterData data)
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
        
        GameObject characterInstance = Instantiate(data.CapturedCharacter, capturedParent.transform, false);

        characterInstance.transform.localPosition = GetRandomPosition();
        CapturedCharacter script = characterInstance.GetComponent<CapturedCharacter>();
        if (script != null)
        {
            script.Initialize(this, data);
        }
    }

    public void SpawnExit(GameObject exit, LevelManager levelManager)
    {
        GameObject exitInstance = Instantiate(exit, transform, false);

        exitInstance.transform.localPosition = GetRandomPosition();
        Exit script = exitInstance.GetComponent<Exit>();
        if (script != null)
        {
            script.Initialize(this, levelManager);
        }
    }

    public Vector3 GetRandomPosition()
    {
        int xExtent = (int) Mathf.Ceil(((float) _bounds.width / 2 - 1)* _tileSize - _tileSize/2);
        int yExtent = (int) Mathf.Ceil(((float) _bounds.height / 2 - 1)* _tileSize - _tileSize/2);
        Vector3 randomPos;
        bool tileTaken;
        int i = 0;
        do
        {
            randomPos = new Vector3(Random.Range(-xExtent, xExtent), 0,
                Random.Range(-yExtent, yExtent));
            tileTaken = Physics.CheckBox(transform.position + randomPos, new Vector3(1f, 1, 1f),
                Quaternion.identity, LayerMask.GetMask("Walls", "Obstacles", "Enemy", "Items"));
            i++;
        } while (tileTaken && i < 100);

        return randomPos;
    }

    private void OnEnemyDeath()
    {
        _enemyCount--;
        if (AllEnemiesDead)
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
