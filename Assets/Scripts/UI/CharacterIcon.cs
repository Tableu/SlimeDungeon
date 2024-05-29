using Controller.Character;
using Controller.Form;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour
{
    [SerializeField] private PartyController partyController;
    [SerializeField] private RawImage image;
    private UIRenderTexture _renderTexture;
    
    private void Start()
    {
        partyController.OnCharacterChanged += OnPartyMemberChange;
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture();
        _renderTexture.ChangeModel(partyController.CurrentCharacter.Data.Model);
        image.texture = _renderTexture.RenderTexture;
    }

    private void OnPartyMemberChange(Character character)
    {
        _renderTexture.ChangeModel(character.Data.Model);
    }

    private void OnDestroy()
    {
        partyController.OnCharacterChanged -= OnPartyMemberChange;
    }
}
