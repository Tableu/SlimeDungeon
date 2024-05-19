using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellInfo : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    public void Initialize(AttackData data)
    {
        icon.sprite = data.Icon;
        text.text = data.Name;
    }
}
