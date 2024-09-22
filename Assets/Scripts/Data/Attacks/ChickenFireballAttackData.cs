using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "ChickenFireball Attack", menuName = "Data/Attacks/ChickenFireball Attack")]
public class ChickenFireballAttackData : AttackData
{
    [SerializeField] private Vector2 launchAngle;
    [SerializeField] private int fireballCount;
    public Vector2 LaunchAngle => launchAngle;
    public int FireballCount => fireballCount;
    public override Attack CreateInstance(CharacterStats characterStats, Transform transform)
    {
        return new ChickenFireballAttack(characterStats, this, transform);
    }
}
