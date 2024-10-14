using System.Collections.Generic;
using Controller;
using UnityEngine;
using Type = Elements.Type;

public class SlimeArea : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    private float _damage;
    private float _duration;
    private float _attackStat;
    private float _slow;
    private Type _type;
    private List<ICharacter> slowedEnemies;
    
    public void Initialize(float damage, float attackStat, float duration, float slow, Type type)
    {
        _damage = damage;
        _duration = duration;
        _attackStat = attackStat;
        _type = type;
        _slow = slow;
        particleSystem.Stop();
        ParticleSystem.MainModule main = particleSystem.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(duration-1, duration);
        particleSystem.Play();
        slowedEnemies = new List<ICharacter>();
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
        foreach (ICharacter enemy in slowedEnemies)
        {
            if (enemy != null)
            {
                enemy.GetStats().Speed.BaseModifier -= _slow;
            }
        }
    }

    private void ApplyDamage(Collider enemy)
    {
        IDamageable damageable = enemy.attachedRigidbody != null ? 
            enemy.attachedRigidbody.gameObject.GetComponent<IDamageable>() 
            : enemy.GetComponent<IDamageable>();
        damageable?.TakeDamage(_damage, _attackStat,Vector3.zero, 0, _type);
    }

    private void ApplySlow(Collider enemy)
    {
        ICharacter character = enemy.attachedRigidbody != null ? 
            enemy.attachedRigidbody.gameObject.GetComponent<ICharacter>() 
            : enemy.GetComponent<ICharacter>();
        
        if (character != null)
        {
            CharacterStats characterStats = character.GetStats();
            characterStats.Speed.MultiplicativeModifer += _slow;
            slowedEnemies.Add(character);
        }
    }

    private void RemoveSlow(Collider enemy)
    {
        ICharacter character = enemy.attachedRigidbody != null ? 
            enemy.attachedRigidbody.gameObject.GetComponent<ICharacter>() 
            : enemy.GetComponent<ICharacter>();
        if (character != null)
        {
            CharacterStats characterStats = character.GetStats();
            characterStats.Speed.MultiplicativeModifer -= _slow;
            slowedEnemies.Remove(character);
        }
    }
}
