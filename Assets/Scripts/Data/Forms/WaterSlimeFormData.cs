using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterSlimeFormData", menuName = "Forms/WaterSlimeForm")]
public class WaterSlimeFormData : FormData
{
    public override FormAnimator AttachScript(GameObject gameObject)
    {
        var formAnimator = gameObject.AddComponent<SlimeFormAnimator>();
        return formAnimator;
    }
}
