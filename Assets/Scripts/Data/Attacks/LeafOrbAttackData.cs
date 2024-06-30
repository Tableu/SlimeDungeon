using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Leaf Orb Attack", menuName = "Data/Attacks/Leaf Orb Attack")]
public class LeafOrbAttackData : AttackData
{
    public override Attack CreateInstance(ICharacterInfo characterInfo)
    {
        return new BasicAttack<LeafOrb>(characterInfo, this);
    }
}
