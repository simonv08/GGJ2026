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
    [SerializeField] private float mainAttackSpeed = 2f;
    [SerializeField] private float mainAttackDistance = 1f;

    [Header("Dash Settings")]
    [SerializeField] private int dashDamage = 15;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashHitRadius = 1.2f;

    [Header("Dash Cooldown")]
    [SerializeField] private float dashCooldown = 1f;

    [Header("Player Settings Adjustment")]
    [SerializeField] private float moveSpeed_Player = 6f;
    [SerializeField] private float gravity_Player = -18f;
    [SerializeField] private float jumpForce_Player = 8f;
    [SerializeField] private float crouchHeight_Player = 1f;
    [SerializeField] private float crouchSpeedMultiplier_Player = 1f;
    [SerializeField] private float crouchSmoothSpeed_Player = 15f;

    private float dashTimer = 0f;
    private bool isDashing = false;

    private InputSystem_Actions playerInput;
    private CharacterController characterController;
    private float attackCooldown = 0f;

    private Player playerScript;

    public int MainAttackDamage => mainAttackDamage;

    void OnEnable()
    {
        playerInput = new InputSystem_Actions();
        playerInput.Player.Enable();

        characterController = GetComponent<CharacterController>();
        playerScript = GetComponent<Player>();

        ApplyPlayerSettings();

        playerInput.Player.AttackSecondairy.performed += OnDashPressed;
    }

    void OnDisable()
    {
        playerInput.Player.AttackSecondairy.performed -= OnDashPressed;
        playerInput.Player.Disable();
    }

    private void OnDashPressed(InputAction.CallbackContext ctx)
    {
        if (!isDashing && dashTimer <= 0f)
        {
            StartCoroutine(SlideDash(dashDistance, dashSpeed));
        }
    }

    void Update()
    {
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        if (dashTimer > 0f)
            dashTimer -= Time.deltaTime;

        HandleMainAttack();
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

        float slideDistance = 0.5f;
        float slideDuration = 0.15f;
        StartCoroutine(SlideForward(slideDistance, slideDuration));

        Instantiate(
            MainAttackPrefab,
            transform.position + transform.forward * mainAttackDistance,
            transform.rotation
        );
    }

    private IEnumerator SlideForward(float distance, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + transform.forward * distance;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            Vector3 move = newPos - transform.position;
            characterController.Move(move);

            elapsed += Time.deltaTime;
            yield return null;
        }

        characterController.Move(targetPos - transform.position);
    }

    private IEnumerator SlideDash(float distance, float speed)
    {
        isDashing = true;

        float remainingDistance = distance;
        HashSet<GameObject> hitTargets = new HashSet<GameObject>();

        while (remainingDistance > 0f)
        {
            float moveStep = speed * Time.deltaTime;
            if (moveStep > remainingDistance)
                moveStep = remainingDistance;

            Vector3 move = transform.forward * moveStep;
            characterController.Move(move);

            DealDashDamage(hitTargets);

            remainingDistance -= moveStep;
            yield return null;
        }

        isDashing = false;
        dashTimer = dashCooldown;
    }

    private void ApplyPlayerSettings()
    {
        if (playerScript == null) return;

        playerScript.moveSpeed = moveSpeed_Player;
        playerScript.gravity = gravity_Player;
        playerScript.jumpForce = jumpForce_Player;
        playerScript.crouchHeight = crouchHeight_Player;
        playerScript.crouchSpeedMultiplier = crouchSpeedMultiplier_Player;
        playerScript.crouchSmoothSpeed = crouchSmoothSpeed_Player;
    }

    private void DealDashDamage(HashSet<GameObject> hitTargets)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, dashHitRadius);

        foreach (Collider hit in hits)
        {
            GameObject root = hit.transform.root.gameObject;

            if (hitTargets.Contains(root))
                continue;

            // EnemyBase damage pattern
            EnemyBase enemy = hit.GetComponentInParent<EnemyBase>();
            if (enemy != null)
            {
                enemy.DoDamage(dashDamage);
                hitTargets.Add(root);
            }
        }
    }

    override public void SecondaryAtackMask()
    {
        // Dash handled via input callback
    }

    override public void SetMaskStats()
    {
        Debug.Log("Moretta Mask Set Stats");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashHitRadius);

        if (Application.isPlaying && characterController != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * dashDistance);
            Gizmos.DrawWireSphere(transform.position + transform.forward * dashDistance, dashHitRadius);
        }
    }
}
