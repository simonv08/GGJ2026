using System;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float damage;

    private Player player;
    void Start()
    {
        Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<Player>();
            player.DoDamage(damage);
            
            Destroy(gameObject);
        }
    }
}
