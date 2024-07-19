using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevelData", menuName = "Data/Dungeon Level Data")]
public class DungeonLevelData : ScriptableObject
{
    [SerializeField] private List<RandomRoomData> floors;
    public List<RandomRoomData> Floors => floors;
}
