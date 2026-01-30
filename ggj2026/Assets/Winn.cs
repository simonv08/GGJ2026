using UnityEngine;

public class Winn : MonoBehaviour
{
    [SerializeField] private Canvas winCanvas;

    private void Start()
    {
        if (winCanvas != null)
            winCanvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check tag
        if (!other.CompareTag("Player"))
            return;

        // Ensure it's the CharacterController player
        if (other.GetComponent<CharacterController>() == null)
            return;

        // Activate win canvas
        if (winCanvas != null)
            winCanvas.gameObject.SetActive(true);
    }
}
