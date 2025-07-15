using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToothMover : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform molar;
    [SerializeField] private Transform positioner;
    [SerializeField] private Transform molar2x;
    [SerializeField] private Transform positioner2x;
    [SerializeField] private Transform molar4x;
    [SerializeField] private Transform positioner4x;
    private Transform transformToMove;
    private Transform positionerToMove;
    [SerializeField] private float moveAmount;
    [SerializeField] private float rotateAmount;

    InputActivator inputActivator;

    void Start()
    {
        Invoke("DelayedStart", 2.0f);
    }

    void DelayedStart()
    {
        inputActivator = FindObjectOfType<InputActivator>();

        switch (inputActivator.magnificationSelector)
        {
            case 1:
                transformToMove = molar;
                positionerToMove = positioner;
                break;
            case 2:
                transformToMove = molar2x;
                positionerToMove = positioner2x;
                break;
            case 4:
                transformToMove = molar4x;
                positionerToMove = positioner4x;
                break;
            default:
                Debug.LogError("Invalid magnification selector");
                break;
        }
    }

    public void MoveUp()
    {
        if (transformToMove == null)
        {
            Debug.LogError("Missing required references in ToothMover");
            return;
        }
        Vector3 localUp = positionerToMove.TransformDirection(Vector3.up);
        transformToMove.Translate(localUp * moveAmount, Space.World);
        positioner.Translate(Vector3.up * moveAmount, Space.Self);
    }

    public void MoveDown()
    {
        if (transformToMove == null)
        {
            Debug.LogError("Missing required references in ToothMover");
            return;
        }
        Vector3 localDown = positionerToMove.TransformDirection(Vector3.down);
        transformToMove.Translate(localDown * moveAmount, Space.World);
        positionerToMove.Translate(Vector3.down * moveAmount, Space.Self);
    }

    public void MoveRight()
    {
        if (transformToMove == null)
        {
            Debug.LogError("Missing required references in ToothMover");
            return;
        }
        Vector3 localRight = positionerToMove.TransformDirection(Vector3.right);
        transformToMove.Translate(localRight * moveAmount, Space.World);
        positionerToMove.Translate(Vector3.right * moveAmount, Space.Self);
    }

    public void MoveLeft()
    {
        if (transformToMove == null)
        {
            Debug.LogError("Missing required references in ToothMover");
            return;
        }
        Vector3 localLeft = positionerToMove.TransformDirection(Vector3.left);
        transformToMove.Translate(localLeft * moveAmount, Space.World);
        positionerToMove.Translate(Vector3.left * moveAmount, Space.Self);
    }

    public void TipBackward()
    {
        if (transformToMove == null)
        {
            Debug.LogError("Missing required references in ToothMover");
            return;
        }
        Vector3 localRight = positionerToMove.TransformDirection(Vector3.right);
        transformToMove.Rotate(localRight * rotateAmount, Space.World);
    }
    public void TipForward()
    {
        if (transformToMove == null)
        {
            Debug.LogError("Missing required references in ToothMover");
            return;
        }
        Vector3 localLeft = positionerToMove.TransformDirection(Vector3.left);
        transformToMove.Rotate(localLeft * rotateAmount, Space.World);
    }

    public void RotateLeft()
    {
        if (transformToMove == null)
        {
            Debug.LogError("Missing required references in ToothMover");
            return;
        }
        Vector3 localUp = positionerToMove.TransformDirection(Vector3.up);
        transformToMove.Rotate(localUp * rotateAmount, Space.World);
    }

    public void RotateRight()
    {
        if (transformToMove == null)
        {
            Debug.LogError("Missing required references in ToothMover");
            return;
        }
        Vector3 localDown = positionerToMove.TransformDirection(Vector3.down);
        transformToMove.Rotate(localDown * rotateAmount, Space.World);
    }
}
