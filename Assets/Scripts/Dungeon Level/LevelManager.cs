using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FischlWorks_FogWar;
using Newtonsoft.Json.Linq;
using Systems.Save;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour, ISavable
{
    [SerializeField] private Generator2D generator2D;
    [SerializeField] private RandomRoomData randomRoomDatas;
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject doorHallwayPrefab;
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject wallColliderPrefab;
    [SerializeField] private GameObject levelCenter;
    [SerializeField] private csFogWar fogOfWar;
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private int levelCount;
    [SerializeField] private GameObject endPopup;
    [SerializeField] private PlayerController playerController;

    private List<RoomController> _roomScripts = new List<RoomController>();
    private List<Transform> _roomColliders = new List<Transform>();
    private int _tileSize;
    private List<Generator2D.LevelData> _dungeonData;
    private bool _saveDataLoaded;
    private int _currentLevel;
    private RoomController _spawnRoom;
    private Generator2D.LevelData LevelData => _dungeonData[_currentLevel];

    public string id { get; } = "LevelManager";

    private void Awake()
    {
        saveManager.Load();
        if (!_saveDataLoaded) //If there is no save data, the player is on a new save and the level manager should generate a new set of levels
        {
            _dungeonData = new List<Generator2D.LevelData>();
            int seed = (int) System.DateTime.Now.Ticks;
            for (int x = 0; x < levelCount; x++)
            {
                _dungeonData.Add(generator2D.Generate(seed+x));
            }

            _currentLevel = 0;
        }

        Random.InitState(LevelData.RandomSeed);
        _tileSize = generator2D.TileSize;

        foreach (RectInt room in LevelData.Rooms)
        {
            _roomScripts.Add(PlaceRoom(room));
        }

        Vector2Int paddedSize = generator2D.Size + new Vector2Int(_tileSize, _tileSize);
        levelCenter.transform.position = new Vector3(
            ((float)generator2D.Size.x * _tileSize)/2, levelCenter.transform.position.y, ((float)generator2D.Size.y * _tileSize)/2);
        fogOfWar.Initialize(paddedSize*2, _tileSize/2);
        
        _spawnRoom = _roomScripts[Random.Range(0, _roomScripts.Count)];
        _spawnRoom.SetAsSpawnRoom();

        GameObject floorCollider = new GameObject("Floor Collider");
        floorCollider.layer = LayerMask.NameToLayer("Floor");
        BoxCollider fc = floorCollider.AddComponent<BoxCollider>();
        fc.size = new Vector3((paddedSize.x-2)*_tileSize, 0.001f, (paddedSize.y-2)*_tileSize);
        floorCollider.transform.position = new Vector3(
            ((float)generator2D.Size.x * _tileSize)/2, levelCenter.transform.position.y, ((float)generator2D.Size.y * _tileSize)/2);
        //Build and initialize navmesh surfaces
        List<NavMeshBuildSource> allSources = new List<NavMeshBuildSource>();
        Vector3 size = new Vector3(generator2D.Size.x * generator2D.TileSize, 10,
            generator2D.Size.y * generator2D.TileSize);
        Vector3 center = new Vector3(size.x / 2, 0, size.z / 2);
        Bounds bounds = new Bounds(center, size);
        foreach (Transform colliders in _roomColliders)
        {
            List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
            NavMeshBuilder.CollectSources(colliders, 
                LayerMask.GetMask("Floor", "Walls"),
                NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), sources);
            allSources.AddRange(sources);
        }
        
        NavMeshData data = NavMeshBuilder.BuildNavMeshData(new NavMeshBuildSettings(){overrideTileSize = true, tileSize = 128}, allSources, bounds, Vector3.zero,
            Quaternion.identity);
        NavMesh.AddNavMeshData(data);
    }

    private void Start()
    {
        GlobalReferences.Instance.Player.transform.position = _spawnRoom.transform.position + _spawnRoom.GetRandomPosition();

        RoomController exitRoom = _roomScripts[Random.Range(0, _roomScripts.Count)];
        while(exitRoom == _spawnRoom) {exitRoom = _roomScripts[Random.Range(0, _roomScripts.Count)];}
        exitRoom.SpawnExit(exitPrefab, this);
    }

    public void ExitLevel()
    {
        _currentLevel++;
        if (_currentLevel < levelCount)
        {
            saveManager.Save();
            StartCoroutine(LoadSceneAsync());
        }
        else
        {
            File.Delete(SaveManager.DefaultSavePath);
            endPopup.SetActive(true);
            Time.timeScale = 0;
            playerController.enabled = false;
        }
    }

    public void LoadTitleScreen()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Scenes/Title Screen");
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Scenes/DungeonGeneration");
        
        loadingScreen.gameObject.SetActive(true);
        while (!loadSceneAsync.isDone)
        {
            loadingScreen.RotateIcon();
            yield return null;
        }
    }

    private bool IsCorner(Vector2Int pos, RectInt bounds)
    {
        return (pos.x == bounds.xMax - 1 || pos.x == bounds.xMin) &&
               (pos.y == bounds.yMax - 1 || pos.y == bounds.yMin);
    }

    private RoomController PlaceRoom(RectInt bounds)
    {
        var location = bounds.position;
        var roomSize = bounds.size;

        GameObject room = Instantiate(roomPrefab, transform);
        room.name = "Room " + location;
        GameObject walls = new GameObject("Walls");
        walls.transform.parent = room.transform;
        walls.layer = LayerMask.NameToLayer("Walls");
        
        GameObject floor = new GameObject("Floor Tiles");
        floor.transform.parent = room.transform;
        floor.layer = LayerMask.NameToLayer("Floor");
        
        GameObject doors = new GameObject("Doors");
        doors.transform.parent = room.transform;
        doors.layer = LayerMask.NameToLayer("Walls");

        GameObject colliders = new GameObject("Colliders");
        colliders.transform.parent = room.transform;
        colliders.layer = LayerMask.NameToLayer("Walls");
        _roomColliders.Add(colliders.transform);
        
        RoomController script = room.GetComponent<RoomController>();
        Vector2 center = bounds.center*_tileSize - new Vector2((float)_tileSize/2, (float)_tileSize/2);
        room.transform.position = new Vector3(center.x, 0, center.y);
        Vector2Int pos;
        for(int x = 0; x < roomSize.x; x++){
                
            for (int y = 0; y < roomSize.y; y++)
            {
                pos = location + new Vector2Int(x, y);
                if (!IsCorner(pos, bounds))
                {
                    if (pos.x == location.x)
                    {
                        DoorOrWall(pos, 90, walls.transform, doors.transform);
                    }
                    else if (pos.x == location.x + roomSize.x - 1)
                    {
                        DoorOrWall(pos, 270, walls.transform, doors.transform);
                    }
                    else if (pos.y == location.y)
                    {
                        DoorOrWall(pos, 0, walls.transform, doors.transform);
                    }
                    else if (pos.y == location.y + roomSize.y - 1)
                    {
                        DoorOrWall(pos, 180, walls.transform, doors.transform);
                    }
                    else
                    {
                        PlaceFloorTile(pos, floor.transform);
                    }
                }
            }
        }

        GameObject floorCollider = new GameObject("Floor Collider");
        floorCollider.transform.parent = colliders.transform;
        floorCollider.layer = LayerMask.NameToLayer("Floor");
        BoxCollider fc = floorCollider.AddComponent<BoxCollider>();
        fc.size = new Vector3((bounds.size.x-2)*_tileSize, 0.001f, (bounds.size.y-2)*_tileSize);
        floorCollider.transform.localPosition = Vector3.zero;

        CreateWallColliders(bounds, roomSize, colliders.transform);

        Grid2D<Generator2D.CellType> roomGrid = new Grid2D<Generator2D.CellType>(roomSize, Vector2Int.zero);
        for (int y = 0; y < roomSize.y; y++)
        {
            for (int x = 0; x < roomSize.x; x++)
            {
                roomGrid[x, y] = LevelData.Grid[location.x+x, location.y+y];
            }
        }

        List<Door> doorScripts = doors.GetComponentsInChildren<Door>().ToList();

        script.Initialize(bounds, _tileSize, doorScripts, randomRoomDatas, colliders.transform);
        return script;
    }

    private void CreateWallColliders(RectInt bounds, Vector2Int roomSize, Transform parent)
    {
        Vector2Int location = bounds.position;
        Vector2Int startPos = location;
        Vector2Int pos;
        for (int x = 0; x < roomSize.x; x++)
        {
            pos = location + new Vector2Int(x, 0);
            if (LevelData.Grid[pos] == Generator2D.CellType.Entrance || IsCorner(pos, bounds))
            {
                if(pos.x - startPos.x - 1 > 0)
                    PlaceWallCollider(startPos*_tileSize, pos.x - startPos.x - 1, 0,1, parent);
                startPos = pos;
            }
        }

        location = new Vector2Int(bounds.x, bounds.yMax-1);
        startPos = location;
        for (int x = 0; x < roomSize.x; x++)
        {
            pos = location + new Vector2Int(x, 0);
            if (LevelData.Grid[pos] == Generator2D.CellType.Entrance || IsCorner(pos, bounds))
            {
                if(pos.x - startPos.x - 1 > 0)
                    PlaceWallCollider(startPos*_tileSize, pos.x - startPos.x - 1, 180,-1, parent);
                startPos = pos;
            }
        }
        
        location = new Vector2Int(bounds.xMax-1, bounds.y);
        startPos = location;
        for (int y = 0; y < roomSize.y; y++)
        {
            pos = location + new Vector2Int(0, y);
            if (LevelData.Grid[pos] == Generator2D.CellType.Entrance || IsCorner(pos, bounds))
            {
                if(pos.y - startPos.y - 1 > 0)
                    PlaceWallCollider(startPos*_tileSize, pos.y - startPos.y - 1, -90,1, parent);
                startPos = pos;
            }
        }
        
        location = new Vector2Int(bounds.xMin, bounds.y);
        startPos = location;
        for (int y = 0; y < roomSize.y; y++)
        {
            pos = location + new Vector2Int(0, y);
            if (LevelData.Grid[pos] == Generator2D.CellType.Entrance || IsCorner(pos, bounds))
            {
                if(pos.y - startPos.y - 1 > 0)
                    PlaceWallCollider(startPos*_tileSize, pos.y - startPos.y - 1, 90, -1,parent);
                startPos = pos;
            }
        }
    }

    private void PlaceWallCollider(Vector2 pos, float length, int rotation, int direction, Transform parent)
    {
        GameObject wall = Instantiate(wallColliderPrefab, parent);
        wall.transform.position = new Vector3(pos.x, 0, pos.y);
        wall.transform.localRotation = Quaternion.Euler(0, rotation, 0);
        BoxCollider col = wall.GetComponent<BoxCollider>();
        col.size = new Vector3(col.size.x*length + 2.7f, col.size.y, col.size.z);
        col.center = new Vector3(direction*(col.center.x+((col.size.x - 2.7f)/2 + 2)), col.center.y, col.center.z);
    }

    private void PlaceFloorTile(Vector2Int location, Transform parent = null)
    {
        GameObject tile = Instantiate(floorTilePrefab, new Vector3(location.x * _tileSize, 0, location.y * _tileSize), Quaternion.identity);
        if (parent != null) 
            tile.transform.parent = parent;
    }

    private void DoorOrWall(Vector2Int pos, int rotation, Transform wallParent = null, Transform doorParent = null)
    {
        if (LevelData.Grid[pos] != Generator2D.CellType.Entrance)
        {
            PlaceTile(wallPrefab, pos, rotation, wallParent);
        }
        else if (LevelData.Grid[pos] == Generator2D.CellType.Entrance)
        {
            GameObject doorTile = PlaceTile(doorPrefab, pos, rotation, doorParent);
            if (!Physics.CheckBox(doorTile.transform.position, new Vector3(2, 2, 2), Quaternion.identity,
                LayerMask.GetMask("Walls")))
            {
                GameObject doorHallway = Instantiate(doorHallwayPrefab, doorTile.transform.position, Quaternion.Euler(0, rotation, 0));
                if (doorParent != null)
                    doorHallway.transform.parent = doorParent;
            }
                
        }
    }

    private GameObject PlaceTile(GameObject prefab, Vector2Int location, int rotation, Transform parent = null)
    {
        GameObject tile = Instantiate(prefab, new Vector3(location.x * _tileSize, 0, location.y * _tileSize), Quaternion.Euler(0, rotation, 0));
        if (parent != null) 
            tile.transform.parent = parent;
        return tile;
    }

    #region Save Logic

    public object SaveState()
    {
        return new SaveData()
        {
            LevelDatas = _dungeonData,
            CurrentLevel = _currentLevel
        };
    }

    public void LoadState(JObject state)
    {
        var saveData = state.ToObject<SaveData>();
        if (saveData.LevelDatas.Count > 0)
        {
            _saveDataLoaded = true;
            _dungeonData = saveData.LevelDatas;
            _currentLevel = saveData.CurrentLevel;
        }
    }

    [Serializable]
    public struct SaveData
    {
        public List<Generator2D.LevelData> LevelDatas;
        public int CurrentLevel;
    }

    #endregion
}
