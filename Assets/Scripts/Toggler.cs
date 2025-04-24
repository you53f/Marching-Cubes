using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggler : MonoBehaviour
{
    // Start is called before the first frame update
    public void ToggleFPS()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
