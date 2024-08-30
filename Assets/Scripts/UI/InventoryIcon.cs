using UnityEngine;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image border;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Button button;
    [SerializeField] private Sprite selectedBorderSprite;
    [SerializeField] private Sprite borderSprite;
    private int _index;
    private InventoryWidget _inventoryWidget;
    private bool _selected;

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
        border.color = Color.white;
        button.enabled = false;
        _selected = false;
        border.sprite = borderSprite;
    }

    public void SetSelected()
    {
        border.sprite = selectedBorderSprite;
        _selected = true;
    }

    public void OnClick()
    {
        _inventoryWidget.ItemClicked(_index, _selected);
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