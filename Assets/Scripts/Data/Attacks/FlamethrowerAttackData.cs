using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Flamethrower Attack", menuName = "Attacks/Flamethrower Attack")]
public class FlamethrowerAttackData : AttackData
{
    [SerializeField] private float initialManaCost;
    public float InitialManaCost => initialManaCost;
    public override Attack CreateInstance(Character character)
    {
        Attack attack = new FlamethrowerAttack(character, this);
        return attack;
    }
}
