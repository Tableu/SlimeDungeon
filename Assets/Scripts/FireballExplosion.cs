using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballExplosion : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireball;
    [SerializeField] private GameObject explosion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.name);
        fireball.Stop();
        GameObject death = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
