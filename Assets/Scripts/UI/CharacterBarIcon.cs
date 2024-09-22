using Controller.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBarIcon : MonoBehaviour
{
    [SerializeField] private RawImage icon;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image selectedIcon;
    private Character _characterInstance;
    private UIRenderTexture _renderTexture;
    private void Awake()
    {
        icon.enabled = false;
        slider.gameObject.SetActive(false);
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture();
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_characterInstance != null)
        {
            slider.value = _characterInstance.Stats.Health;
        }
    }

    public void SetIcon(Character characterInstance)
    {
        gameObject.SetActive(true);
        icon.enabled = true;
        icon.texture = _renderTexture.RenderTexture; 
        _characterInstance = characterInstance;
        _renderTexture.ChangeModel(_characterInstance.Data.Model);
        slider.maxValue = _characterInstance.Data.Health;
        slider.gameObject.SetActive(true);
        text.text = characterInstance.Data.Name;
    }

    public void SetSelected(Character character)
    {
        selectedIcon.enabled = _characterInstance == character;
    }
}
