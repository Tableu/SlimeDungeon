using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private List<Slider> sliders;
    [SerializeField] private TextMeshProUGUI text;
    private EnemyController _controller;
    public void Initialize(EnemyController controller, string name)
    {
        _controller = controller;
        foreach (Slider slider in sliders)
        {
            slider.maxValue = controller.EnemyData.Health;
            slider.value = controller.EnemyData.Health;
        }
        controller.OnDeath += OnDeath;
        text.text = name;
    }

    private void FixedUpdate()
    {
        if (_controller != null)
        {
            foreach (Slider slider in sliders)
            {
                slider.value = _controller.Health;
            }
        }
    }

    private void OnDeath()
    {
        foreach (Slider slider in sliders)
        {
            slider.value = _controller.Health;
        }
        _controller.OnDeath -= OnDeath;
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        if (_controller != null)
        {
            _controller.OnDeath -= OnDeath;
        }
    }
}
