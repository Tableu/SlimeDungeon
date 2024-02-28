using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Leaf Attack", menuName = "Attacks/Leaf Attack")]
public class LeafAttackData : AttackData
{
    public override Attack CreateInstance(Character character)
    {
        Attack attack = new LeafAttack(character, this);
        return attack;
    }
}
