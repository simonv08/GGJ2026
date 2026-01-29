using UnityEngine;

public class SaberAttack : MonoBehaviour
{
    [SerializeField] private MorettaMask morettaMask;
    private int damage;

    void Awake()
    {
        if (morettaMask == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                morettaMask = player.GetComponent<MorettaMask>();
            }
        }
    }

    public int GetMainAttackDamage()
    {
        return morettaMask != null ? morettaMask.MainAttackDamage : 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Enemy"))
            return;

        int damage = GetMainAttackDamage();
        Debug.Log("Damage dealt: " + damage);

        // Example damage application:
        // EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        // if (enemyHealth != null)
        // {
        //     enemyHealth.TakeDamage(damage);
        // }
    }
}
