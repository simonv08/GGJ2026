using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    private float health;
    protected void Start()
    {
        health = 100f;
        gameObject.tag = "Enemy";
    }

    // Update is called once per frame
    protected void Update()
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
