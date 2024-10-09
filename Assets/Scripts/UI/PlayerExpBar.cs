using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PartyController partyController;
    [SerializeField] private TextMeshProUGUI text;
    private void Start()
    {
        slider.maxValue = 1.0f;
    }

    private void FixedUpdate()
    {
        slider.value = partyController.CurrentPlayerCharacter.ExperienceSystem.ExperiencePercentage;
        if (text != null)
        {
            text.text = (int)(slider.value*100) + "%";
        }
    }

}
