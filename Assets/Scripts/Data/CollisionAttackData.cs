using Controller;
using UnityEngine;


[CreateAssetMenu(fileName = "Collision Attack", menuName = "Attacks/Collision Attack")]
public class CollisionAttackData : AttackData
{
    public override Attack EquipAttack(Character character, int index)
    {
        Attack attack = new CollisionAttack(character, this);
        character.attacks.Insert(index, attack);
        return attack;
    }
}
