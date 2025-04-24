using UnityEngine;

public class FPSLogger : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI fpsText;
    [SerializeField] private float interval;
    private float timer = 0f;
    private int frameCount = 0;

    void Update()
    {
        timer += Time.deltaTime;
        frameCount++;

        if (timer >= interval)
        {
            // Calculate average FPS over the last second
            float fps = frameCount / timer;

            fpsText.text = "FPS: " + Mathf.RoundToInt(fps).ToString();

            // Reset for next interval
            timer = 0f;
            frameCount = 0;
        }
    }
}
