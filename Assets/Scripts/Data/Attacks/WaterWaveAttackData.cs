using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Water Wave Attack", menuName = "Data/Attacks/Water Wave Attack")]
public class WaterWaveAttackData : AttackData
{
    public override Attack CreateInstance(ICharacterInfo characterInfo)
    {
        return new WaterWaveAttack(characterInfo, this);
    }
}
