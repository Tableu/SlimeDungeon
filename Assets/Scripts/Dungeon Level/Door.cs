using System;
using FischlWorks_FogWar;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private Collider doorCollider;
    [SerializeField] private Collider frameCollider;
    [SerializeField] private Transform parent;
    private csFogWar _fogWar;
    private bool _lock;
    private void Start()
    {
        // This part is meant to be modified following the project's scene structure later...
        try
        {
            _fogWar = GameObject.Find("FogWar").GetComponent<csFogWar>();
        }
        catch
        {
            Debug.LogErrorFormat("Failed to fetch csFogWar component. " +
                                 "Please rename the gameobject that the module is attached to as \"FogWar\", " +
                                 "or change the implementation located in the csFogVisibilityAgent.cs script.");
        }
    }

    public void Open()
    {
        int parentRotation = Math.Sign(parent.rotation.eulerAngles.y);
        if (parentRotation == 0)
            parentRotation = 1;
        leftDoor.transform.localRotation = Quaternion.Euler(0, 90*parentRotation, 0);
        rightDoor.transform.localRotation = Quaternion.Euler(0, -90*parentRotation, 0);
        doorCollider.enabled = false;
        frameCollider.enabled = false;
        if (_fogWar == null)
            return;
        Vector2Int levelCoords = _fogWar.WorldToLevel(transform.position);
        for (int x = 1; x < _fogWar._UnitScale+2; x++)
        {
            switch (parent.rotation.eulerAngles.y)
            {
                case 0:
                    _fogWar.levelData[levelCoords.x][levelCoords.y-x] = csFogWar.LevelColumn.ETileState.Empty;
                    break;
                case 90:
                    _fogWar.levelData[levelCoords.x-x][levelCoords.y] = csFogWar.LevelColumn.ETileState.Empty;
                    break;
                case 180:
                    _fogWar.levelData[levelCoords.x][levelCoords.y+x] = csFogWar.LevelColumn.ETileState.Empty;
                    break;
                case 270:
                    _fogWar.levelData[levelCoords.x+x][levelCoords.y] = csFogWar.LevelColumn.ETileState.Empty;
                    break;
            }
        }
    }

    public void Close()
    {
        leftDoor.transform.localRotation = Quaternion.Euler(0, 0, 0);
        rightDoor.transform.localRotation = Quaternion.Euler(0, 0, 0);
        doorCollider.enabled = true;
        frameCollider.enabled = true;
        if (_fogWar == null)
            return;
        Vector2Int levelCoords = _fogWar.WorldToLevel(transform.position);
        for (int x = 1; x < _fogWar._UnitScale+2; x++)
        {
            switch (parent.rotation.eulerAngles.y)
            {
                case 0:
                    _fogWar.levelData[levelCoords.x][levelCoords.y-x] = csFogWar.LevelColumn.ETileState.Obstacle;
                    break;
                case 90:
                    _fogWar.levelData[levelCoords.x-x][levelCoords.y] = csFogWar.LevelColumn.ETileState.Obstacle;
                    break;
                case 180:
                    _fogWar.levelData[levelCoords.x][levelCoords.y+x] = csFogWar.LevelColumn.ETileState.Obstacle;
                    break;
                case 270:
                    _fogWar.levelData[levelCoords.x+x][levelCoords.y] = csFogWar.LevelColumn.ETileState.Obstacle;
                    break;
            }
        }
    }

    public void Lock(bool lockDoor)
    {
        _lock = lockDoor;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_lock && other.gameObject.CompareTag("Player"))
        {
            Open();
        }
    }
}
