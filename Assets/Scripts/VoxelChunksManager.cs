using System;
using System.Collections;
using System.Collections.Generic;
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

    private Voxelizer voxelizer;
    private float[,,,] chunkedVoxels;
    private int chunkGridLines;
    private float gridScale;

    private int chunksInX;
    private int chunksInY;
    private int chunksInZ;
    private int totalChunks;



    [Header("Elements")]
    [SerializeField] private EditVoxels editVoxelsprefab;

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
        voxelizer = FindObjectOfType<Voxelizer>();
        voxelizer.StartVoxels();
        chunkGridLines = voxelizer.chunkGridLines;

        gridScale = voxelizer.voxelResolution;

        chunksInX = voxelizer.chunksInX;
        chunksInY = voxelizer.chunksInY;
        chunksInZ = voxelizer.chunksInZ;

        totalChunks = voxelizer.totalChunks;

        chunkedVoxels = voxelizer.GetChunkedVoxels();


        float chunkLength = gridScale * (chunkGridLines - 1);

        for (int w = 0; w < totalChunks; w++)
        {
            int z = Mathf.FloorToInt(w / (chunksInX * chunksInY));
            int r = w % (chunksInX * chunksInY);
            int y = Mathf.FloorToInt(r / chunksInX);
            r = r % chunksInX;
            int x = r;


            // Debug.Log($"Chunk {w} has an index {x}, {y}, {z}");

            Vector3 spawnPosition = Vector3.zero;

            spawnPosition.x = x * chunkLength;
            spawnPosition.y = y * chunkLength;
            spawnPosition.z = z * chunkLength;

            spawnPosition.x -= ((float)chunksInX / 2 * chunkLength) - chunkLength / 2;
            spawnPosition.y -= ((float)chunksInY / 2 * chunkLength) - chunkLength / 2;
            spawnPosition.z -= ((float)chunksInZ / 2 * chunkLength) - chunkLength / 2;

            // Debug.Log($"Chunk {w} has a position {spawnPosition}");


            float[,,] chunksData = new float[chunkGridLines, chunkGridLines, chunkGridLines];

            for (int a = 0; a < chunkGridLines; a++)
            {
                for (int b = 0; b < chunkGridLines; b++)
                {
                    for (int c = 0; c < chunkGridLines; c++)
                    {
                        chunksData[a, b, c] = chunkedVoxels[w, a, b, c];
                        //Debug.Log($"{chunksData[a,b,c]} took it from {chunkedVoxels[w,a,b,c]}");
                    }
                }
            }

            EditVoxels editVoxels = Instantiate(editVoxelsprefab, spawnPosition, Quaternion.identity, transform);


            editVoxels.Initialize(gridScale, chunkGridLines, boxesVisible, brushSize, brushStrength, brushFallback,
            gridcubeSizeFactor, bufferBeforeDestroy, chunksData);
        }


    }
    private int GetLongestDimension(float[,,] values)
    {
        int lengthX = values.GetLength(0);
        int lengthY = values.GetLength(1);
        int lengthZ = values.GetLength(2);
        int length;

        if (lengthX >= lengthY && lengthX >= lengthZ)
            length = lengthX;

        else if (lengthY >= lengthX && lengthY >= lengthZ)
            length = lengthY;

        else
            length = lengthZ;

        return length;
    }
}
