using Controller.Player;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PartyController partyController;

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
    }

    private void OnCharacterChanged(PlayerCharacter playerCharacter)
    {
        slider.maxValue = partyController.CurrentPlayerCharacter.Stats.MaxHealth;
        slider.value = playerCharacter.Stats.Health;
    }

    private void OnDestroy()
    {
        if (partyController != null)
        {
            partyController.OnCharacterChanged -= OnCharacterChanged;
        }
    }
}
