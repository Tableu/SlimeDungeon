using System;
using System.Collections.Generic;
using System.Linq;
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
    private bool _isSpawnRoom = false;
    
    public Action OnAllEnemiesDead;
    public bool AllEnemiesDead => _enemyCount < 1;

    public void Initialize(RectInt bounds,  float tileSize, List<Door> doors, 
        RandomRoomData randomRoomDatas, Transform colliderTransform)
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
        
        do
        {
            _roomData = randomRoomDatas.GetRandomElement();
            if (_bounds.width < _roomData.MaxSize && _bounds.width >= _roomData.MinSize ||
                _bounds.height < _roomData.MaxSize && _bounds.width >= _roomData.MinSize)
            {
                break;
            }
            i++;
        } while (i < 20);

        List<RoomData.DecorationSpot> decorationPositions = PlaceRoomLayout(colliderTransform, bounds, tileSize, 
            doors.Select(o=>o.transform.position).ToList());
        DecorateRoom(decorationPositions);
    }
    
    public void SetAsSpawnRoom()
    {
        _isSpawnRoom = true;
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

    private void Start()
    {
        SpawnEnemies();
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

    private void SpawnEnemies()
    {
        if (_waypoints == null)
        {
            Debug.Log("Enemy spawn failed - waypoints were not created");
            return;
        }

        if (_roomData.RandomEnemyGroups == null || _isSpawnRoom)
            return;

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

    private List<RoomData.DecorationSpot> PlaceRoomLayout(Transform center, RectInt bounds, float tileSize, List<Vector3> doors)
    {
        List<RoomData.DecorationSpot> decorationSpots = new List<RoomData.DecorationSpot>();
        foreach (RoomData.LayoutObject layoutObject in _roomData.LayoutObjects)
        {
            Vector3 pos;
            if (layoutObject.DecorationSpot)
            {
                pos = GetRandomPosition();
                if (doors.Any(o => Vector3.Distance(transform.position + pos, o) < 2))
                    continue;
                GameObject spot = new GameObject("Decoration Spot");
                spot.transform.parent = center;
                spot.transform.localPosition = pos;
                spot.transform.localRotation = Quaternion.Euler(layoutObject.Rotation);
                decorationSpots.Add(new RoomData.DecorationSpot(spot.transform, layoutObject.NearWall));
                continue;
            }
            pos = new Vector3((bounds.width-3)*tileSize/2*layoutObject.RelativePosition.x,
                0, (bounds.height-3)*tileSize/2*layoutObject.RelativePosition.y);
            if (doors.Any(o => Vector3.Distance(transform.position + pos, o) < 2))
                continue;
            GameObject instance = Instantiate(layoutObject.Prefab, center.position + pos, 
                Quaternion.Euler(layoutObject.Rotation), center);
            CharacterItem characterItem = instance.GetComponent<CharacterItem>();
            if (characterItem)
                characterItem.Initialize(_roomData.RandomCharacterItems, this);
            Decorations spots = instance.GetComponent<Decorations>();
            if(spots != null)
                decorationSpots.AddRange(spots.Locations);
        }

        return decorationSpots;
    }
    
    private void DecorateRoom(List<RoomData.DecorationSpot> decorationSpots)
    {
        List<RoomData.RandomDecoration> decorations = new List<RoomData.RandomDecoration>(_roomData.RandomDecorations);
        int i = 0;
        int x = 0;
        while (i < decorationSpots.Count && x < decorationSpots.Count*10)
        {
            RoomData.RandomDecoration decoration = RoomData.GetRandomDecoration(decorations);
            BoxCollider col = decoration.Prefab.GetComponent<BoxCollider>();
            bool tileTaken = false;
            if (col != null)
            {
                tileTaken = Physics.CheckBox(decorationSpots[i].Location.position, col.size/2, 
                    decorationSpots[i].Location.rotation, LayerMask.GetMask("Walls", "Obstacles"));
            }

            if (!tileTaken && (!decoration.RequireWall || decorationSpots[i].NearWall))
            {
                GameObject decorationObject = Instantiate(decoration.Prefab, decorationSpots[i].Location);
                CharacterItem characterItem = decorationObject.GetComponent<CharacterItem>();
                if (characterItem)
                    characterItem.Initialize(_roomData.RandomCharacterItems, this);
                
                col = decorationObject.GetComponent<BoxCollider>();
                if (col != null && decorationSpots[i].NearWall) //Purpose is to snap decorations to the wall
                {
                    Physics.Raycast(decorationSpots[i].Location.position, decorationSpots[i].Location.forward * -1,
                        out RaycastHit hit, Single.PositiveInfinity, LayerMask.GetMask("Walls"));
                    Vector3 diff = hit.point - col.ClosestPoint(hit.point) + decorationSpots[i].Location.forward*0.1f;
                    decorationObject.transform.position += new Vector3(diff.x, 0, diff.z);
                }
                
                i++;
            }
            x++;
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
}
