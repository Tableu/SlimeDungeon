using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float onDamageDuration = 3;
    private Vector3 _offset;
    private PhysicsObstacle _controller;
    private Camera _camera;

    public void Initialize(PhysicsObstacle controller, Vector3 offset, Camera camera)
    {
        _controller = controller;
        slider.maxValue = _controller.Health;
        _controller.OnDeath += OnDeath;
        _controller.OnDamage += OnDamage;
        _offset = offset;
        _camera = camera;
        slider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_controller != null && slider.gameObject.activeSelf)
        { 
            transform.position = _offset + _camera.WorldToScreenPoint(_controller.transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (_controller != null && slider.gameObject.activeSelf)
        {
            slider.value = _controller.Health;
        }
    }

    private void OnDamage(int damage)
    {
        StopCoroutine(FadeCoroutine());
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        slider.gameObject.SetActive(true);
        yield return new WaitForSeconds(onDamageDuration);
        slider.gameObject.SetActive(false);
    }

    private void OnDeath()
    {
        slider.value = _controller.Health;
        _controller.OnDeath -= OnDeath;
        StopAllCoroutines();
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
