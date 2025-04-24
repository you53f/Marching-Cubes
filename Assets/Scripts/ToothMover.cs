using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToothMover : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform transformToMove;
    [SerializeField] private float moveAmount;
    
    public void MoveUp()
    {
        if (transformToMove == null)
        {
            Debug.LogError("Missing required references in ToothMover");
            return;
        }

        transformToMove.position += Vector3.up * moveAmount;
    }

    public void MoveDown()
    {
        if (transformToMove == null)
        {
            Debug.LogError("Missing required references in ToothMover");
            return;
        }

        transformToMove.position += Vector3.down * moveAmount;
    }
}
