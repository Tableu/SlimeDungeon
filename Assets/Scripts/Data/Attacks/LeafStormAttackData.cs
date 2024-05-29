using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Leaf Storm Attack", menuName = "Attacks/Leaf Storm Attack")]
public class LeafStormAttackData : AttackData
{
    [SerializeField] private float duration;

    public float Duration => duration;
    public override Attack CreateInstance(ICharacterInfo characterInfo)
    {
        return new LeafStormAttack(characterInfo, this);
    }
}
