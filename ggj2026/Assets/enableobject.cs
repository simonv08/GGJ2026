using UnityEngine;

public class EnableObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToEnable;

    private void OnTriggerEnter(Collider other)
    {
        // Only react to objects tagged "Player"
        if (other.CompareTag("Player"))
        {
            if (objectToEnable != null)
                objectToEnable.SetActive(true);
        }
    }
}
