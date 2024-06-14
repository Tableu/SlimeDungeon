using Cinemachine;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private BoxCollider boundingCollider;
    [SerializeField] private BoxCollider triggerCollider;

    public void Initialize(RectInt bounds, float tileSize)
    {
        float fov = virtualCamera.m_Lens.FieldOfView;
        float d = virtualCamera.transform.localPosition.y;
        float h = d * 2f * Mathf.Tan(fov * Mathf.Deg2Rad / 2f);
        float w = ((float) Screen.width / Screen.height)*h;
        float x = bounds.size.x * tileSize - w > 0 ? (bounds.size.x + 1) * tileSize - w : 0;
        float y = bounds.size.y * tileSize - h > 0 ? bounds.size.y * tileSize - h : 0;
        boundingCollider.size = new Vector3(x, 2, y);
        triggerCollider.size = new Vector3(tileSize * (bounds.size.x-3) + 1, 5, tileSize * (bounds.size.y-3) + 1);
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
