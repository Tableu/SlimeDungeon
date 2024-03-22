using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "CaptureOrb Attack", menuName = "Attacks/CaptureOrb Attack")]
public class CaptureOrbAttackData : AttackData
{
    public override Attack CreateInstance(Character character)
    {
        return new BasicAttack<CaptureOrb>(character, this);
    }
}
