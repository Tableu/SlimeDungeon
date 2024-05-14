using UnityEngine;

public class UIRenderTexture : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform modelParent;
    private GameObject _model;
    private RenderTexture _renderTexture;

    public RenderTexture RenderTexture => _renderTexture;
    
    public void Initialize(RenderTexture renderTexture)
    {
        RenderTexture rt = new RenderTexture(renderTexture);
        rt.Create();
        rt.Release();
        camera.targetTexture = rt;
        _renderTexture = rt;
    }

    public void ChangeModel(GameObject model)
    {
        if(_model != null)
            Destroy(_model);
        _model = Instantiate(model, modelParent);
    }
}
