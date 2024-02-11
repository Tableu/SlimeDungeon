using Controller.Form;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data")]
public class PlayerData : CharacterData
{
    [FormerlySerializedAs("baseForm")] [SerializeField] private FormData startForm;
    [SerializeField] private int maxFormCount;
    [SerializeField] private int maxSpellCount;
    
    public FormData StartForm => startForm;
    public int MaxFormCount => maxFormCount;
    public int MaxSpellCount => maxSpellCount;
}