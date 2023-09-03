using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Leaf Attack", menuName = "Attacks/Leaf Attack")]
public class LeafAttackData : AttackData
{
    public override Attack EquipAttack(Character character)
    {
        Attack attack = new LeafAttack(character, this);
        return attack;
    }
}
