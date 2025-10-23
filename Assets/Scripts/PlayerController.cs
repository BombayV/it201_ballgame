using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Rigidbody of the player.
    private Rigidbody rb;

    // Movement along X and Y axes.
    private float movementX;
    private float movementY;

    // Speed at which the player moves.
    public float speed = 15f;

    // Start is called before the first frame update.
    private void Start()
    {
        // Get and store the Rigidbody component attached to the player.
        Debug.Log("PlayerController Start");
        rb = GetComponent<Rigidbody>();
    }

    // This function is called when a move input is detected.
    public void OnMove(InputValue movementValue)
    {
        // Convert the input value into a Vector2 for movement.
        Vector2 movementVector = movementValue.Get<Vector2>();

        // Store the X and Y components of the movement.
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // FixedUpdate is called once per fixed frame-rate frame.
    private void FixedUpdate()
    {
        // Only apply force if there's actual input
        if (Mathf.Abs(movementX) > 0.01f || Mathf.Abs(movementY) > 0.01f)
        {
            // Create a 3D movement vector using the X and Y inputs.
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);

            // Apply force to the Rigidbody to move the player.
            rb.AddForce(movement * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit a box
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