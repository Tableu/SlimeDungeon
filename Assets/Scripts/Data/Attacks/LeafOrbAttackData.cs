using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Leaf Orb Attack", menuName = "Data/Attacks/Leaf Orb Attack")]
public class LeafOrbAttackData : AttackData
{
    public override Attack CreateInstance(CharacterStats characterStats, Transform transform)
    {
        return new BasicAttack<LeafOrb>(characterStats, this, transform);
    }
}
