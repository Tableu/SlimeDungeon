using UnityEngine;
using UnityEngine.InputSystem;

public class WindowManager : MonoBehaviour
{
    private static WindowManager _instance;
    public static WindowManager Instance => _instance;

    [SerializeField] private PlayerCursor cursor;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject popupBlocker;
    [SerializeField] private GameObject pauseMenu;

    private GameObject _window;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        if(playerController != null)
            playerController.PlayerInputActions.UI.Escape.started += OnEscape;
    }

    private void OnDestroy()
    {
        if(playerController != null)
            playerController.PlayerInputActions.UI.Escape.started -= OnEscape;
    }

    public void OpenWindow(GameObject window, bool popup = false)
    {
        _window = window;
        _window.SetActive(true);
        cursor.SwitchToCursor();
        if(playerController != null)
            playerController.enabled = false;
        Time.timeScale = popup ? 0 : 1;
        popupBlocker.SetActive(popup);
    }

    public void CloseWindow()
    {
        if (_window == null)
            return;
        _window.SetActive(false);
        _window = null;
        cursor.SwitchToCrossHair();
        if(playerController != null)
            playerController.enabled = true;
        Time.timeScale = 1;
        popupBlocker.SetActive(false);
    }

    private void OnEscape(InputAction.CallbackContext callbackContext)
    {
        if (_window != null)
        {
            CloseWindow();
        }
        else
        {
            OpenWindow(pauseMenu, true);
        }
    }
}