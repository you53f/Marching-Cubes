using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class HapticInput : MonoBehaviour
{
    public static Action<Vector3> onTouching;

    BoxCollider boxCollider;
    [SerializeField] Vector3 sizeFactor;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [HideInInspector] public bool floorHit = false;
    [HideInInspector] public bool diffHit = false;


    // [SerializeField] private GameObject cubeCheck;
    // MeshRenderer meshRenderer;
    void Start()
    {
        // Get the SphereCollider component attached to this GameObject
        boxCollider = GetComponent<BoxCollider>();

        // meshRenderer = cubeCheck.GetComponent<MeshRenderer>();
    }
    void Update()
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
                if (!hitColliders[i].CompareTag("Burr") && hitColliders[i].gameObject.name != "Right Controller" && hitColliders[i].gameObject.name != "Left Controller")
                {
                    if (hitColliders[i].CompareTag("Cubes"))
                    {
                        // Debug.Log($"Overlap detected with: {hitColliders[i].gameObject.name} at position: {hitColliders[i].transform.position}");

                        textMeshProUGUI.text = hitColliders[i].gameObject.name;
                        // meshRenderer.material.color = Color.blue;
                        // onTouching?.Invoke(hitColliders[i].ClosestPoint(transform.position));
                        onTouching?.Invoke(hitColliders[i].transform.position);
                        i += 100;
                    }
                    else if (hitColliders[i].CompareTag("Floor"))
                    {
                        floorHit = true;
                        textMeshProUGUI.text = "Collided with Floor";
                        i += 100;
                    }
                    else if (hitColliders[i].CompareTag("Diff"))
                    {
                        diffHit = true;
                        textMeshProUGUI.text = "Collided with Diff";
                        i += 100;
                    }
                    // else
                    // textMeshProUGUI.text = "New Text";
                    // meshRenderer.material.color = Color.red;
                }
            }
        }
        else
        {
            textMeshProUGUI.text = "New Text";
            // meshRenderer.material.color = Color.red;
            // Debug.Log("No overlaps detected.");
        }
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

    // void OnTriggerExit(Collider other)
    // {
    //     meshRenderer.material.color = Color.red;
    //     textMeshProUGUI.text = "New Text";
    // }
}