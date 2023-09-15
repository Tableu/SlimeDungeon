using System.Collections.Generic;
using UnityEngine;

public class CharacterData : ScriptableObject
{
    [HeaderAttribute("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float stunResist;
    [SerializeField] private float speed;
    [SerializeField] private float manaRegen;
    [SerializeField] private float mana;
    [SerializeField] private Elements.Type elementType;

    [HeaderAttribute("References")] 
    [SerializeField] private List<AttackData> attacks;

    public float Health => health;
    public float StunResist => stunResist;
    public float Speed => speed;
    public List<AttackData> Attacks => attacks;
    public float ManaRegen => manaRegen;
    public float Mana => mana;
    public Elements.Type ElementType => elementType;
    
}
