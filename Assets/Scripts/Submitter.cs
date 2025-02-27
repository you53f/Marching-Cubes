
using TMPro;
using System.IO;
using UnityEngine;

public class Submitter : MonoBehaviour

{
    private EditVoxelsVR AccessOuteditVoxelsVR;
    private EditVoxelsVR AccessMideditVoxelsVR;
    private EditVoxelsVR AccessIneditVoxelsVR;
    private EditVoxelsVR BackIneditVoxelsVR;
    private EditVoxelsVR BackOuteditVoxelsVR;
    private float[,,] AccessOutCurrentGridValues;
    private float[,,] AccessMidCurrentGridValues;
    private float[,,] AccessInCurrentGridValues;
    private float[,,] BackInCurrentGridValues;
    private float[,,] BackOutCurrentGridValues;
    private float[,,] AccessOutbenchmark;
    private float[,,] AccessMidbenchmark;
    private float[,,] AccessInbenchmark;
    private float[,,] BackInbenchmark;
    private float[,,] BackOutbenchmark;
    private float gridScale;
    private float isoValue;

    [HideInInspector] public float AccessOutsimilarityPercentage;
    [HideInInspector] public float AccessMidsimilarityPercentage;
    [HideInInspector] public float AccessInsimilarityPercentage;
    [HideInInspector] public float BackInsimilarityPercentage;
    [HideInInspector] public float BackOutsimilarityPercentage;
    [SerializeField] private TextMeshProUGUI AccessOutText;
    [SerializeField] private TextMeshProUGUI AccessMidText;
    [SerializeField] private TextMeshProUGUI AccessInText;
    [SerializeField] private TextMeshProUGUI BackInText;
    [SerializeField] private TextMeshProUGUI BackOutText;
    [SerializeField] private bool saveIt;
    [SerializeField] private bool compareIt;

    [Header("Edit Voxel Objects")]
    [SerializeField] private GameObject AccessOutObject;
    [SerializeField] private GameObject AccessMidObject;
    [SerializeField] private GameObject AccessInObject;
    [SerializeField] private GameObject BackInObject;
    [SerializeField] private GameObject BackOutObject;

    [Header("Trial Settings")]
    [SerializeField] private string AccessOutPath;
    [SerializeField] private string AccessOutDimensionsPath;
    [SerializeField] private string AccessMidPath;
    [SerializeField] private string AccessMidDimensionsPath;
    [SerializeField] private string AccessInPath;
    [SerializeField] private string AccessInDimensionsPath;
    [SerializeField] private string BackInPath;
    [SerializeField] private string BackInDimensionsPath;
    [SerializeField] private string BackOutPath;
    [SerializeField] private string BackOutDimensionsPath;


    [Header("Becnhmark Setting")]
    [SerializeField] private string AccessOutBenchmarkPath;
    [SerializeField] private string AccessOutBenchmarkPathDimensions;

    [SerializeField] private string AccessMidBenchmarkPath;
    [SerializeField] private string AccessMidBenchmarkPathDimensions;

    [SerializeField] private string AccessInBenchmarkPath;
    [SerializeField] private string AccessInBenchmarkPathDimensions;

    [SerializeField] private string BackInBenchmarkPath;
    [SerializeField] private string BackInBenchmarkPathDimensions;

    [SerializeField] private string BackOutBenchmarkPath;
    [SerializeField] private string BackOutBenchmarkPathDimensions;

    // Start is called before the first frame update
    void Start()
    {
        AccessOuteditVoxelsVR = AccessOutObject.GetComponent<EditVoxelsVR>();
        AccessOutCurrentGridValues = AccessOuteditVoxelsVR.FinishedGridValues();
        isoValue = AccessOuteditVoxelsVR.isoValue;

        AccessMideditVoxelsVR = AccessMidObject.GetComponent<EditVoxelsVR>();
        AccessMidCurrentGridValues = AccessMideditVoxelsVR.FinishedGridValues();

        AccessIneditVoxelsVR = AccessInObject.GetComponent<EditVoxelsVR>();
        AccessInCurrentGridValues = AccessIneditVoxelsVR.FinishedGridValues();

        BackIneditVoxelsVR = BackInObject.GetComponent<EditVoxelsVR>();
        BackInCurrentGridValues = BackIneditVoxelsVR.FinishedGridValues();

        BackOuteditVoxelsVR = BackOutObject.GetComponent<EditVoxelsVR>();
        BackOutCurrentGridValues = BackOuteditVoxelsVR.FinishedGridValues();

        AccessOut();
        AccessMid();
        AccessIn();
        BackIn();
        BackOut();
    }

    public void SaveFloatArray(float[,,] array, string filePath, string dimensionsFilePath, EditVoxelsVR evVR)
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

        using (StreamWriter writer = new StreamWriter(File.Open(dimensionsFilePath, FileMode.Create)))
        {
            writer.WriteLine(evVR.gridScale);
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

        int totalElements = 0;
        int similarElements = 0;

        // Iterate through each element in the arrays
        for (int x = 0; x < arrayA.GetLength(0); x++)
        {
            for (int y = 0; y < arrayA.GetLength(1); y++)
            {
                for (int z = 0; z < arrayA.GetLength(2); z++)
                {
                    if (arrayB[x, y, z] == -9999 || arrayB[x, y, z] > isoValue)
                    {
                        totalElements++;
                    }

                    if (arrayA[x, y, z] == -9999 && arrayB[x, y, z] == -9999)
                    {
                        similarElements++;
                    }

                    else if (arrayA[x, y, z] >= isoValue && arrayB[x, y, z] >= isoValue)
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

    private void AccessOut()
    {
        if (saveIt)
            SaveFloatArray(AccessOutCurrentGridValues, AccessOutPath, AccessOutDimensionsPath, AccessOuteditVoxelsVR);

        if (compareIt)
        {
            AccessOutbenchmark = LoadFloatArray(AccessOutBenchmarkPath, AccessOutBenchmarkPathDimensions);
            AccessOutsimilarityPercentage = Compare3DArrays(AccessOutCurrentGridValues, AccessOutbenchmark, isoValue);
            AccessOutText.text = $"{AccessOutsimilarityPercentage:F2}%";
        }
    }

    private void AccessMid()
    {
        if (saveIt)
            SaveFloatArray(AccessMidCurrentGridValues, AccessMidPath, AccessMidDimensionsPath, AccessMideditVoxelsVR);

        if (compareIt)
        {
            AccessMidbenchmark = LoadFloatArray(AccessMidBenchmarkPath, AccessMidBenchmarkPathDimensions);
            AccessMidsimilarityPercentage = Compare3DArrays(AccessMidCurrentGridValues, AccessMidbenchmark, isoValue);
            AccessMidText.text = $"{AccessMidsimilarityPercentage:F2}%";
        }
    }

    private void AccessIn()
    {
        if (saveIt)
            SaveFloatArray(AccessInCurrentGridValues, AccessInPath, AccessInDimensionsPath, AccessIneditVoxelsVR);

        if (compareIt)
        {
            AccessInbenchmark = LoadFloatArray(AccessInBenchmarkPath, AccessInBenchmarkPathDimensions);
            AccessInsimilarityPercentage = Compare3DArrays(AccessInCurrentGridValues, AccessInbenchmark, isoValue);
            AccessInText.text = $"{AccessInsimilarityPercentage:F2}%";
        }
    }

    private void BackIn()
    {
        if (saveIt)
            SaveFloatArray(BackInCurrentGridValues, BackInPath, BackInDimensionsPath, BackIneditVoxelsVR);

        if (compareIt)
        {
            BackInbenchmark = LoadFloatArray(BackInBenchmarkPath, BackInBenchmarkPathDimensions);
            BackInsimilarityPercentage = Compare3DArrays(BackInCurrentGridValues, BackInbenchmark, isoValue);
            BackInText.text = $"{BackInsimilarityPercentage:F2}%";
        }
    }

    private void BackOut()
    {
        if (saveIt)
            SaveFloatArray(BackOutCurrentGridValues, BackOutPath, BackOutDimensionsPath, BackOuteditVoxelsVR);

        if (compareIt)
        {
            BackOutbenchmark = LoadFloatArray(BackOutBenchmarkPath, BackOutBenchmarkPathDimensions);
            BackOutsimilarityPercentage = Compare3DArrays(BackOutCurrentGridValues, BackOutbenchmark, isoValue);
            BackOutText.text = $"{BackOutsimilarityPercentage:F2}%";
        }
    }
}