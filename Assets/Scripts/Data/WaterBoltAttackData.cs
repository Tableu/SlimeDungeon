using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterBolt Attack", menuName = "Attacks/WaterBolt Attack")]
public class WaterBoltAttackData : AttackData
{
    public override Attack EquipAttack(Character character, int index)
    {
        Attack attack = new WaterBoltAttack(character, this);
        character.attacks.Insert(index, attack);
        return attack;
    }
}
