using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelPlacer : MonoBehaviour
{
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject doorHallwayPrefab;
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject wallColliderPrefab;
    private List<Transform> _roomColliders = new List<Transform>();
    private int _tileSize;
    private LevelGenerationData _generationData;
    private Generator2D.LevelData _levelData;
    private List<RoomController> _roomScripts = new List<RoomController>();

    public struct Results
    {
        public List<Transform> Colliders;
        public RoomController StartRoom;
        public RoomController ExitRoom;
        public Vector2Int FloorSize;
    }

    public Results Run(int tileSize, Generator2D.LevelData levelData, LevelGenerationData generationData)
    {
        _tileSize = tileSize;
        _levelData = levelData;
        _generationData = generationData;
        
        foreach (RectInt room in _levelData.Rooms)
        {
            _roomScripts.Add(PlaceRoom(room));
        }

        Results results = new Results
        {
            Colliders = _roomColliders,
            StartRoom = _roomScripts[0],
            ExitRoom = _roomScripts[_levelData.EndRoomIndex],
            FloorSize = generationData.Size
        };

        return results;
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
                roomGrid[x, y] = _levelData.Grid[location.x+x, location.y+y];
            }
        }

        List<Door> doorScripts = doors.GetComponentsInChildren<Door>().ToList();

        script.Initialize(bounds, _tileSize, doorScripts, _generationData, colliders.transform);
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
            if (_levelData.Grid[pos] == Generator2D.CellType.Entrance || IsCorner(pos, bounds))
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
            if (_levelData.Grid[pos] == Generator2D.CellType.Entrance || IsCorner(pos, bounds))
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
            if (_levelData.Grid[pos] == Generator2D.CellType.Entrance || IsCorner(pos, bounds))
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
            if (_levelData.Grid[pos] == Generator2D.CellType.Entrance || IsCorner(pos, bounds))
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
        if (_levelData.Grid[pos] != Generator2D.CellType.Entrance)
        {
            PlaceTile(wallPrefab, pos, rotation, wallParent);
        }
        else if (_levelData.Grid[pos] == Generator2D.CellType.Entrance)
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
}
