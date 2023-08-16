using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "ChickenFormData", menuName = "Forms/ChickenForm")]
public class ChickenFormData : FormData
{
    public override FormAnimator AttachScript(GameObject gameObject)
    {
        ChickenFormAnimator formAnimator = gameObject.AddComponent<ChickenFormAnimator>();
        return formAnimator;
    }
}
