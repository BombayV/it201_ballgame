using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Setup")]
    public Transform player;
    public float chaseSpeed = 5f;

    [Header("Momentum Settings")]
    public float acceleration = 2f;
    public float angularSpeed = 50f;
    public float stoppingDistance = 0.5f;

    private NavMeshAgent navMeshAgent;
    private bool isActive = true;

    [Header("Rolling Settings")]
    public float rollSpeed = 5f;
    public Vector3 rotationAxis = Vector3.right;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = chaseSpeed;
            navMeshAgent.acceleration = acceleration;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.stoppingDistance = stoppingDistance;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (!isActive || player == null || navMeshAgent == null)
            return;

        navMeshAgent.SetDestination(player.position);
        ApplyRollingRotation();
    }

    private void ApplyRollingRotation()
    {
        Vector3 velocity = navMeshAgent.velocity;

        if (velocity.magnitude > 0.1f)
        {
            float rotationAmount = (velocity.magnitude * rollSpeed) * Time.deltaTime;
            transform.Rotate(rotationAxis * rotationAmount, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy touched the player! Player loses!");
            
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.PlayerCaught();
            }
            
            Destroy(other.gameObject);
        }
    }

    public void Deactivate()
    {
        isActive = false;
        
        if (navMeshAgent != null)
            navMeshAgent.enabled = false;

        gameObject.SetActive(false);
        Debug.Log($"Enemy {name} has been deactivated!");
    }

    public void Reactivate()
    {
        isActive = true;

        if (navMeshAgent != null)
            navMeshAgent.enabled = true;

        gameObject.SetActive(true);
        Debug.Log($"Enemy {name} has been reactivated!");
    }
}