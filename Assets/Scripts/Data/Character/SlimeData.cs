using Controller.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "SlimeData", menuName = "Data/Characters/Slime")]
public class SlimeData : CharacterData
{
    public override CharacterAnimator AttachScript(GameObject gameObject)
    {
        SlimeAnimator characterAnimator = gameObject.AddComponent<SlimeAnimator>();
        return characterAnimator;
    }
}
