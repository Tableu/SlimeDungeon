using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [Serializable]
    public struct TEntry
    {
        public TKey Key;
        public TValue Value;
    }

    [SerializeField] private List<TEntry> entries;

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        Clear();

        foreach(TEntry entry in entries)
        {
            Add(entry.Key, entry.Value);
        }
        
        Debug.Log("Dictionary successfully created with " + entries.Count + " entries");
    }
}
