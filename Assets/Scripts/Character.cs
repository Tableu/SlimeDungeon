using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public float Speed;
    public float Health;
    public float Armor;
    [SerializeField] protected Animator animator;
    public Animator Animator => animator;
}
