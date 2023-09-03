using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Flamethrower Attack", menuName = "Attacks/Flamethrower Attack")]
public class FlamethrowerAttackData : AttackData
{
    public override Attack EquipAttack(Character character)
    {
        Attack attack = new FlamethrowerAttack(character, this);
        return attack;
    }
}
