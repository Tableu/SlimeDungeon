using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGenerationData", menuName = "Data/Dungeon Generation Data")]
public class DungeonGenerationData : ScriptableObject
{
    [SerializeField] private List<LevelGenerationData> floors;
    [SerializeField] private List<BossLevel> bossLevels;
    [SerializeField] private int tileSize;
    public List<LevelGenerationData> Floors => floors;
    public List<BossLevel> BossLevels => bossLevels;
    public int TileSize => tileSize;
}

[Serializable]
public struct BossLevel
{
    public int Index;
    public Vector2Int FloorSize;
    public GameObject Prefab;
}
