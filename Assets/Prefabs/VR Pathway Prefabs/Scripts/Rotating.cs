using UnityEngine;

public class RotateAroundYAxis : MonoBehaviour
{
    public float speed = 10f; // Adjust the rotation speed as needed

    void Update()
    {
        // Rotate the object around the Y axis by the specified speed
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}