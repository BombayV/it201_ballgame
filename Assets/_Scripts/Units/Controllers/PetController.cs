using UnityEngine;

public class PetController : Singleton<PetController>
{
    [Header("Pet Follow Settings")] 
    public Transform player;
    public float followSpeed = 8f;
    public float stoppingDistance = 2f;
    public float rotationSpeed = 5f;

    [Header("Bobbing Animation")] 
    public float bobHeight = 0.5f;
    public float bobSpeed = 2f;
    private float bobOffset = 0f;

    private Vector3 targetOffset = new Vector3(0, 2f, 0);
    private Vector3 lastPlayerPosition;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        lastPlayerPosition = player != null ? player.position : transform.position;
    }

    private void Update()
    {
        if (player == null)
            return;

        DetectTeleport();
        FollowPlayer();
        BobAnimation();
    }

    private void DetectTeleport()
    {
        Vector3 playerPos = player.position;
        float distance = Vector3.Distance(lastPlayerPosition, playerPos);

        if (distance > 50f)
        {
            transform.position = playerPos + targetOffset;
        }

        lastPlayerPosition = playerPos;
    }

    private void FollowPlayer()
    {
        if (player == null)
            return;

        Vector3 targetPosition = player.position + targetOffset;
        targetPosition.y += bobOffset;

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > stoppingDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * (followSpeed * Time.deltaTime);
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void BobAnimation()
    {
        bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
    }
}