using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerController controller;

    private void Start()
    {
        slider.maxValue = controller.Mana;
    }

    private void FixedUpdate()
    {
        slider.value = controller.Mana;
    }
}
