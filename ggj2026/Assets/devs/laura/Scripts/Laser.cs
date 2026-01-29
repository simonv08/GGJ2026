using System;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private Player player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.gameObject.GetComponent<Player>();
            //do player damage
        }
    }
}
