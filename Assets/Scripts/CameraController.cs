using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    private void Start()
    {
        if (player != null)
            offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update, ensuring the camera follows smoothly after player movement
    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }
}
