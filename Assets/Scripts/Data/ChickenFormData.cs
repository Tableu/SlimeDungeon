using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "ChickenFormData", menuName = "Data/Forms/ChickenForm")]
public class ChickenFormData : FormData
{
    public override Form AttachScript(GameObject gameObject)
    {
        ChickenForm form = gameObject.AddComponent<ChickenForm>();
        form.data = this;
        return form;
    }
}
