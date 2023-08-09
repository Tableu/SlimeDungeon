using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "ChickenFormData", menuName = "Data/Forms/ChickenForm")]
public class ChickenFormData : FormData
{
    public override FormAnimator AttachScript(GameObject gameObject)
    {
        ChickenFormAnimator formAnimator = gameObject.AddComponent<ChickenFormAnimator>();
        return formAnimator;
    }
}
