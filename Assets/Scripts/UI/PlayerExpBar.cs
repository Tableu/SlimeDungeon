using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PartyController partyController;
    private void Start()
    {
        slider.maxValue = 1.0f;
    }

    private void Update()
    {
        slider.value = partyController.CurrentPlayerCharacter.ExperienceSystem.ExperiencePercentage;
    }

}
