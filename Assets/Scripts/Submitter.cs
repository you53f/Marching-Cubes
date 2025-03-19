
using TMPro;
using System.IO;
using UnityEngine;

public class Submitter : MonoBehaviour

{
    private EditVoxelsVR AccessOutTopeditVoxelsVR;
    private EditVoxelsVR AccessOutMideditVoxelsVR;
    private EditVoxelsVR AccessOutLoweditVoxelsVR;
    private EditVoxelsVR AccessInTopeditVoxelsVR;
    private EditVoxelsVR AccessInLoweditVoxelsVR;
    private EditVoxelsVR BackIneditVoxelsVR;
    private EditVoxelsVR BackOuteditVoxelsVR;
    private float[,,] AccessOutTopCurrentGridValues;
    private float[,,] AccessOutMidCurrentGridValues;
    private float[,,] AccessOutLowCurrentGridValues;
    private float[,,] AccessInTopCurrentGridValues;
    private float[,,] AccessInLowCurrentGridValues;
    private float[,,] BackInCurrentGridValues;
    private float[,,] BackOutCurrentGridValues;
    private float[,,] AccessOutTopbenchmark;
    private float[,,] AccessOutMidbenchmark;
    private float[,,] AccessOutLowbenchmark;
    private float[,,] AccessInTopbenchmark;
    private float[,,] AccessInLowbenchmark;
    private float[,,] BackInbenchmark;
    private float[,,] BackOutbenchmark;
    private float gridScale;
    private float isoValue;

    [HideInInspector] public float AccessOutTopsimilarityPercentage;
    [HideInInspector] public float AccessOutMidsimilarityPercentage;
    [HideInInspector] public float AccessOutLowsimilarityPercentage;
    [HideInInspector] public float AccessInTopsimilarityPercentage;
    [HideInInspector] public float AccessInLowsimilarityPercentage;
    [HideInInspector] public float BackInsimilarityPercentage;
    [HideInInspector] public float BackOutsimilarityPercentage;
    [SerializeField] private TextMeshProUGUI AccessOutTopText;
    [SerializeField] private TextMeshProUGUI AccessOutMidText;
    [SerializeField] private TextMeshProUGUI AccessOutLowText;
    [SerializeField] private TextMeshProUGUI AccessInTopText;
    [SerializeField] private TextMeshProUGUI AccessInLowText;
    [SerializeField] private TextMeshProUGUI BackInText;
    [SerializeField] private TextMeshProUGUI BackOutText;
    [SerializeField] private bool saveIt;
    [SerializeField] private bool compareIt;

    [Header("Edit Voxel Objects")]
    [SerializeField] private GameObject AccessOutTopObject;
    [SerializeField] private GameObject AccessOutMidObject;
    [SerializeField] private GameObject AccessOutLowObject;
    [SerializeField] private GameObject AccessInTopObject;
    [SerializeField] private GameObject AccessInLowObject;
    [SerializeField] private GameObject BackInObject;
    [SerializeField] private GameObject BackOutObject;

    [Header("Trial Settings")]
    [SerializeField] private string AccessOutTopPath;
    [SerializeField] private string AccessOutTopDimensionsPath;
    [SerializeField] private string AccessOutMidPath;
    [SerializeField] private string AccessOutMidDimensionsPath;
    [SerializeField] private string AccessOutLowPath;
    [SerializeField] private string AccessOutLowDimensionsPath;
    [SerializeField] private string AccessInTopPath;
    [SerializeField] private string AccessInTopDimensionsPath;
    [SerializeField] private string AccessInLowPath;
    [SerializeField] private string AccessInLowDimensionsPath;
    [SerializeField] private string BackInPath;
    [SerializeField] private string BackInDimensionsPath;
    [SerializeField] private string BackOutPath;
    [SerializeField] private string BackOutDimensionsPath;


    [Header("Becnhmark Setting")]
    [SerializeField] private string AccessOutTopBenchmarkPath;
    [SerializeField] private string AccessOutTopBenchmarkPathDimensions;

    [SerializeField] private string AccessOutMidBenchmarkPath;
    [SerializeField] private string AccessOutMidBenchmarkPathDimensions;

    [SerializeField] private string AccessOutLowBenchmarkPath;
    [SerializeField] private string AccessOutLowBenchmarkPathDimensions;

    [SerializeField] private string AccessInTopBenchmarkPath;
    [SerializeField] private string AccessInTopBenchmarkPathDimensions;

    [SerializeField] private string AccessInLowBenchmarkPath;
    [SerializeField] private string AccessInLowBenchmarkPathDimensions;

    [SerializeField] private string BackInBenchmarkPath;
    [SerializeField] private string BackInBenchmarkPathDimensions;

    [SerializeField] private string BackOutBenchmarkPath;
    [SerializeField] private string BackOutBenchmarkPathDimensions;

    // Start is called before the first frame update
    void Start()
    {
        AccessOutTopeditVoxelsVR = AccessOutTopObject.GetComponent<EditVoxelsVR>();
        AccessOutTopCurrentGridValues = AccessOutTopeditVoxelsVR.FinishedGridValues();
        isoValue = AccessOutTopeditVoxelsVR.isoValue;

        AccessOutMideditVoxelsVR = AccessOutMidObject.GetComponent<EditVoxelsVR>();
        AccessOutMidCurrentGridValues = AccessOutMideditVoxelsVR.FinishedGridValues();

        AccessOutLoweditVoxelsVR = AccessOutLowObject.GetComponent<EditVoxelsVR>();
        AccessOutLowCurrentGridValues = AccessOutLoweditVoxelsVR.FinishedGridValues();

        AccessInTopeditVoxelsVR = AccessInTopObject.GetComponent<EditVoxelsVR>();
        AccessInTopCurrentGridValues = AccessInTopeditVoxelsVR.FinishedGridValues();

        AccessInLoweditVoxelsVR = AccessInLowObject.GetComponent<EditVoxelsVR>();
        AccessInLowCurrentGridValues = AccessInLoweditVoxelsVR.FinishedGridValues();

        BackIneditVoxelsVR = BackInObject.GetComponent<EditVoxelsVR>();
        BackInCurrentGridValues = BackIneditVoxelsVR.FinishedGridValues();

        BackOuteditVoxelsVR = BackOutObject.GetComponent<EditVoxelsVR>();
        BackOutCurrentGridValues = BackOuteditVoxelsVR.FinishedGridValues();

        AccessOutTop();
        AccessOutMid();
        AccessOutLow();
        AccessInTop();
        AccessInLow();
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

    private void AccessOutTop()
    {
        if (saveIt)
            SaveFloatArray(AccessOutTopCurrentGridValues, AccessOutTopPath, AccessOutTopDimensionsPath, AccessOutTopeditVoxelsVR);

        if (compareIt)
        {
            AccessOutTopbenchmark = LoadFloatArray(AccessOutTopBenchmarkPath, AccessOutTopBenchmarkPathDimensions);
            AccessOutTopsimilarityPercentage = Compare3DArrays(AccessOutTopCurrentGridValues, AccessOutTopbenchmark, isoValue);
            AccessOutTopText.text = $"{AccessOutTopsimilarityPercentage:F2}%";
        }
    }

    private void AccessOutMid()
    {
        if (saveIt)
            SaveFloatArray(AccessOutMidCurrentGridValues, AccessOutMidPath, AccessOutMidDimensionsPath, AccessOutMideditVoxelsVR);

        if (compareIt)
        {
            AccessOutMidbenchmark = LoadFloatArray(AccessOutMidBenchmarkPath, AccessOutMidBenchmarkPathDimensions);
            AccessOutMidsimilarityPercentage = Compare3DArrays(AccessOutMidCurrentGridValues, AccessOutMidbenchmark, isoValue);
            AccessOutMidText.text = $"{AccessOutMidsimilarityPercentage:F2}%";
        }
    }

    private void AccessOutLow()
    {
        if (saveIt)
            SaveFloatArray(AccessOutLowCurrentGridValues, AccessOutLowPath, AccessOutLowDimensionsPath, AccessOutLoweditVoxelsVR);

        if (compareIt)
        {
            AccessOutLowbenchmark = LoadFloatArray(AccessOutLowBenchmarkPath, AccessOutLowBenchmarkPathDimensions);
            AccessOutLowsimilarityPercentage = Compare3DArrays(AccessOutLowCurrentGridValues, AccessOutLowbenchmark, isoValue);
            AccessOutLowText.text = $"{AccessOutLowsimilarityPercentage:F2}%";
        }
    }

    private void AccessInTop()
    {
        if (saveIt)
            SaveFloatArray(AccessInTopCurrentGridValues, AccessInTopPath, AccessInTopDimensionsPath, AccessInTopeditVoxelsVR);

        if (compareIt)
        {
            AccessInTopbenchmark = LoadFloatArray(AccessInTopBenchmarkPath, AccessInTopBenchmarkPathDimensions);
            AccessInTopsimilarityPercentage = Compare3DArrays(AccessInTopCurrentGridValues, AccessInTopbenchmark, isoValue);
            AccessInTopText.text = $"{AccessInTopsimilarityPercentage:F2}%";
        }
    }

    private void AccessInLow()
    {
        if (saveIt)
            SaveFloatArray(AccessInLowCurrentGridValues, AccessInLowPath, AccessInLowDimensionsPath, AccessInLoweditVoxelsVR);

        if (compareIt)
        {
            AccessInLowbenchmark = LoadFloatArray(AccessInLowBenchmarkPath, AccessInLowBenchmarkPathDimensions);
            AccessInLowsimilarityPercentage = Compare3DArrays(AccessInLowCurrentGridValues, AccessInLowbenchmark, isoValue);
            AccessInLowText.text = $"{AccessInLowsimilarityPercentage:F2}%";
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