using System.Collections;
using UnityEngine;

public class UIRenderTexture : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private Transform modelParent;
    private bool _pauseCamera;
    private GameObject _model;
    private RenderTexture _renderTexture;

    public RenderTexture RenderTexture => _renderTexture;
    
    public void Initialize(RenderTexture renderTexture, bool pauseCamera = true)
    {
        _pauseCamera = pauseCamera;
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
        camera.enabled = true;
        if(_pauseCamera)
            StartCoroutine(PauseCamera());
    }

    private IEnumerator PauseCamera()
    {
        yield return new WaitForEndOfFrame();
        camera.enabled = false;
    }
}
