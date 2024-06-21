using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "RoomTypeData", menuName = "Data/Rooms/Room Type Data")]
public class RoomTypeData : ScriptableObject
{
    [SerializeField] private List<RandomDecoration> randomDecorations;
    [SerializeField] private RandomGameObjectGroups randomEnemyGroups;
    
    public RandomGameObjectGroups RandomEnemyGroups => randomEnemyGroups;

    public void DecorateRoom(List<Transform> decorationSpots)
    {
        List<RandomDecoration> decorations = new List<RandomDecoration>(randomDecorations);
        int i = 0;
        int x = 0;
        while (i < decorationSpots.Count && x < decorationSpots.Count*10)
        {
            GameObject decoration = GetRandomDecoration(decorations);
            BoxCollider collider = decoration.GetComponent<BoxCollider>();
            bool tileTaken = false;
            if (collider != null)
            {
                tileTaken = Physics.CheckBox(decorationSpots[i].position, collider.size/2, 
                    decorationSpots[i].rotation, LayerMask.GetMask("Walls", "Obstacles"));
            }

            if (!tileTaken)
            {
                Instantiate(decoration, decorationSpots[i]);
                i++;
            }
            x++;
        }
    }

    private GameObject GetRandomDecoration(List<RandomDecoration> setPieces)
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
                return data.Prefab;
            }
        }

        return setPieces.Last().Prefab;
    }
    
    [Serializable]
    private struct RandomDecoration
    {
        public GameObject Prefab;
        public int Weight;
        public bool Repeatable;
    }
}