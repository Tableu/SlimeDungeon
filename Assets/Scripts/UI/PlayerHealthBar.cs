using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerController controller;

    private void Start()
    {
        slider.maxValue = controller.CurrentForm.Data.Health;
        slider.value = controller.Health;
        controller.OnDeath += OnDeath;
        controller.OnFormChange += OnFormChange;
    }

    private void FixedUpdate()
    {
        slider.value = controller.Health;
    }

    private void OnDeath()
    {
        slider.value = controller.Health;
        controller.OnDeath -= OnDeath;
        controller.OnFormChange -= OnFormChange;
    }

    private void OnFormChange()
    {
        slider.maxValue = controller.CurrentForm.Data.Health;
        slider.value = controller.Health;
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnDeath -= OnDeath;
            controller.OnFormChange -= OnFormChange;
        }
    }
}
