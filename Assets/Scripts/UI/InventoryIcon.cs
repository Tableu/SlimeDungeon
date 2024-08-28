using UnityEngine;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image background;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Button button;
    private int _index;
    private InventoryWidget _inventoryWidget;

    public void Initialize(int index, InventoryWidget inventoryWidget)
    {
        image.color = new Color(0, 0, 0, 0);
        _index = index;
        _inventoryWidget = inventoryWidget;
    }

    public void SetIcon(Sprite icon)
    {
        image.sprite = icon;
        image.color = Color.white;
        button.enabled = true;
    }

    public void ClearIcon()
    {
        image.color = new Color(0, 0, 0, 0);
        background.color = Color.white;
        button.enabled = false;
    }

    public void SetSelected()
    {
        image.color = Color.white;
        background.color = Color.grey;
        button.enabled = false;
    }

    public void OnClick()
    {
        _inventoryWidget.ItemClicked(_index);
    }

    public void OnDisable()
    {
        image.color = disabledColor;
        button.enabled = false;
    }

    public void OnEnable()
    {
        image.color = Color.white;
        button.enabled = true;
    }
}