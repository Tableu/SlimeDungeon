using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    private void Start()
    {
        exitButton.onClick.AddListener(Exit);
    }

    private void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Scenes/Title Screen");
    }

    public void CloseWindow()
    {
        WindowManager.Instance.CloseWindow();
    }

    private void OnDestroy()
    {
        exitButton.onClick.RemoveListener(Exit);
    }
}
