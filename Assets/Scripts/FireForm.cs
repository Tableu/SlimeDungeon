using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "FireForm", menuName = "Forms/Fire Form")]
public class FireForm : Form
{
    public float Temperature { get; private set; }

    public override void Equip(Character character)
    {
        this.character = character;
        foreach(Attack attack in attacks)
        {
            attack.Equip(character);
            attack.OnSpellCast += IncreaseTemperature;
        }
    }

    public override void Drop()
    {
        foreach(Attack attack in attacks)
        {
            attack.Drop();
            attack.OnSpellCast -= IncreaseTemperature;
        }
    }

    private void IncreaseTemperature()
    {
        Temperature += 10;
    }

    private void DecreaseTemperature()
    {
        Temperature--;
    }
    
}
