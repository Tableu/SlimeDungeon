using System;
using Newtonsoft.Json.Linq;
using Systems.Save;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] private GameObject coin;

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

    public void SpawnCoins(int amount, Vector3 position)
    {
        int variance = Mathf.CeilToInt(amount / 10f);
        amount += Random.Range(-variance, variance+1);
        for (int x = 0; x < amount; x++)
        {
            Vector3 pos = new Vector3(Random.Range(-0.3f, 0.3f), 0.25f, Random.Range(-0.3f, 0.3f));
            Vector3 rot = new Vector3(90, Random.Range(0, 360), Random.Range(0, 360));
            GameObject coinInstance = Instantiate(coin, position + pos, Quaternion.Euler(rot));
            Rigidbody rig = coinInstance.GetComponent<Rigidbody>();
            if(rig != null)
                rig.AddExplosionForce(5f, position+new Vector3(0,0.25f,0), 2f,0f,ForceMode.Impulse);
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
