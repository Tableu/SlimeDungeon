using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data")]
public class PlayerData : CharacterData
{
    [SerializeField] private int maxFormCount;
    [SerializeField] private int maxSpellCount;

    public int MaxFormCount => maxFormCount;
    public int MaxSpellCount => maxSpellCount;
}