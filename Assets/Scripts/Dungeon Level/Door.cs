using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private new Collider collider;
    [SerializeField] private Collider leftCollider;
    [SerializeField] private Collider rightCollider;

    private void Open()
    {
        int parentRotation = Math.Sign(transform.parent.rotation.eulerAngles.y);
        if (parentRotation == 0)
            parentRotation = 1;
        leftDoor.transform.localRotation = Quaternion.Euler(0, 90*parentRotation, 0);
        rightDoor.transform.localRotation = Quaternion.Euler(0, -90*parentRotation, 0);
        leftCollider.enabled = false;
        rightCollider.enabled = false;
        collider.enabled = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Open();
        }
    }
}
