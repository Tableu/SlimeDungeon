using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class RandomizedGroup<T> : ScriptableObject
{
    [Serializable]
    private class Group
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

public abstract class RandomizedList<T> : ScriptableObject
{
    [Serializable]
    private class Element
    {
        public int Weight;
        public T Item;
    }

    [SerializeField] private List<Element> list;
    public T GetRandomElement()
    {
        int totalWeight = list.Sum((x => x.Weight));
        int randomWeight = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;
        
        foreach (Element element in list)
        {
            currentWeight += element.Weight;
            if (randomWeight <= currentWeight)
            {
                return element.Item;
            }
        }

        return list.Last().Item;
    }
}