using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Canvas canvas;
    private EnemyController _controller;

    public void Initialize(EnemyController controller)
    {
        _controller = controller;
        slider.maxValue = _controller.CharacterData.Health;
        _controller.OnDeath += OnDeath;
        canvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        if(_controller != null)
            transform.position = offset + _controller.transform.position;
    }

    private void FixedUpdate()
    {
        if(_controller != null)
            slider.value = _controller.Health;
    }

    private void OnDeath()
    {
        slider.value = _controller.Health;
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
