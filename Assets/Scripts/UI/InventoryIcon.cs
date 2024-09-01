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
    }

    public void SetEmpty()
    {
        image.color = new Color(0, 0, 0, 0);
        button.enabled = false;
        _selected = false;
        border.sprite = borderSprite;
    }

    public void SetDisabled()
    {
        image.color = disabledColor;
        button.enabled = false;
        _selected = false;
        border.sprite = borderSprite;
    }
    
    public void SetEnabled()
    {
        image.color = Color.white;
        button.enabled = true;
        _selected = false;
        border.sprite = borderSprite;
    }

    public void SetSelected()
    {
        image.color = Color.white;
        button.enabled = true;
        _selected = true;
        border.sprite = selectedBorderSprite;
    }

    public void OnClick()
    {
        _inventoryWidget.ItemClicked(_index, _selected);
    }
}