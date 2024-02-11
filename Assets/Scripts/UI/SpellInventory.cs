using System;
using System.Collections.Generic;
using Controller;
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
        
        controller.OnAttackUnEquipped += OnAttackUnEquip;
        controller.OnAttackUnlocked += AddSpell;
        gameObject.SetActive(false);
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

    private void OnAttackUnEquip(Attack attack, int index)
    {
        //todo refactor to handle duplicates
        foreach (var icon in inventoryIconList)
        {
            if (icon.Data == attack.Data)
            {
                icon.enabled = true;
            }
        }
    }

    private void OnDestroy()
    {
        controller.OnAttackUnEquipped -= OnAttackUnEquip;
        controller.OnAttackUnlocked -= AddSpell;
    }
}
