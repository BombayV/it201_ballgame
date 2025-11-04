using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 15f;
    public float jumpForce = 20f;
    public float dashForce = 50f;

    private float dashCooldown = 2f;
    private float lastDashTime = -Mathf.Infinity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(movementX) > 0.01f || Mathf.Abs(movementY) > 0.01f)
        {
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);
            rb.AddForce(movement * speed);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            Debug.Log("Jump");
            rb.AddForce(Vector3.up * jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            Vector3 dashDirection = new Vector3(movementX, 0, movementY).normalized;
            if (dashDirection != Vector3.zero)
            {
                rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
                lastDashTime = Time.time;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CollectibleBox"))
        {
            BoxController box = other.gameObject.GetComponent<BoxController>();
            Debug.Log("Collided with box");
            if (box != null)
            {
                box.CollectBox();
            }
        }
    }
}