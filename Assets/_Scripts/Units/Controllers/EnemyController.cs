using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Setup")] 
    public Transform player;

    private NavMeshAgent navMeshAgent;
    private bool isActive = true;

    private float nextTouchCheckTime = 0f;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Time.time < nextTouchCheckTime)
                return;

            nextTouchCheckTime = Time.time + 2f;
            GameManager.Instance.PlayerCaught();
        }
    }
}