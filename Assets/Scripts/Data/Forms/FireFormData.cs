using UnityEngine;

namespace Controller.Form
{
    [CreateAssetMenu(fileName = "FireFormData", menuName = "Forms/FireForm")]
    public class FireFormData : FormData
    {
        public override FormAnimator AttachScript(GameObject gameObject)
        {
            SlimeFormAnimator formAnimator = gameObject.AddComponent<SlimeFormAnimator>();
            return formAnimator;
        }
    }
}