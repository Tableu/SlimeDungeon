using System.Collections.Generic;
using UnityEngine;

public class Decorations : MonoBehaviour
{
    [SerializeField] private List<RoomLayoutData.DecorationSpot> decorationSpots = new List<RoomLayoutData.DecorationSpot>();

    public List<RoomLayoutData.DecorationSpot> Locations => decorationSpots;
}
