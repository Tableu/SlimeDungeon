using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellInventory : MonoBehaviour
{
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject spellInventoryIconPrefab;
    [SerializeField] private PlayerController controller;
    private List<SpellInventoryIcon> inventoryIconList;
    private void Start()
    {
        inventoryIconList = new List<SpellInventoryIcon>();
        foreach (var attackData in controller.UnlockedAttacks)
        {
            AddSpell(attackData);
        }

        foreach (var icon in inventoryIconList)
        {
            icon.enabled = false;
        }
        
        controller.OnAttackUnEquip += OnAttackUnEquip;
    }

    public void AddSpell(AttackData data)
    {
        GameObject spellIcon = Instantiate(spellInventoryIconPrefab, grid.transform);
        var script = spellIcon.GetComponent<SpellInventoryIcon>();
        script.Initialize(data, this);
        inventoryIconList.Add(script);
    }

    public void EquipSpell(int index, AttackData data)
    {
        controller.EquipAttack(data, index);
    }

    private void OnAttackUnEquip(AttackData data)
    {
        foreach (var icon in inventoryIconList)
        {
            if (icon.Data == data)
            {
                icon.enabled = true;
            }
        }
    }

    private void OnDestroy()
    {
        controller.OnAttackUnEquip -= OnAttackUnEquip;
    }
}
