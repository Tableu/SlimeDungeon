using Controller.Form;
using UnityEngine;
using UnityEngine.UI;

public class FormBarIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Slider slider;
    private int _index;
    private PlayerController _controller;
    private SavedForm _form;
    private void Awake()
    {
        icon.enabled = false;
        slider.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_form != null)
        {
            slider.value = _form.Health;
        }
    }

    public void Initialize(int index, PlayerController controller)
    {
        _controller = controller;
        _index = index;
    }
    
    public void SetIcon(SavedForm form)
    {
        icon.enabled = true;
        icon.sprite = form.Data.Icon;
        _form = form;
        slider.maxValue = form.Health;
        slider.gameObject.SetActive(true);
    }
}
