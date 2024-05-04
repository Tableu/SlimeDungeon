using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class RandomizedGroup<T> : ScriptableObject
{
    [Serializable]
    public class Group
    {
        public int Weight;
        public List<T> GroupList;
    }

    [SerializeField] private List<Group> groups;

    public List<T> GetRandomGroup()
    {
        int totalWeight = groups.Sum((x => x.Weight));
        int randomWeight = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;
        
        foreach (Group group in groups)
        {
            currentWeight += group.Weight;
            if (randomWeight <= currentWeight)
            {
                return group.GroupList;
            }
        }

        return groups.Last().GroupList;
    }
}
