using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallVoxelizer : MonoBehaviour
{
    [SerializeField] bool spheresVisible;
    [SerializeField] float sphereRadius;
    [SerializeField] float voxelResolution;
    [SerializeField] GameObject targetObject; // The prefab or GameObject to voxelize
    GameObject[,,] voxelList;
    Vector3 spawnPosition;

    // Start is called before the first frame update
    void Start()
    {

        MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Bounds bounds = mesh.bounds;
        Vector3 min = bounds.min + targetObject.transform.position;
        Vector3 max = bounds.max + targetObject.transform.position;

        int numVoxelsX = Mathf.CeilToInt((max.x - min.x) / voxelResolution);
        int numVoxelsY = Mathf.CeilToInt((max.y - min.y) / voxelResolution);
        int numVoxelsZ = Mathf.CeilToInt((max.z - min.z) / voxelResolution);

        voxelList = new GameObject[numVoxelsX, numVoxelsY, numVoxelsZ];


        var vxl = new Voxeliser(bounds, numVoxelsX, numVoxelsY, numVoxelsZ);
        vxl.Voxelize(targetObject.transform);
        var data = vxl.VoxelMap;

        for (int z = 0; z < numVoxelsZ; z++)
        {
            for (int y = 0; y < numVoxelsY; y++)
            {
                for (int x = 0; x < numVoxelsX; x++)
                {
                    GameObject voxel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    voxelList[x, y, z] = voxel;
                    voxelList[x, y, z].transform.parent = transform;

                    Vector3 voxelCenter = GetVoxelWorldPosition(new Vector3Int(x, y, z));
                    voxelList[x, y, z].transform.position = voxelCenter;

                    // Scale spheres based on resolution and radius
                    voxelList[x, y, z].transform.localScale =
                        new Vector3(voxelResolution * sphereRadius, voxelResolution * sphereRadius, voxelResolution * sphereRadius);

                    Renderer sphereRenderer = voxel.GetComponent<Renderer>();

                    if (data[x][y][z])
                    {
                        sphereRenderer.material.color = Color.red;
                    }
                    else if (!data[x][y][z])
                    {
                        Destroy(voxel);
                    }

                    sphereRenderer.enabled = spheresVisible;
                }
            }
        }
    }

    private Vector3 GetVoxelWorldPosition(Vector3Int voxelIndex)
    {
        // Calculate the local position of the voxel based on its index
        Vector3 voxelPosition = new Vector3(voxelIndex.x * voxelResolution,
                                             voxelIndex.y * voxelResolution,
                                             voxelIndex.z * voxelResolution);

        // Return the world position by adding the local origin
        return spawnPosition + voxelPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }
}