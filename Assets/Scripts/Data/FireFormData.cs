using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "FireFormData", menuName = "Data/Forms/FireForm")]
public class FireFormData : FormData
{
    [SerializeField] private Material material;
    [SerializeField] private GameObject slider;
    [SerializeField] private float maxTemperature;
    [SerializeField] private float increaseRate;
    [SerializeField] private float decreaseRate;

    public Material Material => material;
    public GameObject Slider => slider;
    public float MaxTemperature => maxTemperature;
    public float IncreaseRate => increaseRate;
    public float DecreaseRate => decreaseRate;

    public override Form AttachScript(GameObject gameObject)
    {
        return gameObject.AddComponent<FireForm>();
    }
}
