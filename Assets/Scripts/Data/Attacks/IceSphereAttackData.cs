using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice Sphere Attack", menuName = "Data/Attacks/Ice Sphere Attack")]
public class IceSphereAttackData : AttackData
{
    public override Attack CreateInstance(ICharacterInfo characterInfo, Transform transform)
    {
        return new BasicAttack<IceSphere>(characterInfo, this, transform);
    }
}
