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
    private PlayerCharacter _playerCharacterInstance;
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
        if (_playerCharacterInstance != null)
        {
            slider.value = _playerCharacterInstance.Stats.Health;
        }
    }

    public void SetIcon(PlayerCharacter playerCharacterInstance)
    {
        gameObject.SetActive(true);
        icon.enabled = true;
        icon.texture = _renderTexture.RenderTexture; 
        _playerCharacterInstance = playerCharacterInstance;
        _renderTexture.ChangeModel(_playerCharacterInstance.Data.Model);
        slider.maxValue = _playerCharacterInstance.Data.Health;
        slider.gameObject.SetActive(true);
        text.text = playerCharacterInstance.Data.Name;
    }

    public void SetSelected(PlayerCharacter playerCharacter)
    {
        selectedIcon.enabled = _playerCharacterInstance == playerCharacter;
    }
}
