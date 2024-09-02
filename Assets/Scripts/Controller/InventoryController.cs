using System;
using System.Collections.Generic;
using Controller.Player;
using Newtonsoft.Json.Linq;
using Systems.Save;
using UnityEngine;

public class InventoryController : MonoBehaviour, ISavable
{
    [Serializable]
    public enum ItemType
    {
        Spells,
        Hats
    }

    public class Item
    {
        public ItemType ItemType { get; }
        public bool Equipped { get; private set; }
        public string ID { get; }
        public object Data { get; }
        public Character Holder { get; private set; }

        public Item(string id, object data, ItemType itemType)
        {
            ID = id;
            Data = data;
            ItemType = itemType;
            Equipped = false;
        }

        public void Equip(Character holder)
        {
            Holder = holder;
            Equipped = true;
        }

        public void UnEquip()
        {
            Holder = null;
            Equipped = false;
        }
    }

    [SerializeField] private IconDictionary inventoryIconDictionary;
    [SerializeField] private PartyController partyController;
    [SerializeField] private InventoryWindow inventoryWindow;

    private bool _loaded = false;
    private int _selectedCharacterIndex = 0;
    private List<Item> _spells = new List<Item>();
    private List<Item> _hats = new List<Item>();
    private List<string> _itemIDs = new List<string>();
    public Character SelectedCharacter => partyController.Characters.Count != 0 ? 
        partyController.Characters[_selectedCharacterIndex] : null;
    public string id { get; } = "InventoryController";

    public void Initialize()
    {
        if (!_loaded)
        {
            foreach (AttackData attackData in
                partyController.CurrentCharacter.Data.StartingSpells)
            {
                Add(attackData.Name, attackData, ItemType.Spells);
                Equip(attackData.Name, partyController.CurrentCharacter, ItemType.Spells);
            }
            inventoryWindow.Refresh();
        }
    }

    private void Start()
    {
        foreach (Character character in partyController.Characters)
        {
            if (character.Equipment != null)
            {
                foreach (Item hat in _hats)
                {
                    if(hat.ID == character.Equipment.Name)
                        hat.Equip(character);
                }
            }

            if (character.Spell != null)
            {
                foreach (Item spell in _spells)
                {
                    if(spell.ID == character.Spell.Data.Name)
                        spell.Equip(character);
                }
            }
        }
        inventoryWindow.Refresh();
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

    private List<Item> GetItems(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Hats:
                return _hats;
            case ItemType.Spells:
                return _spells;
        }

        return null;
    }

    public List<InventoryWidget.IconInfo> GetIcons(ItemType itemType)
    {
        List<InventoryWidget.IconInfo> iconInfos = new List<InventoryWidget.IconInfo>();
        List<Item> items = GetItems(itemType);
        if (items == null)
            return iconInfos;
        foreach (Item item in items)
        {
            iconInfos.Add(new InventoryWidget.IconInfo(
                inventoryIconDictionary.Dictionary[item.ID],
                SelectedCharacter == item.Holder,
                item.Equipped && SelectedCharacter != item.Holder));
        }
        return iconInfos;
    }

    public void ItemClicked(int index, bool selected, ItemType itemType)
    {
        List<Item> items = GetItems(itemType);
        if (items == null || items.Count == 0 || index < 0 || index >= items.Count)
            return;
        Item clickedItem = items[index];
        
        if (selected)
        {
            clickedItem.UnEquip();
            switch (itemType)
            {
                case ItemType.Spells:
                    partyController.UnEquipSpell(_selectedCharacterIndex);
                    break;
                case ItemType.Hats:
                    partyController.RemoveEquipment(_selectedCharacterIndex);
                    break;
            }
        }
        else
        {
            clickedItem.Equip(SelectedCharacter);
            switch (itemType)
            {
                case ItemType.Spells:
                    AttackData oldSpell = partyController.EquipSpell(clickedItem.Data as AttackData, _selectedCharacterIndex);
                    if(oldSpell != null)
                        UnEquip(oldSpell.Name, itemType);
                    break;
                case ItemType.Hats:
                    EquipmentData oldEquipment = partyController.AddEquipment(clickedItem.Data as EquipmentData, _selectedCharacterIndex);
                    if(oldEquipment != null)
                        UnEquip(oldEquipment.Name, itemType);
                    break;
            }
        }
    }
    
    public void Add(string itemID, object data, ItemType itemType)
    {
        List<Item> items = GetItems(itemType);
        if (items != null && !_itemIDs.Contains(itemID))
        {
            items.Add(new Item(itemID, data, itemType));
            _itemIDs.Add(itemID);
            inventoryWindow.Refresh();
        }
    }

    public void Remove(string itemID, ItemType itemType)
    {
        List<Item> items = GetItems(itemType);
        if (items == null)
            return;
        foreach (Item item in items)
        {
            if (item.ID == itemID)
            {
                items.Remove(item);
                _itemIDs.Remove(itemID);
                break;
            }
                
        }
    }

    public void Equip(string itemID, Character character, ItemType itemType)
    {
        List<Item> items = GetItems(itemType);
        if (items == null)
            return;
        foreach (Item item in items)
        {
            if (item.ID == itemID)
            {
                item.Equip(character);
                break;
            }
        }
    }
        
    public void UnEquip(string itemID, ItemType itemType)
    {
        List<Item> items = GetItems(itemType);
        if (items == null)
            return;
        foreach (Item item in items)
        {
            if (item.ID == itemID)
            {
                item.UnEquip();
                break;
            }
        }
    }

    #region Save Methods

    [SerializeField] private AttackDataDictionary attackDataDictionary;
    [SerializeField] private EquipmentDataDictionary equipmentDataDictionary;
    
    public object SaveState()
    {
        List<SavedInventoryItem> savedSpells = new List<SavedInventoryItem>();
        foreach (Item item in _spells)
        {
            savedSpells.Add(new SavedInventoryItem(item.ID, item.ItemType));
        }
        List<SavedInventoryItem> savedHats = new List<SavedInventoryItem>();
        foreach (Item item in _hats)
        {
            savedHats.Add(new SavedInventoryItem(item.ID, item.ItemType));
        }
        return new SaveData()
        {
            Spells = savedSpells,
            Hats = savedHats
        };
    }

    public void LoadState(JObject state)
    {
        _loaded = true;
        var saveData = state.ToObject<SaveData>();
        _spells = new List<Item>();
        _hats = new List<Item>();
        _itemIDs = new List<string>();
        
        foreach (SavedInventoryItem hat in saveData.Hats)
        {
            Add(hat.ID, equipmentDataDictionary.Dictionary[hat.ID], hat.ItemType);
        }
        
        foreach (SavedInventoryItem spell in saveData.Spells)
        {
            Add(spell.ID, attackDataDictionary.Dictionary[spell.ID], spell.ItemType);
        }
    }
    
    [Serializable]
    public struct SaveData
    {
        public List<SavedInventoryItem> Spells;
        public List<SavedInventoryItem> Hats;
    }

    public struct SavedInventoryItem
    {
        public string ID;
        public ItemType ItemType;

        public SavedInventoryItem(string id, ItemType itemType)
        {
            ID = id;
            ItemType = itemType;
        }
    }
    #endregion
}
