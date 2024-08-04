using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoors : MonoBehaviour
{
    [SerializeField] private BoxCollider triggerCollider;
    private List<Door> _doors;
    private RoomController _controller;

    private void Start()
    {
        _controller.OnAllEnemiesDead += delegate
        {
            foreach (Door door in _doors)
            {
                door.Lock(false);
            }
        };
    }

    public void Initialize(RoomController controller, List<Door> doors, RectInt bounds, float tileSize)
    {
        _doors = doors;
        _controller = controller;
        triggerCollider.size = new Vector3(tileSize * (bounds.size.x-3)+0.5f, 5, tileSize * (bounds.size.y-3)+0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_controller.AllEnemiesDead)
            return;
        foreach (Door door in _doors)
        {
            door.Close();
            door.Lock(true);
        }
    }
}
