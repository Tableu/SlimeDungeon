using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "FireForm", menuName = "Forms/Fire Form")]
public class FireForm : Form
{
    [SerializeField] private Material material;
    public float Temperature { get; private set; }

    public override void Equip(Character character)
    {
        this.character = character;
        foreach(Attack attack in attacks)
        {
            attack.Equip(character);
            attack.OnSpellCast += IncreaseTemperature;
        }

        if (character is PlayerController player) 
            player.ChangeForms(material);
    }

    public override void Drop()
    {
        foreach(Attack attack in attacks)
        {
            attack.Drop();
            attack.OnSpellCast -= IncreaseTemperature;
        }

        if (character is PlayerController player) 
            player.ResetForm();
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
