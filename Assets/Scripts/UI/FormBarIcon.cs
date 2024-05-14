using Controller.Form;
using UnityEngine;
using UnityEngine.UI;

public class FormBarIcon : MonoBehaviour
{
    [SerializeField] private RawImage icon;
    [SerializeField] private Slider slider;
    private Form _formInstance;
    private UIRenderTexture _renderTexture;
    private void Awake()
    {
        icon.enabled = false;
        slider.gameObject.SetActive(false);
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture();
    }

    private void FixedUpdate()
    {
        if (_formInstance != null)
        {
            slider.value = _formInstance.Health;
        }
    }

    public void SetIcon(Form formInstance)
    {
        icon.enabled = true;
        icon.texture = _renderTexture.RenderTexture; 
        _formInstance = formInstance;
        _renderTexture.ChangeModel(_formInstance.Data.Model);
        slider.maxValue = _formInstance.Data.Health;
        slider.gameObject.SetActive(true);
    }
}
