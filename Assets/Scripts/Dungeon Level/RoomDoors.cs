using System.Collections.Generic;
using UnityEngine;

public class RoomDoors : MonoBehaviour
{
    private List<Door> _doors;
    private RoomController _controller;
    public void Initialize(RoomController controller, List<Door> doors)
    {
        _doors = doors;
        _controller = controller;
        controller.OnAllEnemiesDead += delegate
        {
            foreach (Door door in _doors)
            {
                door.Lock(false);
            }
        };
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
