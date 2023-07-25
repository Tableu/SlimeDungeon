using Controller.Form;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Data/Enemy Data")]
public class EnemyData : CharacterData
{
    [HeaderAttribute("Enemy")] 
    [SerializeField] private float attackRange;
    [SerializeField] private float aggroRange;
    [SerializeField] private float deAggroRange;
    [SerializeField] private int detectTick;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private FormData formData;
    
    public float AttackRange => attackRange;
    public float StoppingDistance => stoppingDistance;
    public float AggroRange => aggroRange;
    public float DeAggroRange => deAggroRange;
    public int DetectTick => detectTick;
    public FormData FormData => formData;
}
