using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Button exitButton;
    [SerializeField] private Crosshair crosshair;
        private void Start()
    {
        playerController.PlayerInputActions.Other.Escape.started += OnClick;
        exitButton.onClick.AddListener(Exit);
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    private void Exit()
    {
        levelManager.LoadTitleScreen();
    }

    public void CloseWindow()
    {
        TogglePause();
    }

    private void TogglePause()
    {
        Time.timeScale = !pauseMenu.activeSelf ? 0 : 1;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        playerController.enabled = !pauseMenu.activeSelf;
        if (pauseMenu.activeSelf)
        {
            crosshair.Hide();
        }
        else
        {
            crosshair.Show();
        }
    }
    

    private void OnDestroy()
    {
        playerController.PlayerInputActions.Other.Escape.started -= OnClick;
        exitButton.onClick.RemoveListener(Exit);
    }
}
