using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "SlimeFormData", menuName = "Data/Forms/SlimeForm")]
public class SlimeFormData : FormData
{
    public override FormAnimator AttachScript(GameObject gameObject)
    {
        SlimeFormAnimator formAnimator = gameObject.AddComponent<SlimeFormAnimator>();
        return formAnimator;
    }
}
