using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
///     A Canvas UI item that can be dragged (after clicking and holding)
/// </summary>
public class DraggableItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent ItemSelected;
    public UnityEvent ItemReleased;
    public RectTransform RectTransform;
    public string ItemName;
    public float minimumDistanceForDrag;
    public bool UseOffset;
    private bool holding;
    private bool dragging;
    private Vector2 originalPos;
    private Vector2 offset;
    private Vector2 oldPivot;

    private void Update()
    {
        if (holding)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue()/GlobalReferences.Instance.Canvas.scaleFactor;
            var dist = Vector2.Distance(originalPos, mousePos + offset);
            if (dragging || dist >= minimumDistanceForDrag)
            {
                RectTransform.anchoredPosition = mousePos + offset;
                dragging = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            holding = true;
            dragging = false;
            originalPos = RectTransform.anchoredPosition;
            if (UseOffset)
            {
                offset = originalPos - (Vector2) (Mouse.current.position.ReadValue()/GlobalReferences.Instance.Canvas.scaleFactor);
            }

            ItemSelected.Invoke();
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ItemReleased.Invoke();
            dragging = false;
            holding = false;
        }
    }

    public void ReturnToOriginalPosition()
    {
        RectTransform.anchoredPosition = originalPos;
    }
}
