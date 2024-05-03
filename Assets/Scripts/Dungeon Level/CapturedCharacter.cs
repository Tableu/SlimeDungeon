using Controller.Form;
using UnityEngine;

public class CapturedCharacter : MonoBehaviour
{
    [SerializeField] private GameObject cage;
    [SerializeField] private GameObject character;
    [SerializeField] private Collider characterCollider;
    private RoomController _roomController;
    private Form _form;

    public Form Form => _form;
    
    public void Initialize(RoomController roomController, FormData formData)
    {
        _roomController = roomController;
        _roomController.OnAllEnemiesDead += FreeCharacter;
        _form = new Form(formData);
        characterCollider.enabled = false;
    }

    public void SwitchCharacter(Form newCharacter)
    {
        if (newCharacter != null)
        {
            _form = newCharacter;
            Destroy(character.gameObject);
            character = Instantiate(_form.Data.Model, transform);
            character.layer = LayerMask.NameToLayer("Items");
        }
    }

    private void FreeCharacter()
    {
        cage.SetActive(false);
        characterCollider.enabled = true;
    }

    private void OnDestroy()
    {
        _roomController.OnAllEnemiesDead -= FreeCharacter;
    }
}
