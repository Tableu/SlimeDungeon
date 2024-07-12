using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "RoomData", menuName = "Data/Rooms/Room Data")]
public class RoomData : ScriptableObject
{
    [SerializeField] private List<RandomDecoration> randomDecorations;
    [SerializeField] private RandomGameObjectGroups randomEnemyGroups;
    [SerializeField] private RandomCharacterData randomCharacterItems;
    
    public RandomGameObjectGroups RandomEnemyGroups => randomEnemyGroups;
    public List<RandomDecoration> RandomDecorations => randomDecorations;
    public RandomCharacterData RandomCharacterItems => randomCharacterItems;

    public static RandomDecoration GetRandomDecoration(List<RandomDecoration> setPieces)
    {
        int totalWeight = setPieces.Sum((x => x.Weight));
        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;
        
        foreach (RandomDecoration data in setPieces)
        {
            currentWeight += data.Weight;
            if (randomWeight <= currentWeight)
            {
                if (!data.Repeatable)
                    setPieces.Remove(data);
                return data;
            }
        }

        return setPieces.Last();
    }
    
    [Serializable]
    public struct RandomDecoration
    {
        public GameObject Prefab;
        public int Weight;
        public bool Repeatable;
        public bool RequireWall;
    }
}