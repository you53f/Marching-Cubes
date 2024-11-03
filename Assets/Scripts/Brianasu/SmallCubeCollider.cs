using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmallCubeCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // if(collision.collider.CompareTag("Tooth"))
        Debug.Log("Cube entered");
    }
    
    void OnTriggerStay(Collider other)
    {
        // if(collision.collider.CompareTag("Tooth"))
        Debug.Log("Cube inside");
    }
    
    void OnTriggerExit(Collider other)
    {
        // if(collision.collider.CompareTag("Tooth"))
        Debug.Log("Cube exited");
    }
}
