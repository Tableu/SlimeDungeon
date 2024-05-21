using Controller.Form;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionIcon : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Button button;
    private UIRenderTexture _renderTexture;
    private CharacterSelectionScreen _selectionScreen;
    private FormData _formData;
    
    public void Initialize(CharacterSelectionScreen selectionScreen, FormData formData)
    {
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture();
        _renderTexture.ChangeModel(formData.Model);
        image.texture = _renderTexture.RenderTexture;
        _formData = formData;
        _selectionScreen = selectionScreen;
        button.enabled = true;
    }

    public void OnClick()
    {
        _selectionScreen.OnIconClick(_formData);    
    }
}
