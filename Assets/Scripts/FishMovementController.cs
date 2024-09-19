using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class FishMovementController : MonoBehaviour
{
    // Cylinder area for the fish to move within
    [SerializeField] private float _cylinderRadius = 10f; // Radius of the cylinder's base
    [SerializeField] private float _cylinderHeight = 10f; // Height of the cylinder
    [SerializeField] private Vector3 _cylinderCenterOffset = new(0f, -5f, 0f); // Offset for the cylinder center
    [SerializeField, ReadOnly] private Vector3 _fishInitialPosition;

    private Vector3 CylinderCenter => _fishInitialPosition + _cylinderCenterOffset;

    // Movement variables
    public float minDistance = 2.0f;  // Minimum distance between waypoints
    public float moveSpeed = 2.0f;    // Speed at which the fish moves
    public float rotationSpeed = 2.0f; // Speed of rotation towards the target
    public float waitTime = 1.0f;     // Time to wait at each point

    private Vector3 targetPosition;   // Current target position
    private bool isMoving;

    private Rigidbody _rigidbody;

    void Awake()
    {
        _fishInitialPosition = transform.position;
        _rigidbody = GetComponent<Rigidbody>();
        // Set the initial target position
        targetPosition = GetRandomPointWithinCylinder();
        StartCoroutine(MoveToTarget());
    }

    void Update()
    {
        if (!isMoving)
            return;

        // Calculate the direction towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Rotate the fish smoothly towards the target
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move the fish forward in the direction it's facing
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // If the fish reaches the target, stop moving
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            isMoving = false;
            StartCoroutine(MoveToTarget());
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
            newPosition = GetRandomPointWithinCylinder();
        } while (Vector3.Distance(transform.position, newPosition) < minDistance);

        targetPosition = newPosition;
        isMoving = true;
    }

    // Get a random point within the cylinder
    private Vector3 GetRandomPointWithinCylinder()
    {
        // Generate a random point within a circle for the base of the cylinder
        float angle = Random.Range(0f, Mathf.PI * 2); // Random angle for circular distribution
        float radius = Mathf.Sqrt(Random.Range(0f, 1f)) * _cylinderRadius; // Random distance from the center

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Generate a random height within the cylinder
        float y = Random.Range(CylinderCenter.y - _cylinderHeight / 2, CylinderCenter.y + _cylinderHeight / 2);

        return new Vector3(CylinderCenter.x + x, y, CylinderCenter.z + z);
    }

    // Debug: visualize the cylinder in the scene view
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            _fishInitialPosition = transform.position;
#endif
        Gizmos.color = Color.blue;

        // Draw a cylinder by approximating it with circles for the base and top
        DrawWireCylinder(CylinderCenter, _cylinderRadius, _cylinderHeight);
    }

    // Helper function to draw a wireframe cylinder in the scene view
    private void DrawWireCylinder(Vector3 center, float radius, float height)
    {
        int segments = 20; // Number of segments to approximate the circular base
        float angleStep = 360f / segments;

        // Draw the base and top circles
        for (int i = 0; i < segments; i++)
        {
            float angleA = i * angleStep * Mathf.Deg2Rad;
            float angleB = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 pointA = new Vector3(Mathf.Cos(angleA) * radius, 0, Mathf.Sin(angleA) * radius);
            Vector3 pointB = new Vector3(Mathf.Cos(angleB) * radius, 0, Mathf.Sin(angleB) * radius);

            // Base circle
            Gizmos.DrawLine(center + pointA, center + pointB);

            // Top circle
            Gizmos.DrawLine(center + pointA + Vector3.up * height, center + pointB + Vector3.up * height);

            // Vertical lines connecting the base and top
            Gizmos.DrawLine(center + pointA, center + pointA + Vector3.up * height);
        }
    }
}