using UnityEngine;
using System.Collections.Generic;

public class SaberAttack : MonoBehaviour
{
    [SerializeField] private MorettaMask morettaMask;
    public float SelfDestructTime = 0.2f;

    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    void Awake()
    {
        // Find player if reference not set
        if (morettaMask == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                morettaMask = player.GetComponent<MorettaMask>();
        }

        Destroy(gameObject, SelfDestructTime);
    }

    public int GetMainAttackDamage()
    {
        return morettaMask != null ? morettaMask.MainAttackDamage : 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("SaberAttack hit: " + other.gameObject.name);

        if (other.CompareTag("Enemy") && !hitTargets.Contains(other.gameObject))
        {
            hitTargets.Add(other.gameObject);

            ShameEnemy enemy = other.GetComponent<ShameEnemy>();
            if (enemy != null)
            {
                int damage = GetMainAttackDamage();
                enemy.DoDamage(damage);
                Debug.Log("Dealt " + damage + " damage to " + other.gameObject.name);
            }
        }
    }
}
