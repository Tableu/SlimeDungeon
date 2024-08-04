using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossLevelInfo : MonoBehaviour
{
    [SerializeField] private RoomController startRoom;
    [SerializeField] private RoomController exitRoom;
    [SerializeField] private Vector2Int floorSize;

    public RoomController StartRoom => startRoom;
    public RoomController ExitRoom => exitRoom;
    public Vector2Int FloorSize => floorSize;

    public List<Transform> GetColliders()
    {
        return GetComponentsInChildren<Collider>().Select(collider1 => collider1.transform).ToList();
    }
}
