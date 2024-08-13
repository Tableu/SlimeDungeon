using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomController : MonoBehaviour
{
    [SerializeField] private RoomCamera roomCamera;
    [SerializeField] private RoomDoors roomDoors;
    [SerializeField] private List<EnemyController> presetEnemies = new List<EnemyController>();
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private new BoxCollider collider;
    
    private RectInt _bounds;
    private float _tileSize;
    private int _enemyCount = 0;
    private List<EnemyController> _enemies = new List<EnemyController>();
    private LevelGenerationData.Room _roomData;
    private bool _isSpawnRoom = false;
    
    public Action OnAllEnemiesDead;
    public bool AllEnemiesDead => _enemyCount < 1;

    public void Initialize(RectInt bounds,  float tileSize, List<Door> doors, 
        LevelGenerationData levelGenerationData, Transform colliderTransform)
    {
        _bounds = bounds;
        _tileSize = tileSize;
        this.waypoints = new List<Transform>();
        GameObject waypoints = new GameObject("Waypoints");
        waypoints.transform.parent = transform;
        Vector2 waypoint = (bounds.max - new Vector2(2,2))*tileSize;
        this.waypoints.Add(new GameObject("Waypoint 1").transform);
        this.waypoints[0].SetParent(waypoints.transform);
        this.waypoints[0].transform.position = new Vector3(waypoint.x, 0, waypoint.y);
        
        waypoint = (bounds.min + new Vector2(1,1))*tileSize;
        this.waypoints.Add(new GameObject("Waypoint 2").transform);
        this.waypoints[1].SetParent(waypoints.transform);
        this.waypoints[1].transform.position = new Vector3(waypoint.x, 0, waypoint.y);

        _enemies = new List<EnemyController>();
        roomCamera.Initialize(bounds, tileSize);
        roomDoors.Initialize(this, doors, bounds, tileSize);
        if (levelGenerationData != null)
        {
            int i = 0;

            do
            {
                _roomData = levelGenerationData.GetRandomElement();
                if (_bounds.width < _roomData.Layout.MaxSize && _bounds.width >= _roomData.Layout.MinSize ||
                    _bounds.height < _roomData.Layout.MaxSize && _bounds.width >= _roomData.Layout.MinSize)
                {
                    break;
                }

                i++;
            } while (i < 20);

            List<RoomLayoutData.DecorationSpot> decorationPositions = PlaceRoomLayout(colliderTransform, bounds,
                tileSize,
                doors.Select(o => o.transform.position).ToList());
            DecorateRoom(decorationPositions);
        }
    }

    public void AddEnemy(EnemyController controller)
    {
        if(waypoints != null)
            controller.SetWaypoints(waypoints);
        controller.OnDeath += OnEnemyDeath;
        _enemies.Add(controller);
        _enemyCount++;
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
        foreach (EnemyController enemyController in presetEnemies)
        {
            AddEnemy(enemyController);
        }
    }

    private void OnDestroy()
    {
        if (_enemies == null)
            return;
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
        if (waypoints == null)
        {
            Debug.Log("Enemy spawn failed - waypoints were not created");
            return;
        }

        if (_roomData.EnemyGroups == null || _isSpawnRoom)
            return;

        List<GameObject> enemies = _roomData.EnemyGroups.GetRandomGroup();

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
                controller.SetWaypoints(waypoints);
                controller.OnDeath += OnEnemyDeath;
                _enemies.Add(controller);
                _enemyCount++;
            }
        }
    }

    private List<RoomLayoutData.DecorationSpot> PlaceRoomLayout(Transform center, RectInt bounds, float tileSize, List<Vector3> doors)
    {
        List<RoomLayoutData.DecorationSpot> decorationSpots = new List<RoomLayoutData.DecorationSpot>();
        foreach (RoomLayoutData.LayoutObject layoutObject in _roomData.Layout.LayoutObjects)
        {
            Vector3 pos = layoutObject.RandomPosition ? GetRandomPosition() : 
                new Vector3((bounds.width-3)*tileSize/2*layoutObject.RelativePosition.x,
                0, (bounds.height-3)*tileSize/2*layoutObject.RelativePosition.y);;

            if (doors.Any(o => Vector3.Distance(transform.position + pos, o) < 2))
                continue;
            
            if (layoutObject.DecorationSpot)
            {
                GameObject spot = new GameObject("Decoration Spot");
                spot.transform.parent = center;
                spot.transform.localPosition = pos;
                spot.transform.localRotation = Quaternion.Euler(layoutObject.Rotation);
                decorationSpots.Add(new RoomLayoutData.DecorationSpot(spot.transform, layoutObject.NearWall));
                continue;
            }
            GameObject instance = Instantiate(layoutObject.Prefab, center.position + pos, 
                Quaternion.Euler(layoutObject.Rotation), center);
            CharacterItem characterItem = instance.GetComponent<CharacterItem>();
            if (characterItem)
                characterItem.Initialize(_roomData.Characters, this);
            Decorations spots = instance.GetComponent<Decorations>();
            if(spots != null)
                decorationSpots.AddRange(spots.Locations);
        }

        return decorationSpots;
    }
    
    private void DecorateRoom(List<RoomLayoutData.DecorationSpot> decorationSpots)
    {
        List<RoomLayoutData.RandomDecoration> decorations = new List<RoomLayoutData.RandomDecoration>(_roomData.Layout.RandomDecorations);
        int i = 0;
        int x = 0;
        while (i < decorationSpots.Count && x < decorationSpots.Count*10)
        {
            RoomLayoutData.RandomDecoration decoration = RoomLayoutData.GetRandomDecoration(decorations);
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
                    characterItem.Initialize(_roomData.Characters, this);
                
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
    
    private Vector3 GetRandomPosition()
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

    public Vector3 GetRandomPositionInBounds()
    {
        int xExtent = (int) Mathf.Ceil(collider.bounds.extents.x);
        int yExtent = (int) Mathf.Ceil(collider.bounds.extents.y);
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
