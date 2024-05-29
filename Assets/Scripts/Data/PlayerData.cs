using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data")]
[Serializable]
public class PlayerData : ScriptableObject
{
    [SerializeField] private int maxFormCount;
    [SerializeField] private int maxSpellCount;
    [SerializeField] private float manaRegen;
    [SerializeField] private float mana;

    public int MaxFormCount => maxFormCount;
    public int MaxSpellCount => maxSpellCount;
    public float ManaRegen => manaRegen;
    public float Mana => mana;
    
}