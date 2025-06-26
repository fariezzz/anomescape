using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Search, SearchNoise }

    [Header("State")]
    public State currentState;

    [Header("Targets")]
    public Transform player;

    [Header("Detection")]
    public float patrolDetectRange = 10f;
    public float chaseDetectRange = 18f;
    public float patrolDetectAngle = 80f;
    public float chaseDetectAngle = 140f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolAngularSpeed = 180f;
    public float chaseAngularSpeed = 400f;
    public float patrolAccel = 5f;
    public float chaseAccel = 9f;

    [Header("Components")]
    public NavMeshAgent agent;
    public Animator animator;
    public EnemyVision enemyVision;

    [Header("Search")]
    public float searchDuration = 5f;
    [SerializeField] private float searchTimer;

    [Header("Noise")]
    private Vector3 lastHeardPosition;

    void Start()
    {
        agent.stoppingDistance = 0;
        SetState(State.Patrol);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
            case State.Search: Search(); break;
            case State.SearchNoise: SearchNoise(); break;
        }

        LookForPlayer();

        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
    }

    void LookForPlayer()
    {
        if (enemyVision.PlayerVisible && currentState != State.Chase)
        {
            SetState(State.Chase);
        }
    }

    void Patrol()
    {
        enemyVision.detectRange = patrolDetectRange;
        enemyVision.detectAngle = patrolDetectAngle;
        agent.speed = patrolSpeed;
        agent.angularSpeed = patrolAngularSpeed;
        agent.acceleration = patrolAccel;
        animator.SetBool("isRunning", false);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextWaypoint();
    }

    void Chase()
    {
        enemyVision.detectRange = chaseDetectRange;
        enemyVision.detectAngle = chaseDetectAngle;
        agent.speed = chaseSpeed;
        agent.angularSpeed = chaseAngularSpeed;
        agent.acceleration = chaseAccel;
        animator.SetBool("isRunning", true);

        agent.isStopped = false;
        agent.destination = player.position;

        Debug.Log(
        $"pathPending={agent.pathPending}, " +
        $"hasPath={agent.hasPath}, " +
        $"remainingDistance={agent.remainingDistance}, " +
        $"velocity={agent.velocity}"
    );

        if (enemyVision.PlayerFullySafe)
        {
            animator.SetBool("isRunning", false);
            SetState(State.Search);
            searchTimer = searchDuration;
        }
    }

    void Search()
    {
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0)
        {
            SetState(State.Patrol);
            GotoNextWaypoint();
        }
    }

    void SearchNoise()
    {
        enemyVision.detectRange = patrolDetectRange;
        enemyVision.detectAngle = patrolDetectAngle;
        agent.speed = patrolSpeed;
        agent.angularSpeed = patrolAngularSpeed;
        agent.acceleration = patrolAccel;
        animator.SetBool("isRunning", false);

        if (enemyVision.PlayerVisible)
        {
            SetState(State.Chase);
            return;
        }

        agent.destination = lastHeardPosition;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetState(State.Search);
            searchTimer = searchDuration;
        }
    }


    void GotoNextWaypoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * enemyVision.detectRange;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, enemyVision.detectRange, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (currentState == State.Chase) return;
        if (col.CompareTag("Player") && currentState != State.Chase)
        {
            Vector3 dirToPlayer = player.position - transform.position;
            dirToPlayer.y = 0;
            transform.rotation = Quaternion.LookRotation(dirToPlayer);
            SetState(State.Chase);
        }
    }

    void SetState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Patrol:
                AudioManager.Instance.TransitionBGM(
                AudioManager.Instance.chaseBGMSource,
                AudioManager.Instance.patrolBGMSource,
                1f);
            break;

            case State.Chase:
                AudioManager.Instance.TransitionBGM(
                AudioManager.Instance.patrolBGMSource,
                AudioManager.Instance.chaseBGMSource,
                1f);
            break;
        }
    }

    public void ReportNoise(Vector3 noisePosition)
    {
        if (currentState == State.Chase || currentState == State.SearchNoise || currentState == State.Search) return;

        float distance = Vector3.Distance(transform.position, noisePosition);
        if (distance <= patrolDetectRange)
        {
            SetState(State.SearchNoise);
            lastHeardPosition = noisePosition;
        }
    }

}
