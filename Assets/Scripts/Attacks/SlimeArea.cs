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
    private List<Character> slowedEnemies;
    
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
        slowedEnemies = new List<Character>();
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
        ApplyDamage(other.gameObject);
        ApplySlow(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyDamage(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveSlow(other.gameObject);
    }

    private void OnDestroy()
    {
        foreach (Character enemy in slowedEnemies)
        {
            if (enemy != null)
            {
                enemy.Speed.BaseModifier += _slow;
            }
        }
    }

    private void ApplyDamage(GameObject enemy)
    {
        IDamageable damageable = enemy.GetComponent<IDamageable>();
        damageable?.TakeDamage(_damage, Vector3.zero, 0, _type);
    }

    private void ApplySlow(GameObject enemy)
    {
        Character character = enemy.GetComponent<Character>();
        if (character != null)
        {
            character.Speed.BaseModifier -= _slow;
            slowedEnemies.Add(character);
        }
    }

    private void RemoveSlow(GameObject enemy)
    {
        Character character = enemy.GetComponent<Character>();
        if (character != null)
        {
            character.Speed.BaseModifier += _slow;
            slowedEnemies.Remove(character);
        }
    }
}
