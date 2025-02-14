using UnityEngine;

public class OffsetFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private bool followRotation = true;
    private Vector3 offset;
    
    [Header("Optional Settings")]
    [SerializeField] private bool maintainInitialRotationOffset = true;
    private Quaternion rotationOffset;

    [Header("SLERP Settings")]
    [SerializeField] public bool useSlerp = false;
    [SerializeField, Range(0f, 1f)] private float positionLerpSpeed = 0.1f;
    [SerializeField, Range(0f, 1f)] private float rotationLerpSpeed = 0.1f;
    
    // Cached transform positions for SLERP
    private Vector3 currentTargetPosition;
    private Quaternion currentTargetRotation;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to RotationalOffsetFollower!");
            enabled = false;
            return;
        }

        // Calculate the initial position offset in target's local space
        offset = Quaternion.Inverse(target.rotation) * (transform.position - target.position);
        
        // Calculate the initial rotation offset if needed
        if (maintainInitialRotationOffset)
        {
            rotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
        }
        
        // Initialize SLERP positions
        currentTargetPosition = transform.position;
        currentTargetRotation = transform.rotation;
        
        //Debug.Log($"Calculated position offset: {offset}"); // Helpful for debugging
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 rotatedOffset = target.rotation * offset;
        Vector3 desiredPosition = target.position + rotatedOffset;

        // Calculate desired rotation
        Quaternion desiredRotation = followRotation ? 
            (maintainInitialRotationOffset ? target.rotation * rotationOffset : target.rotation) : 
            transform.rotation;

        if (useSlerp)
        {
            // Smoothly interpolate position
            currentTargetPosition = Vector3.Lerp(currentTargetPosition, desiredPosition, positionLerpSpeed);
            transform.position = currentTargetPosition;

            // Smoothly interpolate rotation if following
            if (followRotation)
            {
                currentTargetRotation = Quaternion.Slerp(currentTargetRotation, desiredRotation, rotationLerpSpeed);
                transform.rotation = currentTargetRotation;
            }
        }
        else
        {
            // Direct positioning without SLERP
            transform.position = desiredPosition;
            if (followRotation)
            {
                transform.rotation = desiredRotation;
            }
        }
    }

    // Optional: Method to recalculate offsets at runtime
    public void RecalculateOffsets()
    {
        if (target != null)
        {
            offset = Quaternion.Inverse(target.rotation) * (transform.position - target.position);
            if (maintainInitialRotationOffset)
            {
                rotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
            }
            
            // Reset SLERP positions when recalculating
            currentTargetPosition = transform.position;
            currentTargetRotation = transform.rotation;
        }
    }
}