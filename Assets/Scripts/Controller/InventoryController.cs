using System;
using System.Collections.Generic;
using System.Linq;
using Controller.Player;
using Newtonsoft.Json.Linq;
using Systems.Save;
using UnityEngine;

public class InventoryController : MonoBehaviour, ISavable
{
    [SerializeField] private AttackDataDictionary attackDictionary;
    [SerializeField] private PartyController partyController;
    [SerializeField] private InventoryWindow inventoryWindow;
    
    private List<AttackData> _unlockedAttacks = new List<AttackData>();
    private List<AttackData> _equippedAttacks = new List<AttackData>();
    private bool _loaded = false;
    public Character SelectedCharacter => partyController.Characters.Count != 0 ? 
        partyController.Characters[_selectedCharacterIndex] : null;
    private int _selectedCharacterIndex;
    public List<AttackData> UnlockedAttacks => _unlockedAttacks;
    public string id { get; } = "InventoryController";

    public void Initialize()
    {
        partyController.OnSpellEquipped += delegate(AttackData data) { _equippedAttacks.Add(data); };
        partyController.OnSpellUnEquipped += delegate(AttackData data) { _equippedAttacks.Remove(data); };
        if (!_loaded)
        {
            _unlockedAttacks = new List<AttackData>();
            foreach (AttackData attackData in
                partyController.CurrentCharacter.Data.StartingSpells)
            {
                //Must be initialized from awake to properly trigger events
                UnlockAttack(attackData);
                if(!_equippedAttacks.Contains(attackData))
                    _equippedAttacks.Add(attackData);
            }
            inventoryWindow.RefreshAttacks();
        }
    }

    public void UnlockAttack(AttackData attackData)
    {
        if (!_unlockedAttacks.Contains(attackData))
        {
            _unlockedAttacks.Add(attackData);
            if (inventoryWindow.CurrentCategory == InventoryWindow.Category.Spells)
                inventoryWindow.RefreshAttacks();
        }
    }

    public void RemoveAttack(AttackData attackData)
    {
        _unlockedAttacks.Remove(attackData);
        if (inventoryWindow.CurrentCategory == InventoryWindow.Category.Spells)
            inventoryWindow.RefreshAttacks();
    }

    public void ChangeCharacter(int direction)
    {
        if (Math.Sign(direction) < 0)
        {
            _selectedCharacterIndex++;
            if (_selectedCharacterIndex >= partyController.Characters.Count)
                _selectedCharacterIndex = 0;
        }
        else
        {
            _selectedCharacterIndex--;
            if (_selectedCharacterIndex < 0)
                _selectedCharacterIndex = partyController.Characters.Count-1;
        }
    }

    public List<InventoryWidget.IconInfo> GetAttackIcons()
    {
        List<InventoryWidget.IconInfo> iconInfos = new List<InventoryWidget.IconInfo>();

        foreach (AttackData attack in _unlockedAttacks)
        {
            iconInfos.Add(new InventoryWidget.IconInfo(attack.Icon, 
                SelectedCharacter.Spell.Data.Name == attack.Name,
                _equippedAttacks.Contains(attack)));
        }
        return iconInfos;
    }

    public void ItemClicked(int index)
    {
        partyController.EquipSpell(_unlockedAttacks[index], _selectedCharacterIndex);
        inventoryWindow.RefreshAttacks();
    }

    #region Save Methods
    
    public object SaveState()
    {
        return new SaveData()
        {
            UnlockedAttacks = _unlockedAttacks.Select(x => attackDictionary.Dictionary.
                First(i => i.Value == x).Key).ToList(),
            EquippedAttacks = _equippedAttacks.Select(x => attackDictionary.Dictionary.
                First(i => i.Value == x).Key).ToList()
        };
    }

    public void LoadState(JObject state)
    {
        _loaded = true;
        var saveData = state.ToObject<SaveData>();
        _unlockedAttacks = new List<AttackData>();
        foreach (string attack in saveData.UnlockedAttacks)
        {
            _unlockedAttacks.Add(attackDictionary.Dictionary[attack]);
        }
        
        _equippedAttacks = new List<AttackData>();
        foreach (string attack in saveData.EquippedAttacks)
        {
            _equippedAttacks.Add(attackDictionary.Dictionary[attack]);
        }
    }
    
    [Serializable]
    public struct SaveData
    {
        public List<string> UnlockedAttacks;
        public List<string> EquippedAttacks;
    }
    #endregion
}
