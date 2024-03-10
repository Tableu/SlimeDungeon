using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Water Wave Attack", menuName = "Attacks/Water Wave Attack")]
public class WaterWaveAttackData : AttackData
{
    public override Attack CreateInstance(Character character)
    {
        return new WaterWaveAttack(character, this);
    }
}
