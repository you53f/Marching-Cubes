using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using Vector3 = UnityEngine.Vector3;

using Vector2 = UnityEngine.Vector2;
using Unity.VisualScripting;
using System.IO;

public class VoxelMeshVisualizer : MonoBehaviour
{
    Mesh mesh;

    [Header("Data")]
    [SerializeField] private bool importVoxels;
    [SerializeField] private string voxelGridValuesPath;
    [SerializeField] private string dimensionsFilePath;
    [SerializeField] ScrawkVoxelizer scrawkVoxelizer;
    [SerializeField] private bool boxesVisible;
    private Vector3Int gridLines;
    private float gridScale;
    [SerializeField] private float gridCubeSizeFactor;
    [SerializeField] private float isoValue;
    [SerializeField] private float randomizer;
    [SerializeField] private MeshFilter filter;
    private VolumeGrid volumeGrid;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private float gridCubeSize;
    private float[,,] voxelGridValues;
    private float[,,] gridValues;
    GameObject[,,] dataPointCube;
    GameObject targetObject;

    private void Awake()
    {

        targetObject = scrawkVoxelizer.targetObject;
        if (importVoxels)
        {
            voxelGridValues = LoadFloatArray(voxelGridValuesPath, dimensionsFilePath);
            targetObject.SetActive(false);
        }
        else
        {
            scrawkVoxelizer.StartVoxels();
            voxelGridValues = scrawkVoxelizer.GetVoxelGrid();
            //Debug.Log($"Most Grid Lines = {gridLines}");
            gridScale = scrawkVoxelizer.voxelResolution;
        }
        gridLines.x = voxelGridValues.GetLength(0);
        gridLines.y = voxelGridValues.GetLength(1);
        gridLines.z = voxelGridValues.GetLength(2);


        Debug.Log($"Number of voxels: {gridLines.x * gridLines.y * gridLines.z}");
        targetObject = scrawkVoxelizer.targetObject;

        // Uncomment below for testing with no Chunks
        Initialize(gridScale, gridLines.x, gridLines.y, gridLines.z, boxesVisible, gridCubeSizeFactor);
    }

    private void OnDestroy()
    {
        // DualInputManager.onTouching -= TouchingCallback;
    }

    public void Initialize(float gridScale, int gridLinesx, int gridLinesy, int gridLinesz, bool boxesVisible,
    float gridCubeSizeFactor)
    {
        this.gridScale = gridScale;
        this.gridLines.x = gridLinesx;
        this.gridLines.y = gridLinesy;
        this.gridLines.z = gridLinesz;
        this.boxesVisible = boxesVisible;
        this.gridCubeSizeFactor = gridCubeSizeFactor;

        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        dataPointCube = new GameObject[gridLinesx, gridLinesy, gridLinesz];

        gridValues = new float[gridLinesx, gridLinesy, gridLinesz];
        gridCubeSize = gridScale * gridCubeSizeFactor;

        for (int z = 0; z < gridValues.GetLength(2); z++)
        {
            for (int y = 0; y < gridValues.GetLength(1); y++)
            {
                for (int x = 0; x < gridValues.GetLength(0); x++)
                {
                    gridValues[x, y, z] = voxelGridValues[x, y, z];

                    //gridValues[x,y,z] = isoValue + Random.Range(-0.5f, 0.5f); 
                    //Debug.Log($"Cube ({x}, {y}, {z}) has a value of {value}");

                    if (gridValues[x, y, z] == 1)
                    {
                        gridValues[x, y, z] += Random.Range(0, randomizer);
                    }
                }
            }
        }

        volumeGrid = new VolumeGrid(gridLinesx - 1, gridLinesy - 1, gridLinesz - 1, gridScale, isoValue);

        MeshRenderer targetRenderer = targetObject.GetComponent<MeshRenderer>();
        Vector3 center = targetRenderer.bounds.center;
        transform.position = center;
        transform.localScale = targetObject.transform.localScale;
        transform.rotation = targetObject.transform.rotation;

        GenerateMesh();
    }

    private void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        vertices.Clear();
        triangles.Clear();

        volumeGrid.Update(gridValues);

        mesh.vertices = volumeGrid.GetVertices();
        mesh.triangles = volumeGrid.GetTriangles();

        Vector2[] uvs = volumeGrid.GetUVs();
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] /= gridScale;
            uvs[i] /= (gridLines.x - 1);
        }

        mesh.uv = uvs;
        mesh.RecalculateNormals();

        //Assigning the two lists to the mesh filter
        filter.mesh = mesh;
        //Video 16 -- GenerateCollider not implemented
        //GenerateCollider();
    }


    private void RemoveCubes(GameObject[,,] dataPointCube, float bufferBeforeDestroy)
    {
        for (int z = 0; z < gridLines.z; z++)
        {
            for (int y = 0; y < gridLines.y; y++)
            {
                for (int x = 0; x < gridLines.x; x++)
                {
                    if (gridValues[x, y, z] < isoValue - bufferBeforeDestroy)
                    {
                        if (dataPointCube[x, y, z] != null)
                        {
                            Destroy(dataPointCube[x, y, z]);
                            //Debug.Log($"Destroyed Cube [{x}, {y}, {z}]");
                        }
                        //else
                        //Debug.Log($"already destroyed cube [{x},{y},{z}]");
                    }
                }
            }
        }
    }
    public float[,,] LoadFloatArray(string filePath, string dimensionsFilePath)
    {
        int xLength, yLength, zLength;

        using (StreamReader reader = new StreamReader(File.Open(dimensionsFilePath, FileMode.Open)))
        {
            gridScale = float.Parse(reader.ReadLine());
            xLength = int.Parse(reader.ReadLine());
            yLength = int.Parse(reader.ReadLine());
            zLength = int.Parse(reader.ReadLine());
        }

        float[,,] array = new float[xLength, yLength, zLength];

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        array[x, y, z] = reader.ReadSingle();
                    }
                }
            }
        }

        return array;
    }
    private Vector3 GridToWorldPosition(int x, int y, int z)
    {
        Vector3 worldPosition = new Vector3(x, y, z) * gridScale;
        worldPosition.x -= (gridLines.x * gridScale) / 2 - gridScale / 2;
        worldPosition.y -= (gridLines.y * gridScale) / 2 - gridScale / 2;
        worldPosition.z -= (gridLines.z * gridScale) / 2 - gridScale / 2;
        return worldPosition;
    }

    private Vector3Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3Int gridPosition = new Vector3Int();
        gridPosition.x = Mathf.RoundToInt((worldPosition.x + ((gridLines.x * gridScale / 2)) - (gridScale / 2)) / gridScale);
        gridPosition.y = Mathf.RoundToInt((worldPosition.y + ((gridLines.y * gridScale / 2)) - (gridScale / 2)) / gridScale);
        gridPosition.z = Mathf.RoundToInt((worldPosition.z + ((gridLines.z * gridScale / 2)) - (gridScale / 2)) / gridScale);
        return gridPosition;
    }

#if UNITY_EDITOR
    /* private void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying)
            return;

        Gizmos.color = Color.green;
        for (int z = 0; z < gridValues.GetLength(2); z++)
        {
            for (int y = 0; y < gridValues.GetLength(1); y++)
            {
                for (int x = 0; x < gridValues.GetLength(0); x++)
                {
                    Vector3 worldPosition = GridToWorldPosition(x, y, z);
                    Gizmos.DrawSphere(worldPosition, gridScale / 10f);

                    Handles.Label(worldPosition + Vector3.up * gridScale / 2, gridValues[x, y, z].ToString());
                }
            }
        }
    } */
#endif
}