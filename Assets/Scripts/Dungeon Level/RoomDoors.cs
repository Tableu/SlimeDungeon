using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoors : MonoBehaviour
{
    [SerializeField] private BoxCollider triggerCollider;
    [SerializeField] private List<Door> doors;
    [SerializeField] private RoomController controller;

    private void Start()
    {
        if (controller != null)
        {
            controller.OnAllEnemiesDead += delegate
            {
                foreach (Door door in doors)
                {
                    door.Lock(false);
                }
            };
        }
    }

    public void Initialize(RoomController controller, List<Door> doors, RectInt bounds, float tileSize)
    {
        this.doors = doors;
        this.controller = controller;
        triggerCollider.size = new Vector3(tileSize * (bounds.size.x-3)+0.5f, 5, tileSize * (bounds.size.y-3)+0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(controller != null)
            controller.SpawnEnemies();
        if (controller == null || controller.AllEnemiesDead)
            return;
        foreach (Door door in doors)
        {
            door.Close();
            door.Lock(true);
        }
    }
}
