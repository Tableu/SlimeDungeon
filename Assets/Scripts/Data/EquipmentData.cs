using System;
using System.Collections.Generic;
using Controller.Player;
using UnityEngine;
using Type = Elements.Type;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "Data/Equipment")]
[Serializable]
public class EquipmentData : ScriptableObject
{
    [Serializable]
    public enum EffectType
    {
        Armor,
        Damage,
        Speed
    }

    [Serializable]
    public struct Effect
    {
        public EffectType Type;
        public Type Element;
        public float Value;

        public Effect(EffectType type, Type element, float value)
        {
            Type = type;
            Element = element;
            Value = value;
        }
    }

    [SerializeField] private List<Effect> buffs;
    [SerializeField] private GameObject model;
    [SerializeField] private new string name;
    [SerializeField] private int cost;

    public GameObject Model => model;
    public List<Effect> Buffs => buffs;
    public string Name => name;
    public int Cost => cost;

    public void Equip(PlayerCharacter playerCharacter)
    {
        foreach (Effect buff in buffs)
        {
            switch (buff.Type)
            {
                case EffectType.Armor:
                    playerCharacter.Stats.Defense.BaseModifier += buff.Value;
                    break;
                case EffectType.Damage:
                    playerCharacter.Stats.Attack.BaseModifier += buff.Value;
                    break;
                case EffectType.Speed:
                    playerCharacter.Stats.Speed.BaseModifier += buff.Value;
                    break;
            }
        }
    }

    public void Drop(PlayerCharacter playerCharacter)
    {
        foreach (Effect buff in buffs)
        {
            switch (buff.Type)
            {
                case EffectType.Armor:
                    playerCharacter.Stats.Defense.BaseModifier -= buff.Value;
                    break;
                case EffectType.Damage:
                    playerCharacter.Stats.Attack.BaseModifier -= buff.Value;
                    break;
                case EffectType.Speed:
                    playerCharacter.Stats.Speed.BaseModifier -= buff.Value;
                    break;
            }
        }
    }
}
