using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball Attack", menuName = "Attacks/Fireball Attack")]
public class FireballAttackData : AttackData
{
    public override Attack EquipAttack(Character character, int index)
    {
        Attack attack = new FireballAttack(character, this);
        character.attacks.Insert(index, attack);
        return attack;
    }
}
