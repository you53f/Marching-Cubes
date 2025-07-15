using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerInput : MonoBehaviour
{
    public static Action<Vector3> onTouching;

    BoxCollider boxCollider;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] Vector3 sizeFactor;
    [SerializeField][Range(0, 1)] float hapticStrength = 0.5f;
    [SerializeField][Range(0, 1)] float hapticDuration = 0.5f;
    [SerializeField] XRBaseController controller;
    [HideInInspector] public bool floorHit = false;
    [HideInInspector] public bool diffHit = false;
    private bool holding = false;
    private Coroutine hapticCoroutine;
    private bool on = false;
    [SerializeField] private GameObject cubeCheck;
    MeshRenderer meshRenderer;

    void Start()
    {
        // Get the SphereCollider component attached to this GameObject
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = cubeCheck.GetComponent<MeshRenderer>();
    }
    void Update()
    {
        if (on)
        {
            Collider[] hitColliders = Physics.OverlapBox(boxCollider.transform.position,
                new Vector3(boxCollider.size.x * sizeFactor.x, boxCollider.size.y * sizeFactor.y, boxCollider.size.z * sizeFactor.z),
                boxCollider.transform.rotation);
            // Debug.Log($"Overlapping colliders: {hitColliders.Length}");

            // Check if there are any overlapping colliders
            if (hitColliders.Length > 0)
            {
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    // Debug.Log($"Overlapped voxels = {hitColliders.Length}");
                    if (!hitColliders[i].CompareTag("Burr") && hitColliders[i].gameObject.name != "Right Controller" && hitColliders[i].gameObject.name != "Left Controller")
                    {
                        if (hitColliders[i].CompareTag("Cubes"))
                        {
                            // Debug.Log($"Overlap detected with: {hitColliders[i].gameObject.name} at position: {hitColliders[i].transform.position}");

                            // onTouching?.Invoke(hitColliders[i].ClosestPoint(transform.position));
                            onTouching?.Invoke(hitColliders[i].transform.position);
                            // Trigger haptic feedback on the controller
                            TriggerHapticFeedback(controller);
                            i += 100;

                            textMeshProUGUI.text = "Renderer Hit";
                            meshRenderer.material.color = Color.blue;
                        }
                    }
                }
            }
            else
            {
                textMeshProUGUI.text = "New Text";
                Debug.Log("No overlapping colliders found.");
            }
        }
        else
        {
            meshRenderer.material.color = Color.red;
        }
    }

    public void OnState()
    {
        on = true;
    }

    public void OffState()
    {
        on = false;
    }

    public void StartIdleHaptic()
    {
        if (hapticCoroutine == null)
        {
            hapticCoroutine = StartCoroutine(IdleHapticRoutine());
        }
    }

    public void StopIdleHaptic()
    {
        if (hapticCoroutine != null)
        {
            StopCoroutine(hapticCoroutine);
            hapticCoroutine = null;
        }
    }

    private IEnumerator IdleHapticRoutine()
    {
        while (true)
        {
            if (controller != null)
            {
                controller.SendHapticImpulse(hapticStrength, hapticDuration);
            }
            yield return new WaitForSeconds(hapticDuration);
        }
    }


    public void TriggerHapticFeedback(XRBaseController controller)
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(hapticStrength * 4, hapticDuration);
        }
    }

    public void IdleHapticFeedback(XRBaseController controller)
    {
        while (holding)
        {
            if (controller != null)
            {
                controller.SendHapticImpulse(hapticStrength, hapticDuration);
            }
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