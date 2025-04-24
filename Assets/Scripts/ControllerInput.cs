using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerInput : MonoBehaviour
{
    public static Action<Vector3> onTouching;

    SphereCollider sphereCollider;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] float sizeFactor = 0.0025f;
    [SerializeField][Range(0, 1)] float hapticStrength = 0.5f;
    [SerializeField][Range(0, 1)] float hapticDuration = 0.5f;
    [SerializeField] XRBaseController controller;
    [HideInInspector] public bool floorHit = false;
    [HideInInspector] public bool diffHit = false;


    void Start()
    {
        // Get the SphereCollider component attached to this GameObject
        sphereCollider = GetComponent<SphereCollider>();
    }
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(sphereCollider.transform.position, sphereCollider.radius * sizeFactor);
        // Debug.Log($"Overlapping colliders: {hitColliders.Length}");

        // Check if there are any overlapping colliders
        if (hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (!hitColliders[i].CompareTag("Burr") && hitColliders[i].gameObject.name != "Right Controller" && hitColliders[i].gameObject.name != "Left Controller")
                {
                    if (hitColliders[i].CompareTag("Renderer"))
                    {
                        // Debug.Log($"Overlap detected with: {hitColliders[i].gameObject.name} at position: {hitColliders[i].transform.position}");

                        onTouching?.Invoke(hitColliders[i].ClosestPoint(transform.position));
                        // Trigger haptic feedback on the controller
                        TriggerHapticFeedback(controller);
                        i = 100000;
                    }
                }
            }
        }
        else
        {
            textMeshProUGUI.text = "New Text";
            // Debug.Log("No overlaps detected.");
        }
    }

    public void TriggerHapticFeedback(XRBaseController controller)
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(hapticStrength, hapticDuration);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            floorHit = true;

            textMeshProUGUI.text = "Collided with Floor";
        }
        else if (other.CompareTag("Diff"))
        {
            diffHit = true;

            textMeshProUGUI.text = "Collided with Diff";
        }
    }
}