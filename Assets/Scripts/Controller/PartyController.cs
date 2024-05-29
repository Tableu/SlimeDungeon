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
    [SerializeField] private CharacterDataDictionary characterDictionary;
    
    private int _currentPartyMemberIndex = 0;
    private int _maxPartyCount;
    private Character _currentCharacter;
    private PlayerInputActions _playerInputActions;
    private List<Character> _initialPartyMembers = new List<Character>();
    private List<Character> _partyMembers = new List<Character>();
    
    public List<Character> PartyMembers => _partyMembers;
    public int MaxPartyCount => _maxPartyCount;
    public Character CurrentCharacter => _currentCharacter; 
    public string id { get; } = "PartyController";
    
    public Action<Character> OnCharacterChanged;
    public Action<Character, int> OnPartyMemberAdded;
    public Action<Character> OnPartyMemberRemoved;

    public void Initialize(PlayerInputActions inputActions)
    {
        _playerInputActions = inputActions;
        _partyMembers = new List<Character>();
        _maxPartyCount = playerData.MaxFormCount;
        string initialForm = PlayerPrefs.GetString("Initial Form");
        if (_initialPartyMembers.Count == 0)
        {
            _initialPartyMembers.Add(new Character(characterDictionary.Dictionary[initialForm]));
        }
        InitializeParty();
    }

    private void Start()
    {
        _playerInputActions.Other.SwitchForms.started += SwitchCharacters;
        _playerInputActions.Other.PickUp.started += delegate(InputAction.CallbackContext context)
        {
            Collider[] itemColliders = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("Items"));
            var itemsOrderedByProximity = itemColliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
                .ToArray();
            if (itemsOrderedByProximity.Length == 0)
                return;
            var col = itemsOrderedByProximity[0];

            CapturedCharacter character = col.gameObject.GetComponentInParent<CapturedCharacter>();
            if (character != null)
            {
                Character oldCharacter = AddPartyMember(character.Character);
                if (oldCharacter != null)
                {
                    character.SwitchCharacter(oldCharacter);
                }
                else
                {
                    Destroy(character.gameObject);
                }
            }
        };
    }
    
    private void InitializeParty()
    {
        _partyMembers.Clear();
        foreach (Character character in _initialPartyMembers)
        {
            _partyMembers.Add(character);
            OnPartyMemberAdded?.Invoke(character, _currentPartyMemberIndex);
        }

        int i = 0;
        for (i = 0; i < _partyMembers.Count; i++)
        {
            if (_partyMembers[i].Health > 0)
            {
                ChangeCharacter(_partyMembers[i], _partyMembers.Count - 1);
                break;
            }
        }
    }
    private Character AddPartyMember(Character character)
    {
        Character oldCharacter = null;
        if (_partyMembers.Count >= _maxPartyCount)
        {
            if (_partyMembers.Count > 0)
            {
                oldCharacter = _currentCharacter;
                OnPartyMemberRemoved?.Invoke(_currentCharacter);
            }
            ChangeCharacter(character, _currentPartyMemberIndex);
            _partyMembers[_currentPartyMemberIndex] = character;
            OnPartyMemberAdded?.Invoke(character, _currentPartyMemberIndex);
        }
        else
        {
            _partyMembers.Add(character);
            OnPartyMemberAdded?.Invoke(character, _partyMembers.Count-1);
        }

        return oldCharacter;
    }
    
    private void SwitchCharacters(InputAction.CallbackContext context)
    {
        int oldIndex = _currentPartyMemberIndex;
        int diff = (int)context.ReadValue<float>();
        int formIndex = _currentPartyMemberIndex+diff;
        if (formIndex >= _partyMembers.Count)
        {
            formIndex = 0;
        }

        if (formIndex < 0)
        {
            formIndex = _partyMembers.Count - 1;
        }
        
        while (formIndex != oldIndex)
        {
            if (_partyMembers[formIndex].Health > 0)
            {
                ChangeCharacter(_partyMembers[formIndex],formIndex);
                return;
            }

            formIndex += diff;
            if (formIndex >= _partyMembers.Count)
            {
                formIndex = 0;
            }

            if (formIndex < 0)
            {
                formIndex = _partyMembers.Count - 1;
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
            foreach (var formInstance in _partyMembers)
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
        foreach (var form in _partyMembers)
        {
            form.Health += amount;
        }
    }
    
    public void CharacterFainted()
    {
        int index = 0;
        foreach (Character form in _partyMembers)
        {
            if (form.Health > 0)
            {
                ChangeCharacter(form, index);
                return;
            }

            index++;
        }
        Destroy(gameObject);
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
        foreach (Character character in _partyMembers)
        {
            saveData.Add(new Character.SaveData(characterDictionary.Dictionary.First(i => i.Value == character.Data).Key, character.Health));
        }
        
        return new SaveData()
        {
            Characters = saveData
        };
    }

    public void LoadState(JObject state)
    {
        var saveData = state.ToObject<SaveData>();

        _initialPartyMembers.Clear();
        foreach (Character.SaveData data in saveData.Characters)
        {
            _initialPartyMembers.Add(new Character(characterDictionary.Dictionary[data.Character], data.Health));
        }

        InitializeParty();
    }
    
    [Serializable]
    public struct SaveData
    {
        public List<Character.SaveData> Characters;
    }
    #endregion
}
