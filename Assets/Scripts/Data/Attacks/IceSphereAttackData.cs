using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice Sphere Attack", menuName = "Data/Attacks/Ice Sphere Attack")]
public class IceSphereAttackData : AttackData
{
    public override Attack CreateInstance(ICharacterInfo characterInfo)
    {
        return new BasicAttack<IceSphere>(characterInfo, this);
    }
}
