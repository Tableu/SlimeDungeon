using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data")]
public class PlayerData : CharacterData
{
    [HeaderAttribute("Stats")] 
    [SerializeField] private Vector2 maxVelocity;

    public Vector2 MaxVelocity => maxVelocity;

}
