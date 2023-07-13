using UnityEngine.UI;

namespace Controller.Form
{
    public class FireForm : Form
    {
        public float Temperature { get; private set; }

        internal new FireFormData data;
        private Slider _slider;

        public override void Equip(Character character)
        {
            var sliderObject = Instantiate(this.data.Slider, GlobalReferences.Instance.Canvas.gameObject.transform);
            _slider = sliderObject.GetComponent<Slider>();
            _slider.maxValue = this.data.MaxTemperature;
            this.character = character;
            foreach (AttackData attackData in data.Attacks)
            {
                Attack attack = attackData.EquipAttack(character);
                attack.OnSpellCast += IncreaseTemperature;
            }
            if (character is PlayerController player)
                player.ChangeForms(this.data.Material);
        }

        public override void Drop()
        {
            Destroy(_slider.gameObject);

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
                    damageMultiplier = 2;
                }
                else
                {
                    sizeMultiplier = 1;
                    damageMultiplier = 1;
                }
            }
        }

        private void IncreaseTemperature()
        {
            Temperature += data.IncreaseRate;
            if (Temperature > data.MaxTemperature)
            {
                Temperature = data.MaxTemperature;
            }
        }

        private void DecreaseTemperature()
        {
            if (Temperature > 0)
            {
                Temperature -= data.DecreaseRate;
            }
        }
    }
}