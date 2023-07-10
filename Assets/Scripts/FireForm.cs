using Controller;
using UnityEngine.UI;

public class FireForm : Form
{
    public float Temperature { get; private set; }

    private new FireFormData data;
    private Slider _slider;

    public override void Equip(Character character, FormData data)
    {
        this.data = data as FireFormData;
        var sliderObject = Instantiate(this.data.Slider, GlobalReferences.Instance.Canvas.gameObject.transform);
        _slider = sliderObject.GetComponent<Slider>();
        _slider.maxValue = this.data.MaxTemperature;
        this.character = character;
        foreach(Attack attack in data.Attacks)
        {
            attack.Equip(character);
            attack.OnSpellCast += IncreaseTemperature;
        }
        
        if (character is PlayerController player) 
            player.ChangeForms(this.data.Material);
    }

    public override void Drop()
    {
        Destroy(_slider.gameObject);
        foreach(Attack attack in data.Attacks)
        {
            attack.Drop();
            attack.OnSpellCast -= IncreaseTemperature;
        }

        if (character is PlayerController player) 
            player.ResetForm();
    }

    private void FixedUpdate()
    {
        DecreaseTemperature();
        if (_slider != null)
        {
            _slider.value = Temperature;
            if (Temperature > data.MaxTemperature / 2)
            {
                sizeMultiplier = 2;
            }
            else
            {
                sizeMultiplier = 1;
            }
        }
    }

    private void IncreaseTemperature()
    {
        if (data != null)
        {
            Temperature += data.IncreaseRate;
            if (Temperature > data.MaxTemperature)
            {
                Temperature = data.MaxTemperature;
            }
        }
    }

    private void DecreaseTemperature()
    {
        if (data != null && Temperature > 0)
        {
            Temperature -= data.DecreaseRate;
        }
    }
}