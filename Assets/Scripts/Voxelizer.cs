using System;
using UnityEngine;

public class Voxelizer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool spheresVisible;
    public float voxelResolution; // Size of each voxel
    [SerializeField] private float sphereRadius;
    [SerializeField] private int voxelBuffer;
    public GameObject targetObject; // The prefab or GameObject to voxelize


    private float[,,] voxelGrid; // 3D location of each voxel represented by an integer
    private float[,,] centeredVoxels;
    private Vector3 localOrigin; // The world position of the voxel at index (0, 0, 0)
    //private float[,,] tempGrid;

    private bool editVoxelDisabled;

    public void Start()
    {
        editVoxelDisabled = CheckForEditVoxels();
        if(editVoxelDisabled)
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

    private bool CheckForEditVoxels()
    {  
        GameObject editVoxels = GameObject.Find("Edit Voxels");
        bool state;
        // Check if the GameObject exists and is enabled
        if (editVoxels != null)
        {
            if (editVoxels.activeInHierarchy)
            {
                state = false;
                Debug.Log("Yes Edit Voxels");
            }
            else
            {
                state = true;
                Debug.Log("No Edit Voxels");
            }
        }
        else
        {
            state = true;
            Debug.Log("No Edit Voxels");
        }
        return state;
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
        int numVoxelsX = Mathf.CeilToInt((max.x - min.x) / voxelResolution);
        int numVoxelsY = Mathf.CeilToInt((max.y - min.y) / voxelResolution);
        int numVoxelsZ = Mathf.CeilToInt((max.z - min.z) / voxelResolution);

        int maxNumVoxels = GetLongestDimension(numVoxelsX, numVoxelsY, numVoxelsZ) + voxelBuffer;
        int minNumVoxels = GetShortestDimension(numVoxelsX, numVoxelsY, numVoxelsZ) + voxelBuffer;

        if (maxNumVoxels % 2 == 0)
            maxNumVoxels++;

        int offsetX = Mathf.CeilToInt(((float)maxNumVoxels - (float)numVoxelsX)/2f);
        int offsetY = Mathf.CeilToInt(((float)maxNumVoxels - (float)numVoxelsY)/2f);
        int offsetZ = Mathf.CeilToInt(((float)maxNumVoxels - (float)numVoxelsZ)/2f);

        Debug.Log($"number of voxels is {maxNumVoxels * maxNumVoxels * maxNumVoxels}");

        // Create a 3D array to hold voxel data
        voxelGrid = new float[maxNumVoxels, maxNumVoxels, maxNumVoxels];
        centeredVoxels = new float[maxNumVoxels, maxNumVoxels, maxNumVoxels];

        Debug.Log($"Original Voxel Grid Size: {numVoxelsX} x {numVoxelsY} x {numVoxelsZ}");
        Debug.Log($"Equalized Voxel Grid Size: {maxNumVoxels} x {maxNumVoxels} x {maxNumVoxels}");

        GameObject[,,] dataPointCube = new GameObject[maxNumVoxels, maxNumVoxels, maxNumVoxels];

        // Loop through each voxel
        for (int z = 0; z < maxNumVoxels; z++)
        {
            for (int y = 0; y < maxNumVoxels; y++)
            {
                for (int x = 0; x < maxNumVoxels; x++)
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

        CenterVoxels(maxNumVoxels, centeredVoxels, offsetX, offsetY, offsetZ, dataPointCube);
    }


    private void CenterVoxels(int maxNumVoxels, float[,,] centeredVoxels, int offsetX, int offsetY, int offsetZ, GameObject[,,] dataPointCube)
    {
        // Centering Voxels
        for (int z = 0; z < maxNumVoxels; z++)
        {
            for (int y = 0; y < maxNumVoxels; y++)
            {
                for (int x = 0; x < maxNumVoxels; x++)
                {
                    int newX = x + offsetX;
                    int newY = y + offsetY;
                    int newZ = z + offsetZ;

                    if (newX >= 0 && newX < maxNumVoxels && newY >= 0 && newY < maxNumVoxels && newZ >= 0 && newZ < maxNumVoxels)
                    {
                        centeredVoxels[newX, newY, newZ] = voxelGrid[x, y, z];
                    }
                }
            }
        }
        for (int z = 0; z < maxNumVoxels; z++)
        {
            for (int y = 0; y < maxNumVoxels; y++)
            {
                for (int x = 0; x < maxNumVoxels; x++)
                {

                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    dataPointCube[x, y, z] = sphere;
                    dataPointCube[x, y, z].transform.parent = this.transform;

                    // Set position directly to voxelCenter
                    Vector3Int index = new Vector3Int(x, y, z);
                    Vector3 voxelCenter = GetVoxelWorldPosition(index);
                    dataPointCube[x, y, z].transform.position = voxelCenter;

                    // Scale spheres based on resolution and radius
                    dataPointCube[x, y, z].transform.localScale =
                        new Vector3(voxelResolution * sphereRadius, voxelResolution * sphereRadius, voxelResolution * sphereRadius);

                    Renderer sphereRenderer = sphere.GetComponent<Renderer>();

                    // Change color based on occupancy
                    if (centeredVoxels[x, y, z] == 1)
                    {
                        sphereRenderer.material.color = Color.red;
                    }
                    else if (centeredVoxels[x, y, z] == 0)
                    {
                        sphereRenderer.material.color = Color.black;
                    }

                    sphereRenderer.enabled = spheresVisible;
                }
            }
        }
    }

    private int GetLongestDimension(int value1, int value2, int value3)
    {
        int lengthX = value1;
        int lengthY = value2;
        int lengthZ = value3;
        int length;

        if (lengthX >= lengthY && lengthX >= lengthZ)
            length = lengthX;

        else if (lengthY >= lengthX && lengthY >= lengthZ)
            length = lengthY;

        else
            length = lengthZ;

        Debug.Log($"Longest Dimension is {length}");

        return length;
    }

    private int GetShortestDimension(int value1, int value2, int value3)
    {
        int lengthX = value1;
        int lengthY = value2;
        int lengthZ = value3;
        int length;

        if (lengthX <= lengthY && lengthX <= lengthZ)
            length = lengthX;

        else if (lengthY <= lengthX && lengthY <= lengthZ)
            length = lengthY;

        else
            length = lengthZ;

        Debug.Log($"Shortest Dimension is {length}");

        return length;
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