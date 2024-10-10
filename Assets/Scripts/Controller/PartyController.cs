using System;
using System.Collections.Generic;
using System.Linq;
using Controller.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Systems.Save;
using UnityEngine;
using UnityEngine.InputSystem;

public class PartyController : MonoBehaviour, ISavable
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CharacterDataDictionary characterDictionary;
    [SerializeField] private AttackDataDictionary attackDictionary;
    [SerializeField] private EquipmentDataDictionary equipmentDictionary;
    
    private int _currentPartyMemberIndex = 0;
    private int _maxPartyCount;
    private PlayerCharacter _currentPlayerCharacter;
    private PlayerInputActions _playerInputActions;
    private List<PlayerCharacter> _characters = new List<PlayerCharacter>();
    
    public List<PlayerCharacter> Characters => _characters;
    public int MaxPartyCount => _maxPartyCount;
    public PlayerCharacter CurrentPlayerCharacter => _currentPlayerCharacter;
    public string id { get; } = "PartyController";
    
    public Action<PlayerCharacter> OnCharacterChanged;
    public Action<AttackData> OnSpellEquipped;
    public Action<AttackData> OnSpellUnEquipped;
    public Action<EquipmentData> OnEquipmentAdded;
    public Action<EquipmentData> OnEquipmentRemoved;
    public Action<PlayerCharacter, int> OnPartyMemberAdded;
    public Action<PlayerCharacter> OnPartyMemberRemoved;

    public void Initialize(PlayerInputActions inputActions)
    {
        _maxPartyCount = playerData.MaxFormCount;
        _playerInputActions = inputActions;
        _playerInputActions = playerController.PlayerInputActions;
        _playerInputActions.Combat.SwitchForms.started += SwitchCharacters;
        string initialForm = PlayerPrefs.GetString("Initial Form");
        if (_characters.Count == 0)
        {
            PlayerCharacter playerCharacter = new PlayerCharacter(characterDictionary.Dictionary[initialForm], playerController.transform);
            playerCharacter.EquipSpell(playerCharacter.Data.StartingSpells[0]);
            _characters.Add(playerCharacter);
            OnPartyMemberAdded?.Invoke(playerCharacter, 0);
            ChangeCharacter(_characters[_currentPartyMemberIndex], _currentPartyMemberIndex);
        }
    }
    
    public PlayerCharacter AddPartyMember(PlayerCharacter playerCharacter)
    {
        PlayerCharacter oldPlayerCharacter = null;
        if (_characters.Count >= _maxPartyCount)
        {
            int partyMemberIndex = _currentPartyMemberIndex;
            if (_characters.Count > 0)
            {
                oldPlayerCharacter = _currentPlayerCharacter;

                for (int i = 0; i < _characters.Count; i++)
                {
                    PlayerCharacter partyMember = _characters[i];
                    if (partyMember.Stats.Health <= 0)
                    {
                        oldPlayerCharacter = null;
                        partyMemberIndex = i;
                    }
                }

                OnPartyMemberRemoved?.Invoke(oldPlayerCharacter);
            }
            ChangeCharacter(playerCharacter, partyMemberIndex);
            _characters[partyMemberIndex] = playerCharacter;
            OnPartyMemberAdded?.Invoke(playerCharacter, partyMemberIndex);
        }
        else
        {
            _characters.Add(playerCharacter);
            OnPartyMemberAdded?.Invoke(playerCharacter, _characters.Count-1);
        }

        return oldPlayerCharacter;
    }
    
    private void SwitchCharacters(InputAction.CallbackContext context)
    {
        int oldIndex = _currentPartyMemberIndex;
        int diff = (int)context.ReadValue<float>();
        int formIndex = _currentPartyMemberIndex+diff;
        if (formIndex >= _characters.Count)
        {
            formIndex = 0;
        }

        if (formIndex < 0)
        {
            formIndex = _characters.Count - 1;
        }
        
        while (formIndex != oldIndex)
        {
            if (_characters[formIndex].Stats.Health > 0)
            {
                ChangeCharacter(_characters[formIndex],formIndex);
                return;
            }

            formIndex += diff;
            if (formIndex >= _characters.Count)
            {
                formIndex = 0;
            }

            if (formIndex < 0)
            {
                formIndex = _characters.Count - 1;
            }
        }
    }

    public AttackData EquipSpell(AttackData data, int index)
    {
        if (index < 0 || index > _characters.Count || data == null)
            return null;
        
        AttackData oldSpell = _characters[index].EquipSpell(data);
        OnSpellEquipped?.Invoke(data);
        if (oldSpell != null)
            OnSpellUnEquipped?.Invoke(oldSpell);
        return oldSpell;
    }

    public AttackData UnEquipSpell(int index)
    {
        if (index < 0 || index > _characters.Count)
            return null;
        AttackData oldSpell = _characters[index].UnEquipSpell();
        if (oldSpell != null)
            OnSpellUnEquipped?.Invoke(oldSpell);
        return oldSpell;
    }
    
    public EquipmentData AddEquipment(EquipmentData data, int index)
    {
        if (index < 0 || index > _characters.Count || data == null)
            return null;
        
        EquipmentData oldEquipment = _characters[index].AddEquipment(data);
        if (oldEquipment != null)
            OnEquipmentRemoved?.Invoke(oldEquipment);
        OnEquipmentAdded?.Invoke(data);
        return oldEquipment;
    }

    public EquipmentData RemoveEquipment(int index)
    {
        if (index < 0 || index > _characters.Count)
            return null;
        EquipmentData oldEquipment = _characters[index].RemoveEquipment();
        if (oldEquipment != null)
            OnEquipmentRemoved?.Invoke(oldEquipment);
        return oldEquipment;
    }

    public void AddExperience(int experience)
    {
        foreach (PlayerCharacter character in _characters)
        {
            character.ExperienceSystem.AddExperience(experience);
        }        
    }

    public bool IsPartyAllFainted()
    {
        int index = 0;
        foreach (PlayerCharacter form in _characters)
        {
            if (form.Stats.Health > 0)
            {
                ChangeCharacter(form, index);
                return false;
            }

            index++;
        }
        return true;
    }
    
    private void ChangeCharacter(PlayerCharacter newPlayerCharacter, int newIndex)
    {
        if(_currentPlayerCharacter is not null)
            _currentPlayerCharacter.Drop();
        _currentPlayerCharacter = newPlayerCharacter;
        _currentPartyMemberIndex = newIndex;
        OnCharacterChanged?.Invoke(newPlayerCharacter);
    }

    #region Save Methods
    
    public object SaveState()
    {
        List<PlayerCharacter.SaveData> saveData = new List<PlayerCharacter.SaveData>();
        foreach (PlayerCharacter character in _characters)
        {
            saveData.Add(new PlayerCharacter.SaveData(
                characterDictionary.Dictionary.First(i => i.Value == character.Data).Key, 
                character.Stats, character.Equipment != null ? character.Equipment.Name : null, 
                character.Spell?.Data.Name,character.ExperienceSystem.Level, character.ExperienceSystem.Experience, character.SkillPoints));
        }
        
        return new SaveData()
        {
            Characters = saveData,
            CurrentCharacterIndex = _currentPartyMemberIndex
        };
    }

    public void LoadState(JObject state)
    {
        var saveData = state.ToObject<SaveData>(new JsonSerializer()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        });
        _currentPartyMemberIndex = saveData.CurrentCharacterIndex;
        _characters.Clear();
        foreach (PlayerCharacter.SaveData data in saveData.Characters)
        {
            AttackData attackData;
            if (data.Spell == null)
            {
                attackData = null;
            }
            else
            {
                attackData = attackDictionary.Dictionary.ContainsKey(data.Spell)
                    ? attackDictionary.Dictionary[data.Spell]
                    : null;
            }

            EquipmentData equipmentData;
            if (data.Equipment == null)
            {
                equipmentData = null;
            }
            else
            {
                equipmentData = equipmentDictionary.Dictionary.ContainsKey(data.Equipment)
                    ? equipmentDictionary.Dictionary[data.Equipment]
                    : null;
            }

            _characters.Add(new PlayerCharacter(
                characterDictionary.Dictionary[data.Character],
                data.Stats,
                equipmentData,
                attackData,
                data.Level,
                data.Experience,
                data.SkillPoints,
                playerController.transform));
        }

        ChangeCharacter(_characters[_currentPartyMemberIndex], _currentPartyMemberIndex);
    }
    
    [Serializable]
    public struct SaveData
    {
        public List<PlayerCharacter.SaveData> Characters;
        public int CurrentCharacterIndex;
    }
    #endregion
}
