using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice Sphere Attack", menuName = "Attacks/Ice Sphere Attack")]
public class IceSphereAttackData : AttackData
{
    public override Attack CreateInstance(Character character)
    {
        return new BasicAttack<IceSphere>(character, this);
    }
}
