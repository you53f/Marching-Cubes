using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualInputManager : MonoBehaviour
{
    private bool clicking;

    [Header("Tool Settings")]
    public int brushSize;
    public float brushStrength;
    public float brushFallback;
    public float bufferBeforeDestroy;

    [Header("Actions")]
    public static Action<Vector3> onTouching;
    [SerializeField] private float extentFactor = 0.1f;
    [SerializeField] private GameObject handpiece;
    [SerializeField] Collider hapticCollider;
    public enum InputMethod { Mouse, Controllers, Haptic }

    [SerializeField] private InputMethod inputMethods;
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
        switch (inputMethods)
        {
            case InputMethod.Mouse:
                if (Input.GetMouseButton(0))
                    ClickingWithMouse();
                // Debug.Log("Mouse input");
                break;
            case InputMethod.Controllers:
                Controllers();
                // Debug.Log("Controller input");
                break;
            case InputMethod.Haptic:
                Haptics();
                // Debug.Log("Haptic input");
                break;
        }
    }

    private void ClickingWithMouse()
    {
        GameObject touchActor = GameObject.Find("TouchActor");
        if (touchActor == null)
        {
            // Debug.Log("TouchActor not found!");
        }
        else
        {
            touchActor.SetActive(false);
            // Debug.Log("TouchActor found!");
        }

        
        // Debug.Log("Inside Mouse input");
        RaycastHit hitInfo;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        if (hitInfo.collider == null)
        {
            //Debug.Log("no hit");
            return;
        }

        // Debug.Log($"hit with mouse at {hitInfo.point}");
        onTouching?.Invoke(hitInfo.point);

        /*
        
        The ? after onTouching is called the null-conditional operator. It checks if onTouching is null
        (meaning it doesn't point to any valid action). If onTouching is null, the rest of the line won't be executed
        
        Inside the Invoke() parentheses, hitInfo.point is passed as an argument. It represents the 3D position in the scene where the ray hit an object.
        
        */

    }

    private void Controllers()
    {
        GameObject touchActor = GameObject.Find("TouchActor");
        if (touchActor == null)
        {
            // Debug.LogError("TouchActor not found!");
        }
        else
        {
            touchActor.SetActive(false);
            // Debug.Log("TouchActor found!");
        }
        
        // Check if handpiece has a SphereCollider component
        BoxCollider boxCollider = handpiece.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            // Debug.LogError("handpiece does not have a Capsule Collider component!");
        }

        // Create an array to hold all colliders that overlap with the sphere collider
        Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);
        Vector3 halfExtents = boxCollider.size * extentFactor;

        // Create an array to hold all colliders that overlap with the box collider
        Collider[] overlappingColliders = Physics.OverlapBox(center, halfExtents, boxCollider.transform.rotation);

        // Debug.Log($"Overlapping colliders: {overlappingColliders.Length}");

        // Check if there are any overlapping colliders
        if (overlappingColliders.Length > 0)
        {
            // foreach (var collider in overlappingColliders)
            // {
            // Check if the other collider's GameObject has the tag "Cubes"
            if (overlappingColliders[0].gameObject != handpiece && overlappingColliders[0].CompareTag("Cubes"))
            {
                // Log the position of the overlapping collider
                // Debug.Log($"Overlap detected with: {collider.gameObject.name} at position: {collider.transform.position}");
                onTouching?.Invoke(overlappingColliders[0].transform.position);
            }
            // }
        }
        else
        {
            // Debug.Log("No overlaps detected.");
        }
    }

     private void Haptics()
    {
        // Check if Wigglyhandpiece has a SphereCollider component
        BoxCollider boxCollider = handpiece.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            // Debug.LogError("handpiece does not have a Capsule Collider component!");
        }

        // Create an array to hold all colliders that overlap with the sphere collider
        Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);
        Vector3 halfExtents = boxCollider.size * extentFactor;

        // Create an array to hold all colliders that overlap with the box collider
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, boxCollider.transform.rotation);

        // Debug.Log($"Overlapping colliders: {overlappingColliders.Length}");

        // Check if there are any overlapping colliders
        if (hitColliders.Length > 0)
        {
            // foreach (var hit in hitColliders)
            // {
                // Ensure we don't log the HapticCollider itself
                if (hitColliders[0].gameObject != hapticCollider.gameObject)
                {
                    onTouching?.Invoke(hitColliders[0].transform.position);
                }
            // }
        }
    }
}
