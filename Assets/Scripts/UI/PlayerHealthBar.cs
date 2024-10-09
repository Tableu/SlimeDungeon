using Controller.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PartyController partyController;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        slider.maxValue = partyController.CurrentPlayerCharacter.Data.Health;
        slider.value = partyController.CurrentPlayerCharacter.Stats.Health;
        partyController.OnCharacterChanged += OnCharacterChanged;
    }

    private void FixedUpdate()
    {
        slider.value = partyController.CurrentPlayerCharacter.Stats.Health;
        slider.maxValue = partyController.CurrentPlayerCharacter.Stats.MaxHealth;
        if (text != null)
        {
            text.text = (int)slider.value + "/" + (int)slider.maxValue;
        }
    }

    private void OnCharacterChanged(PlayerCharacter playerCharacter)
    {
        slider.maxValue = partyController.CurrentPlayerCharacter.Stats.MaxHealth;
        slider.value = playerCharacter.Stats.Health;
        if (text != null)
        {
            text.text = (int)slider.value + "/" + (int)slider.maxValue;
        }
    }

    private void OnDestroy()
    {
        if (partyController != null)
        {
            partyController.OnCharacterChanged -= OnCharacterChanged;
        }
    }
}
