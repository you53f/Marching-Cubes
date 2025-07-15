using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class DeepVoxelizer : MonoBehaviour
{
    [Header("Voxel Settings")]
    [SerializeField] private bool startOnitsOwn = false;
    [SerializeField] public float voxelSize;
    [SerializeField] private bool visualize;
    [SerializeField] private bool addBufferLayer = true;

    [Header("Precision Settings")]
    [Tooltip("Increase for small meshes to improve precision")]
    [SerializeField] private float precisionScale = 1000f;
    [SerializeField] private float rayEpsilon = 1e-10f;

    [Header("Visualization Settings")]
    [SerializeField] private float sphereScale = 0.5f;
    public bool visualizeOutside = false;

    private bool[,,] voxelGrid;
    private GameObject visualizationParent;
    private Vector3 gridOrigin;
    private Vector3Int gridDimensions;
    private Matrix4x4 precisionTransform;

    void Start()
    {
        if(startOnitsOwn)
            VoxelizeMesh();
    }

    void OnValidate()
    {
        if (Application.isPlaying && visualize && voxelGrid != null)
        {
            ClearVisualization();
            VisualizeVoxels();
        }
    }

    [ContextMenu("Voxelize Mesh")]
    public void VoxelizeMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("No mesh found!");
            return;
        }

        voxelGrid = Voxelize(meshFilter, voxelSize, addBufferLayer);
        Debug.Log($"{gameObject.name} has a voxel grid of: {voxelGrid.GetLength(0)}x{voxelGrid.GetLength(1)}x{voxelGrid.GetLength(2)} wich is {voxelGrid.Length} voxels in total.");

        if (visualize)
        {
            ClearVisualization();
            VisualizeVoxels();
        }
    }

    private bool[,,] Voxelize(MeshFilter meshFilter, float size, bool addBuffer)
    {
        Mesh mesh = meshFilter.sharedMesh;
        Transform transform = meshFilter.transform;

        // Calculate world bounds with double precision
        Bounds bounds = CalculateWorldBounds(mesh, transform);
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        // Create precision scaling matrix
        precisionTransform = Matrix4x4.Scale(Vector3.one * precisionScale);
        Vector3 scaledMin = precisionTransform.MultiplyPoint3x4(min);
        Vector3 scaledMax = precisionTransform.MultiplyPoint3x4(max);

        // Calculate buffer size
        float buffer = addBuffer ? size * precisionScale : 0;
        gridOrigin = scaledMin - Vector3.one * buffer;

        // Calculate grid dimensions
        Vector3 scaledSize = precisionTransform.MultiplyVector(Vector3.one * size);
        Vector3 gridSize = scaledMax - scaledMin + Vector3.one * (2 * buffer);
        gridDimensions = new Vector3Int(
            Mathf.CeilToInt(gridSize.x / scaledSize.x),
            Mathf.CeilToInt(gridSize.y / scaledSize.y),
            Mathf.CeilToInt(gridSize.z / scaledSize.z)
        );

        // Initialize voxel grid
        bool[,,] voxels = new bool[gridDimensions.x, gridDimensions.y, gridDimensions.z];
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        int triangleCount = triangles.Length / 3;

        // Pre-calculate transformed vertices with precision scaling
        Vector3[] worldVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            worldVertices[i] = transform.TransformPoint(vertices[i]);
            worldVertices[i] = precisionTransform.MultiplyPoint3x4(worldVertices[i]);
        }

        // Pre-calculate triangle data with precision scaling
        Triangle[] worldTriangles = new Triangle[triangleCount];
        for (int i = 0; i < triangleCount; i++)
        {
            int index = i * 3;
            worldTriangles[i] = new Triangle(
                worldVertices[triangles[index]],
                worldVertices[triangles[index + 1]],
                worldVertices[triangles[index + 2]]
            );
        }

        // Process each voxel
        Vector3 scaledVoxelSize = precisionTransform.MultiplyVector(Vector3.one * size);
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                for (int z = 0; z < gridDimensions.z; z++)
                {
                    Vector3 voxelCenter = gridOrigin + new Vector3(
                        (x + 0.5f) * scaledVoxelSize.x,
                        (y + 0.5f) * scaledVoxelSize.y,
                        (z + 0.5f) * scaledVoxelSize.z
                    );

                    voxels[x, y, z] = IsPointInsideMesh(voxelCenter, worldTriangles);
                }
            }
        }

        return voxels;
    }

    private Bounds CalculateWorldBounds(Mesh mesh, Transform transform)
    {
        Vector3[] vertices = mesh.vertices;
        if (vertices.Length == 0) return new Bounds();

        Vector3 min = transform.TransformPoint(vertices[0]);
        Vector3 max = min;

        for (int i = 1; i < vertices.Length; i++)
        {
            Vector3 worldVertex = transform.TransformPoint(vertices[i]);
            min = Vector3.Min(min, worldVertex);
            max = Vector3.Max(max, worldVertex);
        }

        return new Bounds((min + max) * 0.5f, max - min);
    }

    private bool IsPointInsideMesh(Vector3 point, Triangle[] triangles)
    {
        // Cast ray in a direction less likely to be parallel to surfaces
        Vector3 rayDirection = new Vector3(0.678f, 0.731f, 0.567f).normalized;
        Ray ray = new Ray(point, rayDirection);
        int intersectionCount = 0;

        foreach (Triangle triangle in triangles)
        {
            if (RayIntersectsTriangle(ray, triangle.v0, triangle.v1, triangle.v2))
            {
                intersectionCount++;
            }
        }

        // Odd number of intersections = inside mesh
        return intersectionCount % 2 == 1;
    }

    private bool RayIntersectsTriangle(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;
        Vector3 h = Vector3.Cross(ray.direction, edge2);
        float a = Vector3.Dot(edge1, h);

        if (a > -rayEpsilon && a < rayEpsilon)
            return false;

        float f = 1.0f / a;
        Vector3 s = ray.origin - v0;
        float u = f * Vector3.Dot(s, h);

        if (u < 0.0f || u > 1.0f)
            return false;

        Vector3 q = Vector3.Cross(s, edge1);
        float v = f * Vector3.Dot(ray.direction, q);

        if (v < 0.0f || u + v > 1.0f)
            return false;

        float t = f * Vector3.Dot(edge2, q);
        return t > rayEpsilon;
    }

    private void VisualizeVoxels()
    {
        if (voxelGrid == null) return;

        visualizationParent = new GameObject("Voxel Visualization");
        visualizationParent.transform.SetParent(transform);

        Matrix4x4 inverseTransform = precisionTransform.inverse;
        float scaledSize = voxelSize * sphereScale;

        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                for (int z = 0; z < gridDimensions.z; z++)
                {
                    if (!voxelGrid[x, y, z] && !visualizeOutside)
                        continue;

                    Vector3 scaledPosition = gridOrigin + new Vector3(
                        (x + 0.5f) * voxelSize * precisionScale,
                        (y + 0.5f) * voxelSize * precisionScale,
                        (z + 0.5f) * voxelSize * precisionScale
                    );

                    Vector3 worldPosition = inverseTransform.MultiplyPoint3x4(scaledPosition);

                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.position = worldPosition;
                    sphere.transform.localScale = Vector3.one * scaledSize;
                    sphere.transform.SetParent(visualizationParent.transform);

                    Renderer renderer = sphere.GetComponent<Renderer>();
                    if (voxelGrid[x, y, z])
                    {
                        renderer.material = CreateDefaultMaterial(Color.red);
                    }
                    else if (visualizeOutside)
                    {
                        renderer.material = CreateDefaultMaterial(Color.black);
                    }

                    Destroy(sphere.GetComponent<Collider>());
                }
            }
        }
    }

    private Material CreateDefaultMaterial(Color color)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        return mat;
    }

    private void ClearVisualization()
    {
        if (visualizationParent != null)
        {
            Destroy(visualizationParent);
        }
    }

    private struct Triangle
    {
        public readonly Vector3 v0, v1, v2;

        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }
    }
    
    public float[,,] GetVoxelGrid()
    {
        int xLength = voxelGrid.GetLength(0);
        int yLength = voxelGrid.GetLength(1);
        int zLength = voxelGrid.GetLength(2);
        float[,,] voxelValues = new float[xLength, yLength, zLength];

        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    voxelValues[x, y, z] = voxelGrid[x, y, z] ? 1f : 0f;
                }
            }
        }

        return voxelValues;
    }
}