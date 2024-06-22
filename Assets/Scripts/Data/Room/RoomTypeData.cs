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

    public void DecorateRoom(List<RoomLayoutData.DecorationSpot> decorationSpots)
    {
        List<RandomDecoration> decorations = new List<RandomDecoration>(randomDecorations);
        int i = 0;
        int x = 0;
        while (i < decorationSpots.Count && x < decorationSpots.Count*10)
        {
            RandomDecoration decoration = GetRandomDecoration(decorations);
            BoxCollider collider = decoration.Prefab.GetComponent<BoxCollider>();
            bool tileTaken = false;
            if (collider != null)
            {
                tileTaken = Physics.CheckBox(decorationSpots[i].Location.position, collider.size/2, 
                    decorationSpots[i].Location.rotation, LayerMask.GetMask("Walls", "Obstacles"));
            }

            if (!tileTaken && (!decoration.RequireWall || decorationSpots[i].NearWall))
            {
                Instantiate(decoration.Prefab, decorationSpots[i].Location);
                i++;
            }
            x++;
        }
    }

    private RandomDecoration GetRandomDecoration(List<RandomDecoration> setPieces)
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
    private struct RandomDecoration
    {
        public GameObject Prefab;
        public int Weight;
        public bool Repeatable;
        public bool RequireWall;
    }
}