using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    [Header("Movement Settings")]
    private float movementX;
    private float movementY;
    public float speed = 10f;
    public float jumpForce = 1f;
    public float dashForce = 30f;
    public float gravity = -9.81f;

    [Header("Other Settings")]
    public int lives = 3;

    private int maxLives;
    
    private float dashCooldown = 2f;
    private float lastDashTime = -Mathf.Infinity;
    private float dashDuration = 0.3f;
    private float dashElapsedTime = 0f;
    private Vector3 dashVelocity = Vector3.zero;
    private bool isDashing = false;

    private CharacterController controller;
    private Animator animator;
    private Camera mainCamera;
    private Vector3 moveInput;
    private Vector3 velocity;
    
    // Animation parameters
    private int moveSpeedHash;

    private void Start()
    {
        maxLives = lives;
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
        
        // Cache animator parameter hashes for better performance
        if (animator != null)
        {
            moveSpeedHash = Animator.StringToHash("MoveSpeed");
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2.0f * gravity);
        }
    }
    
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            Vector3 dashDirection = GetCameraRelativeMovement().normalized;
            if (dashDirection != Vector3.zero)
            {
                isDashing = true;
                dashElapsedTime = 0f;
                dashVelocity = dashDirection * dashForce;
                lastDashTime = Time.time;
            }
        }
    }

    private void Update()
    {
        if (isDashing)
        {
            dashElapsedTime += Time.deltaTime;
            if (dashElapsedTime >= dashDuration)
            {
                isDashing = false;
                dashVelocity = Vector3.zero;
            }
            else
            {
                float dashProgress = dashElapsedTime / dashDuration;
                Vector3 currentDashVelocity = dashVelocity * (1f - dashProgress);
                controller.Move(currentDashVelocity * Time.deltaTime);
            }
        }

        // Transform movement input based on camera angle
        Vector3 move = GetCameraRelativeMovement();
        controller.Move(move * speed * Time.deltaTime);
        
        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            // Reset velocity when grounded to prevent accumulation
            velocity.y = -2f; // Small downward force to keep grounded
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        
        controller.Move(velocity * Time.deltaTime);

        if (animator != null)
        {
            float moveMagnitude = move.magnitude;
            animator.SetFloat(moveSpeedHash, moveMagnitude);
            if (move != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }
    
    public void ResetPositionToSpawn()
    {
        GameObject respawn = GameObject.FindGameObjectWithTag("Respawn");
        if (respawn != null && controller != null)
        {
            controller.enabled = false; // Disable controller to avoid collision issues
            transform.position = respawn.transform.position;
            controller.enabled = true; // Re-enable controller
            velocity = Vector3.zero; // Reset velocity

            Debug.Log("Player respawned at: " + respawn.transform.position);
        }
    }

    private Vector3 GetCameraRelativeMovement()
    {
        if (mainCamera == null)
        {
            return new Vector3(moveInput.x, 0, moveInput.y);
        }

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 move = (cameraForward * moveInput.y + cameraRight * moveInput.x);
        return move;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CollectibleBox") && !GameManager.Instance.IsGameOver)
        {
            Destroy(other.gameObject);
            GameManager.Instance.BoxCollected();
        }
    }
    
    public void ResetLives()
    {
        lives = 3;
        UpdateLivesDisplay();
    }
    
    public void LoseLife()
    {
        lives = Mathf.Max(0, lives - 1);
        UpdateLivesDisplay();
    }
    
    public int GetLives()
    {
        return lives;
    }

    public void UpdateLivesDisplay()
    {
        CanvasSystem.Instance.UpdateLives(lives, maxLives);
    }
}