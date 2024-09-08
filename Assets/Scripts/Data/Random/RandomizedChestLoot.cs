using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "RandomChestLoot", menuName = "Data/Random Groups/Random Chest Loot")]
public class RandomizedChestLoot : ScriptableObject
{
    [SerializeField] private Vector2Int coinRange;
    [SerializeField] private List<AttackData> spells;
    [SerializeField] private List<EquipmentData> hats;
    public int GetCoins()
    {
        return Random.Range(coinRange.x, coinRange.y);
    }

    public AttackData GetSpell()
    {
        return spells[Random.Range(0, spells.Count)];
    }
    public EquipmentData GetHat()
    {
        return hats[Random.Range(0, hats.Count)];
    }
}