using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment Dictionary", menuName = "Data/Dictionaries/Equipment Dictionary")]
[Serializable]
public class EquipmentDataDictionary : ScriptableObject
{
    [SerializeField] private SerializableDictionary<string, EquipmentData> dataObjects;

    public SerializableDictionary<string, EquipmentData> Dictionary => dataObjects;
}