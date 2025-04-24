using System;
using UnityEngine;

public class InputActivator : MonoBehaviour
{
    [Header("Burr Settings")]
    public int brushSize;
    public float brushStrength;
    public float brushFallback;
    public float bufferBeforeDestroy;

    [Header("Activated Objects")]
    [SerializeField] private GameObject controllerHandpiece;
    [SerializeField] private GameObject hapticHandpiece;
    [SerializeField] private GameObject controllerInput;
    [SerializeField] private GameObject hapticInput;

    public enum InputMethod { Controllers, Haptic }
    [SerializeField] public InputMethod inputMethods;
    [HideInInspector] public int selector;

    void Start()
    {
        switch (inputMethods)
        {
            case InputMethod.Controllers:
                Controllers();
                selector = 0;
                // Debug.Log("Controller input");
                break;
            case InputMethod.Haptic:
                Haptics();
                selector = 1;
                // Debug.Log("Haptic input");
                break;
        }
    }

    private void Controllers()
    {
        controllerInput.SetActive(true);
        controllerHandpiece.SetActive(true);
        hapticInput.SetActive(false);
        hapticHandpiece.SetActive(false);
    }

    private void Haptics()
    {
        controllerInput.SetActive(false);
        controllerHandpiece.SetActive(false);
        // hapticInput.SetActive(true);
        hapticHandpiece.SetActive(true);
    }
}
