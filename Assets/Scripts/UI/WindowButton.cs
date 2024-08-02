using UnityEngine;

public class WindowButton : MonoBehaviour
{
    [SerializeField] private GameObject window;

    public void OnClick()
    {
        window.SetActive(!window.activeSelf);
    }
}
