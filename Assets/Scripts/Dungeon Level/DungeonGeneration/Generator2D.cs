using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using Graphs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Generator2D : MonoBehaviour {
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CellType {
        [EnumMember(Value="None")]
        None,
        [EnumMember(Value="Room")]
        Room,
        [EnumMember(Value="Hallway")]
        Hallway,
        [EnumMember(Value="Entrance")]
        Entrance,
        [EnumMember(Value="Enemy")]
        Enemy,
        [EnumMember(Value="Chest")]
        Chest,
        [EnumMember(Value="Character")]
        Character,
        [EnumMember(Value="Corner")]
        Corner,
        [EnumMember(Value="Exit")]
        Exit
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

    [Serializable]
    public struct LevelData
    {
        public Grid2D<CellType> Grid;
        public List<List<Vector2Int>> Hallways;
        public List<RectInt> Rooms;
        public int RandomSeed;

        public LevelData(Grid2D<CellType> grid, List<List<Vector2Int>> hallways, List<RectInt> rooms, int randomSeed)
        {
            Grid = grid;
            Hallways = hallways;
            Rooms = rooms;
            RandomSeed = randomSeed;
        }
    }

    [SerializeField] private Vector2Int size;
    [SerializeField] private int tileSize;
    [SerializeField] private int roomCount;
    [SerializeField] private Vector2Int roomMaxSize;
    [SerializeField] private Vector2Int roomMinSize;

    private System.Random _sysRandom;
    private Grid2D<CellType> _grid;
    private List<Room> _rooms;
    private Delaunay2D _delaunay;
    private HashSet<Prim.Edge> _selectedEdges;

    public Vector2Int Size => size;
    public int TileSize => tileSize;

    public LevelData Generate(int seed)
    {
        _sysRandom = new System.Random(seed);
        _grid = new Grid2D<CellType>(size, Vector2Int.zero);
        _rooms = new List<Room>();

        CreateRooms();
        Triangulate();
        CreateHallways();
        List<List<Vector2Int>> paths = PathfindHallways();
#if UNITY_EDITOR
        using var sw = new StreamWriter("dungeon_debug.txt");
        for (int y = _grid.Size.y-1; y > -1; y--)
        {
            for (int x = 0; x < _grid.Size.x; x++)
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

        return new LevelData(_grid, paths, _rooms.Select(r => r.bounds).ToList(), seed);
    }

    private void CreateRooms() {
        for (int i = 0; i < roomCount; i++) {
            Vector2Int location = new Vector2Int(
                _sysRandom.Next(0, size.x),
                _sysRandom.Next(0, size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                _sysRandom.Next(roomMinSize.x, roomMaxSize.x + 1),
                _sysRandom.Next(roomMinSize.y, roomMaxSize.y + 1)
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
                    if ((pos.x == newRoom.bounds.xMin || pos.x == newRoom.bounds.xMax-1) &&
                        (pos.y == newRoom.bounds.yMin || pos.y == newRoom.bounds.yMax-1))
                    {
                        _grid[pos] = CellType.Corner;
                    }
                    else
                    {
                        _grid[pos] = CellType.Room;
                    }
                }
            }
        }
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
            if (_sysRandom.NextDouble() < 0.125) {
                _selectedEdges.Add(edge);
            }
        }
    }

    private List<List<Vector2Int>> PathfindHallways() {
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


                if (_grid[b.Position] == CellType.Corner)
                {
                    pathCost.cost += 100;
                }

                pathCost.cost += Math.Abs(startPos.x - endPos.x);
                pathCost.cost += Math.Abs(startPos.y - endPos.y);

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

        //Add entrances
        foreach (var path in paths)
        {
            bool firstHallway = true;
            var prevPos = path[0];
            Vector2Int last = new Vector2Int(-1,-1);
            for (int i = 0; i < path.Count; i++)
            {
                var pos = path[i];
                if (_grid[pos] == CellType.Hallway) {
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

        return paths;
    }
}
