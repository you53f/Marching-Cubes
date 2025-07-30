
using TMPro;
using System.IO;
using UnityEngine;

public class MolarSubmitter : MonoBehaviour

{
    private EditVoxelsVR PCeditVoxlesVR;
    private EditVoxelsVR DBCeditVoxlesVR;
    private EditVoxelsVR MBCeditVoxlesVR;
    private EditVoxelsVR RoofeditVoxlesVR;
    private EditVoxelsVR WallseditVoxlesVR;
    private float[,,] PCCurrentGridValues;
    private float[,,] DBCCurrentGridValues;
    private float[,,] MBCCurrentGridValues;
    private float[,,] RoofCurrentGridValues;
    private float[,,] WallsCurrentGridValues;
    private float[,,] PCbenchmark;
    private float[,,] DBCbenchmark;
    private float[,,] MBCbenchmark;
    private float[,,] Roofbenchmark;
    private float[,,] Wallsbenchmark;
    private float gridScale;

    private bool[,,] PCDrilledList;
    private bool[,,] PCNotDrilledList;

    private bool[,,] DBCDrilledList;
    private bool[,,] DBCNotDrilledList;

    private bool[,,] MBCDrilledList;
    private bool[,,] MBCNotDrilledList;

    private bool[,,] RoofDrilledList;
    private bool[,,] RoofNotDrilledList;

    private bool[,,] WallsDrilledList;
    private bool[,,] WallsNotDrilledList;

    [HideInInspector] public float PCDrilled;
    [HideInInspector] public float PCNotDrilled;
    [HideInInspector] public float DBCDrilled;
    [HideInInspector] public float DBCNotDrilled;
    [HideInInspector] public float MBCDrilled;
    [HideInInspector] public float MBCNotDrilled;
    [HideInInspector] public float RoofDrilled;
    [HideInInspector] public float RoofNotDrilled;
    [HideInInspector] public float WallsDrilled;
    [HideInInspector] public float WallsNotDrilled;
    [HideInInspector] public float DBCsimilarityPercentage;
    [HideInInspector] public float MBCsimilarityPercentage;
    [HideInInspector] public float RoofsimilarityPercentage;
    [HideInInspector] public float WallssimilarityPercentage;
    private bool floorHit;
    private bool floorHit2;
    private bool diffHit;
    private bool diffHit2;
    [SerializeField] private ControllerInput controllerInputx1;
    [SerializeField] private ControllerInput controllerInputx2;
    [SerializeField] private HapticInput hapticInputx1;
    [SerializeField] private HapticInput hapticInputx2;
    private ControllerInput controllerInput;
    private HapticInput hapticInput;
    [SerializeField] private TextMeshProUGUI PCText;
    [SerializeField] private TextMeshProUGUI DBCText;
    [SerializeField] private TextMeshProUGUI MBCText;
    [SerializeField] private TextMeshProUGUI RoofText;
    [SerializeField] private TextMeshProUGUI WallsText;
    [SerializeField] private TextMeshProUGUI FloorText;
    [SerializeField] private TextMeshProUGUI DiffText;
    [SerializeField] private TextMeshProUGUI FinalScoreText;
    [SerializeField] private bool saveIt;
    [SerializeField] private bool compareIt;

    [Header("Edit Voxel Objects 1x")]
    [SerializeField] private GameObject entireMolar1x;
    [SerializeField] private GameObject PCObject1x;
    [SerializeField] private GameObject DBCObject1x;
    [SerializeField] private GameObject MBCObject1x;
    [SerializeField] private GameObject RoofObject1x;
    [SerializeField] private GameObject WallsObject1x;

    [Header("Edit Voxel Objects 2x")]
    [SerializeField] private GameObject entireMolar2x;
    [SerializeField] private GameObject PCObject2x;
    [SerializeField] private GameObject DBCObject2x;
    [SerializeField] private GameObject MBCObject2x;
    [SerializeField] private GameObject RoofObject2x;
    [SerializeField] private GameObject WallsObject2x;
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
    [SerializeField] private string WallsPath;
    [SerializeField] private string WallsDimensionsPath;


    [Header("Becnhmark Setting")]
    [SerializeField] private string PCBenchmarkPath;
    [SerializeField] private string PCBenchmarkPathDimensions;

    [SerializeField] private string DBCBenchmarkPath;
    [SerializeField] private string DBCBenchmarkPathDimensions;

    [SerializeField] private string MBCBenchmarkPath;
    [SerializeField] private string MBCBenchmarkPathDimensions;

    [SerializeField] private string RoofBenchmarkPath;
    [SerializeField] private string RoofBenchmarkPathDimensions;

    [SerializeField] private string WallsBenchmarkPath;
    [SerializeField] private string WallsBenchmarkPathDimensions;

    private float finalScore;
    private float PCScore;
    private float DBCScore;
    private float MBCScore;
    private float RoofScore;
    private float WallsScore;
    private float DiffScore;
    private float FloorScore;

    // Start is called before the first frame update
    void Start()
    {
        inputActivator = FindObjectOfType<InputActivator>();

        switch (inputActivator.magnificationSelector)
        {
            case 1:
                controllerInput = controllerInputx1;
                hapticInput = hapticInputx1;

                molarObject = entireMolar1x;
                PCeditVoxlesVR = PCObject1x.GetComponent<EditVoxelsVR>();
                PCCurrentGridValues = PCeditVoxlesVR.FinishedGridValues();

                DBCeditVoxlesVR = DBCObject1x.GetComponent<EditVoxelsVR>();
                DBCCurrentGridValues = DBCeditVoxlesVR.FinishedGridValues();

                MBCeditVoxlesVR = MBCObject1x.GetComponent<EditVoxelsVR>();
                MBCCurrentGridValues = MBCeditVoxlesVR.FinishedGridValues();

                RoofeditVoxlesVR = RoofObject1x.GetComponent<EditVoxelsVR>();
                RoofCurrentGridValues = RoofeditVoxlesVR.FinishedGridValues();

                WallseditVoxlesVR = WallsObject1x.GetComponent<EditVoxelsVR>();
                WallsCurrentGridValues = WallseditVoxlesVR.FinishedGridValues();
                break;

            case 2:
                controllerInput = controllerInputx2;
                hapticInput = hapticInputx2;

                molarObject = entireMolar2x;
                PCeditVoxlesVR = PCObject2x.GetComponent<EditVoxelsVR>();
                PCCurrentGridValues = PCeditVoxlesVR.FinishedGridValues();

                DBCeditVoxlesVR = DBCObject2x.GetComponent<EditVoxelsVR>();
                DBCCurrentGridValues = DBCeditVoxlesVR.FinishedGridValues();

                MBCeditVoxlesVR = MBCObject2x.GetComponent<EditVoxelsVR>();
                MBCCurrentGridValues = MBCeditVoxlesVR.FinishedGridValues();

                RoofeditVoxlesVR = RoofObject2x.GetComponent<EditVoxelsVR>();
                RoofCurrentGridValues = RoofeditVoxlesVR.FinishedGridValues();

                WallseditVoxlesVR = WallsObject2x.GetComponent<EditVoxelsVR>();
                WallsCurrentGridValues = WallseditVoxlesVR.FinishedGridValues();
                break;
            default:
                Debug.LogError("Invalid magnification selector");
                break;
        }

        PC();
        DBC();
        MBC();
        Roof();
        Walls();
        Floor();
        Diff();

        molarObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        finalScore = 0.2f * PCScore + 0.2f * DBCScore + 0.2f * MBCScore + 0.25f * RoofScore + 0.15f * WallsScore;
        finalScore -= 0.3f * FloorScore + 0.1f * DiffScore;
        FinalScoreText.text = $"{finalScore:F2}%";
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
        WallsDrilledList = new bool[TrialArray.GetLength(0), TrialArray.GetLength(1), TrialArray.GetLength(2)];

        int benchmarkDrilled = 0;
        int trialDrilled = 0;

        // Iterate through each element in the arrays
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
                    if (part == "Walls")
                        WallsDrilledList[x, y, z] = false;

                    if (BenchmarkArray[x, y, z] < 0)
                    {
                        benchmarkDrilled++;
                        if (TrialArray[x, y, z] < 0)
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
                            if (part == "Walls")
                                WallsDrilledList[x, y, z] = true;
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
        WallsNotDrilledList = new bool[TrialArray.GetLength(0), TrialArray.GetLength(1), TrialArray.GetLength(2)];

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
                    if (part == "Walls")
                        WallsNotDrilledList[x, y, z] = false;

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
                            if (part == "Walls")
                                WallsNotDrilledList[x, y, z] = true;
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

            PCScore = 0.8f * PCDrilled + 0.2f * PCNotDrilled;

            if (PCDrilled > 95f)
                PCText.text = $"Almost no undercutting";
            if (PCDrilled > 80f && PCDrilled <= 95f)
                PCText.text = $"Little undercutting represented by the yellow spheres";
            if (PCDrilled < 80f && PCDrilled > 50f)
                PCText.text = $"A noticable amount of undercutting represented by the yellow spheres";
            if (PCDrilled < 50f)
                PCText.text = $"Severe undercutting represented by the yellow spheres";

            if (PCNotDrilled > 95f)
                PCText.text += $"\n\nAlmost no overcutting";
            if (PCNotDrilled > 80f && PCNotDrilled <= 95f)
                PCText.text += $"\n\nLittle overcutting represented by the red spheres";
            if (PCNotDrilled < 80f && PCNotDrilled > 50f)
                PCText.text += $"\n\nA noticable amount of overcutting represented by the red spheres";
            if (PCNotDrilled < 50f)
                PCText.text += $"\n\nSevere overcutting represented by the red spheres";
            // PCText.text += $"\n\nDrilled: {PCDrilled:F2} \nNot Drilled: {PCNotDrilled:F2} \nSubtotal: {PCScore}%";
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

            DBCScore = 0.8f * DBCDrilled + 0.2f * DBCNotDrilled;

            if (DBCDrilled > 95f)
                DBCText.text = $"Almost no undercutting";
            if (DBCDrilled > 80f && DBCDrilled <= 95f)
                DBCText.text = $"Little undercutting represented by the yellow spheres";
            if (DBCDrilled < 80f && DBCDrilled > 50f)
                DBCText.text = $"A noticable amount of undercutting represented by the yellow spheres";
            if (DBCDrilled < 50f)
                DBCText.text = $"Severe undercutting represented by the yellow spheres";

            if (DBCNotDrilled > 95f)
                DBCText.text += $"\n\nAlmost no overcutting";
            if (DBCNotDrilled > 80f && DBCNotDrilled <= 95f)
                DBCText.text += $"\n\nLittle overcutting represented by the red spheres";
            if (DBCNotDrilled < 80f && DBCNotDrilled > 50f)
                DBCText.text += $"\n\nA noticable amount of overcutting represented by the red spheres";
            if (DBCNotDrilled < 50f)
                DBCText.text += $"\n\nSevere overcutting represented by the red spheres";
            // DBCText.text += $"\n\nDrilled: {DBCDrilled:F2} \nNot Drilled: {DBCNotDrilled:F2} \nSubtotal: {DBCScore}%";
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

            MBCScore = 0.8f * MBCDrilled + 0.2f * MBCNotDrilled;

            if (MBCDrilled > 95f)
                MBCText.text = $"Almost no undercutting";
            if (MBCDrilled > 80f && MBCDrilled <= 95f)
                MBCText.text = $"Little undercutting represented by the yellow spheres";
            if (MBCDrilled < 80f && MBCDrilled > 50f)
                MBCText.text = $"A noticable amount of undercutting represented by the yellow spheres";
            if (MBCDrilled < 50f)
                MBCText.text = $"Severe undercutting represented by the yellow spheres";

            if (MBCNotDrilled > 95f)
                MBCText.text += $"\n\nAlmost no overcutting";
            if (MBCNotDrilled > 80f && MBCNotDrilled <= 95f)
                MBCText.text += $"\n\nLittle overcutting represented by the red spheres";
            if (MBCNotDrilled < 80f && MBCNotDrilled > 50f)
                MBCText.text += $"\n\nA noticable amount of overcutting represented by the red spheres";
            if (MBCNotDrilled < 50f)
                MBCText.text += $"\n\nSevere overcutting represented by the red spheres";
            // MBCText.text += $"\n\nDrilled: {MBCDrilled:F2} \nNot Drilled: {MBCNotDrilled:F2} \nSubtotal: {MBCScore}%";
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

            RoofScore = 0.8f * RoofDrilled + 0.2f * RoofNotDrilled;

            if (RoofDrilled > 85f)
                RoofText.text = $"Almost no undercutting";
            if (RoofDrilled > 75f && RoofDrilled <= 85f)
                RoofText.text = $"Little undercutting represented by the yellow spheres";
            if (RoofDrilled < 75f && RoofDrilled > 40f)
                RoofText.text = $"A noticable amount of undercutting represented by the yellow spheres";
            if (RoofDrilled < 40f)
                RoofText.text = $"Severe undercutting represented by the yellow spheres";

            if (RoofNotDrilled > 85f)
                RoofText.text += $"\n\nAlmost no overcutting";
            if (RoofNotDrilled > 75f && RoofNotDrilled <= 85f)
                RoofText.text += $"\n\nLittle overcutting represented by the red spheres";
            if (RoofNotDrilled < 75f && RoofNotDrilled > 40f)
                RoofText.text += $"\n\nA noticable amount of overcutting represented by the red spheres";
            if (RoofNotDrilled < 40f)
                RoofText.text += $"\n\nSevere overcutting represented by the red spheres";
            // RoofText.text += $"\n\nDrilled: {RoofDrilled:F2} \nNot Drilled: {RoofNotDrilled:F2} \nSubtotal: {RoofScore}%";
        }
    }
    private void Walls()
    {
        if (saveIt)
            SaveFloatArray(WallsCurrentGridValues, WallsPath, WallsDimensionsPath, WallseditVoxlesVR);

        if (compareIt)
        {
            Wallsbenchmark = LoadFloatArray(WallsBenchmarkPath, WallsBenchmarkPathDimensions);
            WallsDrilled = CompareDrilled(WallsCurrentGridValues, Wallsbenchmark, "Walls");
            WallsNotDrilled = CompareNotDrilled(WallsCurrentGridValues, Wallsbenchmark, "Walls");

            WallseditVoxlesVR.CreateBenchmarkSpheres(WallsDrilledList, "DrillBenchmark", "Walls");
            WallseditVoxlesVR.CreateBenchmarkSpheres(WallsNotDrilledList, "NotDrillBenchmark", "Walls");

            WallsScore = 0.8f * WallsDrilled + 0.2f * WallsNotDrilled;

            if (WallsDrilled > 85f)
                WallsText.text = $"Almost no undercutting";
            if (WallsDrilled > 60f && WallsDrilled <= 85f)
                WallsText.text = $"Little undercutting represented by the yellow spheres";
            if (WallsDrilled < 60f && WallsDrilled > 30f)
                WallsText.text = $"A noticable amount of undercutting represented by the yellow spheres";
            if (WallsDrilled < 30f)
                WallsText.text = $"Severe undercutting represented by the yellow spheres";

            if (WallsNotDrilled > 85f)
                WallsText.text += $"\n\nAlmost no overcutting";
            if (WallsNotDrilled > 60f && WallsNotDrilled <= 85f)
                WallsText.text += $"\n\nLittle overcutting represented by the red spheres";
            if (WallsNotDrilled < 60f && WallsNotDrilled > 30f)
                WallsText.text += $"\n\nA noticable amount of overcutting represented by the red spheres";
            if (WallsNotDrilled < 30f)
                WallsText.text += $"\n\nSevere overcutting represented by the red spheres";
            // WallsText.text += $"\n\nDrilled: {WallsDrilled:F2} \nNot Drilled: {WallsNotDrilled:F2} \nSubtotal: {WallsScore}%";
        }
    }

    private void Floor()
    {
        floorHit = controllerInput.floorHit;
        floorHit2 = hapticInput.floorHit;
        if (floorHit || floorHit2)
        {
            FloorScore = 100f;
            FloorText.text = $"You hit the floor of the pulp chamber causing a perforation in the tooth, otherwise your score would be 30 percent higher";
        }
        else
        {
            FloorScore = 0f;
            FloorText.text = $"You did not hit the floor of the pulp chamber, no perforation in the tooth";
        }
    }

    private void Diff()
    {
        diffHit = controllerInput.diffHit;
        diffHit2 = hapticInput.diffHit;
        if (diffHit || diffHit2)
        {
            DiffScore = 100f;
            DiffText.text = $"You hit the external surface of the tooth causing some gouging, otherwise your score would be 10 percent higher";
        }
        else
        {
            DiffScore = 0f;
            DiffText.text = $"You did not hit the external surface of the tooth, no gouging";
        }
    }
}