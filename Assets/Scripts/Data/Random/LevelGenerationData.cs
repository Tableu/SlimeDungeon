using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelGenerationData", menuName = "Data/Random Lists/Level Generation Data")]
public class LevelGenerationData : RandomizedList<LevelGenerationData.Room>
{
    [Serializable]
    public struct Room
    {
        public RoomLayoutData Layout;
        public RandomGameObjectGroups EnemyGroups;
        public RandomCharacterData Characters;
    }

    [SerializeField] private Vector2Int roomMaxSize;
    [SerializeField] private Vector2Int roomMinSize;
    [SerializeField] private Vector2Int size;

    public Vector2Int RoomMaxSize => roomMaxSize;
    public Vector2Int RoomMinSize => roomMinSize;
    public Vector2Int Size => size;
}
