using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunks : MonoBehaviour
{
    
    [Header("Brush Settings")]
    [SerializeField] private int brushSize;
    [SerializeField] private float brushStrength;
    [SerializeField] private float brushFallback;
    
    [Header("Settings")]
    [SerializeField] private Vector3Int chunksInOneAxis;
    [SerializeField] private int gridLines;
    [SerializeField] private float gridScale;
    [SerializeField] private float bufferBeforeDestroy;
    [SerializeField] private float gridcubeSizeFactor;
    [SerializeField] private bool boxesVisible;


    [Header("Elements")]
    [SerializeField] private TerrainGen terrainGeneratorPrefab;
    
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
        float terrainWorldSize = gridScale * (gridLines - 1);

        
        Vector3 spawnPosition = new Vector3(0,2,0);

        for (int z = 0; z < chunksInOneAxis.z; z++)
        {
            for (int y = 0; y < chunksInOneAxis.y; y++)
            {
                for (int x = 0; x < chunksInOneAxis.x; x++)
                {

                    spawnPosition.x = x * terrainWorldSize;
                    spawnPosition.y = y * terrainWorldSize;
                    spawnPosition.z = z * terrainWorldSize;

                    spawnPosition.x -= ((float)chunksInOneAxis.x / 2 * terrainWorldSize) - terrainWorldSize / 2;
                    spawnPosition.y -= ((float)chunksInOneAxis.y / 2 * terrainWorldSize) - terrainWorldSize / 2;
                    spawnPosition.z -= ((float)chunksInOneAxis.z / 2 * terrainWorldSize) - terrainWorldSize / 2;

                    // Debug.Log($"Chunk [{x},{y},{z}] has a position of {spawnPosition}");

                    TerrainGen terrain = Instantiate(terrainGeneratorPrefab, new Vector3(0,1.5f,0), Quaternion.identity, transform);

                    
                    terrain.Initialize(gridScale, gridLines, boxesVisible, brushSize, brushStrength, brushFallback,
                    gridcubeSizeFactor, bufferBeforeDestroy);
                }
            }
        }
    }
}
