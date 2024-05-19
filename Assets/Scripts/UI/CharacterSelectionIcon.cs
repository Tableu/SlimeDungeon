using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionIcon : MonoBehaviour
{
    [SerializeField] private RawImage image;
    private UIRenderTexture _renderTexture;

    public void Initialize(GameObject model)
    {
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture();
        _renderTexture.ChangeModel(model);
        image.texture = _renderTexture.RenderTexture;
    }
}
