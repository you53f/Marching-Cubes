using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvas : MonoBehaviour
{
    [Tooltip("The speed at which the canvas fades")]
    public float defaultDuration = 1.0f;

    public Coroutine CurrentRoutine { private set; get; } = null;

    private CanvasGroup canvasGroup = null;
    private float alpha = 0.0f;

    private float quickFadeDuration = 0.25f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartFadeIn()
    {
        StopAllCoroutines();
        CurrentRoutine = StartCoroutine(FadeIn(defaultDuration));
    }

    public void StartFadeOut()
    {
        StopAllCoroutines();
        CurrentRoutine = StartCoroutine(FadeOut(defaultDuration));
    }

    public void QuickFadeIn()
    {
        StopAllCoroutines();
        CurrentRoutine = StartCoroutine(FadeIn(quickFadeDuration));
    }

    public void QuickFadeOut()
    {
        StopAllCoroutines();
        CurrentRoutine = StartCoroutine(FadeOut(quickFadeDuration));
    }

    public void StartFadeInOutSequence()
    {
        StopAllCoroutines();
        CurrentRoutine = StartCoroutine(FadeInOutSequence());
    }

    private IEnumerator FadeInOutSequence()
    {
        // Start fade in and wait for completion
        CurrentRoutine = StartCoroutine(FadeIn(defaultDuration));
        yield return new WaitWhile(() => CurrentRoutine != null);
        
        // Wait 0.5 seconds
        yield return new WaitForSeconds(0.5f);
        
        // Start fade out and wait for completion
        CurrentRoutine = StartCoroutine(FadeOut(defaultDuration));
        yield return new WaitWhile(() => CurrentRoutine != null);
    }

    private IEnumerator FadeIn(float duration)
    {
        float elapsedTime = 0.0f;
        while (alpha < 1.0f)
        {
            SetAlpha(elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(1.0f); // Ensure final alpha value
        CurrentRoutine = null; // Signal completion
    }

    private IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0.0f;
        while (alpha > 0.0f)
        {
            SetAlpha(1 - (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(0.0f); // Ensure final alpha value
        CurrentRoutine = null; // Signal completion
    }

    private void SetAlpha(float value)
    {
        alpha = Mathf.Clamp(value, 0.0f, 1.0f);
        canvasGroup.alpha = alpha;
    }
}
