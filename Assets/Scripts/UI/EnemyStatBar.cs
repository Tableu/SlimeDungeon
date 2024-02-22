using UnityEngine;
using UnityEngine.UI;

public class EnemyStatBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider stunSlider;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Canvas canvas;
    private EnemyController _controller;

    public void Initialize(EnemyController controller)
    {
        _controller = controller;
        healthSlider.maxValue = _controller.CharacterData.Health;
        stunSlider.maxValue = _controller.CharacterData.StunResist;
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
        if (_controller != null)
        {
            healthSlider.value = _controller.Health;
            stunSlider.value = _controller.SuperEffectiveStunMeter;
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
