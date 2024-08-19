using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerController playerController;
    private void Start()
    {
        playerController.PlayerInputActions.Other.Escape.started += OnClick;
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Time.timeScale = !pauseMenu.activeSelf ? 0 : 1;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    public void CloseWindow()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    private void OnDestroy()
    {
        playerController.PlayerInputActions.Other.Escape.started -= OnClick;
    }
}
