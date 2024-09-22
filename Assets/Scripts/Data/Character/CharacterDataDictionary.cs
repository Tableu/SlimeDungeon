using System;
using Controller.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Dictionary", menuName = "Data/Dictionaries/Character Dictionary")]
[Serializable]
public class CharacterDataDictionary : ScriptableObject
{
    [SerializeField] private SerializableDictionary<string, PlayerCharacterData> dataObjects;

    public SerializableDictionary<string, PlayerCharacterData> Dictionary => dataObjects;
}