using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseFormData", menuName = "Data/Forms/BaseForm")]
public class BaseFormData : FormData
{
    public override Form AttachScript(GameObject gameObject)
    {
        BaseForm form = gameObject.AddComponent<BaseForm>();
        form.data = this;
        return form;
    }
}
