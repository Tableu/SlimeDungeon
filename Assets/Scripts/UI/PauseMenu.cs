using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Button exitButton;
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
        Time.timeScale = 1;
        SceneManager.LoadScene("Scenes/Title Screen");
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
            if(PlayerCursor.Instance != null)
                PlayerCursor.Instance.SwitchToCursor();
        }
        else
        {
            if(PlayerCursor.Instance != null)
                PlayerCursor.Instance.SwitchToCrossHair();
        }
    }
    

    private void OnDestroy()
    {
        playerController.PlayerInputActions.Other.Escape.started -= OnClick;
        exitButton.onClick.RemoveListener(Exit);
    }
}
