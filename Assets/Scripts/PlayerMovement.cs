using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Public variables can be seen by other scripts are are visible in the editor
    public float movementSpeed = 1.0f;

    // Private variables can only be seen by this script
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    private void Start()
    {
        // Gets the rigidbody component that is responsible for managing the 2D physics of this object
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Caculate the movement vector using the horizontal and vertical input axies. Normalizing the input sets the magnitude of the vector as 1.
        Vector3 movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
        // Add a force based on the movement vector
        rigidBody.AddForce(movementVector * movementSpeed);
    }
}
