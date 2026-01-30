using UnityEngine;
using UnityEngine.AI;

public class ShameEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    private NavMeshAgent agent;

    [Header("Laser")]
    [SerializeField] private float laserCoolDown = 2f;
    private float laserTimer;

    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject shootPoint;

    private GameObject player;

    public enum State
    {
        Normal,
        Hidden,
    }

    public State currentState;

    void Start()
    {
        base.Start();

        currentState = State.Normal;

        player = GameObject.FindWithTag("Player");

        // NAVMESH (gejat uit Grief 👀)
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

        HandleSwitch();
        DoState();

        laserTimer += Time.deltaTime;
        if (laserTimer >= laserCoolDown && currentState == State.Normal)
        {
            ShootLaser();
        }
    }

    private void HandleSwitch()
    {
        switch (currentState)
        {
            case State.Normal:
                MoveToPlayer();
                break;

            case State.Hidden:
                StopMoving();
                break;
        }
    }

    private void DoState()
    {
        switch (currentState)
        {
            case State.Normal:
                // normaal gedrag
                break;

            case State.Hidden:
                // reset laser als hij bekeken wordt
                laserTimer = 0f;
                break;
        }
    }

    void MoveToPlayer()
    {
        if (agent == null || player == null) return;

        agent.isStopped = false;
        agent.SetDestination(player.transform.position);
    }

    void StopMoving()
    {
        if (agent == null) return;
        agent.isStopped = true;
    }

    public void Hide()
    {
        currentState = State.Hidden;
    }

    public void Show()
    {
        currentState = State.Normal;
    }

    private void ShootLaser()
    {
        Vector3 direction = player.transform.position - shootPoint.transform.position;

        GameObject laser = Instantiate(
            laserPrefab,
            shootPoint.transform.position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = laser.GetComponent<Rigidbody>();
        rb.AddForce(direction.normalized * 10f, ForceMode.Impulse);

        laserTimer = 0f;
    }
}
