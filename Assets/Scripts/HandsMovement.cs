using UnityEngine;

public class HandsMovement : MonoBehaviour
{
    [SerializeField] Transform xrRigTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] private int rotateAngle;
    [SerializeField] private float moveSpeed;

    // Track movement state
    private bool isMovingForward = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

    void Update()
    {
        if (isMovingForward) MoveForwardContinuous();
        if (isRotatingLeft) RotateLeftContinuous();
        if (isRotatingRight) RotateRightContinuous();
    }

    // Called when "move forward" gesture starts
    public void StartMovingForward()
    {
        isMovingForward = true;
    }

    // Called when "move forward" gesture ends
    public void StopMovingForward()
    {
        isMovingForward = false;
    }

    private void MoveForwardContinuous()
    {
        if (xrRigTransform != null && cameraTransform != null)
        {
            Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            // Multiply by Time.deltaTime for frame-rate independent movement
            xrRigTransform.position += cameraForward * moveSpeed * Time.deltaTime;
        }
    }

    public void RotateRight()
    {
        if (xrRigTransform != null)
        {
            xrRigTransform.Rotate(Vector3.up, rotateAngle);
        }
    }

    public void RotateLeft()
    {
        if (xrRigTransform != null)
        {
            xrRigTransform.Rotate(Vector3.up, -rotateAngle);
        }
    }

    public void StartRotatingLeft() => isRotatingLeft = true;
    public void StopRotatingLeft() => isRotatingLeft = false;

    public void StartRotatingRight() => isRotatingRight = true;
    public void StopRotatingRight() => isRotatingRight = false;

    private void RotateLeftContinuous()
    {
        xrRigTransform.Rotate(Vector3.up, -rotateAngle * Time.deltaTime);
    }

    private void RotateRightContinuous()
    {
        xrRigTransform.Rotate(Vector3.up, rotateAngle * Time.deltaTime);
    }
}