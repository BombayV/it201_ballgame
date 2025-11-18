using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 20f;

    [Header("Camera References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;

    private float currentVerticalAngle = 0f;
    private float currentHorizontalAngle = 0f;
    private float currentZoom = 10f;

    private void Start()
    {
        // Get the main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Initialize angles from current rotation
        currentVerticalAngle = transform.eulerAngles.x;
        if (currentVerticalAngle > 180)
            currentVerticalAngle -= 360;

        currentHorizontalAngle = transform.eulerAngles.y;
    }

    private void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Check if right mouse button is held
        if (Input.GetMouseButton(1))
        {
            // Update rotation angles
            currentHorizontalAngle += mouseX * rotationSpeed;
            currentVerticalAngle -= mouseY * rotationSpeed;

            // Clamp vertical angle
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
        }

        // Calculate target position (use target if available, otherwise origin)
        Vector3 targetPos = target != null ? target.position : Vector3.zero;

        // Create rotation and apply offset from target
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0f);
        Vector3 offset = rotation * Vector3.back * currentZoom;
        transform.position = targetPos + offset;

        // Always look at the target
        transform.LookAt(targetPos);
    }

    private void HandleZoom()
    {
        // Get mouse scroll input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            // Update zoom distance
            currentZoom -= scrollInput * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            // Position will be updated in HandleRotation
        }
    }

    /// <summary>
    /// Set the zoom level to a specific distance
    /// </summary>
    public void SetZoom(float zoomDistance)
    {
        currentZoom = Mathf.Clamp(zoomDistance, minZoom, maxZoom);
    }

    /// <summary>
    /// Reset camera to default position and rotation
    /// </summary>
    public void ResetCamera()
    {
        currentVerticalAngle = 30f;
        currentHorizontalAngle = 0f;
        currentZoom = 10f;
    }

    /// <summary>
    /// Get current zoom level
    /// </summary>
    public float GetCurrentZoom()
    {
        return currentZoom;
    }
}