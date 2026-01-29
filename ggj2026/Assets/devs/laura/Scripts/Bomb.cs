using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int force;
    
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
}
