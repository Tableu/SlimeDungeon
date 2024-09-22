using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Water Wave Attack", menuName = "Data/Attacks/Water Wave Attack")]
public class WaterWaveAttackData : AttackData
{
    public override Attack CreateInstance(CharacterStats characterStats, Transform transform)
    {
        return new WaterWaveAttack(characterStats, this, transform);
    }
}
