using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "CaptureOrb Attack", menuName = "Attacks/CaptureOrb Attack")]
public class CaptureOrbAttackData : AttackData
{
    public override Attack EquipAttack(Character character)
    {
        Attack attack = new CaptureAttack(character, this);
        return attack;
    }
}
