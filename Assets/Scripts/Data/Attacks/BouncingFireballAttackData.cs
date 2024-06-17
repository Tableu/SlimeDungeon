using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "BouncingFireball Attack", menuName = "Data/Attacks/BouncingFireball Attack")]
public class BouncingFireballAttackData : AttackData
{
    [SerializeField] private int maxBounceCount;
    [SerializeField] private Vector2 launchAngle;
    public int MaxBounceCount => maxBounceCount;
    public Vector2 LaunchAngle => launchAngle;
    public override Attack CreateInstance(ICharacterInfo characterInfo)
    {
        return new BouncingFireballAttack(characterInfo, this);
    }
}
