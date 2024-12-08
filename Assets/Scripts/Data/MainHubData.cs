using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MainHubData", menuName = "Data/Main Hub Data")]
public class MainHubData : ScriptableObject
{
    [SerializeField] private List<MainHubLevel> _levels;

    public MainHubLevel GetLevelData(int level)
    {
        foreach (MainHubLevel hubLevel in _levels)
        {
            if (hubLevel.RequiredLevel > level)
                return hubLevel;
        }

        return _levels[0];
    }
}

[Serializable]
public struct MainHubLevel
{
    [FormerlySerializedAs("RequiredSavedSlimes")] public int RequiredLevel;
    public int SpellCount;
    public int HatCount;
    public int CharacterCount;
}