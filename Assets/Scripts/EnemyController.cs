using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public SlimeAnimationState CurrentState;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private GameObject smileBody;
    [SerializeField] private Face faces;
    private Material _faceMaterial;
    private int _currentWaypointIndex;
    private bool _attackingPlayer = false;
    private Transform _target;
    
    private void Start()
    {
        _faceMaterial = smileBody.GetComponent<Renderer>().materials[1];
        _target = waypoints[0];
    }

    // Update is called once per frame
    private void Update()
    {
        DetectPlayer();
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
                   
                agent.SetDestination(_target.position);

                // agent reaches the destination
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    if (_attackingPlayer)
                    {
                        CurrentState = SlimeAnimationState.Attack;
                    }
                    else
                    {
                        CurrentState = SlimeAnimationState.Idle;
                        //wait 2s before go to next destionation
                        Invoke(nameof(WalkToNextDestination), 2f);
                    }
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
        _faceMaterial.SetTexture("_MainTex", tex);
    }
    private void WalkToNextDestination()
    {
        CurrentState = SlimeAnimationState.Walk;
        _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        _target = waypoints[_currentWaypointIndex];
        agent.SetDestination(_target.position);
        SetFace(faces.WalkFace);
    }

    private void DetectPlayer()
    {
        if (GlobalReferences.Instance.Player != null)
        {
            var diff = transform.position - GlobalReferences.Instance.Player.transform.position;
            if (diff.magnitude < 10)
            {
                _target = GlobalReferences.Instance.Player.transform;
                _attackingPlayer = true;
            }
            else
            {
                _target = waypoints[_currentWaypointIndex];
                _attackingPlayer = false;
            }
        }
    }
    // Animation Event
    public void AlertObservers(string message)
    {
        if (message.Equals("AnimationDamageEnded"))
        {
            // When Animation ended check distance between current position and first position 
            //if it > 1 AI will back to first position 

            CurrentState = SlimeAnimationState.Idle;

            //Debug.Log("DamageAnimationEnded");
        }

        if (message.Equals("AnimationAttackEnded"))
        {
            CurrentState = SlimeAnimationState.Walk;           
        }

        if (message.Equals("AnimationJumpEnded"))
        {
            CurrentState = SlimeAnimationState.Idle;
        }
    }
    public void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = animator.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }
}
