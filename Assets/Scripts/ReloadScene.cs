using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using TMPro;
using UnityEditor; // Optional: If you want to use TextMeshPro for UI

public class ReloadScene : MonoBehaviour
{
    public void OnButtonClick()
    {
        // Get the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();
        // Reload the current scene
        SceneManager.LoadScene(currentScene.name);
    }

    public void ExitApplication()
    {
        // Check if we are in the Unity Editor
#if UNITY_EDITOR
        // Exit play mode in the Unity Editor
        EditorApplication.isPlaying = false;
#else
            // Exit the application when built
            Application.Quit();
#endif
    }
}
