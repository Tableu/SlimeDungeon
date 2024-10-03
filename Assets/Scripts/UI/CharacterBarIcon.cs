using Controller.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterBarIcon : MonoBehaviour
{
    [SerializeField] private RawImage icon;
    [FormerlySerializedAs("slider")] [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider expSlider;
    [FormerlySerializedAs("text")] [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image selectedIcon;
    private PlayerCharacter _playerCharacterInstance;
    private UIRenderTexture _renderTexture;
    private void Awake()
    {
        icon.enabled = false;
        healthSlider.gameObject.SetActive(false);
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture();
        gameObject.SetActive(false);
        expSlider.maxValue = 1.0f;
    }

    private void FixedUpdate()
    {
        if (_playerCharacterInstance != null)
        {
            healthSlider.maxValue = _playerCharacterInstance.Stats.MaxHealth;
            healthSlider.value = _playerCharacterInstance.Stats.Health;
            levelText.text = (_playerCharacterInstance.ExperienceSystem.Level + 1).ToString();
            expSlider.value = _playerCharacterInstance.ExperienceSystem.ExperiencePercentage;
        }
    }

    public void SetIcon(PlayerCharacter playerCharacterInstance)
    {
        gameObject.SetActive(true);
        icon.enabled = true;
        icon.texture = _renderTexture.RenderTexture; 
        _playerCharacterInstance = playerCharacterInstance;
        _renderTexture.ChangeModel(_playerCharacterInstance.Data.Model);
        healthSlider.maxValue = _playerCharacterInstance.Stats.MaxHealth;
        healthSlider.gameObject.SetActive(true);
        nameText.text = playerCharacterInstance.Data.Name;
    }

    public void SetSelected(PlayerCharacter playerCharacter)
    {
        selectedIcon.enabled = _playerCharacterInstance == playerCharacter;
    }
}
