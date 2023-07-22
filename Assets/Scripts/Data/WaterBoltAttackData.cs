using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterBolt Attack", menuName = "Attacks/WaterBolt Attack")]
public class WaterBoltAttackData : AttackData
{
    public override Attack EquipAttack(Character character)
    {
        Attack attack = new WaterBoltAttack(character, this);
        character.attacks.Add(attack);
        return attack;
    }
}
