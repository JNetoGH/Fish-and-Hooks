using System.Collections;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    // Box area for the fish to move within
    public Vector3 boxSize = new Vector3(10f, 5f, 10f);  // Size of the box
    public Vector3 boxCenter = Vector3.zero;  // Center of the box
    
    // Movement variables
    public float minDistance = 2.0f;  // Minimum distance between waypoints
    public float moveSpeed = 2.0f;    // Speed at which the fish moves
    public float waitTime = 1.0f;     // Time to wait at each point
    
    private Vector3 targetPosition;   // Current target position
    private bool isMoving = false;

    void Start()
    {
        // Set the initial target position
        targetPosition = GetRandomPointWithinBox();
        StartCoroutine(MoveToTarget());
    }

    void Update()
    {
        // Move the fish towards the target position
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // If the fish reaches the target, stop moving
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                StartCoroutine(MoveToTarget());
            }
        }
    }

    // Coroutine that picks a new random target and moves to it
    private IEnumerator MoveToTarget()
    {
        // Wait for some time before moving to the next point
        yield return new WaitForSeconds(waitTime);

        // Get a new target position ensuring it's far enough from the current position
        Vector3 newPosition;
        do
        {
            newPosition = GetRandomPointWithinBox();
        } while (Vector3.Distance(transform.position, newPosition) < minDistance);

        targetPosition = newPosition;
        isMoving = true;
    }

    // Get a random point within the box
    private Vector3 GetRandomPointWithinBox()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2),
            Random.Range(boxCenter.y - boxSize.y / 2, boxCenter.y + boxSize.y / 2),
            Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2)
        );
        
        return randomPosition;
    }

    // Debug: visualize the box in the scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}