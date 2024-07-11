using System.Collections.Generic;
using UnityEngine;

public class Decorations : MonoBehaviour
{
    [SerializeField] private List<RoomDecorationData.DecorationSpot> decorationSpots = new List<RoomDecorationData.DecorationSpot>();

    public List<RoomDecorationData.DecorationSpot> Locations => decorationSpots;
}
