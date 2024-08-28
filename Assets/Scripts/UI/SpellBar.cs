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
            OnCharacterChanged(partyController.CurrentCharacter);
        };
        OnCharacterChanged(partyController.CurrentCharacter);
    }

    private void OnCharacterChanged(Character character)
    {
        spellIcon.UnEquipAttack();
        spellIcon.EquipAttack(character.Spell);
    }
}
