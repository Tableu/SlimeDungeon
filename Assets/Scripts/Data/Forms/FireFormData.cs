using UnityEngine;

namespace Controller.Form
{
    [CreateAssetMenu(fileName = "FireFormData", menuName = "Forms/FireForm")]
    public class FireFormData : FormData
    {
        [SerializeField] private GameObject slider;
        [SerializeField] private float maxTemperature;
        [SerializeField] private float increaseRate;
        [SerializeField] private float decreaseRate;

        public GameObject Slider => slider;
        public float MaxTemperature => maxTemperature;
        public float IncreaseRate => increaseRate;
        public float DecreaseRate => decreaseRate;

        public override FormAnimator AttachScript(GameObject gameObject)
        {
            FireFormAnimator formAnimator = gameObject.AddComponent<FireFormAnimator>();
            return formAnimator;
        }
    }
}