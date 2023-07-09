using Controller;
public class FireForm : Form
{
    public float Temperature { get; private set; }

    private new FireFormData _data;

    public override void Equip(Character character, FormData data)
    {
        _data = data as FireFormData;
        _character = character;
        foreach(Attack attack in data.Attacks)
        {
            attack.Equip(character);
            attack.OnSpellCast += IncreaseTemperature;
        }
        
        if (character is PlayerController player) 
            player.ChangeForms(_data.Material);
    }

    public override void Drop()
    {
        foreach(Attack attack in _data.Attacks)
        {
            attack.Drop();
            attack.OnSpellCast -= IncreaseTemperature;
        }

        if (_character is PlayerController player) 
            player.ResetForm();
    }

    private void FixedUpdate()
    {
        DecreaseTemperature();
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