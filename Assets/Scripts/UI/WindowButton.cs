using UnityEngine;

public class WindowButton : MonoBehaviour
{
    [SerializeField] private GameObject window;

    public void OnClick()
    {
        if (window.activeSelf)
        {
            WindowManager.Instance.CloseWindow();
        }
        else
        {
            WindowManager.Instance.OpenWindow(window);
        }
    }
}
