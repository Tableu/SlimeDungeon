using System;
using Newtonsoft.Json.Linq;
using Systems.Save;
using UnityEngine;

public class Resource
{
    public Resource(int value = 0)
    {
        Value = value;
    }
    public int Value { get; private set; } = 0;

    public void Add(int amount)
    {
        Value += amount;
    }

    public bool Remove(int amount)
    {
        if (Value >= amount)
        {
            Value -= amount;
            return true;
        }
        return false;
    }
}

public class ResourceManager : MonoBehaviour, ISavable
{
    private static ResourceManager _instance;

    public static ResourceManager Instance => _instance;

    public Resource Coins { get; private set; }

    public string id { get; } = "ResourceManager";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            Coins ??= new Resource();
        }
    }
    
    #region Save Logic

    public object SaveState()
    {
        return new SaveData()
        {
            Coins = Coins.Value
        };
    }

    public void LoadState(JObject state)
    {
        var saveData = state.ToObject<SaveData>();
        Coins = new Resource(saveData.Coins);
    }

    [Serializable]
    public struct SaveData
    {
        public int Coins;
    }
    #endregion
}
