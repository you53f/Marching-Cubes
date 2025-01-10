using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualInputManager : MonoBehaviour
{
    private bool clicking;

    [Header("Actions")]
    public static Action<Vector3> onTouching;
    [SerializeField] private GameObject wigglyHandpiece;
    [SerializeField] private bool mouseInput;
    // private bool isColliding = false;
    // private Collider burrCollider;

    // Start is called before the first frame update
    void Start()
    {
        // Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButton(0))
        // {
        if (mouseInput)
        {
            ClickingWithMouse();
            // Debug.Log("Mouse input");
        }
        else
        {
            ClickingWithController();
            // Debug.Log("Controller input");
        }
        // }
    }

    private void ClickingWithMouse()
    {
        RaycastHit hitInfo;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        if (hitInfo.collider == null)
        {
            //Debug.Log("no hit");
            return;
        }

        Debug.Log($"hit with mouse at {hitInfo.point}");
        onTouching?.Invoke(hitInfo.point);

        /*
        
        The ? after onTouching is called the null-conditional operator. It checks if onTouching is null
        (meaning it doesn't point to any valid action). If onTouching is null, the rest of the line won't be executed
        
        Inside the Invoke() parentheses, hitInfo.point is passed as an argument. It represents the 3D position in the scene where the ray hit an object.
        
        */

    }

    private void ClickingWithController()
    {
        // Check if WigglyHandpiece has a SphereCollider component
        BoxCollider boxCollider = wigglyHandpiece.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("WigglyHandpiece does not have a Capsule Collider component!");
            return;
        }

        // Create an array to hold all colliders that overlap with the sphere collider
        Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);
        Vector3 halfExtents = boxCollider.size * 0.22f;

        // Create an array to hold all colliders that overlap with the box collider
        Collider[] overlappingColliders = Physics.OverlapBox(center, halfExtents, boxCollider.transform.rotation);

        // Debug.Log($"Overlapping colliders: {overlappingColliders.Length}");

        // Check if there are any overlapping colliders
        if (overlappingColliders.Length > 0)
        {
            foreach (var collider in overlappingColliders)
            {
                // Ignore the collider itself
                if (collider.gameObject == wigglyHandpiece)
                    continue;

                // Check if the other collider's GameObject has the tag "Cubes"
                if (collider.CompareTag("Cubes"))
                {
                    // Log the position of the overlapping collider
                    Debug.Log($"Overlap detected with: {collider.gameObject.name} at position: {collider.transform.position}");
                    onTouching?.Invoke(collider.transform.position);
                }
            }
        }
        else
        {
            Debug.Log("No overlaps detected.");
        }



        // if (burrCollider == null) return;

        // // Calculate the sphere radius as half of the bounds size of the mesh
        // float sphereRadius = burrCollider.bounds.extents.magnitude;

        // // Get the position of HapticCollider
        // Vector3 position = burrCollider.transform.position;

        // // Perform an overlap sphere check to find colliders within the sphere radius
        // Collider[] hitColliders = Physics.OverlapSphere(position, sphereRadius);

        // // Log hit information for each collider detected
        // if (hitColliders.Length > 0)
        // {
        //     foreach (var hit in hitColliders)
        //     {
        //         // Ensure we don't log the HapticCollider itself
        //         if (hit.gameObject != burrCollider.gameObject)
        //         {
        //             Debug.Log($"hit with mouse at {hit.transform.position}");
        //             onTouching?.Invoke(hit.transform.position);
        //         }
        //     }
        // }
    }
}
