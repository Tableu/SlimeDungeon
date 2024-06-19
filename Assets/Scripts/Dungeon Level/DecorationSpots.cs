using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationSpots : MonoBehaviour
{
    [SerializeField] private List<Transform> decorationSpots = new List<Transform>();

    public List<Transform> Locations => decorationSpots;
}
