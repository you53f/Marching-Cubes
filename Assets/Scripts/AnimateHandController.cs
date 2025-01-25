using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class AnimateHandController : MonoBehaviour
{
    public InputActionReference gripInputActionReference;
    public InputActionReference triggerInputActionReference;

    private Animator handAnimator;
    private float gripValue;
    private float triggerValue;

    // Start is called before the first frame update
    void Start()
    {
        handAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateGrip();
        AnimateTrigger();
    }

    private void AnimateGrip()
    {
        gripValue = gripInputActionReference.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }

    private void AnimateTrigger()
    {
        triggerValue = triggerInputActionReference.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);
    }
}
