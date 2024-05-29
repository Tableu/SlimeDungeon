using System.Collections.Generic;
using Controller.Character;
using UnityEngine;
using UnityEngine.Serialization;

public class PartyBar : MonoBehaviour
{
    [FormerlySerializedAs("formIconPrefab")] [SerializeField] private GameObject characterIconPrefab;
    [SerializeField] private PartyController partyController;
    private List<CharacterBarIcon> characterIcons;

    private void Start()
    {
        characterIcons = new List<CharacterBarIcon>();
        for(int i = 0; i < partyController.MaxPartyCount; i++)
        {
            GameObject icon = Instantiate(characterIconPrefab, transform);
            var script = icon.GetComponent<CharacterBarIcon>();
            if (script != null)
            {
                characterIcons.Add(script);
                if(partyController.PartyMembers.Count > i)
                    script.SetIcon(partyController.PartyMembers[i]);
            }
        }

        partyController.OnPartyMemberAdded += OnPartyMemberAdd;
    }

    private void OnPartyMemberAdd(Character characterInstance, int index)
    {
        characterIcons[index].SetIcon(characterInstance);
    }

    private void OnDestroy()
    {
        partyController.OnPartyMemberAdded -= OnPartyMemberAdd;
    }
}
