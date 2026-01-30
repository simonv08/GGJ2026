using UnityEngine;

public class Knife : MonoBehaviour
{
    [Header("Throw")]
    [SerializeField] private float force = 20f;
    [SerializeField] private float lifetime = 5f;

    [Header("Damage")]
    [SerializeField] private int damage = 10;

    private Rigidbody rb;
    private bool hasHit = false;

    void Start()
    {
        // Auto destroy
        Destroy(gameObject, lifetime);

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.linearVelocity = transform.forward.normalized * force;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.DoDamage(damage);
            }

            Stick(other);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Stick(Collider other)
    {
        hasHit = true;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        transform.SetParent(other.transform);
    }
}