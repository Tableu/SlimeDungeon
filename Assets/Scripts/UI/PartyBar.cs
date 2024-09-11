using System.Collections.Generic;
using Controller.Player;
using UnityEngine;
using UnityEngine.Serialization;

public class PartyBar : MonoBehaviour
{
    [FormerlySerializedAs("formIconPrefab")] [SerializeField] private GameObject characterIconPrefab;
    [SerializeField] private PartyController partyController;
    private List<CharacterBarIcon> _characterIcons;

    private void Start()
    {
        _characterIcons = new List<CharacterBarIcon>();
        for(int i = 0; i < partyController.MaxPartyCount; i++)
        {
            GameObject icon = Instantiate(characterIconPrefab, transform);
            var script = icon.GetComponent<CharacterBarIcon>();
            if (script != null)
            {
                _characterIcons.Add(script);
                if(partyController.Characters.Count > i)
                    script.SetIcon(partyController.Characters[i]);
            }
        }
        if(partyController.CurrentCharacter != null)
            OnCharacterChanged(partyController.CurrentCharacter);
        partyController.OnCharacterChanged += OnCharacterChanged;
        partyController.OnPartyMemberAdded += OnPartyMemberAdd;
    }

    private void OnPartyMemberAdd(Character characterInstance, int index)
    {
        _characterIcons[index].SetIcon(characterInstance);
        _characterIcons[index].SetSelected(partyController.CurrentCharacter);
    }

    private void OnCharacterChanged(Character character)
    {
        if (_characterIcons == null || _characterIcons.Count <= 0)
            return;
        foreach (CharacterBarIcon characterBarIcon in _characterIcons)
        {
            characterBarIcon.SetSelected(character);
        }
    }

    private void OnDestroy()
    {
        if (partyController != null)
        {
            partyController.OnPartyMemberAdded -= OnPartyMemberAdd;
            partyController.OnCharacterChanged -= OnCharacterChanged;
        }
    }
}
