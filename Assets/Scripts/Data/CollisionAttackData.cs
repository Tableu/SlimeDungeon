using Controller;
using UnityEngine;


[CreateAssetMenu(fileName = "Collision Attack", menuName = "Attacks/Collision Attack")]
public class CollisionAttackData : AttackData
{
    public override Attack EquipAttack(Character character)
    {
        Attack attack = new CollisionAttack(character, this);
        character.attacks.Add(attack);
        return attack;
    }
}
