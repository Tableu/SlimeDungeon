using Controller.Form;
using UnityEngine;
using UnityEngine.UI;

public class FormBarIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Slider slider;
    private int _index;
    private PlayerController _controller;
    private Form _formInstance;
    private void Awake()
    {
        icon.enabled = false;
        slider.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_formInstance != null)
        {
            slider.value = _formInstance.Health;
        }
    }

    public void Initialize(int index, PlayerController controller)
    {
        _controller = controller;
        _index = index;
    }
    
    public void SetIcon(Form formInstance)
    {
        icon.enabled = true;
        icon.sprite = formInstance.Data.Icon;
        _formInstance = formInstance;
        slider.maxValue = formInstance.Health;
        slider.gameObject.SetActive(true);
    }
}
