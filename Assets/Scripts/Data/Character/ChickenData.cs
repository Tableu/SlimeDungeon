using Controller.Character;
using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "ChickenData", menuName = "Data/Characters/Chicken")]
public class ChickenData : CharacterData
{
    public override CharacterAnimator AttachScript(GameObject gameObject)
    {
        ChickenAnimator characterAnimator = gameObject.AddComponent<ChickenAnimator>();
        return characterAnimator;
    }
}
