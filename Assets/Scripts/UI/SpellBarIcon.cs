using UnityEngine;
using UnityEngine.UI;

public class SpellBarIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    private int _index;

    public int Index => _index;
    private void Awake()
    {
        icon.enabled = false;
    }
    
    public void Initialize(int index)
    {
        _index = index;
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
