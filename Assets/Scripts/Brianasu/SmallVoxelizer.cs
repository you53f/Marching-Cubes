using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallVoxelizer : MonoBehaviour
{

    public GameObject targetObject; // The prefab or GameObject to voxelize
    public float voxelSize = 0.1f; // Size of each voxel
    private int[,,] voxelGrid;

    // Start is called before the first frame update
    void Start()
    {

        MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Bounds bounds = mesh.bounds;
        Vector3 min = bounds.min + targetObject.transform.position;
        Vector3 max = bounds.max + targetObject.transform.position;

        int numVoxelsX = Mathf.CeilToInt((max.x - min.x) / voxelSize);
        int numVoxelsY = Mathf.CeilToInt((max.y - min.y) / voxelSize);
        int numVoxelsZ = Mathf.CeilToInt((max.z - min.z) / voxelSize);

        voxelGrid = new int[numVoxelsX,numVoxelsY,numVoxelsZ];


        var vxl = new Voxeliser(bounds, numVoxelsX, numVoxelsY, numVoxelsZ);
        vxl.Voxelize(targetObject.transform);
        var data = vxl.VoxelMap;
        Debug.Log(data);
    }

    // Update is called once per frame
    void Update()
    {

    }


    /*private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;

        for (int z = 0; z < voxelGrid.GetLength(2); z++)
        {
            for (int y = 0; y < voxelGrid.GetLength(1); y++)
            {
                for (int x = 0; x < voxelGrid.GetLength(0); x++)
                {
                    Vector3Int voxelIndex = new Vector3Int(x, y, z);
                    if (voxelGrid[x, y, z] == 1)
                    {
                        Gizmos.DrawSphere(data.transform, voxelSize / 5);
                    }
                }
            }
        }
    }*/

}
