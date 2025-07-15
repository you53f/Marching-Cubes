using Unity.Mathematics;
using UnityEngine;

public class HandPieceCatcher : MonoBehaviour
{
    [SerializeField] private GameObject handpiece;
    [SerializeField] private GameObject handpiece2x;
    [SerializeField] private GameObject handpiece4x;
    private InputActivator inputActivator;
    int selector;
    private Vector3 handPieceOriginalPosition;
    private quaternion handpieceRotation;


    private void Start()
    {
        Invoke("DelayedStart", 2.0f);
    }
    void DelayedStart()
    {
        inputActivator = FindObjectOfType<InputActivator>();
        selector = inputActivator.magnificationSelector;

        switch (selector)
        {
            case 1:
                handPieceOriginalPosition = handpiece.transform.position;
                handpieceRotation = handpiece.transform.rotation;
                break;
            case 2:
                handPieceOriginalPosition = handpiece2x.transform.position;
                handpieceRotation = handpiece2x.transform.rotation;
                break;
            case 4:
                handPieceOriginalPosition = handpiece4x.transform.position;
                handpieceRotation = handpiece4x.transform.rotation;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Handpiece"))
        {
            if (selector == 1)
            {
                handpiece.transform.position = handPieceOriginalPosition + Vector3.up * 0.1f;
                handpiece.transform.rotation = handpieceRotation;
                // Debug.Log("Handpiece caught: " + handpiece.name + " with selector: " + selector);
            }
            else if (selector == 2)
            {
                handpiece2x.transform.position = handPieceOriginalPosition + Vector3.up * 0.1f;
                handpiece2x.transform.rotation = handpieceRotation;
                // Debug.Log("Handpiece caught: " + handpiece2x.name + " with selector: " + selector);
            }
            else if (selector == 4)
            {
                handpiece4x.transform.position = handPieceOriginalPosition + Vector3.up * 0.1f;
                handpiece4x.transform.rotation = handpieceRotation;
                // Debug.Log("Handpiece caught: " + handpiece4x.name + " with selector: " + selector);
            }
            else
            {
                // Debug.LogWarning("Selector not recognized: " + selector);
            }
        }

        else
        {
            // Debug.LogWarning("Collider does not have the tag 'Handpiece': " + other.name);
        }
    }
}