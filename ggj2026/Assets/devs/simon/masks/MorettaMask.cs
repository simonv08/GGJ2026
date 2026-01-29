using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class MorettaMask : BaseMask
{
    [Header("Moretta Mask")]
    [SerializeField] private GameObject MainAtackPrefab;

    [Header("Main Attack Stats")]
    [SerializeField] private int mainAtackDamage = 10;
    [SerializeField] private float mainAtackSpeed = 2f; // Higher = Faster
    [SerializeField] private int mainAtackDistance = 1;

    [Header("Dash Attack Stats")]
    [SerializeField] private int dashDamage = 15;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashStartupDelay = 0.15f;
    [SerializeField] private float dashHitRadius = 1.2f;

    private float attackCooldown = 0f;
    private bool isDashing = false;

    private InputSystem_Actions playerInput;
    private CharacterController characterController;

    void OnEnable()
    {
        playerInput = new InputSystem_Actions();
        playerInput.Player.Enable();

        characterController = GetComponent<CharacterController>();
    }

    void OnDisable()
    {
        playerInput.Player.Disable();
    }

    void Update()
    {
        HandleMainAttack();
        HandleSecondaryAttack();
    }

    private void HandleMainAttack()
    {
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        if (playerInput.Player.AttackMain.IsPressed())
        {
            if (attackCooldown <= 0f)
            {
                MainAtackMask();
            }
        }
    }

    private void HandleSecondaryAttack()
    {
        if (playerInput.Player.AttackSecondairy.triggered && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    override public void MainAtackMask()
    {
        attackCooldown = 1f / mainAtackSpeed;

        Instantiate(
            MainAtackPrefab,
            transform.position + transform.forward * mainAtackDistance,
            transform.rotation
        );

        Debug.Log("Moretta Mask Main Attack");
    }

    override public void SecondaryAtackMask()
    {
        // Dash handled in coroutine
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        float dashDuration = dashDistance / dashSpeed;
        float elapsedTime = 0f;

        Vector3 dashDirection = transform.forward;

        // Track hit targets so we don't double-hit later
        HashSet<GameObject> hitTargets = new HashSet<GameObject>();

        // Startup delay before dash movement
        yield return new WaitForSeconds(dashStartupDelay);

        while (elapsedTime < dashDuration)
        {
            // Move player forward
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);

            // Damage detection placeholder
            DealDashDamage(hitTargets);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        Debug.Log("Moretta Mask Dash Attack");
    }

    private void DealDashDamage(HashSet<GameObject> hitTargets)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, dashHitRadius);

        foreach (Collider hit in hits)
        {
            if (hitTargets.Contains(hit.gameObject))
                continue;

            // DAMAGE SYSTEM PLACEHOLDER
            // if (hit.TryGetComponent(out IDamageable damageable))
            // {
            //     damageable.TakeDamage(dashDamage);
            //     hitTargets.Add(hit.gameObject);
            // }

            // Temporary debug log instead
            // Debug.Log($"Dash would hit: {hit.name}");
        }
    }

    override public void SetMaskStats()
    {
        Debug.Log("Moretta Mask Set Stats");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashHitRadius);
    }
}
