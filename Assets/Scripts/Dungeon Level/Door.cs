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

    private void Open()
    {
        int parentRotation = Math.Sign(parent.rotation.eulerAngles.y);
        if (parentRotation == 0)
            parentRotation = 1;
        leftDoor.transform.localRotation = Quaternion.Euler(0, 90*parentRotation, 0);
        rightDoor.transform.localRotation = Quaternion.Euler(0, -90*parentRotation, 0);
        doorCollider.enabled = false;
        frameCollider.enabled = false;
        Vector2Int levelCoords = _fogWar.WorldToLevel(transform.position);
        for (int x = 0; x < _fogWar._UnitScale+1; x++)
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Open();
        }
    }
}
