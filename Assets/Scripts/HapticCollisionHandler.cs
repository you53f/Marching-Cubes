using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class HapticCollisionHandler : MonoBehaviour
{
    [Header("Collider Settings")]
    [Tooltip("Assign the sphere collider used for haptic interaction")]
    [SerializeField] private SphereCollider hapticCollider;

    [Header("Component Management")]
    [Tooltip("Should components be destroyed when not in contact?")]
    [SerializeField] private bool destroyComponentsWhenInactive = false;

    void Start()
    {
        InitializeCollider();
    }

    void InitializeCollider()
    {
        // Auto-get collider if not assigned
        if (hapticCollider == null)
            hapticCollider = GetComponent<SphereCollider>();

        // Ensure proper trigger setup
        if (hapticCollider != null)
        {
            hapticCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError("Missing SphereCollider reference!", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cubes"))
        {
            ManageHapticComponent(other, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cubes"))
        {
            ManageHapticComponent(other, false);
        }
    }

    void ManageHapticComponent(Collider cubeCollider, bool activate)
    {
        // Get or add component
        HapticMaterial hapticComponent = cubeCollider.GetComponent<HapticMaterial>();
        bool componentExisted = hapticComponent != null;

        if (!componentExisted && activate)
        {
            hapticComponent = cubeCollider.gameObject.AddComponent<HapticMaterial>();
        }

        // Handle component state
        if (hapticComponent != null)
        {
            if (activate)
            {
                hapticComponent.enabled = true;
            }
            else
            {
                if (destroyComponentsWhenInactive)
                {
                    Destroy(hapticComponent);
                }
                else
                {
                    hapticComponent.enabled = false;
                }
            }
        }
    }
}
