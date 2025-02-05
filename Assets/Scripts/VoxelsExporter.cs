using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class VoxelsExporter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ScrawkVoxelizer scrawk;
    private float[,,] voxelGridValues;
    [SerializeField] private string voxelGridValuesPath;
    [SerializeField] private string voxelDimensionsPath;
    void Start()
    {
        scrawk.StartVoxels();
        voxelGridValues = scrawk.GetVoxelGrid();
        SaveFloatArray(voxelGridValues, voxelGridValuesPath, voxelDimensionsPath);
    }

    private void SaveFloatArray(float[,,] array, string filePath,string dimensionsFilePath)
    {
        int xLength = array.GetLength(0);
        int yLength = array.GetLength(1);
        int zLength = array.GetLength(2);
        // Debug.Log($"xLength = {xLength}, yLength = {yLength}, zLength = {zLength}");
        float[] flatArray = new float[xLength * yLength * zLength];

        int index = 0;
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    flatArray[index] = array[x, y, z];
                    index++;
                }
            }
        }

        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            foreach (float value in flatArray)
            {
                writer.Write(value);
            }
        }

        using (StreamWriter writer = new StreamWriter(File.Open(dimensionsFilePath, FileMode.Create))) {
        writer.WriteLine(scrawk.voxelResolution);
        writer.WriteLine(xLength);
        writer.WriteLine(yLength);
        writer.WriteLine(zLength);
    }
    }
}