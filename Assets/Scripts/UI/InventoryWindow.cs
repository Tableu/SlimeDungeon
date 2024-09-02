using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private InventoryWidget inventoryWidget;
    [SerializeField] private RawImage image;
    private UIRenderTexture _renderTexture;
    private InventoryController.ItemType _currentItemType;

    private void Start()
    {
        _currentItemType = InventoryController.ItemType.Spells;
        inventoryWidget.OnItemClicked += delegate(int i, bool b)
        {
            inventoryController.ItemClicked(i, b, _currentItemType);
            Refresh();
        };;
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture(false);
        _renderTexture.ChangeModel(inventoryController.SelectedCharacter.Data.Model);
        image.texture = _renderTexture.RenderTexture;
        Refresh();
    }

    public void Next()
    {
        inventoryController.ChangeCharacter(1);
        _renderTexture.ChangeModel(inventoryController.SelectedCharacter?.Data.Model);
        Refresh();
    }

    public void Previous()
    {
        inventoryController.ChangeCharacter(-1);
        _renderTexture.ChangeModel(inventoryController.SelectedCharacter?.Data.Model);
        Refresh();
    }

    public void Refresh()
    {
        inventoryWidget.Refresh(inventoryController.GetIcons(_currentItemType));
    }

    public void ChangeItemType(string itemType)
    {
        switch (itemType)
        {
            case "Spells":
                _currentItemType = InventoryController.ItemType.Spells;
                break;
            case "Hats":
                _currentItemType = InventoryController.ItemType.Hats;
                break;
        }
        Refresh();
    }
}
