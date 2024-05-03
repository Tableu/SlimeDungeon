using System;
using System.Collections.Generic;
using System.Linq;
using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomCapturedCharacters", menuName = "Random Captured Characters")]
public class RandomizedCapturedCharacters : ScriptableObject
{
    [Serializable]
    public class CharacterGroup
    {
        public int Weight;
        public List<FormData> Characters;
    }

    [SerializeField] private List<CharacterGroup> characterGroups;

    public List<FormData> GetRandomCapturedCharacters()
    {
        int totalWeight = characterGroups.Sum((x => x.Weight));
        int randomWeight = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;
        
        foreach (CharacterGroup group in characterGroups)
        {
            currentWeight += group.Weight;
            if (randomWeight <= currentWeight)
            {
                return group.Characters;
            }
        }

        return characterGroups.Last().Characters;
    }
}
