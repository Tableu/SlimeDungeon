using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        if(ResourceManager.Instance != null)
            text.text = ResourceManager.Instance.Coins.Value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(ResourceManager.Instance != null)
            text.text = ResourceManager.Instance.Coins.Value.ToString();
    }
}
