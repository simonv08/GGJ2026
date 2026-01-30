using UnityEngine;
using UnityEngine.AI;

public class DoubtEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Hit Reaction")]
    [SerializeField] private float teleportBackDistance = 2.5f;

    private NavMeshAgent agent;
    private GameObject player;

    void Start()
    {
        base.Start();

        player = GameObject.FindWithTag("Player");

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void Update()
    {
        base.Update();

        if (agent == null || player == null || !agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.SetDestination(player.transform.position);
    }

    public new void DoDamage(int damage)
    {
        base.DoDamage(damage);
        TeleportBack();
    }

    void TeleportBack()
    {
        Vector3 dir = (transform.position - player.transform.position).normalized;
        Vector3 targetPos = transform.position + dir * teleportBackDistance;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }
}