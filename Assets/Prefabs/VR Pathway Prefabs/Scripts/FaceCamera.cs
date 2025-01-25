using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Reference to the Camera (or VR camera)
    public Camera vrCamera;

    // Update is called once per frame
    void Update()
    {
        // Check if the vrCamera is assigned, otherwise try to get the main camera
        if (vrCamera == null)
        {
            vrCamera = Camera.main;
        }

        // Make the canvas rotate only on the Y-axis to face the camera
        if (vrCamera != null)
        {
            Vector3 direction = vrCamera.transform.position - transform.position;
            direction.y = 0; // Keep the direction horizontal by zeroing the Y component

            // Set the rotation to face the camera horizontally
            transform.rotation = Quaternion.LookRotation(direction);
            // Rotate 180 degrees around the Y-axis to face the camera directly
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
