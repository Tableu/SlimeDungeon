using System;
using System.Collections.Generic;
using System.Linq;
using Controller.Player;
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
    
    private int _currentPartyMemberIndex = 0;
    private int _maxPartyCount;
    private Character _currentCharacter;
    private PlayerInputActions _playerInputActions;
    private List<Character> _characters = new List<Character>();
    
    public List<Character> Characters => _characters;
    public int MaxPartyCount => _maxPartyCount;
    public Character CurrentCharacter => _currentCharacter;
    public string id { get; } = "PartyController";
    
    public Action<Character> OnCharacterChanged;
    public Action<AttackData> OnSpellEquipped;
    public Action<AttackData> OnSpellUnEquipped;
    public Action<Character, int> OnPartyMemberAdded;
    public Action<Character> OnPartyMemberRemoved;

    public void Initialize(PlayerInputActions inputActions)
    {
        _playerInputActions = inputActions;
        _characters = new List<Character>();
        _maxPartyCount = playerData.MaxFormCount;
        string initialForm = PlayerPrefs.GetString("Initial Form");
        if (_characters.Count == 0)
        {
            Character character = new Character(characterDictionary.Dictionary[initialForm], playerController);
            character.EquipSpell(character.Data.StartingSpells[0]);
            _characters.Add(character);
            OnPartyMemberAdded?.Invoke(character, 0);
            ChangeCharacter(_characters[_currentPartyMemberIndex], _currentPartyMemberIndex);
        }
    }

    private void Start()
    {
        _playerInputActions.Other.SwitchForms.started += SwitchCharacters;
    }
    
    public Character AddPartyMember(Character character)
    {
        Character oldCharacter = null;
        if (_characters.Count >= _maxPartyCount)
        {
            int partyMemberIndex = _currentPartyMemberIndex;
            if (_characters.Count > 0)
            {
                oldCharacter = _currentCharacter;

                for (int i = 0; i < _characters.Count; i++)
                {
                    Character partyMember = _characters[i];
                    if (partyMember.Health <= 0)
                    {
                        oldCharacter = null;
                        partyMemberIndex = i;
                    }
                }

                OnPartyMemberRemoved?.Invoke(oldCharacter);
            }
            ChangeCharacter(character, partyMemberIndex);
            _characters[partyMemberIndex] = character;
            OnPartyMemberAdded?.Invoke(character, partyMemberIndex);
        }
        else
        {
            _characters.Add(character);
            OnPartyMemberAdded?.Invoke(character, _characters.Count-1);
        }

        return oldCharacter;
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
            if (_characters[formIndex].Health > 0)
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

    public void HealCharacter(float amount)
    {
        if (Math.Abs(_currentCharacter.Health - _currentCharacter.Data.Health) > Mathf.Epsilon)
        {
            _currentCharacter.Health += amount;
        }
        else
        {
            foreach (var formInstance in _characters)
            {
                if (Math.Abs(_currentCharacter.Health - _currentCharacter.Data.Health) > Mathf.Epsilon)
                {
                    formInstance.Health += amount;
                }
            }
        }
    }

    public void HealCharacters(float amount)
    {
        foreach (var form in _characters)
        {
            form.Health += amount;
        }
    }

    public void EquipSpell(AttackData data, int index)
    {
        if (index < 0 || index > _characters.Count || data == null)
            return;
        
        AttackData oldSpell = _characters[index].EquipSpell(data);
        OnSpellEquipped.Invoke(data);
        if (oldSpell != null)
            OnSpellUnEquipped.Invoke(oldSpell);
    }

    public bool IsPartyAllFainted()
    {
        int index = 0;
        foreach (Character form in _characters)
        {
            if (form.Health > 0)
            {
                ChangeCharacter(form, index);
                return false;
            }

            index++;
        }
        return true;
    }
    
    private void ChangeCharacter(Character newCharacter, int newIndex)
    {
        if(_currentCharacter is not null)
            _currentCharacter.Drop();
        _currentCharacter = newCharacter;
        _currentPartyMemberIndex = newIndex;
        OnCharacterChanged?.Invoke(newCharacter);
    }

    #region Save Methods
    
    public object SaveState()
    {
        List<Character.SaveData> saveData = new List<Character.SaveData>();
        foreach (Character character in _characters)
        {
            saveData.Add(new Character.SaveData(
                characterDictionary.Dictionary.First(i => i.Value == character.Data).Key, 
                character.Health, character.Spell.Data.Name));
        }
        
        return new SaveData()
        {
            Characters = saveData,
            CurrentCharacterIndex = _currentPartyMemberIndex
        };
    }

    public void LoadState(JObject state)
    {
        var saveData = state.ToObject<SaveData>();
        _currentPartyMemberIndex = saveData.CurrentCharacterIndex;
        _characters.Clear();
        foreach (Character.SaveData data in saveData.Characters)
        {
            AttackData attackData = attackDictionary.Dictionary.ContainsKey(data.Spell)
                ? attackDictionary.Dictionary[data.Spell]
                : null;
            _characters.Add(new Character(
                characterDictionary.Dictionary[data.Character], 
                playerController, data.Health,
                attackData));
        }

        ChangeCharacter(_characters[_currentPartyMemberIndex], _currentPartyMemberIndex);
    }
    
    [Serializable]
    public struct SaveData
    {
        public List<Character.SaveData> Characters;
        public int CurrentCharacterIndex;
    }
    #endregion
}
