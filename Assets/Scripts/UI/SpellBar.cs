using Controller.Player;
using UnityEngine;

public class SpellBar : MonoBehaviour
{
    [SerializeField] private bool raycastTarget;
    [SerializeField] private PartyController partyController;
    [SerializeField] private SpellBarIcon spellIcon;
    
    private void Start()
    {
        spellIcon.Initialize("mouse_right", raycastTarget);
        partyController.OnCharacterChanged += OnCharacterChanged;
        partyController.OnSpellEquipped += delegate(AttackData data)
        {
            OnCharacterChanged(partyController.CurrentPlayerCharacter);
        };
        partyController.OnSpellUnEquipped += delegate(AttackData data)
        {
            OnCharacterChanged(partyController.CurrentPlayerCharacter);
        };
        OnCharacterChanged(partyController.CurrentPlayerCharacter);
    }

    private void OnCharacterChanged(PlayerCharacter playerCharacter)
    {
        spellIcon.UnEquipAttack();
        if(playerCharacter.Spell != null)
            spellIcon.EquipAttack(playerCharacter.Spell);
    }
}
