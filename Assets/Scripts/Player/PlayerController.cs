using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;
    private float currentMoveSpeed;
    private float targetMoveSpeed;
    private float speedLerpTime;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;
    private bool isSneaking;
    private CharacterController controller;
    private Transform playerBody;
    private float xRotation = 0f;

    // Stamina system variables
    private float currentStamina;
    private bool isStaminaDepleted;
    private float staminaRechargeProgress;

    public LayerMask groundLayer;

    private float bobTimer;
    private float bobAmount;
    private float bobSpeed;
    private float defaultCameraY;
    private float currentBobPosition;
    private Vector3 cameraLocalPosition;
    private Camera playerCamera;
    private bool canLookAround = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerBody = transform;
        playerCamera = Camera.main;
        defaultCameraY = playerCamera.transform.localPosition.y;
        cameraLocalPosition = playerCamera.transform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize movement speeds
        currentMoveSpeed = settings.walkSpeed;
        targetMoveSpeed = settings.walkSpeed;

        // Initialize stamina
        currentStamina = settings.maxStamina;
        isStaminaDepleted = false;
        staminaRechargeProgress = 0f;
    }

    void Update()
    {
        HandleMovementStates();
        LookAround();
        isGrounded = CheckIfGrounded();
        MovePlayer();
        HandleStamina();

        if (settings.enableHeadBob)
        {
            HandleHeadBob();
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isGrounded && Input.GetButtonDown("Jump") && settings.canJump)
        {
            velocity.y = Mathf.Sqrt(settings.jumpHeight * -2f * settings.gravity);
        }

        if (!isGrounded)
        {
            velocity.y += settings.gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canLookAround = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            canLookAround = true;
        }
    }

    void HandleStamina()
    {
        if (isStaminaDepleted)
        {
            // Handle stamina recharge when depleted
            staminaRechargeProgress += Time.deltaTime / settings.staminaFullRechargeTime;

            // Use the recharge curve if assigned, otherwise linear
            float rechargeMultiplier = settings.staminaRechargeCurve != null ?
                settings.staminaRechargeCurve.Evaluate(staminaRechargeProgress) :
                staminaRechargeProgress;

            currentStamina = settings.maxStamina * rechargeMultiplier;

            // Check if fully recharged
            if (staminaRechargeProgress >= 1f)
            {
                currentStamina = settings.maxStamina;
                isStaminaDepleted = false;
                staminaRechargeProgress = 0f;
            }
        }
        else if (isRunning)
        {
            // Decrease stamina while running
            currentStamina -= settings.staminaDepletionRate * Time.deltaTime;

            // Check if stamina is depleted
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isStaminaDepleted = true;
                isRunning = false;
                targetMoveSpeed = settings.walkSpeed;
            }
        }
        else if (currentStamina < settings.maxStamina)
        {
            // Regenerate stamina while not running
            currentStamina += settings.staminaRegenerationRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, settings.maxStamina);
        }
    }

    void HandleMovementStates()
    {
        float previousTarget = targetMoveSpeed;

        // Handle running (Left Control)
        if (Input.GetKey(KeyCode.LeftShift) && !isSneaking && !isStaminaDepleted && currentStamina > 0
        && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.W) == true)
        {
            isRunning = true;
            targetMoveSpeed = settings.runSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || isStaminaDepleted)
        {
            isRunning = false;
            targetMoveSpeed = settings.walkSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            isRunning = false;
            targetMoveSpeed = settings.walkSpeed;
        }

        // Handle sneaking (Left Shift)
        if (Input.GetKey(KeyCode.LeftControl) && !isRunning)
        {
            isSneaking = true;
            targetMoveSpeed = settings.sneakSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isSneaking = false;
            targetMoveSpeed = settings.walkSpeed;
        }

        // Handle speed ramping
        if (settings.useSpeedRamping && targetMoveSpeed != previousTarget)
        {
            speedLerpTime = 0f;
        }

        if (settings.useSpeedRamping)
        {
            float timeToUse = targetMoveSpeed > currentMoveSpeed ?
                settings.accelerationTime : settings.decelerationTime;

            if (timeToUse > 0)
            {
                speedLerpTime += Time.deltaTime;
                float t = Mathf.Clamp01(speedLerpTime / timeToUse);

                if (settings.speedRampCurve != null && settings.speedRampCurve.length > 0)
                {
                    t = settings.speedRampCurve.Evaluate(t);
                }

                currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, targetMoveSpeed, t);
            }
            else
            {
                currentMoveSpeed = targetMoveSpeed;
            }
        }
        else
        {
            currentMoveSpeed = targetMoveSpeed;
        }
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentMoveSpeed * Time.deltaTime);
    }

    void LookAround()
    {
        if (canLookAround == false)
            return;

        float mouseX = Input.GetAxis("Mouse X") * settings.lookSpeedX;
        float mouseY = Input.GetAxis("Mouse Y") * settings.lookSpeedY;

        playerBody.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, settings.upperLookLimit, settings.lowerLookLimit);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleHeadBob()
    {
        if (!isGrounded) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;

        // Set bob parameters based on movement state
        if (isMoving)
        {
            if (isRunning)
            {
                bobSpeed = settings.runBobSpeed;
                bobAmount = settings.runBobAmount;
            }
            else if (isSneaking)
            {
                bobSpeed = settings.sneakBobSpeed;
                bobAmount = settings.sneakBobAmount;
            }
            else
            {
                bobSpeed = settings.walkBobSpeed;
                bobAmount = settings.walkBobAmount;
            }

            // Increment the bob timer
            bobTimer += Time.deltaTime * bobSpeed;

            // Calculate the target bob position using a smooth sine wave
            float targetBobPosition = Mathf.Sin(bobTimer) * bobAmount;

            // Smoothly interpolate to the target position
            currentBobPosition = Mathf.Lerp(currentBobPosition, targetBobPosition, Time.deltaTime * settings.bobSmoothing);
        }
        else
        {
            // Reset the bob when not moving
            bobTimer = 0;
            currentBobPosition = Mathf.Lerp(currentBobPosition, 0f, Time.deltaTime * settings.bobResetSpeed);
        }

        // Apply the bob and vertical offset to the camera
        cameraLocalPosition.y = defaultCameraY + currentBobPosition + settings.verticalOffset;

        // Add a subtle horizontal motion for more natural movement
        cameraLocalPosition.x = Mathf.Sin(bobTimer * 0.5f) * (bobAmount * 0.5f);

        // Apply the position
        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            cameraLocalPosition,
            Time.deltaTime * settings.bobSmoothing
        );
    }

    bool CheckIfGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        return Physics.Raycast(ray, out _, settings.groundCheckDistance, groundLayer);
    }

    public bool IsRunning() => isRunning;
    public bool IsSneaking() => isSneaking;
    public float GetCurrentStamina() => currentStamina;
    public float GetMaxStamina() => settings.maxStamina;
    public bool IsStaminaDepleted() => isStaminaDepleted;

    private void OnDestroy()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
    }
}