using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Flamethrower Attack", menuName = "Attacks/Flamethrower Attack")]
public class FlamethrowerAttackData : AttackData
{
    public override Attack EquipAttack(Character character, int index)
    {
        Attack attack = new FlamethrowerAttack(character, this);
        character.attacks.Insert(index, attack);
        return attack;
    }
}
