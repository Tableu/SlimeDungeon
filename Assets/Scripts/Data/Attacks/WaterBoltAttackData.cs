using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterBolt Attack", menuName = "Data/Attacks/WaterBolt Attack")]
public class WaterBoltAttackData : AttackData
{
    public override Attack CreateInstance(CharacterStats characterStats, Transform transform)
    {
        return new WaterBoltAttack(characterStats, this, transform);
    }
}
