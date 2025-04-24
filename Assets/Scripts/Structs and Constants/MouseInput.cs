using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    [Header("Tool Settings")]
    public int brushSize;
    public float brushStrength;
    public float brushFallback;
    public float bufferBeforeDestroy;

    [Header("Actions")]
    public static Action<Vector3> onTouching;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            ClickingWithMouse();
        // Debug.Log("Mouse input");
    }

    private void ClickingWithMouse()
    {
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
}
