
using UnityEngine;
using UnityEngine.UI;

public class StatSlider : MonoBehaviour
{
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;

    public void HideElements(float value)
    {
        left.SetActive(value != 0);
        right.SetActive(value != 0);
    }
}
