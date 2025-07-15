
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

    private bool[,,] PCDrilledList;
    private bool[,,] PCNotDrilledList;

    private bool[,,] DBCDrilledList;
    private bool[,,] DBCNotDrilledList;

    private bool[,,] MBCDrilledList;
    private bool[,,] MBCNotDrilledList;

    private bool[,,] RoofDrilledList;
    private bool[,,] RoofNotDrilledList;

    [HideInInspector] public float PCDrilled;
    [HideInInspector] public float PCNotDrilled;
    [HideInInspector] public float DBCDrilled;
    [HideInInspector] public float DBCNotDrilled;
    [HideInInspector] public float MBCDrilled;
    [HideInInspector] public float MBCNotDrilled;
    [HideInInspector] public float RoofDrilled;
    [HideInInspector] public float RoofNotDrilled;
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

    [Header("Edit Voxel Objects 1x")]
    [SerializeField] private GameObject entireMolar1x;
    [SerializeField] private GameObject PCObject1x;
    [SerializeField] private GameObject DBCObject1x;
    [SerializeField] private GameObject MBCObject1x;
    [SerializeField] private GameObject RoofObject1x;

    [Header("Edit Voxel Objects 2x")]
    [SerializeField] private GameObject entireMolar2x;
    [SerializeField] private GameObject PCObject2x;
    [SerializeField] private GameObject DBCObject2x;
    [SerializeField] private GameObject MBCObject2x;
    [SerializeField] private GameObject RoofObject2x;
    InputActivator inputActivator;
    GameObject molarObject;

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
        inputActivator = FindObjectOfType<InputActivator>();

        switch (inputActivator.magnificationSelector)
        {
            case 1:
                molarObject = entireMolar1x;
                PCeditVoxlesVR = PCObject1x.GetComponent<EditVoxelsVR>();
                PCCurrentGridValues = PCeditVoxlesVR.FinishedGridValues();

                DBCeditVoxlesVR = DBCObject1x.GetComponent<EditVoxelsVR>();
                DBCCurrentGridValues = DBCeditVoxlesVR.FinishedGridValues();

                MBCeditVoxlesVR = MBCObject1x.GetComponent<EditVoxelsVR>();
                MBCCurrentGridValues = MBCeditVoxlesVR.FinishedGridValues();

                RoofeditVoxlesVR = RoofObject1x.GetComponent<EditVoxelsVR>();
                RoofCurrentGridValues = RoofeditVoxlesVR.FinishedGridValues();
                break;
            case 2:

                molarObject = entireMolar2x;
                PCeditVoxlesVR = PCObject2x.GetComponent<EditVoxelsVR>();
                PCCurrentGridValues = PCeditVoxlesVR.FinishedGridValues();

                DBCeditVoxlesVR = DBCObject2x.GetComponent<EditVoxelsVR>();
                DBCCurrentGridValues = DBCeditVoxlesVR.FinishedGridValues();

                MBCeditVoxlesVR = MBCObject2x.GetComponent<EditVoxelsVR>();
                MBCCurrentGridValues = MBCeditVoxlesVR.FinishedGridValues();

                RoofeditVoxlesVR = RoofObject2x.GetComponent<EditVoxelsVR>();
                RoofCurrentGridValues = RoofeditVoxlesVR.FinishedGridValues();
                break;
            default:
                Debug.LogError("Invalid magnification selector");
                break;
        }

        PC();
        DBC();
        MBC();
        Roof();
        Floor();
        Diff();

        molarObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        RotateBenchmark rotate = molarObject.GetComponent<RotateBenchmark>();
        // rotate.enabled = true;

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
    private float CompareDrilled(float[,,] TrialArray, float[,,] BenchmarkArray, string part)
    {
        // Check if both arrays have the same dimensions
        if (TrialArray.GetLength(0) != BenchmarkArray.GetLength(0) ||
            TrialArray.GetLength(1) != BenchmarkArray.GetLength(1) ||
            TrialArray.GetLength(2) != BenchmarkArray.GetLength(2))
        {
            Debug.LogError("Arrays must have the same dimensions.");
            return 0f;
        }

        PCDrilledList = new bool[TrialArray.GetLength(0), TrialArray.GetLength(1), TrialArray.GetLength(2)];
        DBCDrilledList = new bool[TrialArray.GetLength(0), TrialArray.GetLength(1), TrialArray.GetLength(2)];
        MBCDrilledList = new bool[TrialArray.GetLength(0), TrialArray.GetLength(1), TrialArray.GetLength(2)];
        RoofDrilledList = new bool[TrialArray.GetLength(0), TrialArray.GetLength(1), TrialArray.GetLength(2)];

        int benchmarkDrilled = 0;
        int trialDrilled = 0;
        // // Iterate through each element in the arrays
        for (int x = 0; x < TrialArray.GetLength(0); x++)
        {
            for (int y = 0; y < TrialArray.GetLength(1); y++)
            {
                for (int z = 0; z < TrialArray.GetLength(2); z++)
                {
                    if (part == "PC")
                        PCDrilledList[x, y, z] = false;
                    if (part == "DBC")
                        DBCDrilledList[x, y, z] = false;
                    if (part == "MBC")
                        MBCDrilledList[x, y, z] = false;
                    if (part == "Roof")
                        RoofDrilledList[x, y, z] = false;

                    if (BenchmarkArray[x, y, z] == -9999)
                    {
                        benchmarkDrilled++;
                        if (TrialArray[x, y, z] == -9999)
                        {
                            trialDrilled++;
                        }
                        else
                        {
                            if (part == "PC")
                                PCDrilledList[x, y, z] = true;
                            if (part == "DBC")
                                DBCDrilledList[x, y, z] = true;
                            if (part == "MBC")
                                MBCDrilledList[x, y, z] = true;
                            if (part == "Roof")
                                RoofDrilledList[x, y, z] = true;
                        }
                    }
                }
            }
        }

        float similarityPercentage = ((float)trialDrilled / benchmarkDrilled) * 100;
        return similarityPercentage;
    }

    private float CompareNotDrilled(float[,,] TrialArray, float[,,] BenchmarkArray, string part)
    {
        // Check if both arrays have the same dimensions
        if (TrialArray.GetLength(0) != BenchmarkArray.GetLength(0) ||
            TrialArray.GetLength(1) != BenchmarkArray.GetLength(1) ||
            TrialArray.GetLength(2) != BenchmarkArray.GetLength(2))
        {
            Debug.LogError("Arrays must have the same dimensions.");
            return 0f;
        }

        PCNotDrilledList = new bool[TrialArray.GetLength(0), TrialArray.GetLength(1), TrialArray.GetLength(2)];
        DBCNotDrilledList = new bool[TrialArray.GetLength(0), TrialArray.GetLength(1), TrialArray.GetLength(2)];
        MBCNotDrilledList = new bool[TrialArray.GetLength(0), TrialArray.GetLength(1), TrialArray.GetLength(2)];
        RoofNotDrilledList = new bool[TrialArray.GetLength(0), TrialArray.GetLength(1), TrialArray.GetLength(2)];

        int benchmarkNotDrilled = 0;
        int trialNotDrilled = 0;
        // // Iterate through each element in the arrays
        for (int x = 0; x < TrialArray.GetLength(0); x++)
        {
            for (int y = 0; y < TrialArray.GetLength(1); y++)
            {
                for (int z = 0; z < TrialArray.GetLength(2); z++)
                {
                    if (part == "PC")
                        PCNotDrilledList[x, y, z] = false;
                    if (part == "DBC")
                        DBCNotDrilledList[x, y, z] = false;
                    if (part == "MBC")
                        MBCNotDrilledList[x, y, z] = false;
                    if (part == "Roof")
                        RoofNotDrilledList[x, y, z] = false;

                    if (BenchmarkArray[x, y, z] > 0)
                    {
                        benchmarkNotDrilled++;
                        if (TrialArray[x, y, z] > 0)
                        {
                            trialNotDrilled++;
                        }
                        else
                        {
                            if (part == "PC")
                                PCNotDrilledList[x, y, z] = true;
                            if (part == "DBC")
                                DBCNotDrilledList[x, y, z] = true;
                            if (part == "MBC")
                                MBCNotDrilledList[x, y, z] = true;
                            if (part == "Roof")
                                RoofNotDrilledList[x, y, z] = true;
                        }
                    }
                }
            }
        }

        float similarityPercentage = ((float)trialNotDrilled / benchmarkNotDrilled) * 100;
        return similarityPercentage;
    }

    private void PC()
    {
        if (saveIt)
            SaveFloatArray(PCCurrentGridValues, PCPath, PCDimensionsPath, PCeditVoxlesVR);

        if (compareIt)
        {
            PCbenchmark = LoadFloatArray(PCBenchmarkPath, PCBenchmarkPathDimensions);
            PCDrilled = CompareDrilled(PCCurrentGridValues, PCbenchmark, "PC");
            PCNotDrilled = CompareNotDrilled(PCCurrentGridValues, PCbenchmark, "PC");

            PCeditVoxlesVR.CreateBenchmarkSpheres(PCDrilledList, "DrillBenchmark", "PC");
            PCeditVoxlesVR.CreateBenchmarkSpheres(PCNotDrilledList, "NotDrillBenchmark", "PC");

            PCText.text = $"Drilled: {PCDrilled:F2} \nNot Drilled: {PCNotDrilled:F2}%";
        }
    }

    private void DBC()
    {
        if (saveIt)
            SaveFloatArray(DBCCurrentGridValues, DBCPath, DBCDimensionsPath, DBCeditVoxlesVR);

        if (compareIt)
        {
            DBCbenchmark = LoadFloatArray(DBCBenchmarkPath, DBCBenchmarkPathDimensions);
            DBCDrilled = CompareDrilled(DBCCurrentGridValues, DBCbenchmark, "DBC");
            DBCNotDrilled = CompareNotDrilled(DBCCurrentGridValues, DBCbenchmark, "DBC");

            DBCeditVoxlesVR.CreateBenchmarkSpheres(DBCDrilledList, "DrillBenchmark", "DBC");
            DBCeditVoxlesVR.CreateBenchmarkSpheres(DBCNotDrilledList, "NotDrillBenchmark", "DBC");

            DBCText.text = $"Drilled: {DBCDrilled:F2} \nNot Drilled: {DBCNotDrilled:F2}%";
        }
    }

    private void MBC()
    {
        if (saveIt)
            SaveFloatArray(MBCCurrentGridValues, MBCPath, MBCDimensionsPath, MBCeditVoxlesVR);

        if (compareIt)
        {
            MBCbenchmark = LoadFloatArray(MBCBenchmarkPath, MBCBenchmarkPathDimensions);
            MBCDrilled = CompareDrilled(MBCCurrentGridValues, MBCbenchmark, "MBC");
            MBCNotDrilled = CompareNotDrilled(MBCCurrentGridValues, MBCbenchmark, "MBC");

            MBCeditVoxlesVR.CreateBenchmarkSpheres(MBCDrilledList, "DrillBenchmark", "MBC");
            MBCeditVoxlesVR.CreateBenchmarkSpheres(MBCNotDrilledList, "NotDrillBenchmark", "MBC");

            MBCText.text = $"Drilled: {MBCDrilled:F2} \nNot Drilled: {MBCNotDrilled:F2}%";
        }
    }

    private void Roof()
    {
        if (saveIt)
            SaveFloatArray(RoofCurrentGridValues, RoofPath, RoofDimensionsPath, RoofeditVoxlesVR);

        if (compareIt)
        {
            Roofbenchmark = LoadFloatArray(RoofBenchmarkPath, RoofBenchmarkPathDimensions);
            RoofDrilled = CompareDrilled(RoofCurrentGridValues, Roofbenchmark, "Roof");
            RoofNotDrilled = CompareNotDrilled(RoofCurrentGridValues, Roofbenchmark, "Roof");

            RoofeditVoxlesVR.CreateBenchmarkSpheres(RoofDrilledList, "DrillBenchmark", "Roof");
            RoofeditVoxlesVR.CreateBenchmarkSpheres(RoofNotDrilledList, "NotDrillBenchmark", "Roof");

            RoofText.text = $"Drilled: {RoofDrilled:F2} \nNot Drilled: {RoofNotDrilled:F2}%";
        }
    }

    private void Floor()
    {
        floorHit = controllerInput.floorHit;
        if (floorHit)
            FloorText.text = "Floor Hit!";
        else
            FloorText.text = "Clear Floor!";
    }

    private void Diff()
    {
        diffHit = controllerInput.diffHit;
        if (diffHit)
            DiffText.text = "Diff Hit!";
        else
            DiffText.text = "Clear Diff!";
    }
}