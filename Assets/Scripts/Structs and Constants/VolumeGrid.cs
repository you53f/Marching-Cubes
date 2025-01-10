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

    public VolumeGrid(int numberofCubesX, int numberofCubesY, int numberofCubesZ, float gridScale, float isoValue)
    {
        cubes = new Cube[numberofCubesX, numberofCubesY, numberofCubesZ];
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        // Debug.Log($"cubes: {numberofCubesX} x {numberofCubesY} x {numberofCubesZ}");
        // Debug.Log($"cubes total: {numberofCubesX*numberofCubesY*numberofCubesZ}");

        this.isoValue = isoValue;

        for (int z = 0; z < numberofCubesZ; z++)
        {
            for (int y = 0; y < numberofCubesY; y++)
            {
                for (int x = 0; x < numberofCubesX; x++)
                {
                    Vector3 cubePosition = new Vector3(x, y, z) * gridScale;
                    cubePosition.x -= ((numberofCubesX * gridScale) / 2) - (gridScale / 2);
                    cubePosition.y -= ((numberofCubesY * gridScale) / 2) - (gridScale / 2);
                    cubePosition.z -= ((numberofCubesZ * gridScale) / 2) - (gridScale / 2);
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
        

        for (int z = 0; z < cubes.GetLength(2); z++)
        {
            for (int y = 0; y < cubes.GetLength(1); y++)
            {
                for (int x = 0; x < cubes.GetLength(0); x++)
                {
                    Cube currentCube = cubes[x, y, z];

                    float[] cornerValues = new float[8];

                    cornerValues[0] = gridValues[x + 1, y + 1, z];
                    cornerValues[1] = gridValues[x, y + 1, z];
                    cornerValues[2] = gridValues[x, y, z];
                    cornerValues[3] = gridValues[x + 1, y, z];
                    cornerValues[4] = gridValues[x + 1, y + 1, z + 1];
                    cornerValues[5] = gridValues[x, y + 1, z + 1];
                    cornerValues[6] = gridValues[x, y, z + 1];
                    cornerValues[7] = gridValues[x + 1, y, z + 1];

                    currentCube.TriangulateWithInterpolation(isoValue, cornerValues);

                    vertices.AddRange(currentCube.GetVertices());

                    int[] currentCubeTriangles = currentCube.GetTriangles();

                    for (int i = 0; i < currentCubeTriangles.Length; i++)
                        currentCubeTriangles[i] += triangleStartIndex;

                    triangles.AddRange(currentCubeTriangles);
                    triangleStartIndex += currentCube.GetVertices().Length;

                    uvs.AddRange(currentCube.GetUvs());
                }
            }
        }
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
