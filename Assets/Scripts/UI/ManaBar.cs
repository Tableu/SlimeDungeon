using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerController controller;

    private void Start()
    {
        slider.maxValue = controller.Mana;

        if (slider.transform is RectTransform rt) 
            rt.sizeDelta = new Vector2(controller.Mana * 2, rt.sizeDelta.y);
    }

    private void FixedUpdate()
    {
        slider.value = controller.Mana;
    }
}
