using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "EggRainAttack", menuName = "Data/Attacks/Egg Rain Attack")]
public class EggRainAttackData : AttackData
{
    [SerializeField] private int eggCount;
    [SerializeField] private Vector2 spawnHeight;
    [SerializeField] private float spawnInterval;
    [SerializeField] private GameObject enemyPrefab;
    
    public int EggCount => eggCount;
    public Vector2 SpawnHeight => spawnHeight;
    public float SpawnInterval => spawnInterval;
    public GameObject EnemyPrefab => enemyPrefab;
    public override Attack CreateInstance(CharacterStats characterStats, Transform transform)
    {
        return new EggRainAttack(characterStats, this, transform);
    }
}
