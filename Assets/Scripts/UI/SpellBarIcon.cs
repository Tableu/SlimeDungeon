using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellBarIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private TextMeshProUGUI manaText;
    private int _index;
    private PlayerController _controller;

    public int Index => _index;
    private void Awake()
    {
        icon.enabled = false;
    }
    
    public void Initialize(int index, PlayerController controller)
    {
        _controller = controller;
        var inputMap = controller.PlayerInputActions.Spells.Get();
        keyText.text = inputMap.actions[index].controls[0].name.ToUpper();
        _index = index;
    }

    public void OnCooldown(float duration)
    {
        icon.fillAmount = duration;
    }

    public void SetIcon(Sprite sprite, float manaCost)
    {
        icon.enabled = true;
        icon.sprite = sprite;
        manaText.text = manaCost.ToString();
    }
}
