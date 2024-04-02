using Controller.Form;

public class FormItem : Item
{
    private FormData _data;
    private Form _form;
    public void Initialize(FormData data)
    {
        _data = data;
    }

    public void Initialize(Form form)
    {
        _form = form;
        _data = _form.Data;
    }

    public override void PickUp(PlayerController character)
    {
        character.FormManager.AddForm(_form != null ? _form : new Form(_data, character));
        Destroy(gameObject);
    }
}
