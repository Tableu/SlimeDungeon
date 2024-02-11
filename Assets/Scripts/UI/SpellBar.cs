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
        var inputMap = playerController.PlayerInputActions.Spells.Get();
        for (int i = 0; i < ((PlayerData) playerController.CharacterData).MaxSpellCount; i++)
        {
            GameObject icon = Instantiate(spellIconPrefab, transform);
            var script = icon.GetComponent<SpellBarIcon>();
            if (script != null)
            {
                spellIcons.Add(script);
                script.Initialize(i, inputMap.actions[i].controls[0].name.ToUpper());
            }
        }

        playerController.OnAttackEquipped += OnAttackEquipped;
        playerController.OnAttackUnEquipped += OnAttackUnEquipped;
        playerController.InitializeAttacks();
    }

    private void OnAttackEquipped(Attack attack, int index)
    {
        spellIcons[index].EquipAttack(attack);
    }

    private void OnAttackUnEquipped(Attack attack, int index)
    {
        spellIcons[index].UnEquipAttack();
    }

    private void OnDestroy()
    {
        playerController.OnAttackEquipped -= OnAttackEquipped;
    }
}
