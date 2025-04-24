
using TMPro;
using System.IO;
using UnityEngine;

public class MolarSubmitter : MonoBehaviour

{
    private EditVoxelsVR PCeditVoxlesVR;
    private EditVoxelsVR DBCeditVoxlesVR;
    private EditVoxelsVR MBCeditVoxlesVR;
    private EditVoxelsVR RoofeditVoxlesVR;
    private float[,,] PCCurrentGridValues;
    private float[,,] DBCCurrentGridValues;
    private float[,,] MBCCurrentGridValues;
    private float[,,] RoofCurrentGridValues;
    private float[,,] PCbenchmark;
    private float[,,] DBCbenchmark;
    private float[,,] MBCbenchmark;
    private float[,,] Roofbenchmark;
    private float gridScale;
    private float isoValue;

    [HideInInspector] public float PCsimilarityPercentage;
    [HideInInspector] public float DBCsimilarityPercentage;
    [HideInInspector] public float MBCsimilarityPercentage;
    [HideInInspector] public float RoofsimilarityPercentage;
    private bool floorHit;
    private bool diffHit;
    [SerializeField] private ControllerInput controllerInput;
    [SerializeField] private HapticInput hapticInput;
    [SerializeField] private TextMeshProUGUI PCText;
    [SerializeField] private TextMeshProUGUI DBCText;
    [SerializeField] private TextMeshProUGUI MBCText;
    [SerializeField] private TextMeshProUGUI RoofText;
    [SerializeField] private TextMeshProUGUI FloorText;
    [SerializeField] private TextMeshProUGUI DiffText;
    [SerializeField] private bool saveIt;
    [SerializeField] private bool compareIt;

    [Header("Edit Voxel Objects")]
    [SerializeField] private GameObject PCObject;
    [SerializeField] private GameObject DBCObject;
    [SerializeField] private GameObject MBCObject;
    [SerializeField] private GameObject RoofObject;

    [Header("Trial Settings")]
    [SerializeField] private string PCPath;
    [SerializeField] private string PCDimensionsPath;
    [SerializeField] private string DBCPath;
    [SerializeField] private string DBCDimensionsPath;
    [SerializeField] private string MBCPath;
    [SerializeField] private string MBCDimensionsPath;
    [SerializeField] private string RoofPath;
    [SerializeField] private string RoofDimensionsPath;


    [Header("Becnhmark Setting")]
    [SerializeField] private string PCBenchmarkPath;
    [SerializeField] private string PCBenchmarkPathDimensions;

    [SerializeField] private string DBCBenchmarkPath;
    [SerializeField] private string DBCBenchmarkPathDimensions;

    [SerializeField] private string MBCBenchmarkPath;
    [SerializeField] private string MBCBenchmarkPathDimensions;

    [SerializeField] private string RoofBenchmarkPath;
    [SerializeField] private string RoofBenchmarkPathDimensions;

    // Start is called before the first frame update
    void Start()
    {
        PCeditVoxlesVR = PCObject.GetComponent<EditVoxelsVR>();
        PCCurrentGridValues = PCeditVoxlesVR.FinishedGridValues();
        isoValue = PCeditVoxlesVR.isoValue;

        DBCeditVoxlesVR = DBCObject.GetComponent<EditVoxelsVR>();
        DBCCurrentGridValues = DBCeditVoxlesVR.FinishedGridValues();

        MBCeditVoxlesVR = MBCObject.GetComponent<EditVoxelsVR>();
        MBCCurrentGridValues = MBCeditVoxlesVR.FinishedGridValues();

        RoofeditVoxlesVR = RoofObject.GetComponent<EditVoxelsVR>();
        RoofCurrentGridValues = RoofeditVoxlesVR.FinishedGridValues();

        PC();
        DBC();
        MBC();
        Roof();
        Floor();
        Diff();
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

    private void PC()
    {
        if (saveIt)
            SaveFloatArray(PCCurrentGridValues, PCPath, PCDimensionsPath, PCeditVoxlesVR);

        if (compareIt)
        {
            PCbenchmark = LoadFloatArray(PCBenchmarkPath, PCBenchmarkPathDimensions);
            PCsimilarityPercentage = Compare3DArrays(PCCurrentGridValues, PCbenchmark, isoValue);
            PCText.text = $"{PCsimilarityPercentage:F2}%";
        }
    }

    private void DBC()
    {
        if (saveIt)
            SaveFloatArray(DBCCurrentGridValues, DBCPath, DBCDimensionsPath, DBCeditVoxlesVR);

        if (compareIt)
        {
            DBCbenchmark = LoadFloatArray(DBCBenchmarkPath, DBCBenchmarkPathDimensions);
            DBCsimilarityPercentage = Compare3DArrays(DBCCurrentGridValues, DBCbenchmark, isoValue);
            DBCText.text = $"{DBCsimilarityPercentage:F2}%";
        }
    }

    private void MBC()
    {
        if (saveIt)
            SaveFloatArray(MBCCurrentGridValues, MBCPath, MBCDimensionsPath, MBCeditVoxlesVR);

        if (compareIt)
        {
            MBCbenchmark = LoadFloatArray(MBCBenchmarkPath, MBCBenchmarkPathDimensions);
            MBCsimilarityPercentage = Compare3DArrays(MBCCurrentGridValues, MBCbenchmark, isoValue);
            MBCText.text = $"{MBCsimilarityPercentage:F2}%";
        }
    }

    private void Roof()
    {
        if (saveIt)
            SaveFloatArray(RoofCurrentGridValues, RoofPath, RoofDimensionsPath, RoofeditVoxlesVR);

        if (compareIt)
        {
            Roofbenchmark = LoadFloatArray(RoofBenchmarkPath, RoofBenchmarkPathDimensions);
            RoofsimilarityPercentage = Compare3DArrays(RoofCurrentGridValues, Roofbenchmark, isoValue);
            RoofText.text = $"{RoofsimilarityPercentage:F2}%";
        }
    }

    private void Floor()
    {
        floorHit = controllerInput.floorHit;
        if (floorHit)
            FloorText.text = "Floor Hit!";
        else
            FloorText.text = "No Floor Hit!";
    }

    private void Diff()
    {
        diffHit = controllerInput.diffHit;
        if (diffHit)
            DiffText.text = "Diff Hit!";
        else
            DiffText.text = "No Diff Hit!";
    }
}