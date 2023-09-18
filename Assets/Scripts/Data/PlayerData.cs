using Controller.Form;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data")]
public class PlayerData : CharacterData
{
    [SerializeField] private Vector2 maxVelocity;
    [FormerlySerializedAs("baseForm")] [SerializeField] private FormData startForm;
    [SerializeField] private int maxFormCount;

    public Vector2 MaxVelocity => maxVelocity;
    public FormData StartForm => startForm;
    public int MaxFormCount => maxFormCount;
}