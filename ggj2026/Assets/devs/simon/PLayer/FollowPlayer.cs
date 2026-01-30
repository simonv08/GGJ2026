using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float lerpSpeed = 5f;

    private float lockedZ;

    void Start()
    {
        // Store original Z so it never changes
        lockedZ = transform.position.z;

        // Auto-find player if not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 currentPos = transform.position;

        // Lerp only X, allow Y manual changes, lock Z
        float newX = Mathf.Lerp(currentPos.x, target.position.x, lerpSpeed * Time.fixedDeltaTime);
        Vector3 targetPos = new Vector3(newX, currentPos.y, lockedZ);
        transform.position = targetPos;

        // Lock rotation completely
        transform.rotation = Quaternion.identity;
    }
}
