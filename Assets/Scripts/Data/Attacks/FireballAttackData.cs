using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball Attack", menuName = "Attacks/Fireball Attack")]
public class FireballAttackData : AttackData
{
    public override Attack CreateInstance(ICharacterInfo characterInfo)
    {
        return new BasicAttack<Fireball>(characterInfo, this);
    }
}
