using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "SlimeBall Attack", menuName = "Attacks/SlimeBall Attack")]
public class SlimeBallAttackData : AttackData
{
    [HeaderAttribute("Slime AOE")]
    [SerializeField] private GameObject slimeArea;
    [SerializeField] private float slow;
    [SerializeField] private float duration;
    [SerializeField] private float damagePerTick;

    public GameObject SlimeArea => slimeArea;
    public float Slow => slow;
    public float Duration => duration;
    public float DamagePerTick => damagePerTick;

    public override Attack CreateInstance(Character character)
    {
        return new SlimeBallAttack(character, this);
    }
}
