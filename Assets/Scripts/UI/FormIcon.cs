using Controller.Form;
using UnityEngine;
using UnityEngine.UI;

public class FormIcon : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image image;

    private void Start()
    {
        playerController.FormManager.OnFormChange += OnFormChange;
    }

    private void OnFormChange()
    {
        image.sprite = playerController.FormManager.CurrentForm.Data.Icon;
    }

    private void OnDestroy()
    {
        playerController.FormManager.OnFormChange -= OnFormChange;
    }
}
