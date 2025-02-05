using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDelay : MonoBehaviour
{
    [SerializeField] private float startDelay;   // Delay before starting the fade
    [SerializeField] private GameObject canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartingDelay());
    }
    private IEnumerator StartingDelay()
    {
        // Wait for the specified delay before starting the fade-in
        yield return new WaitForSeconds(startDelay);
        canvasGroup.SetActive(true);
    }
}
