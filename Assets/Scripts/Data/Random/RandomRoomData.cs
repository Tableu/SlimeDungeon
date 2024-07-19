using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomRoomData", menuName = "Data/Random Lists/Random Room Data")]
public class RandomRoomData : RandomizedList<RandomRoomData.Room>
{
    [Serializable]
    public struct Room
    {
        public RoomLayoutData Layout;
        public RandomGameObjectGroups EnemyGroups;
        public RandomCharacterData Characters;
    }
}
