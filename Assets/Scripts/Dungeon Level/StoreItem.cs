using UnityEngine;

public class StoreItem : MonoBehaviour
{
    private int _cost;
    private bool _bought;

    private void Start()
    {
        CharacterItem characterItem = GetComponent<CharacterItem>();
        SpellItem spellItem = GetComponent<SpellItem>();
        if (characterItem != null)
        {
            _cost = characterItem.Character.Data.Cost;
        }
        else if (spellItem != null)
        {
            _cost = spellItem.Data.Cost;
        }
    }

    public bool Buy()
    {
        if (_bought)
            return true;
        if (ResourceManager.Instance.Coins.Value >= _cost)
        {
            ResourceManager.Instance.Coins.Remove(_cost);
            _bought = true;
            return true;
        }
        return false;
    }
}
