using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class MorettaMask : BaseMask
{
    [Header("Moretta Mask")]
    [SerializeField] private GameObject MainAttackPrefab;

    [Header("Main Attack Stats")]
    [SerializeField] private int mainAttackDamage = 10;
    [SerializeField] private float mainAttackSpeed = 2f; // Attacks per second
    [SerializeField] private float mainAttackDistance = 1f;

    [Header("Dash Attack Stats")]
    [SerializeField] private int dashDamage = 15;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashStartupDelay = 0.15f;
    [SerializeField] private float dashHitRadius = 1.2f;

    [Header("Dash Cooldown")]
    [SerializeField] private float dashCooldown = 1f; // Delay between dashes
    private float dashTimer = 0f;

    private bool isDashing = false;
    private InputSystem_Actions playerInput;
    private CharacterController characterController;
    private float attackCooldown = 0f;

    public int MainAttackDamage => mainAttackDamage;

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

    void FixedUpdate()
    {
        // Reduce cooldown timers
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        if (dashTimer > 0f)
            dashTimer -= Time.deltaTime;

        HandleMainAttack();
        HandleDash();
    }

    private void HandleMainAttack()
    {
        if (playerInput.Player.AttackMain.IsPressed() && attackCooldown <= 0f)
        {
            MainAttack();
        }
    }

    override public void MainAtackMask()
    {
        MainAttack();
    }

    private void MainAttack()
    {
        attackCooldown = 1f / mainAttackSpeed;

        // Start sliding forward coroutine
        float slideDistance = 0.5f;   // How far the player slides
        float slideDuration = 0.15f;  // How long the slide lasts
        StartCoroutine(SlideForward(slideDistance, slideDuration));

        // Spawn the attack prefab
        Instantiate(
            MainAttackPrefab,
            transform.position + transform.forward * mainAttackDistance,
            transform.rotation
        );

        Debug.Log("Moretta Mask Main Attack with Sliding Step");
    }

    // Smooth slide coroutine
    private IEnumerator SlideForward(float distance, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + transform.forward * distance;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Smooth interpolation
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            Vector3 move = newPos - transform.position;
            characterController.Move(move);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exact
        characterController.Move(targetPos - transform.position);
    }



    private void HandleDash()
    {
        if (playerInput.Player.AttackSecondairy.triggered && !isDashing && dashTimer <= 0f)
        {
            StartCoroutine(DashRoutine());
            dashTimer = dashCooldown; // Start cooldown
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        Vector3 dashDirection = transform.forward;
        float dashDuration = dashDistance / dashSpeed;
        float elapsedTime = 0f;

        HashSet<GameObject> hitTargets = new HashSet<GameObject>();

        // Startup delay before dash begins
        yield return new WaitForSeconds(dashStartupDelay);

        while (elapsedTime < dashDuration)
        {
            // Move player forward
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);

            // Damage detection
            DealDashDamage(hitTargets);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        Debug.Log("Moretta Mask Dash Finished");
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

            // Temporary debug
            // Debug.Log($"Dash would hit: {hit.name}");
        }
    }

    override public void SecondaryAtackMask()
    {
        HandleDash();
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
