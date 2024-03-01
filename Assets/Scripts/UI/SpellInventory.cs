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

        controller.OnAttackEquipped += OnSpellEquipped;
        controller.OnAttackUnEquipped += OnSpellUnEquip;
        controller.OnAttackUnlocked += AddSpell;
        controller.OnAttackRemoved += RemoveSpell;
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

    public void RemoveSpell(AttackData data)
    {
        int index = inventoryIconList.FindIndex(icon => icon.Data == data);
        if (index != -1)
        {
            Destroy(inventoryIconList[index].gameObject);
            inventoryIconList.RemoveAt(index);
        }
    }

    private void OnSpellEquipped(Attack attack, int index)
    {
        int iconIndex = inventoryIconList.FindIndex(icon => icon.Data == attack.Data);
        inventoryIconList[iconIndex].enabled = false;
    }

    private void OnSpellUnEquip(Attack attack, int index)
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
        controller.OnAttackUnEquipped -= OnSpellUnEquip;
        controller.OnAttackUnlocked -= AddSpell;
        controller.OnAttackRemoved -= RemoveSpell;
    }
}
