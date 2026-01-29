using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private int force;
    [SerializeField] private int damage;
    
    
    void Start()
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();

        // vooruit + beetje omhoog
        Vector3 direction = transform.forward + Vector3.up * 0.5f;
        rb.AddForce(direction.normalized * force, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            EnemyBase EnemyScript = other.GetComponent<EnemyBase>(); ////change after enemyScripts made
            EnemyScript.DoDamage(damage);
        }
    }
}
