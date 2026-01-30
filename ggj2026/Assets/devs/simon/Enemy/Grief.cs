using UnityEngine;
using UnityEngine.AI;

public class Grief : EnemyBase
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Vision")]
    public float viewDistance = 12f;
    public float viewHeightOffset = 1.2f;
    public float sphereCastRadius = 0.3f;

    [Header("Attack")]
    public float attackRange = 1.8f;
    public float attackCooldown = 1.2f;
    public float firstAttackWindup = 2.0f; // NEW
    public int attackDamage = 15;

    private NavMeshAgent agent;
    private Transform player;
    private Collider playerCollider;
    private float attackTimer;

    private bool hasLoggedNavmeshError = false;
    private bool hasLoggedLostSight = false;

    private bool firstAttackPending = true; // NEW
    private float windupTimer = 0f;          // NEW

    protected new void Start()
    {
        base.Start();

        Debug.Log("Grief initialized");

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Grief ERROR: No NavMeshAgent found!");
            return;
        }

        agent.speed = moveSpeed;
        agent.updateRotation = true;
        agent.updatePosition = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerCollider = playerObj.GetComponent<Collider>();
            Debug.Log("Player found: " + playerObj.name);
        }
        else
        {
            Debug.LogError("Grief ERROR: No Player found with tag 'Player'");
        }
    }

    protected new void Update()
    {
        base.Update();

        if (player == null || agent == null)
            return;

        if (!agent.isOnNavMesh)
        {
            if (!hasLoggedNavmeshError)
            {
                Debug.LogError("Grief ERROR: Enemy is NOT on NavMesh!");
                hasLoggedNavmeshError = true;
            }
            return;
        }

        attackTimer -= Time.deltaTime;

        bool seesPlayer = CanSeePlayer();

        if (seesPlayer)
        {
            hasLoggedLostSight = false;

            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > attackRange)
                ChasePlayer();
            else
                Attack();
        }
        else
        {
            StopMoving();
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void StopMoving()
    {
        if (!agent.isStopped && !hasLoggedLostSight)
        {
            Debug.Log("Grief lost sight of player");
            hasLoggedLostSight = true;
        }

        agent.isStopped = true;
    }

    bool CanSeePlayer()
    {
        Vector3 origin = transform.position + Vector3.up * viewHeightOffset;

        Vector3 target = playerCollider != null
            ? playerCollider.bounds.center
            : player.position + Vector3.up;

        Vector3 direction = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        if (distance < 2.5f)
            return true;

        if (Physics.SphereCast(origin, sphereCastRadius, direction, out RaycastHit hit, viewDistance))
        {
            Debug.DrawLine(origin, hit.point, Color.green);
            return hit.collider.CompareTag("Player");
        }

        Debug.DrawLine(origin, origin + direction * viewDistance, Color.red);
        return false;
    }

    void Attack()
    {
        agent.isStopped = true;

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // FIRST ATTACK WINDUP
        if (firstAttackPending)
        {
            windupTimer += Time.deltaTime;

            if (windupTimer < firstAttackWindup)
                return; // wait until windup finishes

            firstAttackPending = false;
            windupTimer = 0f;
        }

        // NORMAL ATTACK COOLDOWN
        if (attackTimer <= 0f)
        {
            Debug.Log("Grief attacks player!");

            // player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);

            attackTimer = attackCooldown;
        }
    }

    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Vector3 origin = transform.position + Vector3.up * viewHeightOffset;
            Vector3 target = player.position + Vector3.up;
            Gizmos.DrawLine(origin, target);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
