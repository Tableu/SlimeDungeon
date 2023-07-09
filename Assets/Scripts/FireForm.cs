using Controller;
using UnityEngine.UI;

public class FireForm : Form
{
    public float Temperature { get; private set; }

    private new FireFormData _data;
    private Slider _slider;

    public override void Equip(Character character, FormData data)
    {
        _data = data as FireFormData;
        var sliderObject = Instantiate(_data.Slider, GlobalReferences.Instance.Canvas.gameObject.transform);
        _slider = sliderObject.GetComponent<Slider>();
        _slider.maxValue = _data.MaxTemperature;
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
        Destroy(_slider.gameObject);
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
        if (_slider != null)
        {
            _slider.value = Temperature;
        }
    }

    private void IncreaseTemperature()
    {
        if (_data != null)
        {
            Temperature += _data.IncreaseRate;
            if (Temperature > _data.MaxTemperature)
            {
                Temperature = _data.MaxTemperature;
            }
        }
    }

    private void DecreaseTemperature()
    {
        if (_data != null && Temperature > 0)
        {
            Temperature -= _data.DecreaseRate;
        }
    }
}