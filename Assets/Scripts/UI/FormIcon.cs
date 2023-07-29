using UnityEngine;
using UnityEngine.UI;

public class FormIcon : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image image;

    private void Start()
    {
        playerController.OnFormChange += OnFormChange;
    }

    private void OnFormChange()
    {
        image.sprite = playerController.CurrentForm.data.Icon;
    }

    private void OnDestroy()
    {
        playerController.OnFormChange -= OnFormChange;
    }
}
