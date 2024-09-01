using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWidget : MonoBehaviour
{
    [SerializeField] private List<InventoryIcon> inventoryIconList;
    
    public Action<int, bool> OnItemClicked;
    private void Awake()
    {
        for (var index = 0; index < inventoryIconList.Count; index++)
        {
            InventoryIcon icon = inventoryIconList[index];
            icon.Initialize(index, this);
        }
    }

    public void Refresh(List<IconInfo> icons)
    {
        using List<IconInfo>.Enumerator iconInfoEnumerator = icons.GetEnumerator();
        foreach (InventoryIcon icon in inventoryIconList)
        {
            if (iconInfoEnumerator.MoveNext())
            {
                icon.SetIcon(iconInfoEnumerator.Current.Icon);
                if (iconInfoEnumerator.Current.Selected)
                {
                    icon.SetSelected();
                }else if (iconInfoEnumerator.Current.Disabled)
                {
                    icon.SetDisabled();
                }
                else
                {
                    icon.SetEnabled();
                }
            }
            else
            {
                icon.SetEmpty();
            }
        }
    }

    public void SelectIcon(int index)
    {
        if (index < 0 || index >= inventoryIconList.Count || inventoryIconList[index] == null)
            return;
        inventoryIconList[index].enabled = true;
        inventoryIconList[index].SetSelected();
    }

    public void DisableIcon(int index)
    {
        if (index < 0 || index >= inventoryIconList.Count || inventoryIconList[index] == null)
            return;
        inventoryIconList[index].enabled = false;
    }

    public void ItemClicked(int index, bool selected)
    {
        OnItemClicked?.Invoke(index, selected);
    }

    public readonly struct IconInfo
    {
        public readonly Sprite Icon;
        public readonly bool Selected;
        public readonly bool Disabled;

        public IconInfo(Sprite icon, bool selected, bool disabled)
        {
            Icon = icon;
            Selected = selected;
            Disabled = disabled;
        }
    }
}
