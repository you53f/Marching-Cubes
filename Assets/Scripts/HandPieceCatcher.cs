using UnityEngine;

public class HandPieceCatcher : MonoBehaviour
{
    
    [SerializeField] private GameObject handpiece;
    [SerializeField] private GameObject notebook;
        private Vector3 handPieceOriginalPosition;
        private Vector3 notebookOriginalPosition;
        

    private void Start()
    {
        // Store the initial position of the GameObject this script is attached to
        handPieceOriginalPosition = handpiece.transform.position;
        notebookOriginalPosition = notebook.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering GameObject has a specific tag (optional)
        // You can remove this check if you want it to work for any GameObject
        if (other.CompareTag("Handpiece"))
        {
            // Reset the position of the entering GameObject to its original position
            other.transform.position = handPieceOriginalPosition;
        }
        else
        {
            // Reset the position of the entering GameObject to its original position
            other.transform.position = notebookOriginalPosition;
        }
    }
}