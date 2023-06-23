using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public SlimeAnimationState CurrentState;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private GameObject smileBody;
    [SerializeField] private Face faces;
    private Material faceMaterial;
    private int currentWaypointIndex;
    
    private void Start()
    {
        faceMaterial = smileBody.GetComponent<Renderer>().materials[1];
    }

    // Update is called once per frame
    private void Update()
    {
        switch (CurrentState)
        {
            case SlimeAnimationState.Idle:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return;
                StopAgent();
                SetFace(faces.Idleface);
                break;
            case SlimeAnimationState.Walk:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) return;
                agent.isStopped = false;
                agent.updateRotation = true;
                if (waypoints[0] == null) return;
                   
                agent.SetDestination(waypoints[currentWaypointIndex].position);

                // agent reaches the destination
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    CurrentState = SlimeAnimationState.Idle;

                    //wait 2s before go to next destionation
                    Invoke(nameof(WalkToNextDestination), 2f);
                }
                animator.SetFloat("Speed", agent.velocity.magnitude);
                break;
            case SlimeAnimationState.Damage:
                if(animator.GetCurrentAnimatorStateInfo(0).IsName("Damage0")
                   || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage1")
                   || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage2") ) return;
                StopAgent();
                animator.SetTrigger("Damage");
                animator.SetInteger("DamageType", 0);
                SetFace(faces.damageFace);
                break;
            case SlimeAnimationState.Jump:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) return;
                StopAgent();
                SetFace(faces.jumpFace);
                animator.SetTrigger("Jump");
                break;
            case SlimeAnimationState.Attack:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
                StopAgent();
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
                break;
        }
    }
    private void StopAgent()
    {
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);
        agent.updateRotation = false;
    }
    private void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }
    private void WalkToNextDestination()
    {
        CurrentState = SlimeAnimationState.Walk;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        SetFace(faces.WalkFace);
    }
    void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = animator.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }
}
