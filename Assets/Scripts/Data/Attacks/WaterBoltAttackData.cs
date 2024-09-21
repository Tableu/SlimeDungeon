using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterBolt Attack", menuName = "Data/Attacks/WaterBolt Attack")]
public class WaterBoltAttackData : AttackData
{
    public override Attack CreateInstance(ICharacterInfo characterInfo, Transform transform)
    {
        return new WaterBoltAttack(characterInfo, this, transform);
    }
}
