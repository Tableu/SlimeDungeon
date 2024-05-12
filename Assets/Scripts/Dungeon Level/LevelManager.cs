using System;
using System.Collections.Generic;
using System.Linq;
using Controller.Form;
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
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject levelCenter;
    [SerializeField] private csFogWar fogOfWar;
    
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
            for (int x = 0; x < 10; x++)
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
        NavMeshBuilder.CollectSources(bounds, 
            LayerMask.GetMask("Floor", "Walls"),
            NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), sources);
        NavMeshData data = NavMeshBuilder.BuildNavMeshData(new NavMeshBuildSettings(), sources, bounds, Vector3.zero,
            Quaternion.identity);
        NavMesh.AddNavMeshData(data);
        
        List<FormData> capturedCharacters = randomCapturedCharacters.GetRandomGroup();
        List<GameObject> treasureChests = randomTreasureChests.GetRandomGroup();
        
        //Generate random indexes for placing the random characters
        List<int> capturedCharacterIndexes = GetUniqueRandomIndexes(_roomScripts.Count, capturedCharacters.Count);
        List<int> treasureChestIndexes = GetUniqueRandomIndexes(_roomScripts.Count, treasureChests.Count);
        int i = 0;
        using List<FormData>.Enumerator characterEnumerator = capturedCharacters.GetEnumerator();
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
        
        GameObject room = new GameObject("Room " + location);
        room.transform.parent = transform;
        GameObject walls = new GameObject("Walls");
        walls.transform.parent = room.transform;
        walls.layer = LayerMask.NameToLayer("Walls");
        
        GameObject floor = new GameObject("Floor Tiles");
        floor.transform.parent = room.transform;
        floor.layer = LayerMask.NameToLayer("Floor");
        
        GameObject doors = new GameObject("Doors");
        doors.transform.parent = room.transform;
        doors.layer = LayerMask.NameToLayer("Walls");
        
        RoomController script = room.AddComponent<RoomController>();
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
                if (LevelData.Grid.InBounds(left) && LevelData.Grid[left] == Generator2D.CellType.None)
                {
                    PlaceWall(left, 90, walls.transform);
                }
                if (LevelData.Grid.InBounds(right) && LevelData.Grid[right] == Generator2D.CellType.None)
                {
                    PlaceWall(right, 270, walls.transform);
                }
                if (LevelData.Grid.InBounds(down) && LevelData.Grid[down] == Generator2D.CellType.None)
                {
                    PlaceWall(down, 0, walls.transform);
                }
                if (LevelData.Grid.InBounds(up) && LevelData.Grid[up] == Generator2D.CellType.None)
                {
                    PlaceWall(up, 180, walls.transform);
                }
                
                var leftUp = pos + new Vector2Int(-1,1);
                var rightUp = pos + new Vector2Int(1,1);
                var leftDown = pos + new Vector2Int(-1,-1);
                var rightDown = pos + new Vector2Int(1, -1);
                
                if (LevelData.Grid.InBounds(leftUp) && LevelData.Grid[leftUp] == Generator2D.CellType.None)
                {
                    PlaceWall(leftUp, 90, walls.transform);
                }
                if (LevelData.Grid.InBounds(rightUp) && LevelData.Grid[rightUp] == Generator2D.CellType.None)
                {
                    PlaceWall(rightUp, 270, walls.transform);
                }
                if (LevelData.Grid.InBounds(leftDown) && LevelData.Grid[leftDown] == Generator2D.CellType.None)
                {
                    PlaceWall(leftDown, 0, walls.transform);
                }
                if (LevelData.Grid.InBounds(rightDown) && LevelData.Grid[rightDown] == Generator2D.CellType.None)
                {
                    PlaceWall(rightDown, 180, walls.transform);
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
