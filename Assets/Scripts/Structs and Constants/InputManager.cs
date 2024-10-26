using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool clicking;

    [Header("Actions")]
    public static Action<Vector3> onTouching;

    // Start is called before the first frame update
    void Start()
    {
        // Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            Clicking();

    }

    private void Clicking()
    {
        RaycastHit hitInfo;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        if (hitInfo.collider == null)
        {
            //Debug.Log("no hit");
            return;
        }

        onTouching?.Invoke(hitInfo.point);

        /*
        
        The ? after onTouching is called the null-conditional operator. It checks if onTouching is null
        (meaning it doesn't point to any valid action). If onTouching is null, the rest of the line won't be executed
        
        Inside the Invoke() parentheses, hitInfo.point is passed as an argument. It represents the 3D position in the scene where the ray hit an object.
        
        */

    }
}
