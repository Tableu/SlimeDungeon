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
        partyController.OnCharacterChanged += OnFormChange;
    }

    private void FixedUpdate()
    {
        slider.value = partyController.CurrentPlayerCharacter.Stats.Health;;
    }

    private void OnFormChange(PlayerCharacter playerCharacter)
    {
        slider.maxValue = playerCharacter.Data.Health;
        slider.value = playerCharacter.Stats.Health;
    }

    private void OnDestroy()
    {
        if (partyController != null)
        {
            partyController.OnCharacterChanged -= OnFormChange;
        }
    }
}
