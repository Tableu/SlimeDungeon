using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "RoomTypeData", menuName = "Data/Rooms/Room Type Data")]
public class RoomTypeData : ScriptableObject
{
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private int setPieceCount;
    [SerializeField] private List<RandomSetPiece> randomSetPieces;

    public float MaxSize => maxSize;
    public float MinSize => minSize;

    public void DecorateRoom(Transform center, RectInt bounds, float tileSize)
    {
        List<RandomSetPiece> setPiecesCopy = new List<RandomSetPiece>(randomSetPieces);
        int i = 0;
        int x = 0;
        while (i < setPieceCount && x < setPieceCount*10)
        {
            SetPieceData data = GetRandomSetPiece(setPiecesCopy);
            int xExtent = (int) Mathf.Ceil(((float) bounds.width / 2 - 1)* tileSize - tileSize/2);
            int yExtent = (int) Mathf.Ceil(((float) bounds.height / 2 - 1)* tileSize - tileSize/2);
            Vector3 randomPos = new Vector3(Random.Range(-xExtent, xExtent),0,
                Random.Range(-yExtent, yExtent));
            bool tileTaken = Physics.CheckBox(center.transform.position + randomPos, new Vector3(data.Size.x/2, 1, data.Size.y/2), 
                Quaternion.identity, LayerMask.GetMask("Walls", "Obstacles"));

            if (!tileTaken)
            {
                GameObject setPiece = Instantiate(data.Prefab, center);
                setPiece.transform.localPosition = randomPos;
                i++;
            }
            x++;
        }
    }

    private SetPieceData GetRandomSetPiece(List<RandomSetPiece> setPieces)
    {
        int totalWeight = setPieces.Sum((x => x.Weight));
        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;
        
        foreach (RandomSetPiece data in setPieces)
        {
            currentWeight += data.Weight;
            if (randomWeight <= currentWeight)
            {
                if (!data.Repeatable)
                    setPieces.Remove(data);
                return data.Data;
            }
        }

        return setPieces.Last().Data;
    }
    
    [Serializable]
    private struct RandomSetPiece
    {
        public SetPieceData Data;
        public int Weight;
        public bool Repeatable;
    }
}