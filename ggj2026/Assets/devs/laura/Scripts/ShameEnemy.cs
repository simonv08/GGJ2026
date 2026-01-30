using UnityEngine;

public class ShameEnemy : EnemyBase
{
    [SerializeField] private float LaserCoolDown;
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
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        HandleSwitch();
        DoState();
        
        laserTimer += Time.deltaTime;
        if (laserTimer >= LaserCoolDown)
        {
            ShootLaser();
        }

    }

    private void HandleSwitch()
    {
        switch (currentState)
        {
            case State.Normal:
                //switchtohidden
                break;
            case State.Hidden:
                //switchtonormal
                break;
        }
    }

    private void DoState()
    {
        switch (currentState)
        {
            case State.Normal:
                Debug.Log("Normal");
                break;
            case State.Hidden:
                Debug.Log("Hidden");
                laserTimer = 0f;
                break;
        }
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
        Debug.Log("Shoot!");
        Vector3 direction = player.transform.position - shootPoint.transform.position;
        //Debug.DrawRay(shootPoint.transform.position, direction, Color.red, 2f);


        GameObject laser = Instantiate(
            laserPrefab,
            shootPoint.transform.position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = laser.GetComponent<Rigidbody>();
        rb.AddForce(direction * 10f, ForceMode.Impulse);

        laserTimer = 0f;
    }
}
