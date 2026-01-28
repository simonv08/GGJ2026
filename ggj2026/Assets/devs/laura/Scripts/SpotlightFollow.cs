using UnityEngine;

public class SpotlightFollow : MonoBehaviour
{
    public GameObject target;
    
    
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform.position);
    }
}
