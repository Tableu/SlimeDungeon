using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterBolt Attack", menuName = "Attacks/WaterBolt Attack")]
public class WaterBoltAttackData : AttackData
{
    public override Attack CreateInstance(Character character)
    {
        Attack attack = new WaterBoltAttack(character, this);
        return attack;
    }
}
