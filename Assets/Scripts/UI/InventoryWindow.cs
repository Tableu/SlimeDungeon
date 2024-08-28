using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    public enum Category
    {
        Spells
    }
    
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private InventoryWidget inventoryWidget;
    [SerializeField] private RawImage image;
    private UIRenderTexture _renderTexture;
    
    public Category CurrentCategory
    {
        get;
        private set;
    }
    
    private void Start()
    {
        inventoryWidget.OnItemClicked += inventoryController.ItemClicked;
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture(false);
        _renderTexture.ChangeModel(inventoryController.SelectedCharacter.Data.Model);
        image.texture = _renderTexture.RenderTexture;
        RefreshAttacks();
    }

    public void Next()
    {
        inventoryController.ChangeCharacter(1);
        _renderTexture.ChangeModel(inventoryController.SelectedCharacter?.Data.Model);
        RefreshAttacks();
    }

    public void Previous()
    {
        inventoryController.ChangeCharacter(-1);
        _renderTexture.ChangeModel(inventoryController.SelectedCharacter?.Data.Model);
        RefreshAttacks();
    }

    public void RefreshAttacks()
    {
        inventoryWidget.Refresh(inventoryController.GetAttackIcons());
    }
    
    private void OnDestroy()
    {
        if(inventoryController != null)
            inventoryWidget.OnItemClicked -= inventoryController.ItemClicked;
    }
}
