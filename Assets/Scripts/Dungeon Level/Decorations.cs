using System.Collections.Generic;
using UnityEngine;

public class Decorations : MonoBehaviour
{
    [SerializeField] private List<RoomData.DecorationSpot> decorationSpots = new List<RoomData.DecorationSpot>();

    public List<RoomData.DecorationSpot> Locations => decorationSpots;
}
