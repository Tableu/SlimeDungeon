using System.Collections.Generic;
using Controller;
using UnityEngine;

public class SpellBar : MonoBehaviour
{
    [SerializeField] private GameObject spellIconPrefab;
    [SerializeField] private PlayerController playerController;
    private List<SpellBarIcon> spellIcons;
    
    private void Start()
    {
        spellIcons = new List<SpellBarIcon>();
        var i = 0;
        foreach (Attack attack in playerController.attacks)
        {
            GameObject icon = Instantiate(spellIconPrefab, transform);
            var script = icon.GetComponent<SpellBarIcon>();
            if (script != null)
            {
                spellIcons.Add(script);
                attack.OnCooldown += script.OnCooldown;
                script.SetIcon(playerController.CharacterData.Attacks[i].Icon);
                script.Initialize(i);
            }

            i++;
        }
        
        playerController.OnAttackEquip += OnAttackEquip;
    }

    private void OnAttackEquip(AttackData attackData, int index)
    {
        playerController.attacks[index].OnCooldown += spellIcons[index].OnCooldown;
        spellIcons[index].SetIcon(attackData.Icon);
    }

    private void OnDestroy()
    {
        var i = 0;
        foreach (Attack attack in playerController.attacks)
        {
            attack.OnCooldown -= spellIcons[i].OnCooldown;
            i++;
        }
        playerController.OnAttackEquip -= OnAttackEquip;
    }
}
