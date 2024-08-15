using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private Vector3 _offset;
    private EnemyController _controller;
    private Camera _camera;

    public void Initialize(EnemyController controller, Vector3 offset, Camera camera)
    {
        _controller = controller;
        slider.maxValue = _controller.EnemyData.Health;
        _controller.OnDeath += OnDeath;
        _offset = offset;
        _camera = camera;
    }

    private void Update()
    {
        if (_controller != null)
        { 
            slider.gameObject.SetActive(_controller.Visible);
            if(_controller.Visible)
                transform.position = _offset + _camera.WorldToScreenPoint(_controller.transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (_controller != null && _controller.Visible)
        {
            slider.value = _controller.Health;
        }
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
