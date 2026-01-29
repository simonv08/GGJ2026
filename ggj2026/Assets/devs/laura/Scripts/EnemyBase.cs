using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private float health;
    void Start()
    {
        health = 100f;
        gameObject.tag = "Enemy";
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f)
        {
            Die();
        }
    }

    public void DoDamage(int damage)
    {
        health -= damage;
    }

    private void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }
}
