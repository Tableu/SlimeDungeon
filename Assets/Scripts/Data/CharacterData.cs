using System.Collections.Generic;
using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Data/Character Data")]
//todo split into enemy and player child classes
public class CharacterData : ScriptableObject
{
    [HeaderAttribute("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float armor;
    [SerializeField] private float speed;
    [SerializeField] private float manaRegen;
    [SerializeField] private float mana;
    [SerializeField] private Elements.Type elementType;

    [HeaderAttribute("Player")] 
    [SerializeField] private bool isPlayer;
    [HeaderAttribute("Enemy")]
    [SerializeField] private float aggroRange;
    [SerializeField] private float deAggroRange;
    [SerializeField] private int detectTick;

    [HeaderAttribute("References")] 
    [SerializeField] private List<AttackData> attacks;

    public float Health => health;
    public float Armor => armor;
    public float Speed => speed;
    public float AggroRange => aggroRange;
    public float DeAggroRange => deAggroRange;
    public int DetectTick => detectTick;
    public bool IsPlayer => isPlayer;
    public List<AttackData> Attacks => attacks;
    public float ManaRegen => manaRegen;
    public float Mana => mana;
    public Elements.Type ElementType => elementType;
}
