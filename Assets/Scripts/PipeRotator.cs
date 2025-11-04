using UnityEngine;

public class PipeRotator : MonoBehaviour
{
    [Header("Rotation Settings")] public float rotationSpeed = 100f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("Physics Settings")] public float pushForce = 10f;
    public float pushUpwardForce = 5f;

    private void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 directionAway = (other.transform.position - transform.position).normalized;
                Vector3 pushDirection = new Vector3(directionAway.x, 0, directionAway.z).normalized;

                rb.linearVelocity = new Vector3(
                    pushDirection.x * pushForce,
                    rb.linearVelocity.y + pushUpwardForce * Time.deltaTime,
                    pushDirection.z * pushForce
                );
            }
        }
    }
}