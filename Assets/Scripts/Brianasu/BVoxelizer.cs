using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BVoxelizer : MonoBehaviour
{
    public GameObject targetMesh;
    [SerializeField] float voxelResolution;
    [SerializeField] int voxelBuffer;

    [HideInInspector] public int numVoxelsX;
    [HideInInspector] public int numVoxelsY;
    [HideInInspector] public int numVoxelsZ;

    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = targetMesh.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Bounds bounds = mesh.bounds;

        Vector3 min = bounds.min + targetMesh.transform.position;
        Vector3 max = bounds.max + targetMesh.transform.position;

        

        // Calculate the number of voxels based on the voxel size
        int xGridDimensions = AdjustToNextOdd(Mathf.CeilToInt((max.x - min.x) / voxelResolution)+voxelBuffer);
        int yGridDimensions = AdjustToNextOdd(Mathf.CeilToInt((max.y - min.y) / voxelResolution)+voxelBuffer);
        int zGridDimensions = AdjustToNextOdd(Mathf.CeilToInt((max.z - min.z) / voxelResolution)+voxelBuffer);

        
        numVoxelsX = AdjustToNextOdd(xGridDimensions + voxelBuffer);
        numVoxelsY = AdjustToNextOdd(yGridDimensions + voxelBuffer);
        numVoxelsZ = AdjustToNextOdd(zGridDimensions + voxelBuffer);

        var vxl = new Voxeliser(bounds, xGridDimensions, yGridDimensions, zGridDimensions);
        vxl.Voxelize(targetMesh);


        GameObject[,,] dataPointCube = new GameObject[numVoxelsX, numVoxelsY, numVoxelsZ];

        

    }

    // Update is called once per frame
    void Update()
    {

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

}
