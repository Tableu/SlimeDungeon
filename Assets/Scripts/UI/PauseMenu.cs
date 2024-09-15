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
        playerController.PlayerInputActions.UI.Escape.started += OnClick;
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
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        if (pauseMenu.activeSelf)
            WindowManager.Instance.OnPopupOpened();
        else
            WindowManager.Instance.OnPopupClosed();
        
    }
    

    private void OnDestroy()
    {
        playerController.PlayerInputActions.UI.Escape.started -= OnClick;
        exitButton.onClick.RemoveListener(Exit);
    }
}
