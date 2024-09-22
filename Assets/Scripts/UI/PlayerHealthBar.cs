using Controller.Player;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PartyController partyController;

    private void Start()
    {
        slider.maxValue = partyController.CurrentCharacter.Data.Health;
        slider.value = partyController.CurrentCharacter.Stats.Health;
        partyController.OnCharacterChanged += OnFormChange;
    }

    private void FixedUpdate()
    {
        slider.value = partyController.CurrentCharacter.Stats.Health;;
    }

    private void OnFormChange(Character character)
    {
        slider.maxValue = character.Data.Health;
        slider.value = character.Stats.Health;
    }

    private void OnDestroy()
    {
        if (partyController != null)
        {
            partyController.OnCharacterChanged -= OnFormChange;
        }
    }
}
