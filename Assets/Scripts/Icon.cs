using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    [SerializeField] private Image icon;
    void Start()
    {
        EmptyIcon();
    }

    public void EmptyIcon()
    {
        icon.enabled = false;
    }

    public void OnCooldown(float duration)
    {
        icon.fillAmount = duration;
    }

    public void SetIcon(Sprite sprite)
    {
        icon.enabled = true;
        icon.sprite = sprite;
    }
}
