using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "SlimeFormData", menuName = "Data/Forms/SlimeForm")]
public class SlimeFormData : FormData
{
    public override Form AttachScript(GameObject gameObject)
    {
        SlimeForm form = gameObject.AddComponent<SlimeForm>();
        form.data = this;
        return form;
    }
}
