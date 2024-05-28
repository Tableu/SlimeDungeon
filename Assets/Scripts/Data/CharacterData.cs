using UnityEngine;

public class CharacterData : ScriptableObject
{
    [HeaderAttribute("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float manaRegen;
    [SerializeField] private float mana;
    [SerializeField] private Elements.Type elementType;

    public float Health => health;
    public float Speed => speed;
    public float ManaRegen => manaRegen;
    public float Mana => mana;
    public Elements.Type ElementType => elementType;
    
}
