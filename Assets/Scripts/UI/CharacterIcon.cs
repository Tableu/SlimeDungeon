using Controller.Player;
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
        _renderTexture.ChangeModel(partyController.CurrentPlayerCharacter.Data.Model);
        image.texture = _renderTexture.RenderTexture;
    }

    private void OnPartyMemberChange(PlayerCharacter playerCharacter)
    {
        _renderTexture.ChangeModel(playerCharacter.Data.Model);
    }

    private void OnDestroy()
    {
        partyController.OnCharacterChanged -= OnPartyMemberChange;
    }
}
