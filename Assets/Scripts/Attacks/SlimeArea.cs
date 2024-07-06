using System.Collections.Generic;
using Controller;
using UnityEngine;
using Type = Elements.Type;

public class SlimeArea : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    private float _damage;
    private float _duration;
    private float _slow;
    private Type _type;
    private List<ICharacterInfo> slowedEnemies;
    
    public void Initialize(float damage, float duration, float slow, Type type)
    {
        _damage = damage;
        _duration = duration;
        _type = type;
        _slow = slow;
        particleSystem.Stop();
        ParticleSystem.MainModule main = particleSystem.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(duration-1, duration);
        particleSystem.Play();
        slowedEnemies = new List<ICharacterInfo>();
    }

    private void FixedUpdate()
    {
        if (particleSystem.time > _duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyDamage(other);
        ApplySlow(other);
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyDamage(other);
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveSlow(other);
    }

    private void OnDestroy()
    {
        foreach (ICharacterInfo enemy in slowedEnemies)
        {
            if (enemy != null)
            {
                enemy.Speed.BaseModifier -= _slow;
            }
        }
    }

    private void ApplyDamage(Collider enemy)
    {
        IDamageable damageable = enemy.attachedRigidbody != null ? 
            enemy.attachedRigidbody.gameObject.GetComponent<IDamageable>() 
            : enemy.GetComponent<IDamageable>();
        damageable?.TakeDamage(_damage, Vector3.zero, 0, _type);
    }

    private void ApplySlow(Collider enemy)
    {
        ICharacterInfo characterInfo = enemy.attachedRigidbody != null ? 
            enemy.attachedRigidbody.gameObject.GetComponent<ICharacterInfo>() 
            : enemy.GetComponent<ICharacterInfo>();
        if (characterInfo != null)
        {
            characterInfo.Speed.MultiplicativeModifer += _slow;
            slowedEnemies.Add(characterInfo);
        }
    }

    private void RemoveSlow(Collider enemy)
    {
        ICharacterInfo characterInfo = enemy.attachedRigidbody != null ? 
            enemy.attachedRigidbody.gameObject.GetComponent<ICharacterInfo>() 
            : enemy.GetComponent<ICharacterInfo>();
        if (characterInfo != null)
        {
            characterInfo.Speed.MultiplicativeModifer -= _slow;
            slowedEnemies.Remove(characterInfo);
        }
    }
}
