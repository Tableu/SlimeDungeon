using UnityEngine;

public class UIRenderTextureManager : MonoBehaviour
{
    private static UIRenderTextureManager _instance;

    public static UIRenderTextureManager Instance => _instance;
    
    [SerializeField] private GameObject renderTexturePrefab;
    [SerializeField] private RenderTexture renderTexture;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public UIRenderTexture SpawnRenderTexture(bool pauseCamera = true)
    {
        GameObject rtObject = Instantiate(renderTexturePrefab, transform);
        rtObject.transform.localPosition =  new Vector3(10 * transform.childCount, 0, 0);
        UIRenderTexture script = rtObject.GetComponent<UIRenderTexture>();
        if (script != null)
        {
            script.Initialize(renderTexture, pauseCamera);
        }

        return script;
    }
}
