using TMPro;
using UnityEngine;

public class DamageNumberText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    
    public void Initialize(int damage, Color color)
    {
        text.text = damage.ToString();
        text.color = color;
    }

    private void Update()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a-0.01f);
        transform.position += Vector3.up * 0.5f*Time.deltaTime;
        if(text.color.a <= 0)
            Destroy(gameObject);
    }
}
