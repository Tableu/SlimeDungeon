using UnityEngine;

[CreateAssetMenu(fileName = "SetPieceData", menuName = "Data/Rooms/Set Piece Data")]
public class SetPieceData : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2 size;
    
    public GameObject Prefab => prefab;
    public Vector2 Size => size;
}
