using System.Collections.Generic;
using Controller.Player;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsWidget : MonoBehaviour
{
    [SerializeField] private List<Slider> sliders;

    public void Refresh(PlayerCharacter character)
    {
        sliders[0].value = character.Stats.Health;
        sliders[1].value = character.Stats.Damage;
        sliders[2].value = character.Stats.Armor;
    }
}
