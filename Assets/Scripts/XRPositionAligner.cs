using System.Collections;
using UnityEngine;

public class XRPositionAligner : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Transform xrRigRoot;

    public void AlignXRWithTarget()
    {
        if (targetObject == null || xrRigRoot == null)
        {
            Debug.LogError("Missing required references in XRPositionAligner");
            return;
        }

        StartCoroutine(DelayedAlignment());
    }

    private IEnumerator DelayedAlignment()
    {
        yield return new WaitForSeconds(0.5f);
        xrRigRoot.position = targetObject.transform.position;
        xrRigRoot.rotation = targetObject.transform.rotation;
    }
}
