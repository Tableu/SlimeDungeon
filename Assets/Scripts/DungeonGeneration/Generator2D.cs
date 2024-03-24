using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;

public class Generator2D : MonoBehaviour {
    private enum CellType {
        None,
        Room,
        Hallway
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
    [SerializeField] private int roomCount;
    [SerializeField] private Vector2Int roomMaxSize;
    [SerializeField] private Vector2Int roomMinSize;
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject wallPrefab;

    private Random _random;
    private Grid2D<CellType> _grid;
    private List<Room> _rooms;
    private Delaunay2D _delaunay;
    private HashSet<Prim.Edge> _selectedEdges;

    private void Start() {
        Generate();
    }

    private void Generate() {
        _random = new Random(0);
        _grid = new Grid2D<CellType>(size, Vector2Int.zero);
        _rooms = new List<Room>();

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
    }

    private void PlaceRooms() {
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
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    _grid[pos] = CellType.Room;
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

            if (path != null) {
                paths.Add(path);
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (_grid[current] == CellType.None) {
                        _grid[current] = CellType.Hallway;
                    }

                    if (i > 0) {
                        var prev = path[i - 1];

                        var delta = current - prev;
                    }
                }
            }
        }

        foreach (var path in paths)
        {
            foreach (var pos in path) {
                if (_grid[pos] == CellType.Hallway) {
                    PlaceHallway(pos);
                    if (_grid.InBounds(new Vector2Int(pos.x - 1, pos.y)) && _grid[pos.x - 1, pos.y] == CellType.None)
                    {
                        PlaceWall(pos, 90);
                    }
                    if (_grid.InBounds(new Vector2Int(pos.x + 1, pos.y)) && _grid[pos.x + 1, pos.y] == CellType.None)
                    {
                        PlaceWall(pos, 270);
                    }
                    if (_grid.InBounds(new Vector2Int(pos.x, pos.y-1)) && _grid[pos.x, pos.y-1] == CellType.None)
                    {
                        PlaceWall(pos, 0);
                    }
                    if (_grid.InBounds(new Vector2Int(pos.x, pos.y+1)) && _grid[pos.x, pos.y+1] == CellType.None)
                    {
                        PlaceWall(pos, 180);
                    }
                }
            }
        }
    }

    private void PlaceRoom(Vector2Int location, Vector2Int size) {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                PlaceFloorTile(new Vector2Int(location.x + x, location.y + y));
            }
        }

        for (int x = 0; x < size.x; x++)
        {
            PlaceWall(new Vector2Int(location.x + x, location.y), 0);
        }

        for (int x = 0; x < size.x; x++)
        {
            PlaceWall(new Vector2Int(location.x + x, location.y + size.y - 1), 180);
        }
        
        for (int y = 0; y < size.y; y++)
        {
            PlaceWall(new Vector2Int(location.x, location.y + y), 90);
        }
        
        for (int y = 0; y < size.y; y++)
        {
            PlaceWall(new Vector2Int(location.x + size.x - 1, location.y + y), 270);
        }
    }

    private void PlaceHallway(Vector2Int location) {
        PlaceFloorTile(location);
    }

    private void PlaceFloorTile(Vector2Int location)
    {
        Instantiate(floorTilePrefab, new Vector3(location.x * 4, 0, location.y * 4), Quaternion.identity);
    }

    private void PlaceWall(Vector2Int location, int rotation)
    {
        Instantiate(wallPrefab, new Vector3(location.x * 4, 0, location.y * 4), Quaternion.Euler(0, rotation, 0));
    }
}
