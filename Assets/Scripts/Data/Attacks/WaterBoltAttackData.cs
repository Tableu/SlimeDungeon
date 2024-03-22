using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterBolt Attack", menuName = "Attacks/WaterBolt Attack")]
public class WaterBoltAttackData : AttackData
{
    public override Attack CreateInstance(Character character)
    {
        return new WaterBoltAttack(character, this);
    }
}
