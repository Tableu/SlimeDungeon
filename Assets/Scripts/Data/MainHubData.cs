using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MainHubData", menuName = "Data/Main Hub Data")]
public class MainHubData : ScriptableObject
{
    [SerializeField] private List<MainHubLevel> _levels;

    public MainHubLevel GetLevelData(int slimesSaved)
    {
        foreach (MainHubLevel hubLevel in _levels)
        {
            if (hubLevel.RequiredSavedSlimes > slimesSaved)
                return hubLevel;
        }

        return _levels[0];
    }
}

[Serializable]
public struct MainHubLevel
{
    public int RequiredSavedSlimes;
    public int SpellCount;
    public int HatCount;
    public int CharacterCount;
}