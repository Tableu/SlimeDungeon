using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeButtonTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highlightColor;

    public void OnPointerDown(PointerEventData eventData)
    {
        text.color = defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = defaultColor;
    }
}
