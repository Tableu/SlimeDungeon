using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball Attack", menuName = "Data/Attacks/Fireball Attack")]
public class FireballAttackData : AttackData
{
    public override Attack CreateInstance(ICharacterInfo characterInfo, Transform transform)
    {
        return new BasicAttack<Fireball>(characterInfo, this, transform);
    }
}
