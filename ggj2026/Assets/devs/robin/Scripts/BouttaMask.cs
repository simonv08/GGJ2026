using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BouttaMask : BaseMask
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private LayerMask hitLayer = ~0;

    [Header("Saber Slash Settings")]
    [SerializeField] private int slashDamage = 25;
    [SerializeField] private float slashRange = 1.5f;
    [SerializeField] private float slashRadius = 1.2f;
    [SerializeField] private float slashCooldown = 0.6f;

    [Header("Blunderbuss Settings")]
    [SerializeField] private int pelletCount = 8;
    [SerializeField] private int pelletDamage = 10;
    [SerializeField] private float spreadAngleDeg = 25f;
    [SerializeField] private float pelletRange = 20f;
    [SerializeField] private float shootCooldown = 0.25f;
    [SerializeField] private float bulletSpawnOffset = 0.15f;

    [Header("Player Settings Adjustment")]
    [SerializeField] private float moveSpeed_Player = 6f;
    [SerializeField] private float gravity_Player = -18f;
    [SerializeField] private float jumpForce_Player = 8f;
    [SerializeField] private float crouchHeight_Player = 1f;
    [SerializeField] private float crouchSpeedMultiplier_Player = 1f;
    [SerializeField] private float crouchSmoothSpeed_Player = 15f;

    private InputSystem_Actions playerInput;
    private CharacterController characterController;
    private float attackCooldown = 0f;

    private Player playerScript;

    void OnEnable()
    {
        playerInput = new InputSystem_Actions();
        playerInput.Player.Enable();

        characterController = GetComponent<CharacterController>();
        playerScript = GetComponent<Player>();

        ApplyPlayerSettings();
    }

    // MAIN ATTACK -> Saber Slash
    override public void MainAtackMask()
    {
        PerformSaberSlash();
    }

    // SECONDARY ATTACK -> Shoot (Blunderbuss)
    override public void SecondaryAtackMask()
    {
        Shoot();
    }

    private void PerformSaberSlash()
    {
        print("Boutta Mask - Main Attack");

        Vector3 center = transform.position + transform.forward * slashRange;

        Collider[] hits = Physics.OverlapSphere(center, slashRadius, hitLayer, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hit = hits[i];

            if (hit.transform.IsChildOf(transform))
            {
                continue;
            }

            if (hit.gameObject.CompareTag("Enemy"))
            {
                hit.GetComponent<EnemyBase>().DoDamage(slashDamage);
            }
        }

#if UNITY_EDITOR
        DebugDrawSphere(center, slashRadius, Color.red, 0.25f);
#endif
    }

    private void Shoot()
    {
        print("Boutta Mask - Secondary Attack");
        Transform spawn = muzzlePoint != null ? muzzlePoint : transform;
        Vector3 spawnPos = spawn.position + spawn.forward * bulletSpawnOffset;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 pelletDir = RandomDirectionInCone(spawn.forward, spreadAngleDeg * 0.5f);
            Ray ray = new Ray(spawnPos, pelletDir);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pelletRange, hitLayer, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.IsChildOf(transform))
                {
                    continue;
                }

                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<EnemyBase>().DoDamage(pelletDamage);
                }
#if UNITY_EDITOR
                Debug.DrawRay(spawnPos, pelletDir * pelletRange, Color.yellow, 0.25f);
#endif
            }
        }
    }

    // returns a random unit direction within a cone of half-angle maxAngleDeg around baseDir
    private Vector3 RandomDirectionInCone(Vector3 baseDir, float maxHalfAngleDeg)
    {
        // If angle is zero, return base dir
        if (maxHalfAngleDeg <= 0f)
            return baseDir.normalized;

        // pick a random rotation around an axis perpendicular to baseDir
        float theta = Random.Range(0f, 2f * Mathf.PI);
        float z = Mathf.Cos(Random.Range(0f, maxHalfAngleDeg * Mathf.Deg2Rad));
        float sinT = Mathf.Sqrt(1f - z * z);

        // direction in local cone coordinates
        Vector3 localDir = new Vector3(sinT * Mathf.Cos(theta), sinT * Mathf.Sin(theta), z);

        // find a basis that maps local cone z to baseDir
        Quaternion q = Quaternion.FromToRotation(Vector3.forward, baseDir.normalized);
        return q * localDir;
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

#if UNITY_EDITOR
    // small helper for debug drawing
    private void DebugDrawSphere(Vector3 center, float radius, Color color, float duration)
    {
        int steps = 24;
        for (int i = 0; i < steps; i++)
        {
            float a1 = (i / (float)steps) * Mathf.PI * 2f;
            float a2 = ((i + 1) / (float)steps) * Mathf.PI * 2f;
            Vector3 p1 = center + new Vector3(Mathf.Cos(a1), 0f, Mathf.Sin(a1)) * radius;
            Vector3 p2 = center + new Vector3(Mathf.Cos(a2), 0f, Mathf.Sin(a2)) * radius;
            Debug.DrawLine(p1, p2, color, duration);
            p1 = center + new Vector3(0f, Mathf.Cos(a1), Mathf.Sin(a1)) * radius;
            p2 = center + new Vector3(0f, Mathf.Cos(a2), Mathf.Sin(a2)) * radius;
            Debug.DrawLine(p1, p2, color, duration);
        }
    }
#endif
}