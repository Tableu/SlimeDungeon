using System;
using System.Collections.Generic;
using System.Linq;
using Controller.Player;
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
    [SerializeField] private RandomGameObjects randomEnemyGroups;
    [SerializeField] private RandomGameObjects randomTreasureChests;
    [SerializeField] private RandomFormData randomCapturedCharacters;
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject cornerWallPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject wallColliderPrefab;
    [SerializeField] private GameObject levelCenter;
    [SerializeField] private csFogWar fogOfWar;
    [SerializeField] private int levelCount;
    
    private List<RoomController> _roomScripts = new List<RoomController>();
    private int _tileSize;
    private List<Generator2D.LevelData> _dungeonData;
    private bool _saveDataLoaded;
    private int _currentLevel;
    private Generator2D.LevelData LevelData => _dungeonData[_currentLevel];

    public string id { get; } = "LevelManager";

    private void Awake()
    {
        saveManager.Load();
    }

    private void Start()
    {
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

        foreach (List<Vector2Int> path in LevelData.Hallways)
        {
            PlaceHallway(path);
        }
        
        Vector2Int paddedSize = generator2D.Size + new Vector2Int(_tileSize, _tileSize);
        levelCenter.transform.position = new Vector3(
            ((float)generator2D.Size.x * _tileSize)/2, levelCenter.transform.position.y, ((float)generator2D.Size.y * _tileSize)/2);
        fogOfWar.Initialize(paddedSize*2, _tileSize/2);
        
        RoomController spawnRoom = _roomScripts[Random.Range(0, _roomScripts.Count)];
        GlobalReferences.Instance.Player.transform.position = spawnRoom.transform.position;

        //Build and initialize navmesh surfaces
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        Vector3 size = new Vector3(generator2D.Size.x * generator2D.TileSize, 10,
            generator2D.Size.y * generator2D.TileSize);
        Vector3 center = new Vector3(size.x / 2, 0, size.z / 2);
        Bounds bounds = new Bounds(center, size);
        NavMeshBuilder.CollectSources(transform, 
            LayerMask.GetMask("Floor", "Walls"),
            NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), sources);
        NavMeshData data = NavMeshBuilder.BuildNavMeshData(new NavMeshBuildSettings(){overrideTileSize = true, tileSize = 512}, sources, bounds, Vector3.zero,
            Quaternion.identity);
        NavMesh.AddNavMeshData(data);
        
        List<CharacterData> capturedCharacters = randomCapturedCharacters.GetRandomGroup();
        List<GameObject> treasureChests = randomTreasureChests.GetRandomGroup();
        
        //Generate random indexes for placing the random characters
        List<int> capturedCharacterIndexes = GetUniqueRandomIndexes(_roomScripts.Count, capturedCharacters.Count);
        List<int> treasureChestIndexes = GetUniqueRandomIndexes(_roomScripts.Count, treasureChests.Count);
        int i = 0;
        using List<CharacterData>.Enumerator characterEnumerator = capturedCharacters.GetEnumerator();
        using List<GameObject>.Enumerator treasureEnumerator = treasureChests.GetEnumerator();
        foreach (RoomController spawner in _roomScripts)
        {
            if (spawner != spawnRoom)
            {
                spawner.SpawnEnemies(randomEnemyGroups.GetRandomGroup());
                if (capturedCharacterIndexes.Contains(i))
                {
                    characterEnumerator.MoveNext();
                    spawner.SpawnCapturedCharacter(characterEnumerator.Current);
                }

                if (treasureChestIndexes.Contains(i))
                {
                    treasureEnumerator.MoveNext();
                    spawner.SpawnTreasureChest(treasureEnumerator.Current);
                }
            }
            i++;
        }

        
        RoomController exitRoom = _roomScripts[Random.Range(0, _roomScripts.Count)];
        while(exitRoom == spawnRoom) {exitRoom = _roomScripts[Random.Range(0, _roomScripts.Count)];}
        exitRoom.SpawnExit(exitPrefab, this);
    }

    public void ExitLevel()
    {
        _currentLevel++;
        saveManager.Save();
        SceneManager.LoadScene("Scenes/DungeonGeneration");
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
        
        RoomController script = room.GetComponent<RoomController>();
        Vector2 center = bounds.center*_tileSize - new Vector2((float)_tileSize/2, (float)_tileSize/2);
        room.transform.position = new Vector3(center.x, 0, center.y);
        Vector2Int pos;
        for(int x = 0; x < roomSize.x; x++){
                
            for (int y = 0; y < roomSize.y; y++)
            {
                pos = location + new Vector2Int(x, y);
                if (pos.x == location.x)
                {
                    PlaceTile(pos, 90, walls.transform, doors.transform);
                }
                else if (pos.x == location.x + roomSize.x - 1)
                {
                    PlaceTile(pos, 270, walls.transform, doors.transform);
                }
                else if (pos.y == location.y)
                {
                    PlaceTile(pos, 0, walls.transform, doors.transform);
                } 
                else if (pos.y == location.y + roomSize.y - 1)
                {
                    PlaceTile(pos, 180, walls.transform, doors.transform);
                }
                else
                {
                    PlaceFloorTile(pos, floor.transform);    
                }
            }
        }

        GameObject floorCollider = new GameObject("Floor Collider");
        floorCollider.transform.parent = colliders.transform;
        floorCollider.layer = LayerMask.NameToLayer("Walls");
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

        script.Initialize(bounds, _tileSize, roomGrid);
        return script;
    }

    private void CreateWallColliders(RectInt bounds, Vector2Int roomSize, Transform parent)
    {
        Vector2Int location = bounds.position;
        Vector2Int startPos = location;
        Vector2Int pos;
        for (int x = 1; x < roomSize.x; x++)
        {
            pos = location + new Vector2Int(x, 0);
            if (LevelData.Grid[pos] == Generator2D.CellType.Entrance || LevelData.Grid[pos] == Generator2D.CellType.Corner)
            {
                PlaceWallCollider(startPos*_tileSize, pos.x - startPos.x - 1, 0,1, parent);
                startPos = pos;
            }
        }

        location = new Vector2Int(bounds.x, bounds.yMax-1);
        startPos = location;
        for (int x = 1; x < roomSize.x; x++)
        {
            pos = location + new Vector2Int(x, 0);
            if (LevelData.Grid[pos] == Generator2D.CellType.Entrance || LevelData.Grid[pos] == Generator2D.CellType.Corner)
            {
                PlaceWallCollider(startPos*_tileSize, pos.x - startPos.x - 1, 180,-1, parent);
                startPos = pos;
            }
        }
        
        location = new Vector2Int(bounds.xMax-1, bounds.y);
        startPos = location;
        for (int y = 1; y < roomSize.y; y++)
        {
            pos = location + new Vector2Int(0, y);
            if (LevelData.Grid[pos] == Generator2D.CellType.Entrance || LevelData.Grid[pos] == Generator2D.CellType.Corner)
            {
                PlaceWallCollider(startPos*_tileSize, pos.y - startPos.y - 1, -90,1, parent);
                startPos = pos;
            }
        }
        
        location = new Vector2Int(bounds.xMin, bounds.y);
        startPos = location;
        for (int y = 1; y < roomSize.y; y++)
        {
            pos = location + new Vector2Int(0, y);
            if (LevelData.Grid[pos] == Generator2D.CellType.Entrance || LevelData.Grid[pos] == Generator2D.CellType.Corner)
            {
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

    private bool CheckHallway(Vector2Int direction)
    {
        return LevelData.Grid.InBounds(direction) && (LevelData.Grid[direction] == Generator2D.CellType.None ||
                                                      LevelData.Grid[direction] == Generator2D.CellType.Room ||
                                                      LevelData.Grid[direction] == Generator2D.CellType.Corner);
    }

    private bool IsHallway(Vector2Int direction)
    {
        return LevelData.Grid.InBounds(direction) && (LevelData.Grid[direction] == Generator2D.CellType.Hallway ||
                                                      LevelData.Grid[direction] == Generator2D.CellType.Entrance);
    }

    private void PlaceHallway(List<Vector2Int> path)
    {
        GameObject hallway = new GameObject("Hallway " + path[0]);
        hallway.transform.parent = transform;
        GameObject walls = new GameObject("Walls");
        walls.transform.parent = hallway.transform;
        walls.layer = LayerMask.NameToLayer("Walls");
        
        GameObject floor = new GameObject("Floor Tiles");
        floor.transform.parent = hallway.transform;
        floor.layer = LayerMask.NameToLayer("Floor");
        
        Vector2Int center = path[(path.Count-1) / 2] * _tileSize;
        hallway.transform.position = new Vector3(center.x, 0, center.y) ;
        for (int i = 0; i < path.Count; i++)
        {
            var pos = path[i];
            if (LevelData.Grid[pos] == Generator2D.CellType.Hallway) {
                PlaceFloorTile(pos, floor.transform);
                var left = pos + Vector2Int.left;
                var right = pos + Vector2Int.right;
                var up = pos + Vector2Int.up;
                var down = pos + Vector2Int.down;
                if (CheckHallway(left))
                {
                    PlaceWall(left, 90, walls.transform);
                }
                if (CheckHallway(right))
                {
                    PlaceWall(right, 270, walls.transform);
                }
                if (CheckHallway(down))
                {
                    PlaceWall(down, 0, walls.transform);
                }
                if (CheckHallway(up))
                {
                    PlaceWall(up, 180, walls.transform);
                }


                if (IsHallway(up) && (IsHallway(left) || IsHallway(right)))
                {
                    PlaceCorner(up, 180, walls.transform);
                }
                if (IsHallway(down) && (IsHallway(left) || IsHallway(right)))
                {
                    PlaceCorner(down, 0, walls.transform);
                }
                if (IsHallway(left) && (IsHallway(up) || IsHallway(down)))
                {
                    PlaceCorner(left, 90, walls.transform);
                }
                if (IsHallway(right) && (IsHallway(up) || IsHallway(down)))
                {
                    PlaceCorner(right, 270, walls.transform);
                }
                
            }
        }
    }
    
    private List<int> GetUniqueRandomIndexes(int indexRange, int randomIndexCount)
    {
        System.Random rnd = new System.Random(LevelData.RandomSeed);
        return Enumerable.Range(0, indexRange)
            .OrderBy(i => rnd.Next()).Take(randomIndexCount).ToList();
    }
    
    private void PlaceFloorTile(Vector2Int location, Transform parent = null)
    {
        GameObject tile = Instantiate(floorTilePrefab, new Vector3(location.x * _tileSize, 0, location.y * _tileSize), Quaternion.identity);
        if (parent != null) 
            tile.transform.parent = parent;
    }

    private void PlaceTile(Vector2Int pos, int rotation, Transform wallParent = null, Transform doorParent = null)
    {
        if (LevelData.Grid[pos] == Generator2D.CellType.Corner)
            return;
        if (LevelData.Grid[pos] != Generator2D.CellType.Entrance)
        {
            PlaceWall(pos, rotation, wallParent);
        }
        else if (LevelData.Grid[pos] == Generator2D.CellType.Entrance)
        {
            PlaceDoor(pos, rotation, doorParent);
        }
    }

    private void PlaceWall(Vector2Int location, int rotation, Transform parent = null)
    {
        GameObject wall = Instantiate(wallPrefab, new Vector3(location.x * _tileSize, 0, location.y * _tileSize), Quaternion.Euler(0, rotation, 0));
        if (parent != null) 
            wall.transform.parent = parent;
    }

    private void PlaceCorner(Vector2Int location, int rotation, Transform parent = null)
    {
        GameObject wall = Instantiate(cornerWallPrefab, new Vector3(location.x * _tileSize, 0, location.y * _tileSize), Quaternion.Euler(0, rotation, 0));
        if (parent != null) 
            wall.transform.parent = parent;
    }

    private void PlaceDoor(Vector2Int location, int rotation, Transform parent = null)
    {
        GameObject door = Instantiate(doorPrefab, new Vector3(location.x * _tileSize, 0, location.y * _tileSize), Quaternion.Euler(0, rotation, 0));
        if (parent != null)
            door.transform.parent = parent;
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
