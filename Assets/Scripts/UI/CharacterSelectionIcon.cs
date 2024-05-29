using Controller.Character;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionIcon : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Button button;
    private UIRenderTexture _renderTexture;
    private CharacterSelectionScreen _selectionScreen;
    private CharacterData _characterData;
    
    public void Initialize(CharacterSelectionScreen selectionScreen, CharacterData characterData)
    {
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture();
        _renderTexture.ChangeModel(characterData.Model);
        image.texture = _renderTexture.RenderTexture;
        _characterData = characterData;
        _selectionScreen = selectionScreen;
        button.enabled = true;
    }

    public void OnClick()
    {
        _selectionScreen.OnIconClick(_characterData);    
    }
}
