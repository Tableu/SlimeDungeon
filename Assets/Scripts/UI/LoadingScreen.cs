using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private UIRenderTexture loadingTexture;
    [SerializeField] private GameObject characterModel;
    [SerializeField] private RawImage loadingIcon;
    private float angle = 10;
    private void Start()
    {
        loadingTexture.Initialize(renderTexture);
        loadingIcon.texture = loadingTexture.RenderTexture;
    }

    public void RotateIcon()
    {
        characterModel.transform.Rotate(Vector3.up, angle);
    }
}
