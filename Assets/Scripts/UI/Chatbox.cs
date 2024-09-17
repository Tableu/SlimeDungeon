using System;
using TMPro;
using UnityEngine;

public class Chatbox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Vector2 padding;
    private Transform _character;
    private Vector3 _offset;
    private Camera _camera;

    public void Initialize(Transform character, Vector3 offset, Camera camera)
    {
        _character = character;
        _offset = offset;
        _camera = camera;
    }

    private void Update()
    {
        if (_character != null)
        {
            transform.position = _offset + _camera.WorldToScreenPoint(_character.position);
        }
    }

    private void OnEnable()
    {
        Update();
    }

    public void SetMessage(string message)
    {
        text.text = message;
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = text.GetPreferredValues() + padding;
    }
}
