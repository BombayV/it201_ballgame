using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 15f;

    private void Start()
    {
        Debug.Log("PlayerController Start");
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