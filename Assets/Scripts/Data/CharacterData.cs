using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Data/Character Data")]
public class CharacterData : ScriptableObject
{
    [HeaderAttribute("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float armor;
    [SerializeField] private float speed;
    [SerializeField] private float manaRegen;
    [SerializeField] private float mana;
    [SerializeField] private Elements.Type elementType;

    [HeaderAttribute("References")] 
    [SerializeField] private List<AttackData> attacks;

    public float Health => health;
    public float Armor => armor;
    public float Speed => speed;
    public List<AttackData> Attacks => attacks;
    public float ManaRegen => manaRegen;
    public float Mana => mana;
    public Elements.Type ElementType => elementType;
    
}
