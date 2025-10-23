using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Setup")]
    // Reference to the player's transform
    public Transform player;
    // Chase speed (optional, can be adjusted per enemy)
    public float chaseSpeed = 5f;

    [Header("Momentum Settings")]
    // Lower acceleration = slower to speed up (heavier feeling)
    public float acceleration = 2f;
    // Lower angular speed = slower to turn (more momentum)
    public float angularSpeed = 50f;
    // How quickly agent slows down (higher = slower deceleration)
    public float stoppingDistance = 0.5f;

    private NavMeshAgent navMeshAgent;
    private bool isActive = true;

    [Header("Rolling Settings")]
    // How fast the sphere rotates while moving (higher = faster rolling)
    public float rollSpeed = 5f;
    // Rotation axis: X=tilts left/right, Y=spins (like top), Z=rolls forward/backward
    // For spheres: use Z (forward rolling)
    // For cubes: use Z (forward rolling) or X (tilting)
    // For custom shapes: adjust as needed
    public Vector3 rotationAxis = Vector3.right; // Default: roll forward/backward

    private void Start()
    {
        // Get and store the NavMeshAgent component attached to this object
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = chaseSpeed;
            // Set momentum/inertia properties
            navMeshAgent.acceleration = acceleration;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.stoppingDistance = stoppingDistance;
        }

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void Update()
    {
        // Only chase if enemy is active
        if (!isActive || player == null || navMeshAgent == null)
            return;

        // Set the enemy's destination to the player's current position
        navMeshAgent.SetDestination(player.position);

        // Apply rolling rotation based on movement velocity
        ApplyRollingRotation();
    }

    private void ApplyRollingRotation()
    {
        // Get the current velocity of the NavMeshAgent
        Vector3 velocity = navMeshAgent.velocity;

        // Only rotate if the agent is actually moving
        if (velocity.magnitude > 0.1f)
        {
            // Calculate rotation amount based on velocity magnitude and roll speed
            float rotationAmount = (velocity.magnitude * rollSpeed) * Time.deltaTime;

            // Apply rotation on the specified axis
            // The rotation direction is automatically correct because it follows the velocity direction
            transform.Rotate(rotationAxis * rotationAmount, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the enemy touched the player
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy touched the player! Player loses!");
            
            // Notify GameManager before destroying the player
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.PlayerCaught();
            }
            
            // Remove the player from the game
            Destroy(other.gameObject);
        }
    }

    // Called by GameManager when stage is complete
    public void Deactivate()
    {
        isActive = false;
        
        // Stop the NavMeshAgent
        if (navMeshAgent != null)
            navMeshAgent.enabled = false;

        // Disable the enemy GameObject
        gameObject.SetActive(false);
        Debug.Log($"Enemy {name} has been deactivated!");
    }

    // Called by GameManager when initializing a stage
    public void Reactivate()
    {
        isActive = true;

        // Re-enable the NavMeshAgent
        if (navMeshAgent != null)
            navMeshAgent.enabled = true;

        // Ensure the enemy is visible
        gameObject.SetActive(true);
        Debug.Log($"Enemy {name} has been reactivated!");
    }
}