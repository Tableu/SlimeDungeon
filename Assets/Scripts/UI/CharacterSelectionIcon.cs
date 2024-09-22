using Controller.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectionIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    [SerializeField] private RawImage image;
    [SerializeField] private Button button;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highlightColor;
    private UIRenderTexture _renderTexture;
    private CharacterSelectionScreen _selectionScreen;
    private PlayerCharacterData _playerCharacterData;
    
    public void Initialize(CharacterSelectionScreen selectionScreen, PlayerCharacterData playerCharacterData)
    {
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture();
        _renderTexture.ChangeModel(playerCharacterData.Model);
        image.texture = _renderTexture.RenderTexture;
        _playerCharacterData = playerCharacterData;
        _selectionScreen = selectionScreen;
        button.enabled = true;
    }

    public void OnClick()
    {
        _selectionScreen.OnIconClick(_playerCharacterData);    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = defaultColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnClick();
    }
}
