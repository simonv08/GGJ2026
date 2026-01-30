using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Throw")]
    [SerializeField] private float force = 10f;

    [Header("Damage")]
    [SerializeField] private float damagePerSecond = 5f;
    [SerializeField] private float cloudDuration = 3f;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 10f;

    private Rigidbody rb;
    private MeshRenderer meshRenderer;

    private bool exploded = false;
    private float cloudTimer = 0f;

    void Start()
    {
        // Auto destroy after lifetime
        Destroy(gameObject, lifetime);

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        Vector3 direction = transform.forward + Vector3.up * 0.5f;
        rb.AddForce(direction.normalized * force, ForceMode.Impulse);

        meshRenderer = transform.Find("Sphere").GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (!exploded) return;

        cloudTimer += Time.deltaTime;
        if (cloudTimer >= cloudDuration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!exploded)
        {
            Explode();
        }

        ApplyDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!exploded) return;

        ApplyDamage(other);
    }

    void Explode()
    {
        exploded = true;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        meshRenderer.enabled = false;

        cloudTimer = 0f;
    }

    void ApplyDamage(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.DoDamage(Mathf.RoundToInt(damagePerSecond * Time.deltaTime));
        }
    }
}