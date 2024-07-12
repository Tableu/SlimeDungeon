using TMPro;
using UnityEngine;

public class Chatbox : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshPro text;

    public void Initialize()
    {
        canvas.worldCamera = Camera.main;
    }

    public void SetMessage(string message)
    {
        text.text = message;
    }

    public void SetIcon(string icon)
    {
        
    }
}
