using UnityEngine;
using UnityEngine.XR.OpenXR.Input;

public class HapticCollisionHandler : MonoBehaviour
{
    [Header("Collider Settings")]
    [SerializeField] private bool enableHaptics;
    [Range(0,1)] [SerializeField] float stiffness;
    [Range(0,1)] [SerializeField] float damping;
    [Range(0,1)] [SerializeField] private float viscosity;
    [Range(0,1)] [SerializeField] private float staticFriction;
    [Range(0,1)] [SerializeField] private float dynamicFriction;
    [SerializeField] Vector3 force;
    [SerializeField] float forceMagnitude;
    [SerializeField] private bool useContactNormal;
    [SerializeField] private bool useContactNormalInverse;
    [Range(0,7)] [SerializeField] private float popThrough;
    [SerializeField] private Collider bufferCollider;

    void Start()
    {
        InitializeCollider();
    }

    void InitializeCollider()
    {
        // Auto-get collider if not assigned
        if (bufferCollider == null)
            bufferCollider = GetComponent<SphereCollider>();

        // Ensure proper trigger setup
        if (bufferCollider != null)
        {
            bufferCollider.isTrigger = true;
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


        if (hapticComponent == null && activate)
        {
            hapticComponent = cubeCollider.gameObject.AddComponent<HapticMaterial>();

            SetHaptics(hapticComponent);
            // Handle component state
            if (hapticComponent != null)
            {
                if (activate)
                {
                    hapticComponent.enabled = true;
                    SetHaptics(hapticComponent);
                }
                else
                {
                    hapticComponent.enabled = false;
                }
            }
        }
    }

    private void SetHaptics(HapticMaterial hapticComponent)
    {
        if (enableHaptics)
            {
                hapticComponent.hStiffness = stiffness;
                hapticComponent.hDamping = damping;
                hapticComponent.hViscosity = viscosity;
                hapticComponent.hFrictionS = staticFriction;
                hapticComponent.hFrictionD = dynamicFriction;
                hapticComponent.hConstForceDir = force;
                hapticComponent.hConstForceMag = forceMagnitude;
                hapticComponent.UseContactNormalCF = useContactNormal;
                hapticComponent.ContactNormalInverseCF = useContactNormalInverse;
                hapticComponent.hPopthAbs = popThrough;
            }

            else
            {
                hapticComponent.hStiffness = 0;
                hapticComponent.hDamping = 0;
                hapticComponent.hViscosity = 0;
                hapticComponent.hFrictionS = 0;
                hapticComponent.hFrictionD = 0;
                hapticComponent.hConstForceDir = Vector3.zero;
                hapticComponent.hConstForceMag = 0;
                hapticComponent.UseContactNormalCF = useContactNormal;
                hapticComponent.ContactNormalInverseCF = useContactNormalInverse;
                hapticComponent.hPopthAbs = 0;
            }
    }
}
