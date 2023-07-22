using UnityEngine.UI;

namespace Controller.Form
{
    public class FireForm : Form
    {
        public float Temperature { get; private set; }

        internal new FireFormData data;
        private Slider _slider;

        public override void Equip(PlayerController playerController)
        {
            var sliderObject = Instantiate(data.Slider, GlobalReferences.Instance.Canvas.gameObject.transform);
            _slider = sliderObject.GetComponent<Slider>();
            _slider.maxValue = data.MaxTemperature;
            this.playerController = playerController;
            foreach (Attack attack in playerController.attacks)
            {
                attack.OnSpellCast += IncreaseTemperature;
            }
            playerController.elementType = data.ElementType;
            playerController.ChangeModel(data);
        }

        public override void Drop()
        {
            Destroy(_slider.gameObject);
            foreach (Attack attack in playerController.attacks)
            {
                attack.OnSpellCast -= IncreaseTemperature;
            }
            playerController.DropForm();
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