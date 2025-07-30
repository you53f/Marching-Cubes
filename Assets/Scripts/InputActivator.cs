using System;
using UnityEngine;

public class InputActivator : MonoBehaviour
{
    public static Action<Vector3> MouseTouching;

    [Header("Burr Settings")]
    public int brushSize;
    public float brushStrength;
    public float brushFallback;
    public float bufferBeforeDestroy;

    public enum InputMethod { Mouse, Controllers, Haptic }
    public enum Magnification { x1, x2 }
    [SerializeField] public InputMethod inputMethods;
    [SerializeField] public Magnification magnification;

    [Header("Original Size")]
    [SerializeField] private GameObject controllerHandpiece;
    [SerializeField] private GameObject hapticHandpiece;
    [SerializeField] private GameObject molar;

    [Header("2x Size")]
    [SerializeField] private GameObject controllerHandpiece2x;
    [SerializeField] private GameObject hapticHandpiece2x;
    [SerializeField] private GameObject molar2x;

    private int inputSelector;
    [HideInInspector] public int magnificationSelector;

    void Start()
    {
        switch (magnification)
        {
            case Magnification.x1:
                magnificationSelector = 1;
                break;
            case Magnification.x2:
                magnificationSelector = 2;
                break;
        }

        switch (inputMethods)
        {
            case InputMethod.Mouse:
                Mouse();
                inputSelector = 2;
                // Debug.Log("Mouse input");
                break;
            case InputMethod.Controllers:
                Controllers(magnification);
                inputSelector = 0;
                // Debug.Log("Controller input");
                break;
            case InputMethod.Haptic:
                Haptics();
                inputSelector = 1;
                // Debug.Log("Haptic input");
                break;

        }
    }

    void Update()
    {
        if (inputSelector == 2)
        {
            if (Input.GetMouseButton(0))
                ClickingWithMouse();
        }
    }

    private void ClickingWithMouse()
    {
        // Debug.Log("Inside Mouse input");
        RaycastHit hitInfo;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        if (hitInfo.collider == null)
        {
            Debug.Log("no hit");
            return;
        }

        Debug.Log($"hit with mouse at {hitInfo.point}");
        MouseTouching?.Invoke(hitInfo.point);

    }

    private void Mouse()
    {
        // controllerInput.SetActive(false);
        // controllerHandpiece.SetActive(false);
        // hapticInput.SetActive(false);
        // hapticHandpiece.SetActive(false);
    }

    private void Controllers(Magnification magnification)
    {
        switch (magnification)
        {
            case Magnification.x1:
                controllerHandpiece.SetActive(true);
                hapticHandpiece.SetActive(false);
                molar.SetActive(true);

                controllerHandpiece2x.SetActive(false);
                hapticHandpiece2x.SetActive(false);
                molar2x.SetActive(false);
                break;

            case Magnification.x2:
                controllerHandpiece.SetActive(false);
                hapticHandpiece.SetActive(false);
                molar.SetActive(false);

                controllerHandpiece2x.SetActive(true);
                hapticHandpiece2x.SetActive(false);
                molar2x.SetActive(true);
                break;
        }
    }

    private void Haptics()
    {
        switch (magnification)
        {
            case Magnification.x1:
                controllerHandpiece.SetActive(false);
                hapticHandpiece.SetActive(true);
                molar.SetActive(true);

                controllerHandpiece2x.SetActive(false);
                hapticHandpiece2x.SetActive(false);
                molar2x.SetActive(false);
                break;

            case Magnification.x2:
                controllerHandpiece.SetActive(false);
                hapticHandpiece.SetActive(false);
                molar.SetActive(false);

                controllerHandpiece2x.SetActive(false);
                hapticHandpiece2x.SetActive(true);
                molar2x.SetActive(true);

                break;
        }
    }
}
