using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Search }
    public State currentState = State.Patrol;

    public Transform player;
    public Transform[] waypoints;
    int currentWaypoint = 0;

    public float searchDuration = 5f;

    public float patrolDetectRange = 10f;
    public float chaseDetectRange = 18f;

    public float patrolDetectAngle = 80f;
    public float chaseDetectAngle = 140f;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    public float patrolAngularSpeed = 180f;
    public float chaseAngularSpeed = 400f;

    public float patrolAccel = 5f;
    public float chaseAccel = 9f;

    public NavMeshAgent agent;
    public Animator animator;
    public EnemyVision enemyVision;

    float searchTimer;

    void Start() => SetState(State.Patrol);

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
            case State.Search: Search(); break;
        }

        LookForPlayer();

        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
    }

    void LookForPlayer()
    {
        if (enemyVision.PlayerVisible && currentState != State.Chase)
        {
            SetState(State.Chase); // ganti state
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

        agent.destination = player.position;

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

    void GotoNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        agent.destination = waypoints[currentWaypoint].position;
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && currentState != State.Chase)
        {
            Debug.Log("Player touched enemy");
            Vector3 dirToPlayer = player.position - transform.position;
            dirToPlayer.y = 0;
            transform.rotation = Quaternion.LookRotation(dirToPlayer);
            SetState(State.Chase); // masuk chase
        }
    }

    void SetState(State newState)
    {
        currentState = newState;

        // Atur BGM di sini
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
}
