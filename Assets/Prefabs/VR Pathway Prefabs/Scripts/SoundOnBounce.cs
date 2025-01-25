using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnBounce : MonoBehaviour
{
    public AudioClip collisionSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

        }
        audioSource.clip = collisionSound;
    }

    void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(collisionSound);
    }
}
