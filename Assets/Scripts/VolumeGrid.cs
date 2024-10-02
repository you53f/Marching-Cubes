using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VolumeGrid
{
    public Cube[,,] cubes;

    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uvs;

    private float isoValue;

    public VolumeGrid(int numberofCubes, float gridScale, float isoValue)
    {
        cubes = new Cube[numberofCubes, numberofCubes, numberofCubes];
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        this.isoValue = isoValue;

        for (int z = 0; z < numberofCubes; z++)
        {
            for (int y = 0; y < numberofCubes; y++)
            {
                for (int x = 0; x < numberofCubes; x++)
                {
                    Vector3 cubePosition = new Vector3(x, y, z) * gridScale;
                    cubePosition.x -= (numberofCubes * gridScale) / 2 - gridScale / 2;
                    cubePosition.y -= (numberofCubes * gridScale) / 2 - gridScale / 2;
                    cubePosition.z -= (numberofCubes * gridScale) / 2 - gridScale / 2;
                    //Debug.Log("cube [" + x + ", " + y + ", " + z + "] has a position " + cubePosition);

                    cubes[x, y, z] = new Cube(cubePosition, gridScale);
                }
            }
        }
    }

    public void Update(float[,,] gridValues)
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

        int triangleStartIndex = 0;

        //int cubesnum = cubes.GetLength(0);
        //Debug.Log("we have " + cubesnum + " cubes in each direction");

        for (int z = 0; z < cubes.GetLength(2); z++)
        {
            for (int y = 0; y < cubes.GetLength(1); y++)
            {
                for (int x = 0; x < cubes.GetLength(0); x++)
                {
                    Cube currentcube = cubes[x, y, z];

                    float[] cornerValues = new float[8];

                    cornerValues[0] = gridValues[x + 1, y + 1, z];
                    cornerValues[1] = gridValues[x, y + 1, z];
                    cornerValues[2] = gridValues[x, y, z];
                    cornerValues[3] = gridValues[x + 1, y, z];
                    cornerValues[4] = gridValues[x + 1, y + 1, z + 1];
                    cornerValues[5] = gridValues[x, y + 1, z + 1];
                    cornerValues[6] = gridValues[x, y, z + 1];
                    cornerValues[7] = gridValues[x + 1, y, z + 1];

                    currentcube.TriangulateWithInterpolation(isoValue, cornerValues);

                    vertices.AddRange(currentcube.GetVertices());

                    int[] currentCubeTriangles = currentcube.GetTriangles();

                    for (int i = 0; i < currentCubeTriangles.Length; i++)
                        currentCubeTriangles[i] += triangleStartIndex;

                    triangles.AddRange(currentCubeTriangles);
                    triangleStartIndex += 12;

                    uvs.AddRange(currentcube.GetUvs());
                }
            }
        }

        //Debug.Log("Vertices World Position: " + vertices.Count);
 /*        foreach (Vector3 element in vertices)
        {
            Debug.Log("Vertex " + element);
            
            GameObject vertex = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            vertex.transform.localPosition = element;
            vertex.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            Material redMaterial = new Material(Shader.Find("Standard"));
            redMaterial.color = Color.red;
            vertex.GetComponent<Renderer>().material = redMaterial;
        } 
        Debug.Log("Traingles List: " + triangles.Count);
        foreach (int element in triangles)
        {
            Debug.Log("Triangle " + element);
        } */
    }

    public int[] GetTriangles()
    {
        return triangles.ToArray();
    }

    public Vector3[] GetVertices()
    {
        return vertices.ToArray();
    }

    public Vector2[] GetUVs()
    {
        return uvs.ToArray();
    }
}
