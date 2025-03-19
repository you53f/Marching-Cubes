using UnityEngine;

public class VRRotate : MonoBehaviour
{
    [Tooltip("Reference to the XR Origin/Rig Transform")]
    [SerializeField] Transform xrRigTransform;
    [SerializeField] private int rotateAngle;

    public void RotateRight()
    {
        if (xrRigTransform != null)
        {
            // Rotate the XR Rig around the Y-axis by 45 degrees
            xrRigTransform.Rotate(Vector3.up, rotateAngle);
            // Debug.Log("Rotating Right");
        }
        else
        {
            // Debug.LogError("XR Rig Transform reference not set!", this);
        }
    }
    public void RotateLeft()
    {
        if (xrRigTransform != null)
        {
            // Rotate the XR Rig around the Y-axis by 45 degrees
            xrRigTransform.Rotate(Vector3.up, -rotateAngle);
            // Debug.Log("Rotating Left");
        }
        else
        {
            // Debug.LogError("XR Rig Transform reference not set!", this);
        }
    }
}
