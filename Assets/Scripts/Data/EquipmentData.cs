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

    public GameObject Model => model;
    public List<Effect> Buffs => buffs;
    public string Name => name;

    public void Equip(Character character)
    {
        foreach (Effect buff in buffs)
        {
            switch (buff.Type)
            {
                case EffectType.Armor:
                    break;
                case EffectType.Damage:
                    break;
                case EffectType.Speed:
                    character.Speed.BaseModifier += buff.Value;
                    break;
            }
        }
    }

    public void Drop(Character character)
    {
        foreach (Effect buff in buffs)
        {
            switch (buff.Type)
            {
                case EffectType.Armor:
                    break;
                case EffectType.Damage:
                    break;
                case EffectType.Speed:
                    character.Speed.BaseModifier -= buff.Value;
                    break;
            }
        }
    }
}
