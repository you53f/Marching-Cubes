using System;
using UnityEngine;

public class Voxelizer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool spheresVisible;
    public float voxelResolution;
    [SerializeField] private float sphereRadius;
    [SerializeField] private int voxelBuffer;
    public GameObject targetObject; // The prefab or GameObject to voxelize


    private float[,,] voxelGrid; // 3D location of each voxel represented by an integer
    private float[,,] centeredVoxels;
    private Vector3 localOrigin; // The world position of the voxel at index (0, 0, 0)
    //private float[,,] tempGrid;
    [HideInInspector] public int numVoxelsX;
    [HideInInspector] public int numVoxelsY;
    [HideInInspector] public int numVoxelsZ;

    public void Start()
    {
        StartVoxels();
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

        numVoxelsX = AdjustToNextOdd(vX + voxelBuffer);
        numVoxelsY = AdjustToNextOdd(vY + voxelBuffer);
        numVoxelsZ = AdjustToNextOdd(vZ + voxelBuffer);

        Debug.Log($"number of voxels is {numVoxelsX * numVoxelsY * numVoxelsZ}");

        // Create a 3D array to hold voxel data
        voxelGrid = new float[numVoxelsX, numVoxelsY, numVoxelsZ];
        centeredVoxels = new float[numVoxelsX, numVoxelsY, numVoxelsZ];

        Debug.Log($"Voxel Grid Size: {numVoxelsX} x {numVoxelsY} x {numVoxelsZ}");

        GameObject[,,] dataPointCube = new GameObject[numVoxelsX, numVoxelsY, numVoxelsZ];

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

        int offsetX = voxelBuffer / 2;
        int offsetY = voxelBuffer / 2;
        int offsetZ = voxelBuffer / 2;

        CenterVoxels(numVoxelsX, numVoxelsY, numVoxelsZ, centeredVoxels, offsetX, offsetY, offsetZ, dataPointCube);
    }

    public int AdjustToNextOdd(int number)
    {
        if (number % 2 == 0)
        {
            return number++;
        }
        else
        {
            return number;
        }
    }

    private void CenterVoxels(int numVoxelsX, int numVoxelsY, int numVoxelsZ, float[,,] centeredVoxels, int offsetX, int offsetY, int offsetZ, GameObject[,,] dataPointCube)
    {
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

                    if (newX >= 0 && newX < numVoxelsX && newY >= 0 && newY < numVoxelsY && newZ >= 0 && newZ < numVoxelsZ)
                    {
                        centeredVoxels[newX, newY, newZ] = voxelGrid[x, y, z];
                    }
                }
            }
        }
        // for (int z = 0; z < numVoxelsZ; z++)
        // {
        //     for (int y = 0; y < numVoxelsY; y++)
        //     {
        //         for (int x = 0; x < numVoxelsX; x++)
        //         {

        //             GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //             dataPointCube[x, y, z] = sphere;
        //             dataPointCube[x, y, z].transform.parent = this.transform;

        //             // Set position directly to voxelCenter
        //             Vector3Int index = new Vector3Int(x, y, z);
        //             Vector3 voxelCenter = GetVoxelWorldPosition(index);
        //             dataPointCube[x, y, z].transform.position = voxelCenter;

        //             // Scale spheres based on resolution and radius
        //             dataPointCube[x, y, z].transform.localScale =
        //                 new Vector3(voxelResolution * sphereRadius, voxelResolution * sphereRadius, voxelResolution * sphereRadius);

        //             Renderer sphereRenderer = sphere.GetComponent<Renderer>();

        //             // Change color based on occupancy
        //             if (centeredVoxels[x, y, z] == 1)
        //             {
        //                 sphereRenderer.material.color = Color.red;
        //             }
        //             else if (centeredVoxels[x, y, z] == 0)
        //             {
        //                 sphereRenderer.material.color = Color.black;
        //             }

        //             sphereRenderer.enabled = spheresVisible;
        //         }
        //     }
        // }
    }
    private int IsVoxelOccupied(Vector3 voxelCenter)
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