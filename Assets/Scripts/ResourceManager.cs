using UnityEngine;

public class Resource
{
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

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;

    public static ResourceManager Instance => _instance;

    public Resource Coins { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            Coins = new Resource();
        }
    }
}
