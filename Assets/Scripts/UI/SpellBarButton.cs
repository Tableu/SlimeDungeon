using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBarButton : MonoBehaviour
{
    [SerializeField] private GameObject spellInventory;

    public void OnClick()
    {
        spellInventory.SetActive(!spellInventory.activeSelf);
    }
}
