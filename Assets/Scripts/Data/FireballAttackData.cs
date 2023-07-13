using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball Attack", menuName = "Attacks/Fireball Attack")]
public class FireballAttackData : AttackData
{
    public override Attack EquipAttack(Character character)
    {
        Attack attack = new FireballAttack(character, this);
        character.attacks.Add(attack);
        return attack;
    }
}
