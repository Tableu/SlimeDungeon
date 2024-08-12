using System.Collections.Generic;
using UnityEngine;

public class BossLevelInfo : MonoBehaviour
{
    [SerializeField] private List<Transform> colliders;
    [SerializeField] private RoomController startRoom;
    [SerializeField] private RoomController exitRoom;
    [SerializeField] private Vector2Int floorSize;

    public RoomController StartRoom => startRoom;
    public RoomController ExitRoom => exitRoom;
    public Vector2Int FloorSize => floorSize;
    public List<Transform> Colliders => colliders;
}
