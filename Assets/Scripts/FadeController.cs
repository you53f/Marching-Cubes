using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Include this if you're working with UI elements

public class FadeController : MonoBehaviour
{
    [SerializeField] private float fadeDuration; // Duration of the fade effect
    [SerializeField] private float delay;        // Delay
    [SerializeField] private bool button;        // Check if the fade is triggered by a button
    [SerializeField] private GameObject activateAfterFade; // GameObject to activate after fade

    private CanvasGroup canvasGroup; // For UI elements

    void Awake()
    {
        // Get the CanvasGroup component attached to this GameObject
        canvasGroup = GetComponent<CanvasGroup>();

        // Ensure the canvas starts fully transparent
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            FadeIn();
        }

        else
        {
            Debug.LogWarning("CanvasGroup component not found on " + gameObject.name);
        }

        if (!button)
        {
            StartCoroutine(WaitAndKillCanvas());
        }
    }

    private IEnumerator WaitAndKillCanvas()
    {
        yield return new WaitForSeconds(delay);
        KillCanvas();
    }

    public void KillCanvas()
    {
        StartCoroutine(KillCanvasCoroutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeCoroutine(0f, 1f, fadeDuration));
    }

    private IEnumerator KillCanvasCoroutine()
    {
        // Call FadeOut and wait for it to complete
        yield return FadeCoroutine(1f, 0f, fadeDuration);

        // Activate the specified GameObject after fading out
        if (activateAfterFade != null)
        {
            activateAfterFade.SetActive(true);
        }

        // Now deactivate the GameObject after fading out
        gameObject.SetActive(false);
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCoroutine(1f, 0f, fadeDuration));
    }

    /// <summary>
    /// Coroutine to handle the fading effect.
    /// </summary>
    private IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float fadeDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alphaValue = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            SetAlpha(alphaValue);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        SetAlpha(endAlpha); // Ensure final alpha is set correctly
    }

    /// <summary>
    /// Sets the alpha value of the canvas.
    /// </summary>
    private void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }
}
