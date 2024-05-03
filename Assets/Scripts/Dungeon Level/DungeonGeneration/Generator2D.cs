using System;
using System.Collections.Generic;
using System.IO;
using FischlWorks_FogWar;
using UnityEngine;
using Random = System.Random;
using Graphs;

public class Generator2D : MonoBehaviour {
    private enum CellType {
        None,
        Room,
        Hallway,
        Entrance
    }

    private class Room {
        public RectInt bounds;

        public Room(Vector2Int location, Vector2Int size) {
            bounds = new RectInt(location, size);
        }

        public static bool Intersect(Room a, Room b) {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
        }
    }

    [SerializeField] private Vector2Int size;
    [SerializeField] private int tileSize;
    [SerializeField] private int roomCount;
    [SerializeField] private Vector2Int roomMaxSize;
    [SerializeField] private Vector2Int roomMinSize;
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject levelCenter;
    [SerializeField] private GameObject levelParent;
    [SerializeField] private csFogWar fogOfWar;

    private Random _random;
    private Grid2D<CellType> _grid;
    private List<Room> _rooms;
    private Delaunay2D _delaunay;
    private HashSet<Prim.Edge> _selectedEdges;

    public Vector2Int Size => size;
    public int TileSize => tileSize;

    public List<RoomController> Generate() {
        _random = new Random((int)System.DateTime.Now.Ticks);
        _grid = new Grid2D<CellType>(size, Vector2Int.zero);
        _rooms = new List<Room>();

        CreateRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
        List<RoomController> roomScripts = PlaceRooms();
        Vector2Int paddedSize = size + new Vector2Int(tileSize, tileSize);
        levelCenter.transform.position = new Vector3(
            ((float)size.x * tileSize)/2, levelCenter.transform.position.y, ((float)size.y * tileSize)/2);
        fogOfWar.Initialize(paddedSize*2, tileSize/2);
        #if UNITY_EDITOR
        using var sw = new StreamWriter("dungeon_debug.txt");
        for (int y = 0; y < _grid.Size.y; y++)
        {
            for (int x = _grid.Size.x-1; x > -1; x--)
            {
                char s = _grid[new Vector2Int(x, y)].ToString()[0];
                if (s == 'N')
                    s = ' ';
                sw.Write(s);
            }
            sw.Write("\n");
        }
        sw.Flush();
        sw.Close();
        #endif
        return roomScripts;
    }

    private void CreateRooms() {
        for (int i = 0; i < roomCount; i++) {
            Vector2Int location = new Vector2Int(
                _random.Next(0, size.x),
                _random.Next(0, size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                _random.Next(roomMinSize.x, roomMaxSize.x + 1),
                _random.Next(roomMinSize.y, roomMaxSize.y + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in _rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y) {
                add = false;
            }

            if (add) {
                _rooms.Add(newRoom);

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    _grid[pos] = CellType.Room;
                }
            }
        }
    }

    private List<RoomController> PlaceRooms()
    {
        List<RoomController> roomScripts = new List<RoomController>();
        foreach (var room in _rooms)
        {
            roomScripts.Add(PlaceRoom(room.bounds));
        }

        return roomScripts;
    }

    private void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in _rooms) {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        _delaunay = Delaunay2D.Triangulate(vertices);
    }

    private void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in _delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        _selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(_selectedEdges);

        foreach (var edge in remainingEdges) {
            if (_random.NextDouble() < 0.125) {
                _selectedEdges.Add(edge);
            }
        }
    }

    private void PathfindHallways() {
        DungeonPathfinder2D aStar = new DungeonPathfinder2D(size);
        List<List<Vector2Int>> paths = new List<List<Vector2Int>>();
        foreach (var edge in _selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                var pathCost = new DungeonPathfinder2D.PathCost();
                
                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic
                //if (a.direction != b.direction)
                //    pathCost.cost += 1;

                if (_grid[b.Position] == CellType.Room) {
                    pathCost.cost += 10;
                } else if (_grid[b.Position] == CellType.None) {
                    pathCost.cost += 5;
                } else if (_grid[b.Position] == CellType.Hallway) {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null)
            {
                paths.Add(path);
                foreach (var current in path)
                {
                    if (_grid[current] == CellType.None) 
                        _grid[current] = CellType.Hallway;
                }
            }
        }

        foreach (var path in paths)
        {
            GameObject hallway = new GameObject("Hallway " + path[0]);
            hallway.transform.parent = levelParent.transform;
            GameObject walls = new GameObject("Walls");
            walls.transform.parent = hallway.transform;
            walls.layer = LayerMask.NameToLayer("Walls");
            
            GameObject floor = new GameObject("Floor Tiles");
            floor.transform.parent = hallway.transform;
            floor.layer = LayerMask.NameToLayer("Floor");
            
            Vector2Int center = path[(path.Count-1) / 2] * tileSize;
            hallway.transform.position = new Vector3(center.x, 0, center.y) ;
            bool firstHallway = true;
            var prevPos = path[0];
            Vector2Int last = new Vector2Int(-1,-1);
            for (int i = 0; i < path.Count; i++)
            {
                var pos = path[i];
                if (_grid[pos] == CellType.Hallway) {
                    PlaceFloorTile(pos, floor.transform);
                    var left = pos + Vector2Int.left;
                    var right = pos + Vector2Int.right;
                    var up = pos + Vector2Int.up;
                    var down = pos + Vector2Int.down;
                    if (_grid.InBounds(left) && _grid[left] == CellType.None)
                    {
                        PlaceWall(left, 90, walls.transform);
                    }
                    if (_grid.InBounds(right) && _grid[right] == CellType.None)
                    {
                        PlaceWall(right, 270, walls.transform);
                    }
                    if (_grid.InBounds(down) && _grid[down] == CellType.None)
                    {
                        PlaceWall(down, 0, walls.transform);
                    }
                    if (_grid.InBounds(up) && _grid[up] == CellType.None)
                    {
                        PlaceWall(up, 180, walls.transform);
                    }
                    
                    var leftUp = pos + new Vector2Int(-1,1);
                    var rightUp = pos + new Vector2Int(1,1);
                    var leftDown = pos + new Vector2Int(-1,-1);
                    var rightDown = pos + new Vector2Int(1, -1);
                    
                    if (_grid.InBounds(leftUp) && _grid[leftUp] == CellType.None)
                    {
                        PlaceWall(leftUp, 90, walls.transform);
                    }
                    if (_grid.InBounds(rightUp) && _grid[rightUp] == CellType.None)
                    {
                        PlaceWall(rightUp, 270, walls.transform);
                    }
                    if (_grid.InBounds(leftDown) && _grid[leftDown] == CellType.None)
                    {
                        PlaceWall(leftDown, 0, walls.transform);
                    }
                    if (_grid.InBounds(rightDown) && _grid[rightDown] == CellType.None)
                    {
                        PlaceWall(rightDown, 180, walls.transform);
                    }

                    if (firstHallway)
                    {
                        firstHallway = false;
                        _grid[prevPos] = CellType.Entrance;
                    }
                }
                else
                {
                    if (_grid[prevPos] == CellType.Hallway)
                    {
                        last = pos;
                    }
                }
                
                prevPos = pos;
            }
            
            if (last != new Vector2Int(-1, -1))
                _grid[last] = CellType.Entrance;
        }
    }

    private RoomController PlaceRoom(RectInt bounds)
    {
        var location = bounds.position;
        var roomSize = bounds.size;
        
        GameObject room = new GameObject("Room " + location);
        room.transform.parent = levelParent.transform;
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
        Vector2 center = bounds.center*tileSize - new Vector2((float)tileSize/2, (float)tileSize/2);
        room.transform.position = new Vector3(center.x, 0, center.y);
        for (int x = 1; x < roomSize.x-1; x++)
        {
            for (int y = 1; y < roomSize.y-1; y++)
            {
                PlaceFloorTile(new Vector2Int(location.x + x, location.y + y), floor.transform);
            }
        }

        for (int x = 0; x < roomSize.x; x++)
        {
            var l = new Vector2Int(location.x + x, location.y);
            var hallwayTile = l + Vector2Int.down;
            if (_grid[l] != CellType.Entrance || !_grid.InBounds(hallwayTile) || _grid[hallwayTile] != CellType.Hallway)
            {
                PlaceWall(l, 0, walls.transform);
            }

            if (_grid[l] == CellType.Entrance && _grid.InBounds(hallwayTile) && _grid[hallwayTile] == CellType.Hallway)
            {
                PlaceDoor(l, 0, doors.transform);
            }
        }

        for (int x = 0; x < roomSize.x; x++)
        {
            var l = new Vector2Int(location.x + x, location.y + roomSize.y - 1);
            var hallwayTile = l + Vector2Int.up;
            if (_grid[l] != CellType.Entrance || !_grid.InBounds(hallwayTile) || _grid[hallwayTile] != CellType.Hallway)
            {
                PlaceWall(l, 180, walls.transform);
            }
            if (_grid[l] == CellType.Entrance && _grid.InBounds(hallwayTile) && _grid[hallwayTile] == CellType.Hallway)
            {
                PlaceDoor(l, 180, doors.transform);
            }
        }
        
        for (int y = 0; y < roomSize.y; y++)
        {
            var l = new Vector2Int(location.x, location.y + y);
            var hallwayTile = l + Vector2Int.left;
            if (_grid[l] != CellType.Entrance || !_grid.InBounds(hallwayTile) || _grid[hallwayTile] != CellType.Hallway)
            {
                PlaceWall(l, 90, walls.transform);
            }
            if (_grid[l] == CellType.Entrance && _grid.InBounds(hallwayTile) && _grid[hallwayTile] == CellType.Hallway)
            {
                PlaceDoor(l, 90, doors.transform);
            }
        }
        
        for (int y = 0; y < roomSize.y; y++)
        {
            var l = new Vector2Int(location.x + roomSize.x - 1, location.y + y);
            var hallwayTile = l + Vector2Int.right;
            if (_grid[l] != CellType.Entrance || !_grid.InBounds(hallwayTile) || _grid[hallwayTile] != CellType.Hallway)
            {
                PlaceWall(l, 270, walls.transform);
            }
            if (_grid[l] == CellType.Entrance && _grid.InBounds(hallwayTile) && _grid[hallwayTile] == CellType.Hallway)
            {
                PlaceDoor(l, 270, doors.transform);
            }
        }
        
        script.Initialize(center, roomSize*tileSize, tileSize);
        return script;
    }

    private void PlaceFloorTile(Vector2Int location, Transform parent = null)
    {
        GameObject tile = Instantiate(floorTilePrefab, new Vector3(location.x * tileSize, 0, location.y * tileSize), Quaternion.identity);
        if (parent != null) 
            tile.transform.parent = parent;
    }

    private void PlaceWall(Vector2Int location, int rotation, Transform parent = null)
    {
        GameObject wall = Instantiate(wallPrefab, new Vector3(location.x * tileSize, 0, location.y * tileSize), Quaternion.Euler(0, rotation, 0));
        if (parent != null) 
            wall.transform.parent = parent;
    }

    private void PlaceDoor(Vector2Int location, int rotation, Transform parent = null)
    {
        GameObject door = Instantiate(doorPrefab, new Vector3(location.x * tileSize, 0, location.y * tileSize), Quaternion.Euler(0, rotation, 0));
        if (parent != null) 
            door.transform.parent = parent;
    }
}
