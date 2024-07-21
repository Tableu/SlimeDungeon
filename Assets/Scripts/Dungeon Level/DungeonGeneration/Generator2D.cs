using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Generator2D {
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CellType {
        [EnumMember(Value="None")]
        None,
        [EnumMember(Value="Room")]
        Room,
        [EnumMember(Value="Entrance")]
        Entrance,
        [EnumMember(Value="Corner")]
        Corner,
        [EnumMember(Value="Wall")]
        Wall
    }

    private class Room {
        public RectInt bounds;

        public Room(Vector2Int location, Vector2Int size) {
            bounds = new RectInt(location, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return (a.bounds.xMin < (b.bounds.xMax-1)) && ((a.bounds.xMax-1) > b.bounds.xMin) &&
                   (a.bounds.yMin < (b.bounds.yMax-1)) && ((a.bounds.yMax-1) > b.bounds.yMin);
        }
    }

    [Serializable]
    public struct LevelData
    {
        public Grid2D<CellType> Grid;
        public List<RectInt> Rooms;
        public int RandomSeed;
        public int EndRoomIndex;

        public LevelData(Grid2D<CellType> grid, List<RectInt> rooms, int randomSeed, int endRoomIndex)
        {
            Grid = grid;
            Rooms = rooms;
            RandomSeed = randomSeed;
            EndRoomIndex = endRoomIndex;
        }
    }

    private Vector2Int _roomMaxSize;
    private Vector2Int _roomMinSize;
    private Vector2Int _size;
    private System.Random _sysRandom;
    private Grid2D<CellType> _grid;
    private List<Room> _rooms;
    private int _endRoomIndex = 0;

    public LevelData Generate(int seed, LevelGenerationData levelGenerationData, int index)
    {
        _roomMaxSize = levelGenerationData.RoomMaxSize;
        _roomMinSize = levelGenerationData.RoomMinSize;
        _size = levelGenerationData.Size;
        _sysRandom = new System.Random(seed);
        _grid = new Grid2D<CellType>(_size, Vector2Int.zero);
        _rooms = new List<Room>();
        
        MakeRooms();
#if UNITY_EDITOR
        using var sw = new StreamWriter("Logs/dungeon_debug "+index+".txt");
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

        return new LevelData(_grid, _rooms.Select(r => r.bounds).ToList(), seed, _endRoomIndex);
    }

    private Vector2Int GetRandomWall(Room room)
    {
        List<Vector2Int> walls = new List<Vector2Int>();
        foreach (var pos in room.bounds.allPositionsWithin)
        {
            if(_grid[pos] == CellType.Wall)
                walls.Add(pos);
        }

        return walls[_sysRandom.Next(walls.Count)];
    }

    private Vector2Int GetWallDirection(Vector2Int wall, Room room)
    {
        if (wall.x == room.bounds.xMin)
        {
            return Vector2Int.left;
        }
        if (wall.x == room.bounds.xMax-1)
        {
            return Vector2Int.right;   
        }
        if (wall.y == room.bounds.yMin)
        {
            return Vector2Int.down;
        }
        if (wall.y == room.bounds.yMax-1)
        {
            return Vector2Int.up;
        }
        return Vector2Int.zero;
    }

    private Vector2Int GetRoomLocation(Vector2Int wall, Vector2Int roomSize, Vector2Int direction)
    {
        if (direction == Vector2Int.left)
        {
            return new Vector2Int(1+wall.x+(roomSize.x*direction.x), wall.y-1);
        }
        if (direction == Vector2Int.right)
        {
            return new Vector2Int(wall.x, wall.y-1);
        }
        if (direction == Vector2Int.down)
        {
            return new Vector2Int(wall.x-1, 1+wall.y+(roomSize.y*direction.y));
        }
        if (direction == Vector2Int.up)
        {
            return new Vector2Int(wall.x-1, wall.y);
        }
        return Vector2Int.zero;
    }

    private bool RoomIntersects(Room newRoom)
    {
        foreach (var room in _rooms) {
            if (Room.Intersect(room, newRoom))
                return true;
        }
        return false;
    }
    
    private void MakeRooms()
    {
        Room room;
        while (true)
        {
            Vector2Int location = new Vector2Int(
                _sysRandom.Next(0, _size.x),
                _sysRandom.Next(0, _size.y)
            );

            Vector2Int roomSize = _roomMinSize;
            room = new Room(location, roomSize);
            if (room.bounds.xMin < 0 || room.bounds.xMax > _size.x || 
                room.bounds.yMin < 0 || room.bounds.yMax > _size.y)
                continue;
            break;
        }

        AddRoom(room);
        int doors = 0;
        int i = 0;
        while (i < 10 && doors < 3)
        {
            bool doorCreated = CreateRoom(room);
            if (doorCreated)
            {
                doors++;
                if (doors == 1)
                    _endRoomIndex = _rooms.Count - 1;
            }

            i++;
        }

        if (_endRoomIndex == 0)
            _endRoomIndex = _rooms.Count - 1;
    }

    private bool CreateRoom(Room startingRoom)
    {
        Vector2Int wall = GetRandomWall(startingRoom);
        Vector2Int direction = GetWallDirection(wall, startingRoom);
        Vector2Int roomSize = new Vector2Int(_roomMinSize.x, _roomMinSize.y);
        Vector2Int location = GetRoomLocation(wall, roomSize, direction);
        Room newRoom = new Room(location, roomSize);
        
        if (RoomIntersects(newRoom) || 
            (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax > _size.x || 
             newRoom.bounds.yMin < 0 || newRoom.bounds.yMax > _size.y))
        {
            if (_grid.InBounds(wall + direction) && _grid[wall + direction] == CellType.Room)
            {
                Vector2Int pos;
                if (direction.x == 0)
                {
                    for (int j = startingRoom.bounds.xMin; j < startingRoom.bounds.xMax; j++)
                    {
                        pos = new Vector2Int(j, wall.y);
                        if (_grid.InBounds(pos) && _grid[pos] == CellType.Entrance)
                            return false;
                    }
                }
                else
                {
                    for (int j = startingRoom.bounds.yMin; j < startingRoom.bounds.yMax; j++)
                    {
                        pos = new Vector2Int(wall.x, j);
                        if (_grid.InBounds(pos) && _grid[pos] == CellType.Entrance)
                            return false;
                    }
                }
                _grid[wall] = CellType.Entrance;
                return true;
            }
            return false;
        }
        
        Vector2Int targetSize = new Vector2Int(
            _sysRandom.Next(_roomMinSize.x, _roomMaxSize.x),
            _sysRandom.Next(_roomMinSize.y, _roomMaxSize.y)
        );

        while (roomSize.y < targetSize.y)
        {
            roomSize += Vector2Int.up;
            location += Vector2Int.down;
            newRoom = new Room(location, roomSize);
            if (RoomIntersects(newRoom))
            {
                roomSize -= Vector2Int.up;
                location -= Vector2Int.down;
                newRoom = new Room(location, roomSize);
                break;
            }
        }
        while (roomSize.y < targetSize.y)
        {
            roomSize += Vector2Int.up;
            newRoom = new Room(location, roomSize);
            if (RoomIntersects(newRoom))
            {
                roomSize -= Vector2Int.up;
                newRoom = new Room(location, roomSize);
                break;
            }
        }
        while (roomSize.x < targetSize.x)
        {
            roomSize += Vector2Int.right;
            newRoom = new Room(location, roomSize);
            if (RoomIntersects(newRoom))
            {
                roomSize -= Vector2Int.right;
                newRoom = new Room(location, roomSize);
                break;
            }
        }
        while (roomSize.x < targetSize.x)
        {
            roomSize += Vector2Int.right;
            location += Vector2Int.left;
            newRoom = new Room(location, roomSize);
            if (RoomIntersects(newRoom))
            {
                roomSize -= Vector2Int.right;
                location -= Vector2Int.left;
                newRoom = new Room(location, roomSize);
                break;
            }
        }
        
        if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax > _size.x || 
            newRoom.bounds.yMin < 0 || newRoom.bounds.yMax > _size.y)
            return false;
        
        AddRoom(newRoom);
        _grid[wall] = CellType.Entrance;
        int doors = 1;
        int i = 0;
        while (i < 5 && doors < 3)
        {
            bool doorCreated = CreateRoom(newRoom);
            if (doorCreated)
                doors++;
            i++;
        }

        return true;
    }

    private void AddRoom(Room newRoom)
    {
        _rooms.Add(newRoom);

        foreach (var pos in newRoom.bounds.allPositionsWithin) {
            if(_grid[pos] == CellType.Entrance || _grid[pos] == CellType.Corner)
                continue;
            if ((pos.x == newRoom.bounds.xMin || pos.x == newRoom.bounds.xMax-1) &&
                (pos.y == newRoom.bounds.yMin || pos.y == newRoom.bounds.yMax-1))
            {
                _grid[pos] = CellType.Corner;
            }
            else if (pos.x == newRoom.bounds.xMin || pos.x == newRoom.bounds.xMax-1 ||
                     pos.y == newRoom.bounds.yMin || pos.y == newRoom.bounds.yMax-1)
            {
                _grid[pos] = CellType.Wall;
            }
            else
            {
                _grid[pos] = CellType.Room;
            }
        }
    }
}
