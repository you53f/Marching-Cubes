using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Vector3 = UnityEngine.Vector3;

using Vector2 = UnityEngine.Vector2;

public class EditVoxelsJobified : MonoBehaviour
{
    [Header("Brush Settings")]
    [SerializeField] private int brushSize;
    [SerializeField] private float brushStrength;
    [SerializeField] private float brushFallback;
    [SerializeField] private float bufferBeforeDestroy;
    float deminishingValue;

    Mesh mesh;

    [Header("Data")]
    [SerializeField] private bool boxesVisible;
    private int gridLines;
    private float gridScale;
    [SerializeField] private float gridCubeSizeFactor;
    [SerializeField] private float isoValue;
    [SerializeField] private MeshFilter filter;
    private VolumeGrid volumeGrid;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private float gridCubeSize;
    private float[,,] voxelGridValues;
    private float[,,] gridValues;
    GameObject[,,] dataPointCube;
    private Voxelizer voxelizer;

    private void Awake()
    {
        InputManager.onTouching += TouchingCallback;
        voxelizer = FindObjectOfType<Voxelizer>();
        voxelizer.StartVoxels();
        voxelGridValues = voxelizer.GetVoxelGrid();
        gridLines = GetLongestDimension(voxelGridValues);
        Debug.Log($"Most Grid Lines = {gridLines}");
        gridScale = voxelizer.voxelResolution;

        // Uncomment below for testing with no Chunks
        Initialize(gridScale, gridLines, boxesVisible, brushSize, brushStrength, brushFallback, gridCubeSizeFactor, bufferBeforeDestroy);
    }

    private void OnDestroy()
    {
        InputManager.onTouching -= TouchingCallback;
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

    public void Initialize(float gridScale, int gridLines, bool boxesVisible, int brushSize, float brushStrength, float brushFallback,
    float gridCubeSizeFactor, float bufferBeforeDestroy)
    {
        this.gridScale = gridScale;
        this.gridLines = gridLines;
        this.boxesVisible = boxesVisible;
        this.brushSize = brushSize;
        this.brushStrength = brushStrength;
        this.brushFallback = brushFallback;
        this.gridCubeSizeFactor = gridCubeSizeFactor;
        this.bufferBeforeDestroy = bufferBeforeDestroy;

        mesh = new Mesh();

        dataPointCube = new GameObject[gridLines, gridLines, gridLines];

        gridValues = new float[gridLines, gridLines, gridLines];
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

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    dataPointCube[x, y, z] = cube;
                    dataPointCube[x, y, z].transform.parent = this.transform;
                    dataPointCube[x, y, z].transform.localPosition = GridToWorldPosition(x, y, z);
                    dataPointCube[x, y, z].transform.localScale = new Vector3(gridCubeSize, gridCubeSize, gridCubeSize);
                    dataPointCube[x, y, z].GetComponent<Collider>().isTrigger = true;

                    MeshRenderer meshRenderer = dataPointCube[x, y, z].GetComponent<MeshRenderer>();
                    meshRenderer.material.color = Color.grey;
                    meshRenderer.enabled = boxesVisible;
                }
            }
        }

        volumeGrid = new VolumeGrid(gridLines - 1, gridScale, isoValue);

        GenerateMesh();
    }

    // What's responsible for scalar field editing
    private void TouchingCallback(Vector3 worldPosition)
    {
        //Debug.Log($"World-World Position: {worldPosition}");
        worldPosition = transform.InverseTransformPoint(worldPosition);
        //Debug.Log($"Inverse-World Position: {worldPosition}");
        Vector3Int gridPosition = WorldToGridPosition(worldPosition);
        //Debug.Log($"Grid Position converted: {gridPosition}");

        bool shouldGenerate = false;

        for (int z = gridPosition.z - brushSize; z <= gridPosition.z + brushSize; z++)
        {
            for (int y = gridPosition.y - brushSize; y <= gridPosition.y + brushSize; y++)
            {
                for (int x = gridPosition.x - brushSize; x <= gridPosition.x + brushSize; x++)
                {
                    Vector3Int currentGridPoisition = new Vector3Int(x, y, z);

                    if (!IsValidGridPosition(currentGridPoisition))
                    {
                        //Debug.Log($"it hit {currentGridPoisition} which is invalid");
                        continue;
                    }

                    else
                    {
                        float distance = Vector3.Distance(currentGridPoisition, gridPosition);
                        if (brushSize == 0)
                        {
                            gridValues[currentGridPoisition.x, currentGridPoisition.y, currentGridPoisition.z] -= brushStrength;
                            shouldGenerate = true;
                            //Debug.Log($"Successful decrease in cube {currentGridPoisition}");
                        }
                        else
                        {
                            deminishingValue = brushStrength * Mathf.Exp(-distance * brushFallback / brushSize);
                            gridValues[currentGridPoisition.x, currentGridPoisition.y, currentGridPoisition.z] -= deminishingValue;
                            shouldGenerate = true;
                            //Debug.Log($"Successful decrease in cube {currentGridPoisition}");
                        }
                    }
                }
            }
        }
        if (shouldGenerate)
        {
            GenerateMesh();
        }
    }

    private void GenerateMesh()
    {
        mesh = new Mesh();
        vertices.Clear();
        triangles.Clear();

        volumeGrid.Update(gridValues);

        mesh.vertices = volumeGrid.GetVertices();
        mesh.triangles = volumeGrid.GetTriangles();

        Vector2[] uvs = volumeGrid.GetUVs();
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] /= gridScale;
            uvs[i] /= (gridLines - 1);
        }

        mesh.uv = uvs;

        //Assigning the two lists to the mesh filter
        filter.mesh = mesh;

        //Video 16 -- GenerateCollider not implemented
        //GenerateCollider();

        RemoveCubes(dataPointCube);
    }

    private void RemoveCubes(GameObject[,,] dataPointCube)
    {
        for (int z = 0; z < gridLines; z++)
        {
            for (int y = 0; y < gridLines; y++)
            {
                for (int x = 0; x < gridLines; x++)
                {
                    if (gridValues[x, y, z] < isoValue - bufferBeforeDestroy)
                    {
                        Destroy(dataPointCube[x, y, z]);
                        //Debug.Log($"Destroyed Cube [{x}, {y}, {z}]");
                    }
                }
            }
        }
    }


    /* private void GenerateCollider()
    {
        if(filter.TryGetComponent(out MeshCollider meshCollider))
        meshCollider.sharedMesh = mesh;
        else
        filter.gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
    } */

    private Vector3 GridToWorldPosition(int x, int y, int z)
    {
        Vector3 worldPosition = new Vector3(x, y, z) * gridScale;
        worldPosition.x -= (gridLines * gridScale) / 2 - gridScale / 2;
        worldPosition.y -= (gridLines * gridScale) / 2 - gridScale / 2;
        worldPosition.z -= (gridLines * gridScale) / 2 - gridScale / 2;
        return worldPosition;
    }

    private Vector3Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3Int gridPosition = new Vector3Int();
        gridPosition.x = Mathf.RoundToInt((worldPosition.x + ((gridLines * gridScale / 2)) - (gridScale / 2)) / gridScale);
        gridPosition.y = Mathf.RoundToInt((worldPosition.y + ((gridLines * gridScale / 2)) - (gridScale / 2)) / gridScale);
        gridPosition.z = Mathf.RoundToInt((worldPosition.z + ((gridLines * gridScale / 2)) - (gridScale / 2)) / gridScale);
        return gridPosition;
    }


    // Just to avoid getting an error in the console when out of bounds
    private bool IsValidGridPosition(Vector3Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < gridLines &&
               gridPosition.y >= 0 && gridPosition.y < gridLines &&
               gridPosition.z >= 0 && gridPosition.z < gridLines;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
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
    }
#endif
}
