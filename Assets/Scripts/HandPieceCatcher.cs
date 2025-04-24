using Unity.Mathematics;
using UnityEngine;

public class HandPieceCatcher : MonoBehaviour
{

    [SerializeField] private GameObject handpiece;
    [SerializeField] private GameObject notebook;
    private Vector3 handPieceOriginalPosition;
    private Vector3 notebookOriginalPosition;
    private quaternion notebookRotation;


    private void Start()
    {
        handPieceOriginalPosition = handpiece.transform.position;
        notebookOriginalPosition = notebook.transform.position;
        notebookRotation = notebook.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Handpiece"))
        {
            other.transform.position = handPieceOriginalPosition;
        }
        else if (other.CompareTag("Respawn"))
        {
            Destroy(other.gameObject);
            Instantiate(other.gameObject, notebookOriginalPosition, notebookRotation);
        }
    }
}