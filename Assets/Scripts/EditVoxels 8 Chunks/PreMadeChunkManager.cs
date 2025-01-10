using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreMadeChunkManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool boxesVisible;
    [SerializeField] private int brushSize;
    [SerializeField] private int brushStrength;
    [SerializeField] private float brushFallback;
    [SerializeField] private float bufferBeforeDestroy;
    [SerializeField] private Vector3 offset;

    [SerializeField] private float gridCubeSizeFactor;

    [Header("Elements")]
    [SerializeField] GameObject chunkModel0;
    [SerializeField] GameObject chunkModel1;
    [SerializeField] GameObject chunkModel2;
    [SerializeField] GameObject chunkModel3;
    [SerializeField] GameObject chunkModel4;
    [SerializeField] GameObject chunkModel5;
    [SerializeField] GameObject chunkModel6;
    [SerializeField] GameObject chunkModel7;
    [SerializeField] private Chunk0EditVoxels ch0;
    [SerializeField] private Chunk1EditVoxels ch1;
    [SerializeField] private Chunk2EditVoxels ch2;
    [SerializeField] private Chunk3EditVoxels ch3;
    [SerializeField] private Chunk4EditVoxels ch4;
    [SerializeField] private Chunk5EditVoxels ch5;
    [SerializeField] private Chunk6EditVoxels ch6;
    [SerializeField] private Chunk7EditVoxels ch7;


    void Start()
    {
        MeshFilter meshFilter0 = chunkModel0.GetComponent<MeshFilter>();
        MeshFilter meshFilter1 = chunkModel1.GetComponent<MeshFilter>();
        MeshFilter meshFilter2 = chunkModel2.GetComponent<MeshFilter>();
        MeshFilter meshFilter3 = chunkModel3.GetComponent<MeshFilter>();
        MeshFilter meshFilter4 = chunkModel4.GetComponent<MeshFilter>();
        MeshFilter meshFilter5 = chunkModel5.GetComponent<MeshFilter>();
        MeshFilter meshFilter6 = chunkModel6.GetComponent<MeshFilter>();
        MeshFilter meshFilter7 = chunkModel7.GetComponent<MeshFilter>();


        Mesh mesh0 = meshFilter0.sharedMesh;
        Mesh mesh1 = meshFilter1.sharedMesh;
        Mesh mesh2 = meshFilter2.sharedMesh;
        Mesh mesh3 = meshFilter3.sharedMesh;
        Mesh mesh4 = meshFilter4.sharedMesh;
        Mesh mesh5 = meshFilter5.sharedMesh;
        Mesh mesh6 = meshFilter6.sharedMesh;
        Mesh mesh7 = meshFilter7.sharedMesh;

        // Calculate the bounds of the mesh
        Bounds bounds0 = mesh0.bounds;
        Vector3 min0 = bounds0.min;
        Vector3 max0 = bounds0.max;
        Bounds bounds1 = mesh1.bounds;
        Vector3 min1 = bounds1.min;
        Vector3 max1 = bounds1.max;
        Bounds bounds2 = mesh2.bounds;
        Vector3 min2 = bounds2.min;
        Vector3 max2 = bounds2.max;
        Bounds bounds3 = mesh3.bounds;
        Vector3 min3 = bounds3.min;
        Vector3 max3 = bounds3.max;
        Bounds bounds4 = mesh4.bounds;
        Vector3 min4 = bounds4.min;
        Vector3 max4 = bounds4.max;
        Bounds bounds5 = mesh5.bounds;
        Vector3 min5 = bounds5.min;
        Vector3 max5 = bounds5.max;
        Bounds bounds6 = mesh6.bounds;
        Vector3 min6 = bounds6.min;
        Vector3 max6 = bounds6.max;
        Bounds bounds7 = mesh7.bounds;
        Vector3 min7 = bounds7.min;
        Vector3 max7 = bounds7.max;

        float x0 = (max0.x - min0.x) / 2f;
        float y0 = (max0.y - min0.y) / 2f;
        float z0 = (max0.z - min0.z) / 2f;
        float x1 = (max1.x - min1.x) / 2f;
        float y1 = (max1.y - min1.y) / 2f;
        float z1 = (max1.z - min1.z) / 2f;
        float x2 = (max2.x - min2.x) / 2f;
        float y2 = (max2.y - min2.y) / 2f;
        float z2 = (max2.z - min2.z) / 2f;
        float x3 = (max3.x - min3.x) / 2f;
        float y3 = (max3.y - min3.y) / 2f;
        float z3 = (max3.z - min3.z) / 2f;
        float x4 = (max4.x - min4.x) / 2f;
        float y4 = (max4.y - min4.y) / 2f;
        float z4 = (max4.z - min4.z) / 2f;
        float x5 = (max5.x - min5.x) / 2f;
        float y5 = (max5.y - min5.y) / 2f;
        float z5 = (max5.z - min5.z) / 2f;
        float x6 = (max6.x - min6.x) / 2f;
        float y6 = (max6.y - min6.y) / 2f;
        float z6 = (max6.z - min6.z) / 2f;
        float x7 = (max7.x - min7.x) / 2f;
        float y7 = (max7.y - min7.y) / 2f;
        float z7 = (max7.z - min7.z) / 2f;

        Vector3 spawnPos0 = new Vector3(-x0, -y0, -z0) + offset;
        Vector3 spawnPos1 = new Vector3(x1 - offset.x, -y1 + offset.y, -z1+offset.z);
        Vector3 spawnPos2 = new Vector3(-x2 + offset.x, y2- offset.y, -z2 + offset.z);
        Vector3 spawnPos3 = new Vector3(x3 - offset.x, y3 - offset.y, -z3 + offset.z);
        Vector3 spawnPos4 = new Vector3(-x4 + offset.x, -y4 + offset.y, z4 - offset.z);
        Vector3 spawnPos5 = new Vector3(x5 - offset.x, -y5 + offset.y, z5 - offset.z);
        Vector3 spawnPos6 = new Vector3(-x6 + offset.x, y6 - offset.y, z6 - offset.z);
        Vector3 spawnPos7 = new Vector3(x7 - offset.x, y7 - offset.y, z7 - offset.z);

        Chunk0EditVoxels chunk0 = Instantiate(ch0, spawnPos0, Quaternion.identity, transform);
        Chunk1EditVoxels chunk1 = Instantiate(ch1, spawnPos1, Quaternion.identity, transform);
        Chunk2EditVoxels chunk2 = Instantiate(ch2, spawnPos2, Quaternion.identity, transform);
        Chunk3EditVoxels chunk3 = Instantiate(ch3, spawnPos3, Quaternion.identity, transform);
        Chunk4EditVoxels chunk4 = Instantiate(ch4, spawnPos4, Quaternion.identity, transform);
        Chunk5EditVoxels chunk5 = Instantiate(ch5, spawnPos5, Quaternion.identity, transform);
        Chunk6EditVoxels chunk6 = Instantiate(ch6, spawnPos6, Quaternion.identity, transform);
        Chunk7EditVoxels chunk7 = Instantiate(ch7, spawnPos7, Quaternion.identity, transform);


        chunk0.Initialize(boxesVisible, brushSize, brushStrength, brushFallback, gridCubeSizeFactor, bufferBeforeDestroy);
        chunk1.Initialize(boxesVisible, brushSize, brushStrength, brushFallback, gridCubeSizeFactor, bufferBeforeDestroy);
        chunk2.Initialize(boxesVisible, brushSize, brushStrength, brushFallback, gridCubeSizeFactor, bufferBeforeDestroy);
        chunk3.Initialize(boxesVisible, brushSize, brushStrength, brushFallback, gridCubeSizeFactor, bufferBeforeDestroy);
        chunk4.Initialize(boxesVisible, brushSize, brushStrength, brushFallback, gridCubeSizeFactor, bufferBeforeDestroy);
        chunk5.Initialize(boxesVisible, brushSize, brushStrength, brushFallback, gridCubeSizeFactor, bufferBeforeDestroy);
        chunk6.Initialize(boxesVisible, brushSize, brushStrength, brushFallback, gridCubeSizeFactor, bufferBeforeDestroy);
        chunk7.Initialize(boxesVisible, brushSize, brushStrength, brushFallback, gridCubeSizeFactor, bufferBeforeDestroy);

        
        
        GameObject models = GameObject.Find("Tooth Chunks");
        models.SetActive(false);
    }
}
