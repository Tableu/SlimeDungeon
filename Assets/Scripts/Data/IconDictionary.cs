using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Icon Dictionary", menuName = "Data/Dictionaries/Icon Dictionary")]
[Serializable]
public class IconDictionary : ScriptableObject
{
    [SerializeField] private SerializableDictionary<string, Sprite> dataObjects;

    public SerializableDictionary<string, Sprite> Dictionary => dataObjects;
}
