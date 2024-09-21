using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "BouncingFireball Attack", menuName = "Data/Attacks/BouncingFireball Attack")]
public class BouncingFireballAttackData : AttackData
{
    [SerializeField] private int maxBounceCount;
    [SerializeField] private Vector2 launchAngle;
    [SerializeField] private float explosionDamageRadius = 2.5f;
    public int MaxBounceCount => maxBounceCount;
    public Vector2 LaunchAngle => launchAngle;
    public float ExplosionDamageRadius => explosionDamageRadius;
    public override Attack CreateInstance(ICharacterInfo characterInfo, Transform transform)
    {
        return new BouncingFireballAttack(characterInfo, this, transform);
    }
}
