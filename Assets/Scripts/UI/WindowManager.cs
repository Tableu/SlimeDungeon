using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    private static WindowManager _instance;
    public static WindowManager Instance => _instance;

    [SerializeField] private PlayerCursor cursor;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject popupBlocker;

    private List<GameObject> _windows = new List<GameObject>();
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

    public void OnWindowChanged()
    {
        foreach (GameObject window in _windows)
        {
            if (window.activeSelf)
            {
                cursor.SwitchToCursor();
                playerController.enabled = false;
                return;
            }
        }
        cursor.SwitchToCrossHair();
        playerController.enabled = true;
    }

    public void OnPopupOpened()
    {
        cursor.SwitchToCursor();
        Time.timeScale = 0;
        playerController.enabled = false;
        popupBlocker.SetActive(true);
    }

    public void OnPopupClosed()
    {
        Time.timeScale = 1;
        OnWindowChanged();
        popupBlocker.SetActive(false);
    }

    public void RegisterWindow(GameObject window)
    {
        _windows.Add(window);
    }

    public void UnRegisterWindow(GameObject window)
    {
        _windows.Remove(window);
    }

    public void CloseAllWindows()
    {
        foreach (GameObject window in _windows)
        {
            window.SetActive(false);
        }
    }
}