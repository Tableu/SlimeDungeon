using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomEnemyGroups", menuName = "Random Enemy Groups")]
public class RandomizedEnemyGroups : ScriptableObject
{
    [Serializable]
    public class EnemyGroup
    {
        public int Weight;
        public List<GameObject> Enemies;
    }

    [SerializeField] private List<EnemyGroup> enemyGroups;

    public List<GameObject> GetRandomEnemyGroup()
    {
        int totalWeight = enemyGroups.Sum(x => x.Weight);
        int randomWeight = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;
        foreach (EnemyGroup group in enemyGroups)
        {
            currentWeight += group.Weight;
            if (randomWeight <= currentWeight)
            {
                return group.Enemies;
            }
        }

        return enemyGroups.Last().Enemies;
    }
}
