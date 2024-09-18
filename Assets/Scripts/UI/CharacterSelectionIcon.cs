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
