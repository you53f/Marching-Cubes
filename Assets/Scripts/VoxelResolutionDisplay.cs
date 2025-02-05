using UnityEngine;
using TMPro; // Include TextMeshPro namespace

public class VoxelResolutionDisplay : MonoBehaviour
{
    // Reference to the ScrawkVoxelizer script
    public ScrawkVoxelizer voxelizerScript;

    // Reference to the TextMeshProUGUI component
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        // Get the TextMeshProUGUI component attached to this GameObject
        textMeshPro = GetComponent<TextMeshProUGUI>();

        textMeshPro.text = "Voxel Resolution: " + voxelizerScript.voxelResolution.ToString();
    }

    void Update()
    {
        // Update the text to display the current voxel resolution
        textMeshPro.text = "Voxel Resolution: " + voxelizerScript.voxelResolution.ToString();
    }
}
