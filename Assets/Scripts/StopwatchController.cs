using System.Collections;
using TMPro;
using UnityEngine;

public class StopwatchController : MonoBehaviour
{
    // Serialized field for the TextMesh Pro UI Text
    [SerializeField]
    private TextMeshProUGUI stopwatchText;

    // Flag to control the stopwatch
    private bool isRunning = false;

    // Elapsed time
    private float elapsedTime = 0f;

    private void Start()
    {
        // Initialize the stopwatch text
        stopwatchText.text = "00:00";
        StartStopwatch();
    }

    // Function to start the stopwatch
    public void StartStopwatch()
    {
        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(UpdateStopwatch());
        }
    }

    // Function to stop the stopwatch
    public void StopStopwatch()
    {
        isRunning = false;
    }

    // Coroutine to update the stopwatch
    private IEnumerator UpdateStopwatch()
    {
        while (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateStopwatchText();
            yield return null;
        }
    }

    // Function to update the stopwatch text
    private void UpdateStopwatchText()
    {
        int minutes = (int)(elapsedTime % 3600) / 60;
        int seconds = (int)elapsedTime % 60;

        stopwatchText.text = $"{minutes:D2}:{seconds:D2}";
    }
}
