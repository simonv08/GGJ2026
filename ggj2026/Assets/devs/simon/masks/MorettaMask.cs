using UnityEngine;
using UnityEngine.InputSystem;

public class MorettaMask : BaseMask
{
    [Header("Moretta Mask")]
    [SerializeField] private GameObject MainAtackPrefab;

    [Header("Main Attack Stats")]
    [SerializeField] private int mainAtackDamage = 10;
    [SerializeField] private float mainAtackSpeed = 2f; // Higher = Faster
    [SerializeField] private int mainAtackDistance = 1;

    private bool Attacking = false;
    private float attackCooldown = 0f;

    private InputSystem_Actions playerInput;

    void OnEnable()
    {
        playerInput = new InputSystem_Actions();
        playerInput.Player.Enable();
    }

    void OnDisable()
    {
        playerInput.Player.Disable();
    }

    void Update()
    {
        // Reduce cooldown timer
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        // Try attack
        if (playerInput.Player.AttackMain.triggered)
        {
            if (attackCooldown > 0f) return;

            MainAtackMask();
        }
    }

    override public void MainAtackMask()
    {
        // Lock attack
        attackCooldown = 1f / mainAtackSpeed;

        Instantiate(
            MainAtackPrefab,
            new Vector3(transform.position.x, transform.position.y, transform.position.z + mainAtackDistance),
            transform.rotation
        );

        Debug.Log("Moretta Mask Main Attack");
    }

    override public void SecondaryAtackMask()
    {
        return;
    }

    override public void SetMaskStats()
    {
        Debug.Log("Moretta Mask Set Stats");
    }
}
