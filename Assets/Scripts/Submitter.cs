
using TMPro;
using System.IO;
using UnityEngine;

public class Submitter : MonoBehaviour

{
    private EditVoxelsVR editVoxelsVR;
    private float[,,] importedGridValues;
    private float[,,] benchmark;
    private float gridScale;
    private float isoValue;
    [HideInInspector] public float similarityPercentage;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Trial Setting")]
    [SerializeField] private string voxelGridValuesPath;
    [SerializeField] private string voxelGridValuesDimensionsPath;


    [Header("Becnhmark Setting")]
    [SerializeField] private string voxelGridValuesPath1;
    [SerializeField] private string voxelGridValuesDimensionsPath1;

    // Start is called before the first frame update
    void Start()
    {
        GameObject holderObject = GameObject.Find("Edit Voxels");
        editVoxelsVR = holderObject.GetComponent<EditVoxelsVR>();
        importedGridValues = editVoxelsVR.FinishedGridValues();
        isoValue = editVoxelsVR.isoValue;
        //SaveFloatArray(importedGridValues, voxelGridValuesPath,voxelGridValuesDimensionsPath);
        benchmark = LoadFloatArray(voxelGridValuesPath1, voxelGridValuesDimensionsPath1);
        similarityPercentage = Compare3DArrays(importedGridValues, benchmark, isoValue);
        resultText.text = $"{similarityPercentage:F2}%";
    }
    
    public void SaveFloatArray(float[,,] array, string filePath, string dimensionsFilePath)
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
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            foreach (float value in flatArray)
            {
                writer.Write(value);
            }
        }

        using (StreamWriter writer = new StreamWriter(File.Open(dimensionsFilePath, FileMode.Create))) {
        writer.WriteLine(editVoxelsVR.gridScale);
        writer.WriteLine(xLength);
        writer.WriteLine(yLength);
        writer.WriteLine(zLength);
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
    private float Compare3DArrays(float[,,] arrayA, float[,,] arrayB, float isoValue)
    {
        // Check if both arrays have the same dimensions
        if (arrayA.GetLength(0) != arrayB.GetLength(0) ||
            arrayA.GetLength(1) != arrayB.GetLength(1) ||
            arrayA.GetLength(2) != arrayB.GetLength(2))
        {
            Debug.LogError("Arrays must have the same dimensions.");
            return 0f;
        }

        int totalElements = arrayA.Length;
        int similarElements = 0;

        // Iterate through each element in the arrays
        for (int x = 0; x < arrayA.GetLength(0); x++)
        {
            for (int y = 0; y < arrayA.GetLength(1); y++)
            {
                for (int z = 0; z < arrayA.GetLength(2); z++)
                {
                    if (arrayA[x, y, z] <= isoValue && arrayB[x, y, z] <= isoValue)
                    {
                        similarElements++;
                    }
                    else if (arrayA[x,y,z] > isoValue && arrayB[x,y,z] > isoValue)
                    {
                        similarElements++;
                    }
                }
            }
        }

        // Calculate the percentage of similar elements
        float similarityPercentage = ((float)similarElements / totalElements) * 100;
        return similarityPercentage;
    }
}
