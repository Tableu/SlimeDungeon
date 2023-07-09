using Controller;
using UnityEngine;

[CreateAssetMenu(fileName = "FireFormData", menuName = "Data/Forms/FireForm")]
public class FireFormData : FormData
{
    [SerializeField] private Material material;

    public Material Material => material;

    public override Form AttachScript(GameObject gameObject)
    {
        return gameObject.AddComponent<FireForm>();
    }
}
