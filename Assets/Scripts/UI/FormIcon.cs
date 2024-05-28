using Controller.Form;
using UnityEngine;
using UnityEngine.UI;

public class FormIcon : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private RawImage image;
    private UIRenderTexture _renderTexture;
    
    private void Start()
    {
        playerController.OnFormChange += OnFormChange;
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture();
        _renderTexture.ChangeModel(playerController.CurrentForm.Data.Model);
        image.texture = _renderTexture.RenderTexture;
    }

    private void OnFormChange()
    {
        _renderTexture.ChangeModel(playerController.CurrentForm.Data.Model);
    }

    private void OnDestroy()
    {
        playerController.OnFormChange -= OnFormChange;
    }
}
