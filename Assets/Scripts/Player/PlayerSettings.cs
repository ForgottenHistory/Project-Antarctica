using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Settings/Player Settings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float sneakSpeed = 2.5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public bool canSprint = true;
    public bool canSneak = true;

    [Header("Speed Ramping")]
    public bool useSpeedRamping = true;
    public float accelerationTime = 0.5f;  // Time to reach full speed
    public float decelerationTime = 0.3f;  // Time to slow back down
    [Tooltip("Smoothing curve for speed changes. Leave empty for linear acceleration")]
    public AnimationCurve speedRampCurve;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDepletionRate = 20f;  // Stamina units per second while running
    public float staminaRegenerationRate = 10f;  // Stamina units per second while not running
    public float staminaFullRechargeTime = 2f;  // Time to fully recharge when depleted
    [Tooltip("Curve for stamina recharge when depleted. Leave empty for linear recharge")]
    public AnimationCurve staminaRechargeCurve;

    [Header("Look Settings")]
    public float lookSpeedX = 2f;
    public float lookSpeedY = 2f;
    public float upperLookLimit = -60f;
    public float lowerLookLimit = 60f;

    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.3f;
    public bool canJump = true;

    [Header("Footstep Settings")]
    public float normalStepInterval = 0.5f;
    public float runStepInterval = 0.3f;
    public float sneakStepInterval = 0.7f;
    public float sneakVolumeMultiplier = 0.5f;
    public float runVolumeMultiplier = 1.5f;

    [Header("Head Bob Settings")]
    public bool enableHeadBob = true;
    public float walkBobSpeed = 14f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 18f;
    public float runBobAmount = 0.1f;
    public float sneakBobSpeed = 10f;
    public float sneakBobAmount = 0.025f;
    [Tooltip("Smoothing applied to head bob motion")]
    public float bobSmoothing = 10f;
    [Tooltip("How quickly the head bob resets when standing still")]
    public float bobResetSpeed = 5f;
    [Tooltip("Additional vertical offset for the camera")]
    public float verticalOffset = 0.0f;
}