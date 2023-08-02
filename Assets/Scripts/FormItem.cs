using Controller.Form;

public class FormItem : Item
{
    private FormData _data;
    public void Initialize(FormData data)
    {
        _data = data;
    }

    public override void PickUp(PlayerController playerController)
    {
        playerController.FormManager.AddForm(_data);
        Destroy(gameObject);
    }
}
