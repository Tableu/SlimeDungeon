using UnityEngine;
using UnityEngine.UI;

public class EnemyStatBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Canvas canvas;
    private EnemyController _controller;

    public void Initialize(EnemyController controller)
    {
        _controller = controller;
        healthSlider.maxValue = _controller.EnemyData.Health;
        _controller.OnDeath += OnDeath;
        canvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        if (_controller != null)
        { 
            canvas.enabled = _controller.Visible;
            transform.position = offset + _controller.transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (_controller != null)
        {
            healthSlider.value = _controller.Health;
        }
    }

    private void OnDeath()
    {
        healthSlider.value = _controller.Health;
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
