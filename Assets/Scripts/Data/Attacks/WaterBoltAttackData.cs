using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterBolt Attack", menuName = "Data/Attacks/WaterBolt Attack")]
public class WaterBoltAttackData : AttackData
{
    public override Attack CreateInstance(ICharacterInfo characterInfo)
    {
        return new WaterBoltAttack(characterInfo, this);
    }
}
