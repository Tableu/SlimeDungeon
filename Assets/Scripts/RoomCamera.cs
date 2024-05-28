using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private BoxCollider boundingCollider;
    [SerializeField] private BoxCollider triggerCollider;

    public void Initialize(RectInt bounds, float tileSize)
    {
        boundingCollider.size = new Vector3(bounds.size.x, 2, tileSize/2*(bounds.size.y-2));
        triggerCollider.size = new Vector3(tileSize * (bounds.size.x-2), 5, tileSize * (bounds.size.y-2));
    }
    
    private void OnTriggerEnter(Collider other)
    {
        virtualCamera.Follow = other.transform.parent; 
        virtualCamera.enabled = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        virtualCamera.Follow = null;
        virtualCamera.enabled = false;
    }
}
