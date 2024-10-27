using System;
using UnityEngine;

public class EnemyPathingController : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;

    public Vector3 Velocity => _direction;
    public float StoppingDistance { get; set; }
    public float RemainingDistance => (Destination - transform.position).magnitude;
    
    public bool IsStopped { get; set; }

    public bool UpdateRotation { get; set; }

    private Vector3 _direction;
    
    public float Speed { get; set; }
    
    public Vector3 Destination { get; private set; }

    public void SetDestination(Vector3 position)
    {
        Destination = position;
    }

    private void FixedUpdate()
    {
        if (IsStopped)
            return;
        _direction = (Destination - transform.position).normalized;
        rigidbody.velocity = _direction * Speed;
        
        if(UpdateRotation)
            transform.rotation =
            Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, Destination, Mathf.Infinity, 0.0f));
    }
}
