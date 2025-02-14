using UnityEngine;
using Ami.Extension;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private Ami.BroAudio.SoundID _footstepSound = default;
    [SerializeField] private float footstepInterval = 0.5f; // Time between footsteps
    [SerializeField] private float minMovementSpeed = 0.1f; // Minimum speed to trigger footsteps
    
    private Vector3 lastPosition;
    private float timeSinceLastStep;
    
    private void Start()
    {
        lastPosition = transform.position;
        timeSinceLastStep = 0f;
    }
    
    private void Update()
    {
        // Calculate movement speed
        Vector3 currentPosition = transform.position;
        float movementSpeed = Vector3.Distance(lastPosition, currentPosition) / Time.deltaTime;
        
        // Update timer
        timeSinceLastStep += Time.deltaTime;
        
        // Check if we should play a footstep
        if (movementSpeed > minMovementSpeed && timeSinceLastStep >= footstepInterval)
        {
            PlayFootstep();
            timeSinceLastStep = 0f;
        }
        
        lastPosition = currentPosition;
    }
    
    private void PlayFootstep()
    {
        // Play footstep sound at current position
        Ami.BroAudio.BroAudio.Play(_footstepSound);
    }
}