using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGenerationData", menuName = "Data/Dungeon Generation Data")]
public class DungeonGenerationData : ScriptableObject
{
    [SerializeField] private List<LevelGenerationData> floors;
    [SerializeField] private int tileSize;
    public List<LevelGenerationData> Floors => floors;
    public int TileSize => tileSize;
}
