using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack Dictionary", menuName = "Data/Dictionaries/Attack Dictionary")]
[Serializable]
public class AttackDataDictionary : ScriptableObject
{
    [SerializeField] private SerializableDictionary<string, AttackData> dataObjects;

    public SerializableDictionary<string, AttackData> Dictionary => dataObjects;
}