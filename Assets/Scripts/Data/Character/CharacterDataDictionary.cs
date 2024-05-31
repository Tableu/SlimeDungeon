using System;
using Controller.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Dictionary", menuName = "Character Dictionary")]
[Serializable]
public class CharacterDataDictionary : ScriptableObject
{
    [SerializeField] private SerializableDictionary<string, CharacterData> dataObjects;

    public SerializableDictionary<string, CharacterData> Dictionary => dataObjects;
}