using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Leaf Attack", menuName = "Attacks/Leaf Attack")]
public class LeafAttackData : AttackData
{
    public override Attack CreateInstance(ICharacterInfo characterInfo)
    {
        return new LeafAttack(characterInfo, this);
    }
}
