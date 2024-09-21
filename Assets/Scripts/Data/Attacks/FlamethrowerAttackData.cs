using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "Flamethrower Attack", menuName = "Data/Attacks/Flamethrower Attack")]
public class FlamethrowerAttackData : AttackData
{
    [SerializeField] private float initialManaCost;
    public float InitialManaCost => initialManaCost;
    public override Attack CreateInstance(ICharacterInfo characterInfo, Transform transform)
    {
        return new FlamethrowerAttack(characterInfo, this, transform);
    }
}
