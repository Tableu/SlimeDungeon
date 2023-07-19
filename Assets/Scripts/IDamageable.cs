using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type type);
}
