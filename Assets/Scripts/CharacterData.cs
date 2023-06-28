using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Data/Character Data")]
public class CharacterData : ScriptableObject
{
    [HeaderAttribute("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float armor;
    [SerializeField] private float speed;
    [HeaderAttribute("Enemy")]
    [SerializeField] private float bodyDamage;
    [SerializeField] private float aggroRange;
    [SerializeField] private int detectTick;

    public float Health => health;
    public float Armor => armor;
    public float Speed => speed;
    public float BodyDamage => bodyDamage;
    public float AggroRange => aggroRange;
    public int DetectTick => detectTick;
}
