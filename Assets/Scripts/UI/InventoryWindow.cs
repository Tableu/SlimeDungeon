using System;
using Controller.Player;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private PartyController partyController;
    [SerializeField] private InventoryWidget inventoryWidget;
    [SerializeField] private RawImage image;
    private UIRenderTexture _renderTexture;
    private InventoryController.ItemType _currentItemType;

    private void Awake()
    {
        if(WindowManager.Instance != null)
            WindowManager.Instance.RegisterWindow(gameObject);
    }

    private void Start()
    {
        _currentItemType = InventoryController.ItemType.Spells;
        partyController.OnEquipmentAdded += RefreshHat;
        partyController.OnEquipmentRemoved += delegate(EquipmentData data)
        {
            RefreshHat(null);
        };
        inventoryWidget.OnItemClicked += delegate(int i, bool b)
        {
            inventoryController.ItemClicked(i, b, _currentItemType);
            Refresh();
        };
        _renderTexture = UIRenderTextureManager.Instance.SpawnRenderTexture(false);
        _renderTexture.ChangeModel(inventoryController.SelectedCharacter.Data.Model);
        _renderTexture.AdjustCamera(new Vector3(0,0,-0.5f));
        RefreshHat(inventoryController.SelectedCharacter.Equipment);
        image.texture = _renderTexture.RenderTexture;
        Refresh();
    }

    public void Next()
    {
        inventoryController.ChangeCharacter(1);
        if (inventoryController.SelectedCharacter != null)
        {
            _renderTexture.ChangeModel(inventoryController.SelectedCharacter.Data.Model);
            RefreshHat(inventoryController.SelectedCharacter.Equipment);
            Refresh();
        }
    }

    public void Previous()
    {
        inventoryController.ChangeCharacter(-1);
        if (inventoryController.SelectedCharacter != null)
        {
            _renderTexture.ChangeModel(inventoryController.SelectedCharacter.Data.Model);
            RefreshHat(inventoryController.SelectedCharacter.Equipment);
            Refresh();
        }
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

    private void RefreshHat(EquipmentData data)
    {
        if (_renderTexture != null && _renderTexture.Model != null)
        {
            CharacterAnimator animator = _renderTexture.Model.GetComponentInChildren<CharacterAnimator>();
            if (animator != null)
            {
                animator.RefreshHat(data);
            }
        }
    }

    private void OnEnable()
    {
        WindowManager.Instance.OnWindowChanged();
    }

    private void OnDisable()
    {
        WindowManager.Instance.OnWindowChanged();
    }

    private void OnDestroy()
    {
        if(WindowManager.Instance != null)
            WindowManager.Instance.UnRegisterWindow(gameObject);
        if(partyController != null)
            partyController.OnEquipmentAdded -= RefreshHat;
    }
}
