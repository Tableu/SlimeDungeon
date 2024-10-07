using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage, float attackStat, Vector3 knockback, float hitStun, Elements.Type type);
}
