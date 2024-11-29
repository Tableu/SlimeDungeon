using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Leaf Storm Attack", menuName = "Data/Attacks/Leaf Storm Attack")]
public class LeafStormAttackData : AttackData
{
    [SerializeField] private float duration;
    [SerializeField] private int tick;

    public float Duration => duration;
    public int Tick => tick;
    public override Attack CreateInstance(CharacterStats characterStats, Transform transform)
    {
        return new LeafStormAttack(characterStats, this, transform);
    }
}
