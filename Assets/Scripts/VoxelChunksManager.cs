using UnityEngine;

public class VoxelChunksManager : MonoBehaviour
{

    [Header("Brush Settings")]
    [SerializeField] private int brushSize;
    [SerializeField] private float brushStrength;
    [SerializeField] private float brushFallback;

    [Header("Settings")]
    [SerializeField] private float bufferBeforeDestroy;
    [SerializeField] private float gridcubeSizeFactor;
    [SerializeField] private bool boxesVisible;

    private ChunkingVoxelizer voxelizer;
    private float[,,,,,] chunkedVoxels;
    private int chunkGridLines;
    private float gridScale;

    private int chunksInX;
    private int chunksInY;
    private int chunksInZ;



    [Header("Elements")]
    [SerializeField] private EditVoxelsChunked editVoxelsprefab;

    void Start()
    {
        Go();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Go()
    {

        int chunksMade = 0;
        voxelizer = FindObjectOfType<ChunkingVoxelizer>();
        voxelizer.StartVoxels();
        chunkGridLines = voxelizer.chunkGridLines;

        gridScale = voxelizer.voxelResolution;

        chunksInX = voxelizer.chunksInX;
        chunksInY = voxelizer.chunksInY;
        chunksInZ = voxelizer.chunksInZ;

        chunkedVoxels = voxelizer.GetChunkedVoxels();


        float chunkLength = gridScale * (chunkGridLines - 1);

        for (int chunkIndexZ = 0; chunkIndexZ < chunksInZ; chunkIndexZ++)
        {
            for (int chunkIndexY = 0; chunkIndexY < chunksInY; chunkIndexY++)
            {
                for (int chunkIndexX = 0; chunkIndexX < chunksInX; chunkIndexX++)
                {
                    Vector3 spawnPosition = Vector3.zero;

                    spawnPosition.x = chunkIndexX * chunkLength;
                    spawnPosition.y = chunkIndexY * chunkLength;
                    spawnPosition.z = chunkIndexZ * chunkLength;

                    spawnPosition.x -= ((float)chunksInX / 2 * chunkLength) - chunkLength / 2;
                    spawnPosition.y -= ((float)chunksInY / 2 * chunkLength) - chunkLength / 2;
                    spawnPosition.z -= ((float)chunksInZ / 2 * chunkLength) - chunkLength / 2;

                    float[,,] chunksData = new float[chunkGridLines, chunkGridLines, chunkGridLines];

                    for (int a = 0; a < chunkGridLines; a++)
                    {
                        for (int b = 0; b < chunkGridLines; b++)
                        {
                            for (int c = 0; c < chunkGridLines; c++)
                            {
                                chunksData[a, b, c] = chunkedVoxels[chunkIndexX, chunkIndexY, chunkIndexZ, a, b, c];
                                //Debug.Log($"{chunksData[a,b,c]} took it from {chunkedVoxels[w,a,b,c]}");
                            }
                        }
                    }

                    bool shouldInstantiate = IsArrayNotAllZeros(chunksData);

                    if (shouldInstantiate)
                    {
                        EditVoxelsChunked editVoxels = Instantiate(editVoxelsprefab, spawnPosition, Quaternion.identity, transform);

                        editVoxels.Initialize(gridScale, chunkGridLines, chunkGridLines, chunkGridLines, boxesVisible, brushSize, brushStrength, brushFallback,
                        gridcubeSizeFactor, bufferBeforeDestroy, chunksData);

                        chunksMade++;
                    }
                }
            }
        }

        Debug.Log($"Chunks Created: {chunksMade}");
    }

    public static bool IsArrayNotAllZeros(float[,,] array)
    {
        // Check if the array is null or empty
        if (array == null || array.Length == 0)
        {
            return false; // Return false for null or empty arrays
        }

        // Iterate through each element in the 3D array
        foreach (float value in array)
        {
            if (value != 0.0f)
            {
                return true; // Return true if any value is not zero
            }
        }

        return false; // Return false if all values are zero
    }

    //     for (int w = 0; w < totalChunks; w++)
    //     {
    //         int z = Mathf.FloorToInt(w / (chunksInX * chunksInY));
    //         int r = w % (chunksInX * chunksInY);
    //         int y = Mathf.FloorToInt(r / chunksInX);
    //         r = r % chunksInX;
    //         int x = r;


    //         // Debug.Log($"Chunk {w} has an index {x}, {y}, {z}");

    //         Vector3 spawnPosition = Vector3.zero;

    //         spawnPosition.x = x * chunkLength;
    //         spawnPosition.y = y * chunkLength;
    //         spawnPosition.z = z * chunkLength;

    //         spawnPosition.x -= ((float)chunksInX / 2 * chunkLength) - chunkLength / 2;
    //         spawnPosition.y -= ((float)chunksInY / 2 * chunkLength) - chunkLength / 2;
    //         spawnPosition.z -= ((float)chunksInZ / 2 * chunkLength) - chunkLength / 2;

    //         // Debug.Log($"Chunk {w} has a position {spawnPosition}");


    //         float[,,] chunksData = new float[chunkGridLines, chunkGridLines, chunkGridLines];

    //         for (int a = 0; a < chunkGridLines; a++)
    //         {
    //             for (int b = 0; b < chunkGridLines; b++)
    //             {
    //                 for (int c = 0; c < chunkGridLines; c++)
    //                 {
    //                     chunksData[a, b, c] = chunkedVoxels[w, a, b, c];
    //                     //Debug.Log($"{chunksData[a,b,c]} took it from {chunkedVoxels[w,a,b,c]}");
    //                 }
    //             }
    //         }

    //         EditVoxelsChunked editVoxels = Instantiate(editVoxelsprefab, spawnPosition, Quaternion.identity, transform);


    //         editVoxels.Initialize(gridScale, chunkGridLines, chunkGridLines, chunkGridLines, boxesVisible, brushSize, brushStrength, brushFallback,
    //         gridcubeSizeFactor, bufferBeforeDestroy, chunksData);
    //     }


    // }
}
