using Controller.Form;
using UnityEngine;

public class CapturedCharacter : MonoBehaviour
{
    [SerializeField] private GameObject cage;
    private RoomController _roomController;
    private FormData _formData;
    
    public void Initialize(RoomController roomController, FormData formData)
    {
        _roomController = roomController;
        _roomController.OnAllEnemiesDead += FreeCharacter;
        _formData = formData;
    }

    private void FreeCharacter()
    {
        cage.SetActive(false);
    }

    private void OnDestroy()
    {
        _roomController.OnAllEnemiesDead -= FreeCharacter;
    }
}
