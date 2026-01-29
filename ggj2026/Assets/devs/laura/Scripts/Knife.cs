using UnityEngine;

public class Knife : MonoBehaviour
{
    public int force;
    void Start()
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        
        rb.useGravity = false;
        //rb.AddForce(direction.normalized * force,  ForceMode.Impulse);
        rb.linearVelocity = transform.forward.normalized * force;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
