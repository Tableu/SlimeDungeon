using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Leaf Attack", menuName = "Data/Attacks/Leaf Attack")]
public class LeafAttackData : AttackData
{
    [SerializeField] private float randomAngle;

    public float RandomAngle => randomAngle;
    public override Attack CreateInstance(CharacterStats characterStats, Transform transform)
    {
        return new LeafAttack(characterStats, this, transform);
    }
}
