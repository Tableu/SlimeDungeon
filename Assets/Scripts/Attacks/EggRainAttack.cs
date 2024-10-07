using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class EggRainAttack : Attack
{
    private EggRainAttackData _data;
    public override bool Begin()
    {
        if (CheckCooldown())
        {
            RoomController roomController = Transform.GetComponentInParent<RoomController>();
            if (roomController == null)
            {
                if(Transform.parent != null)
                    roomController = Transform.parent.GetComponentInParent<RoomController>();
            }

            for (int i = 0; i < _data.EggCount; i++)
            {
                Vector3 startPos = roomController.transform.position +
                                   roomController.GetRandomPositionInBounds() +
                                   Vector3.up * Random.Range(_data.SpawnHeight.x, _data.SpawnHeight.y);
                GameObject projectile = GameObject.Instantiate(Data.Prefab, startPos, Quaternion.identity);
                SetLayer(projectile);
                Egg script = projectile.GetComponent<Egg>();
                if (script == null)
                    continue;
                script.Initialize(Data.Damage,CharacterStats.Attack, Data.Knockback, Data.HitStun, Vector3.down*Data.Speed, Data.ElementType,
                    _data.EnemyPrefab, roomController);
            }
            Cooldown(Data.Cooldown);
            return true;
        }
        return false;
    }

    public override void End()
    {
        return;
    }

    public override void LinkInput(InputAction action)
    {
        UnlinkInput();
        inputAction = action;
        action.started += Begin;
    }

    public override void UnlinkInput()
    {
        if (inputAction != null)
        {
            inputAction.started -= Begin;
        }
    }
    
    public EggRainAttack(CharacterStats characterStats, EggRainAttackData data, Transform transform) : base(characterStats, data, transform)
    {
        _data = data;
    }
}
