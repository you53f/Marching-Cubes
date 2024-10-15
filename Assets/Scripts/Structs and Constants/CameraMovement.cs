using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the camera movement
    public float mouseSensitivity = 2f; // Sensitivity of mouse movement
    public float verticalRotationLimit = 80f; // Limit for vertical rotation
    public float verticalMoveSpeed = 2f; // Speed of vertical movement

    private float rotationX = 0f; // Store the vertical rotation

    void Update()
    {
        // Get input from WASD keys
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveVertical = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow

        // Get input for vertical movement
        float moveUp = 0f;
        if (Input.GetKey(KeyCode.E))
        {
            moveUp = 1f;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            moveUp = -1f;
        }

        // Calculate the forward and right directions based on the camera's rotation
        Vector3 forward = transform.forward; // Forward direction
        Vector3 right = transform.right; // Right direction

        // Create a movement vector based on the camera's orientation
        Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized;

        // Move the camera horizontally
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // Move the camera vertically
        transform.Translate(Vector3.up * moveUp * verticalMoveSpeed * Time.deltaTime, Space.World);

        // Get mouse input
        float mouseX = 0f;
        float mouseY = 0f;

        // Check if spacebar is not pressed
        if (Input.GetMouseButton(1))
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        }

        // Update the vertical rotation and clamp it
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -verticalRotationLimit, verticalRotationLimit);

        // Apply the rotation
        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y + mouseX, 0f);
    }
}