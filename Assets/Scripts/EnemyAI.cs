using UnityEngine;
using UnityEngine.AI; // untuk pathfinding

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Search }
    public State currentState = State.Patrol;

    public Transform player;
    public Transform[] waypoints;
    int currentWaypoint = 0;

    public float searchDuration = 5f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    public NavMeshAgent agent;
    public Animator animator;
    public EnemyVision enemyVision;
    float searchTimer;

    void Start()
    {
        GotoNextWaypoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Search:
                Search();
                break;
        }

        LookForPlayer();

        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
    }

    void LookForPlayer()
    {
        if (enemyVision.PlayerVisible)
        {
            currentState = State.Chase;
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
        animator.SetBool("isRunning", false);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GotoNextWaypoint();
        }
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        animator.SetBool("isRunning", true);

        agent.destination = player.position;

        if (enemyVision.PlayerFullySafe)
        {
            animator.SetBool("isRunning", false);
            currentState = State.Search;
            searchTimer = searchDuration;
        }
    }

    void Search()
    {
        searchTimer -= Time.deltaTime;

        if (searchTimer <= 0)
        {
            currentState = State.Patrol;
            GotoNextWaypoint();
        }
    }

    void GotoNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        agent.destination = waypoints[currentWaypoint].position;
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }
}
