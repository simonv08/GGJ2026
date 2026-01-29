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
        // Prevent hitting non-enemies
        EnemyBase enemy = other.GetComponentInParent<EnemyBase>();
        if (enemy == null)
            return;

        GameObject enemyRoot = enemy.gameObject;

        // Prevent double hits in one attack
        if (hitTargets.Contains(enemyRoot))
            return;

        int damage = GetMainAttackDamage();

        enemy.DoDamage(damage);
        hitTargets.Add(enemyRoot);

        Debug.Log($"Saber hit {enemy.name} for {damage}");
    }
}
