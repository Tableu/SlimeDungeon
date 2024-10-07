using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Attribute = Controller.Attribute;

public class CharacterStatsWidget : MonoBehaviour
{
    [SerializeField] private List<Slider> sliders;
    [SerializeField] private List<Button> plusIcons;
    public Action<Attribute> OnUpgradeIconClicked;

    private void Start()
    {
        plusIcons[0].onClick.AddListener(delegate
        {
            OnUpgradeIconClicked?.Invoke(Attribute.Health);
        });
        plusIcons[1].onClick.AddListener(delegate
        {
            OnUpgradeIconClicked?.Invoke(Attribute.Attack);
        });
        plusIcons[2].onClick.AddListener(delegate
        {
            OnUpgradeIconClicked?.Invoke(Attribute.Armor);
        });
    }

    public void Refresh(List<float> stats)
    {
        for (var index = 0; index < sliders.Count; index++)
        {
            if (index >= stats.Count)
                break;
            Slider slider = sliders[index];
            slider.value = stats[index];
        }
    }

    public void ToggleUpgrade(bool upgrade)
    {
        foreach (Button plusIcon in plusIcons)
        {
            plusIcon.gameObject.SetActive(upgrade);
        }
    }
}
