using System;
using TMPro;
using UnityEngine;

public class HapticInput : MonoBehaviour
{
    public static Action<Vector3> onTouching;

    SphereCollider sphereCollider;
    
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [HideInInspector] public bool floorHit = false;
    [HideInInspector] public bool diffHit = false;
    [SerializeField] float sizeFactor;
    [SerializeField] Vector3 compensate;
    void Start()
    {
        // Get the SphereCollider component attached to this GameObject
        sphereCollider = GetComponent<SphereCollider>();
    }
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(sphereCollider.transform.position - compensate, sphereCollider.radius * sizeFactor);
        //     Debug.Log($"Overlapping colliders: {hitColliders.Length}");

        // Check if there are any overlapping colliders
        if (hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].CompareTag("Renderer"))
                {
                    // Debug.Log($"Overlap detected with: {hitColliders[i].gameObject.name} at position: {hitColliders[i].ClosestPoint(transform.position)}");
                    onTouching?.Invoke(hitColliders[i].ClosestPoint(transform.position));
                    i = 100000;
                }
            }
        }
        else
        {
            // Debug.Log("No overlaps detected.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, sphereCollider.radius * sizeFactor);
        Gizmos.color = Color.red;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            floorHit = true;

            // textMeshProUGUI.text = "Collided with Floor";
        }
        else if (other.CompareTag("Diff"))
        {
            diffHit = true;

            // textMeshProUGUI.text = "Collided with Diff";
        }
    }
}