using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using TMPro; // Optional: If you want to use TextMeshPro for UI

public class ReloadScene : MonoBehaviour
{
    public void OnButtonClick()
    {
        // Get the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();
        // Reload the current scene
        SceneManager.LoadScene(currentScene.name);
    }
}
