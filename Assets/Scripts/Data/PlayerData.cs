using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data")]
public class PlayerData : CharacterData
{
    [SerializeField] private Vector2 maxVelocity;
    [SerializeField] private FormData baseForm;
    [SerializeField] private int maxFormCount;

    public Vector2 MaxVelocity => maxVelocity;
    public FormData BaseForm => baseForm;
    public int MaxFormCount => maxFormCount;

}
