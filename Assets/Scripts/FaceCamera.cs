using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Reference to the Camera (or VR camera)
    [SerializeField] private Camera vrCamera;

    // Easing speed factor
    [SerializeField] private float rotationSpeed;

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

            // Calculate the target rotation to face the camera
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
