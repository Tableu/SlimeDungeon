using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball Attack", menuName = "Attacks/Fireball Attack")]
public class FireballAttackData : AttackData
{
    public override Attack CreateInstance(Character character)
    {
        return new BasicAttack<Fireball>(character, this);
    }
}
