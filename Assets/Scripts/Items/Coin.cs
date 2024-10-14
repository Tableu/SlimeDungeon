using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    private void OnCollisionEnter(Collision other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            ResourceManager.Instance.Coins.Add(value);
            Destroy(gameObject);
        }
    }
}
