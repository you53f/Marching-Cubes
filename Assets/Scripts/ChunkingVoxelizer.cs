using System;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector4 = UnityEngine.Vector4;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class ChunkingVoxelizer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public int chunkGridLines;
    [SerializeField] public float voxelResolution;
    [SerializeField] private float sphereRadius;
    [SerializeField] private int voxelBuffer;
    [SerializeField] bool Visualize;

    public GameObject targetObject; // The prefab or GameObject to voxelize


    private float[,,] voxelGrid; // 3D location of each voxel represented by an integer
    private float[,,] centeredVoxels;
    private Vector3 localOrigin; // The world position of the voxel at index (0, 0, 0)
    //private float[,,] tempGrid;

    [HideInInspector] public int chunksInX;
    [HideInInspector] public int chunksInY;
    [HideInInspector] public int chunksInZ;
    [HideInInspector] public int totalChunks;

    int numVoxelsX;
    int numVoxelsY;
    int numVoxelsZ;
    private float[,,,,,] chunkedVoxels;

    public void Start()
    {
    }


    public void StartVoxels()
    {
        // Set localOrigin to the minimum bounds of the mesh in world space
        localOrigin = targetObject.GetComponent<MeshFilter>().sharedMesh.bounds.min + targetObject.transform.position;
        // Debug.Log($"Local origin is {localOrigin}");
        VoxelizeMesh();
        // tempGrid = GetVoxelGrid();
        // Debug.Log($"Voxels from method is {tempGrid.GetLength(0)}, {tempGrid.GetLength(1)},and {tempGrid.GetLength(2)}");

    }

    #region Original Voxelization Array
    public void VoxelizeMesh()
    {
        MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("Target object does not have a MeshFilter.");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;

        // Calculate the bounds of the mesh
        Bounds bounds = mesh.bounds;
        //Debug.Log($"Minimum Bounds: {bounds.min} ");
        //Debug.Log($"Maximum Bounds: {bounds.max}");

        Vector3 min = bounds.min + targetObject.transform.position;
        Vector3 max = bounds.max + targetObject.transform.position;

        // Calculate the number of voxels based on the voxel size
        int vX = Mathf.CeilToInt((max.x - min.x) / voxelResolution);
        int vY = Mathf.CeilToInt((max.y - min.y) / voxelResolution);
        int vZ = Mathf.CeilToInt((max.z - min.z) / voxelResolution);

        numVoxelsX = AdjustToNextDivisible(vX + voxelBuffer);
        numVoxelsY = AdjustToNextDivisible(vY + voxelBuffer);
        numVoxelsZ = AdjustToNextDivisible(vZ + voxelBuffer);


        // Create a 3D array to hold voxel data
        voxelGrid = new float[numVoxelsX, numVoxelsY, numVoxelsZ];

        // Debug.Log($"Original Voxel Grid Size: {vX} x {vY} x {vZ}");
        Debug.Log($"Adjusted Voxel Grid Size: {numVoxelsX} x {numVoxelsY} x {numVoxelsZ}");


        // Loop through each voxel
        for (int z = 0; z < numVoxelsZ; z++)
        {
            for (int y = 0; y < numVoxelsY; y++)
            {
                for (int x = 0; x < numVoxelsX; x++)
                {
                    Vector3Int index = new Vector3Int(x, y, z);
                    Vector3 voxelCenter = GetVoxelWorldPosition(index);

                    // Debug log for voxel center position
                    //Debug.Log($"Voxel Center at Index ({x}, {y}, {z}): {voxelCenter}");

                    // Check if the voxel intersects with the mesh
                    voxelGrid[x, y, z] = IsVoxelOccupied(voxelCenter);
                }
            }
        }

        // To be used in CenterVoxels
        int offsetX = Mathf.FloorToInt(voxelBuffer / 2);
        int offsetY = Mathf.FloorToInt(voxelBuffer / 2);
        int offsetZ = Mathf.FloorToInt(voxelBuffer / 2);

        CenterVoxels(numVoxelsX, numVoxelsY, numVoxelsZ, offsetX, offsetY, offsetZ);
    }

    #endregion
    public int AdjustToNextDivisible(int number)
    {
        // Check if the number is divisible by chunkGridLines
        if (number % chunkGridLines == 0)
        {
            // If divisible, return the original number
            return number;
        }
        else
        {
            // If not divisible, calculate the next divisible integer
            int remainder = number % chunkGridLines;
            int nextDivisible = number + (chunkGridLines - remainder);
            return nextDivisible;
        }
    }

    #region  Centering Voxels
    private void CenterVoxels(int numVoxelsX, int numVoxelsY, int numVoxelsZ, int offsetX, int offsetY, int offsetZ)
    {
        centeredVoxels = new float[numVoxelsX, numVoxelsY, numVoxelsZ];

        // Centering Voxels
        for (int z = 0; z < numVoxelsZ; z++)
        {
            for (int y = 0; y < numVoxelsY; y++)
            {
                for (int x = 0; x < numVoxelsX; x++)
                {
                    int newX = x + offsetX;
                    int newY = y + offsetY;
                    int newZ = z + offsetZ;

                    if (newX < numVoxelsX && newY < numVoxelsY && newZ < numVoxelsZ)
                    {
                        centeredVoxels[newX, newY, newZ] = voxelGrid[x, y, z];
                    }
                }
            }
        }
        #endregion

        #region Visualization

        if (Visualize)
        {
            // To hold the spheres for visualization
            GameObject[,,] dataPointSphere = new GameObject[numVoxelsX, numVoxelsY, numVoxelsZ];

            for (int z = 0; z < numVoxelsZ; z++)
            {
                for (int y = 0; y < numVoxelsY; y++)
                {
                    for (int x = 0; x < numVoxelsX; x++)
                    {
                        if (centeredVoxels[x, y, z] == 1)
                        {
                            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            dataPointSphere[x, y, z] = sphere;
                            dataPointSphere[x, y, z].transform.parent = this.transform;

                            // Set position directly to voxelCenter
                            Vector3Int index = new Vector3Int(x, y, z);
                            Vector3 voxelCenter = GetVoxelWorldPosition(index);
                            dataPointSphere[x, y, z].transform.position = voxelCenter;

                            // Scale spheres based on resolution and radius
                            dataPointSphere[x, y, z].transform.localScale =
                                new Vector3(voxelResolution * sphereRadius, voxelResolution * sphereRadius, voxelResolution * sphereRadius);

                            Renderer sphereRenderer = sphere.GetComponent<Renderer>();

                            sphereRenderer.material.color = Color.red;
                        }


                        else if (centeredVoxels[x, y, z] == 0)
                        {
                            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            dataPointSphere[x, y, z] = sphere;
                            dataPointSphere[x, y, z].transform.parent = this.transform;

                            // Set position directly to voxelCenter
                            Vector3Int index = new Vector3Int(x, y, z);
                            Vector3 voxelCenter = GetVoxelWorldPosition(index);
                            dataPointSphere[x, y, z].transform.position = voxelCenter;

                            // Scale spheres based on resolution and radius
                            dataPointSphere[x, y, z].transform.localScale =
                                new Vector3(voxelResolution * sphereRadius, voxelResolution * sphereRadius, voxelResolution * sphereRadius);

                            Renderer sphereRenderer = sphere.GetComponent<Renderer>();

                            sphereRenderer.material.color = Color.black;
                        }

                        // Debug.Log($"centered voxels at {x},{y},{z} is {centeredVoxels[x, y, z]}");
                    }
                }
            }
        }
        #endregion

        chunkedVoxels = ChunkingData(centeredVoxels);

    }

    #region Chunking Voxels
    private float[,,,,,] ChunkingData(float[,,] data)
    {
        if (data == null)
        {
            Debug.LogError("Input data array for chunking is null.");
            return null;
        }

        chunksInX = numVoxelsX / chunkGridLines;
        chunksInY = numVoxelsY / chunkGridLines;
        chunksInZ = numVoxelsZ / chunkGridLines;
        totalChunks = chunksInX * chunksInY * chunksInZ;

        Debug.Log($"Chunks in X: {chunksInX} | Y: {chunksInY} | Z: {chunksInZ}");
        Debug.Log($"Total Chunks is {totalChunks}");

        float[,,,,,] chunked = new float[chunksInX, chunksInY, chunksInZ, chunkGridLines, chunkGridLines, chunkGridLines];

        for (int z = 0; z < numVoxelsZ; z++)
        {
            for (int y = 0; y < numVoxelsY; y++)
            {
                for (int x = 0; x < numVoxelsX; x++)
                {
                    int chunkIndexX = Mathf.FloorToInt(x / chunkGridLines);
                    int chunkIndexY = Mathf.FloorToInt(y / chunkGridLines);
                    int chunkIndexZ = Mathf.FloorToInt(z / chunkGridLines);

                    // Debug.Log($"Chunk Index X is {chunkIndexX}");
                    // Debug.Log($"Chunk Index X is {chunkIndexX}");
                    // Debug.Log($"Chunk Index X is {chunkIndexX}");

                    int i = x % chunkGridLines;
                    int j = y % chunkGridLines;
                    int k = z % chunkGridLines;
                    
                    chunked[chunkIndexX, chunkIndexY, chunkIndexZ, i, j, k] = data[x, y, z];

                    // Debug.Log($"centered {data[x, y, z]} of {x},{y},{z} is now chunk data {chunkIndexX},{chunkIndexY},{chunkIndexZ},{i},{j},{k} with value {chunked[chunkIndexX, chunkIndexY, chunkIndexZ, i, j, k]}");

                }
            }
        }

        return chunked;
    }

    #endregion

    /* Perplexity Method of Chunking   

    private float[,,,] ChunkingData(float[,,] data)
   {
       if (data == null)
       {
           Debug.LogError("Input data array is null.");
           return null; // or handle as appropriate
       }

       chunksInX = numVoxelsX / chunkGridLines;
       chunksInY = numVoxelsY / chunkGridLines;
       chunksInZ = numVoxelsZ / chunkGridLines;
       totalChunks = chunksInX * chunksInY * chunksInZ;

       // Debug.Log($"Chunks in X: {chunksInX} | Y: {chunksInY} | Z: {chunksInZ}");
       // Debug.Log($"Total Chunks is {totalChunks}");

       float[,,,] chunked = new float[totalChunks, chunkGridLines, chunkGridLines, chunkGridLines];

       // int dim1 = chunked.GetLength(0);
       // int dim2 = chunked.GetLength(1);
       // int dim3 = chunked.GetLength(2);
       // int dim4 = chunked.GetLength(3);

       // Debug.Log($"chunked Dimensions are {dim1}, {dim2}, {dim3} and {dim4}");

       for (int w = 0; w < totalChunks; w++)
       {
           // Debug.Log($"Transferring Data of chunk {w}");
           for (int z = 0; z < chunkGridLines; z++)
           {
               // Debug.Log($"in Z number {z}");
               for (int y = 0; y < chunkGridLines; y++)
               {
                   // Debug.Log($"in Y number {y}");
                   for (int x = 0; x < chunkGridLines; x++)
                   {
                       // Debug.Log($"in X number {x}");

                       int xGlobal = (w % chunksInX) * chunkGridLines + x;
                       int yGlobal = ((w / chunksInX) % chunksInY) * chunkGridLines + y;
                       int zGlobal = (w / (chunksInX * chunksInY)) * chunkGridLines + z;

                       // Ensure we don't go out of bounds
                       if (xGlobal < numVoxelsX && yGlobal < numVoxelsY && zGlobal < numVoxelsZ)
                       {
                           chunked[w, x, y, z] = data[xGlobal, yGlobal, zGlobal];
                       }

                       // Debug.Log($"centered {data[xGlobal, yGlobal, zGlobal]} of {x},{y},{z} is now chunk data {w},{x},{y},{z} with value {chunked[w, x, y, z]}");
                   }
               }
           }
       }
       return chunked;
   } */
    private float IsVoxelOccupied(Vector3 voxelCenter)
    {
        // Use OverlapSphere to check if the voxel sphere overlaps with the target object's collider
        Collider[] colliders = Physics.OverlapSphere(voxelCenter, voxelResolution);
        foreach (Collider collider in colliders)
        {
            if (collider == targetObject.GetComponent<Collider>())
                return 1;
        }
        return 0;
    }
    public float[,,] GetVoxelGrid()
    {
        return centeredVoxels;
    }

    public float[,,,,,] GetChunkedVoxels()
    {
        return chunkedVoxels;
    }

    private Vector3 GetVoxelWorldPosition(Vector3Int voxelIndex)
    {
        // Calculate the local position of the voxel based on its index
        Vector3 voxelPosition = new Vector3(voxelIndex.x * voxelResolution,
                                             voxelIndex.y * voxelResolution,
                                             voxelIndex.z * voxelResolution);

        // Return the world position by adding the local origin
        return localOrigin + voxelPosition;
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;

        for (int z = 0; z < centeredVoxels.GetLength(2); z++)
        {
            for (int y = 0; y < centeredVoxels.GetLength(1); y++)
            {
                for (int x = 0; x < centeredVoxels.GetLength(0); x++)
                {
                    Vector3Int voxelIndex = new Vector3Int(x, y, z);
                    if (centeredVoxels[x, y, z] == 1)
                    {
                        Gizmos.color = Color.red;
                        Vector3 spawnPosition = GetVoxelWorldPosition(voxelIndex);
                        Gizmos.DrawSphere(spawnPosition, voxelResolution / 5);
                    }
                    else if (centeredVoxels[x, y, z] == 0)
                    {

                        Gizmos.color = Color.black;
                        Vector3 spawnPosition = GetVoxelWorldPosition(voxelIndex);
                        Gizmos.DrawSphere(spawnPosition, voxelResolution / 5);
                    }
                }
            }
        }
    }
}