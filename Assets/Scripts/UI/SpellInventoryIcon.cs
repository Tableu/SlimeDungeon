using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellInventoryIcon : DraggableItem
{
    [SerializeField] private Image image;
    [SerializeField] private Color disabledColor;
    private AttackData _data;
    private PlayerController _controller;
    private int _index;
    private SpellInventory _inventory;
    public AttackData Data => _data;

    public void Initialize(AttackData data, SpellInventory inventory)
    {
        _data = data;
        _inventory = inventory;
        image.sprite = data.Icon;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                var spellBarIcon = result.gameObject.GetComponent<SpellBarIcon>();
                if (spellBarIcon != null)
                {
                    _inventory.EquipSpell(spellBarIcon.Index, _data);
                    this.enabled = false;
                    return;
                }
            }

        }
    }

    public void OnDisable()
    {
        image.color = disabledColor;
    }

    public void OnEnable()
    {
        image.color = Color.white;
    }
}
