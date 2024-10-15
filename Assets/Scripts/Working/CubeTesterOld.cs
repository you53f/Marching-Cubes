using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeTesterOld : MonoBehaviour
{
    Vector3 topRightFront;
    Vector3 topLeftFront;
    Vector3 bottomLeftFront;
    Vector3 bottomRightFront;
    Vector3 topRightBack;
    Vector3 topLeftBack;
    Vector3 bottomLeftBack;
    Vector3 bottomRightBack;

    Vector3 topCenterFront;
    Vector3 topCenterBack;
    Vector3 rightCenterFront;
    Vector3 rightCenterBack;
    Vector3 leftCenterFront;
    Vector3 leftCenterBack;
    Vector3 bottomCenterFront;
    Vector3 bottomCenterBack;
    Vector3 topCenterRight;
    Vector3 bottomCenterRight;
    Vector3 topCenterLeft;
    Vector3 bottomCenterLeft;

    [Header("Configuration")]
    [SerializeField] float topRightFrontValue;
    [SerializeField] float topLeftFrontValue;
    [SerializeField] float bottomLeftFrontValue;
    [SerializeField] float bottomRightFrontValue;
    [SerializeField] float topRightBackValue;
    [SerializeField] float topLeftBackValue;
    [SerializeField] float bottomLeftBackValue;
    [SerializeField] float bottomRightBackValue;


    [Header("Settings")]
    [SerializeField] private float gridLength;
    [SerializeField] private float isoValue;

    [Header("Elements")]
    [SerializeField] private MeshFilter filter;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        PositionInitialization();
    }

    // Update is called once per frame
    void Update()
    {
        //New mesh to store the values in
        Mesh mesh = new Mesh();
        vertices.Clear();
        triangles.Clear();

        Interpolate();
        Triangulate(GetConfiguration());
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        //Assigning the two lists to the mesh filter
        filter.mesh = mesh;
        Debug.Log("Configuration = " + GetConfiguration());
    }

    private void PositionInitialization()
    {
        //Front side positions of the corners
        bottomLeftFront = -gridLength * Vector3.one / 2;
        bottomRightFront = bottomLeftFront + Vector3.right * gridLength;
        topRightFront = bottomRightFront + Vector3.up * gridLength;
        topLeftFront = topRightFront + Vector3.left * gridLength;

        //Back side positions of the corners
        topLeftBack = topLeftFront + Vector3.forward * gridLength;
        topRightBack = topLeftBack + Vector3.right * gridLength;
        bottomRightBack = topRightBack + Vector3.down * gridLength;
        bottomLeftBack = bottomRightBack + Vector3.left * gridLength;

        //Midpoints positions of the front side
        topCenterFront = topLeftFront + Vector3.right / 2 * gridLength;
        rightCenterFront = topRightFront + Vector3.down / 2 * gridLength;
        bottomCenterFront = bottomRightFront + Vector3.left / 2 * gridLength;
        leftCenterFront = bottomLeftFront + Vector3.up / 2 * gridLength;

        //Midpoints positions of the back side
        topCenterBack = topLeftBack + Vector3.right / 2 * gridLength;
        rightCenterBack = topRightBack + Vector3.down / 2 * gridLength;
        bottomCenterBack = bottomRightBack + Vector3.left / 2 * gridLength;
        leftCenterBack = bottomLeftBack + Vector3.up / 2 * gridLength;

        //Midpoints positions of the middle bridges between the front and back sides
        topCenterRight = topRightFront + Vector3.forward / 2 * gridLength;
        topCenterLeft = topLeftFront + Vector3.forward / 2 * gridLength;
        bottomCenterRight = bottomRightFront + Vector3.forward / 2 * gridLength;
        bottomCenterLeft = bottomLeftFront + Vector3.forward / 2 * gridLength;
    }

    private void Interpolate()
    {
        float topCenterFrontLerp = Mathf.InverseLerp(topLeftFrontValue, topRightFrontValue, isoValue);
        topCenterFront = topLeftFront + (topRightFront - topLeftFront) * topCenterFrontLerp;

        float rightCenterFrontLerp = Mathf.InverseLerp(bottomRightFrontValue, topRightFrontValue, isoValue);
        rightCenterFront = bottomRightFront + (topRightFront - bottomRightFront) * rightCenterFrontLerp;

        float bottomCenterFrontLerp = Mathf.InverseLerp(bottomLeftFrontValue, bottomRightFrontValue, isoValue);
        bottomCenterFront = bottomLeftFront + (bottomRightFront - bottomLeftFront) * bottomCenterFrontLerp;

        float leftCenterFrontLerp = Mathf.InverseLerp(bottomLeftFrontValue, topLeftFrontValue, isoValue);
        leftCenterFront = bottomLeftFront + (topLeftFront - bottomLeftFront) * leftCenterFrontLerp;

        float topCenterBackLerp = Mathf.InverseLerp(topLeftBackValue, topRightBackValue, isoValue);
        topCenterBack = topLeftBack + (topRightBack - topLeftBack) * topCenterBackLerp;

        float rightCenterBackLerp = Mathf.InverseLerp(bottomRightBackValue, topRightBackValue, isoValue);
        rightCenterBack = bottomRightBack + (topRightBack - bottomRightBack) * rightCenterBackLerp;

        float bottomCenterBackLerp = Mathf.InverseLerp(bottomLeftBackValue, bottomRightBackValue, isoValue);
        bottomCenterBack = bottomLeftBack + (bottomRightBack - bottomLeftBack) * bottomCenterBackLerp;

        float leftCenterBackLerp = Mathf.InverseLerp(bottomLeftBackValue, topLeftBackValue, isoValue);
        leftCenterBack = bottomLeftBack + (topLeftBack - bottomLeftBack) * leftCenterBackLerp;

        float topCenterRightLerp = Mathf.InverseLerp(topRightFrontValue,topRightBackValue, isoValue);
        topCenterRight = topRightFront + (topRightBack - topRightFront) * topCenterRightLerp;

        float bottomCenterRightLerp = Mathf.InverseLerp(bottomRightFrontValue,bottomRightBackValue, isoValue);
        bottomCenterRight = bottomRightFront + (bottomRightBack - bottomRightFront) * bottomCenterRightLerp;

        float topCenterLeftLerp = Mathf.InverseLerp(topLeftFrontValue,topLeftBackValue, isoValue);
        topCenterLeft = topLeftFront + (topLeftBack - topLeftFront) * topCenterLeftLerp;

        float bottomCenterLeftLerp = Mathf.InverseLerp(bottomLeftFrontValue,bottomLeftBackValue, isoValue);
        bottomCenterLeft = bottomLeftFront + (bottomLeftBack - bottomLeftFront) * bottomCenterLeftLerp;

    }

    private void Triangulate(int configuration)
    {

        vertices.AddRange(new Vector3[] { topCenterFront, rightCenterFront, bottomCenterFront, leftCenterFront,
        topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, topCenterRight, bottomCenterRight,
        topCenterLeft, bottomCenterLeft });
        switch (configuration)
        {
            case 0:
                break;

            case 1:
                triangles.AddRange(new int[] { 1, 8, 0 });
                break;

            case 2:
                triangles.AddRange(new int[] { 3, 0, 10 });
                break;

            case 3:
                triangles.AddRange(new int[] { 3, 1, 8, 10, 3, 8 });
                break;

            case 4:
                triangles.AddRange(new int[] { 3, 11, 2 });
                break;

            case 5:
                triangles.AddRange(new int[] { 0, 1, 8, 2, 1, 0, 3, 2, 0, 11, 2, 3 });
                break;

            case 6:
                triangles.AddRange(new int[] { 11, 2, 0, 10, 11, 0 });
                break;

            case 7:
                triangles.AddRange(new int[] { 2, 1, 8, 11, 2, 8, 10, 11, 8 });
                break;

            case 8:
                triangles.AddRange(new int[] { 2, 9, 1 });
                break;

            case 9:
                triangles.AddRange(new int[] { 0, 9, 8, 0, 2, 9 });
                break;

            case 10:
                triangles.AddRange(new int[] { 3, 0, 10, 3, 2, 0, 0, 2, 1, 2, 9, 1 });
                break;

            case 11:
                triangles.AddRange(new int[] { 2, 9, 8, 2, 8, 10, 3, 2, 10 });
                break;

            case 12:
                triangles.AddRange(new int[] { 11, 9, 1, 3, 11, 1 });
                break;

            case 13:
                triangles.AddRange(new int[] { 11, 9, 8, 11, 8, 0, 3, 11, 0 });
                break;

            case 14:
                triangles.AddRange(new int[] { 9, 1, 0, 11, 9, 0, 10, 11, 0 });
                break;

            case 15:
                triangles.AddRange(new int[] { 11, 9, 10, 8, 10, 9 });
                break;

            case 16:
                triangles.AddRange(new int[] { 8, 5, 4 });
                break;

            case 17:
                triangles.AddRange(new int[] { 4, 0, 1, 4, 1, 5 });
                break;

            case 18:
                triangles.AddRange(new int[] { 10, 3, 0, 0, 4, 10, 0, 8, 4, 8, 5, 4 });
                break;

            case 19:
                triangles.AddRange(new int[] { 5, 3, 1, 4, 3, 5, 10, 3, 4 });
                break;

            case 20:
                triangles.AddRange(new int[] { 8, 5, 4, 3, 11, 2 });
                break;

            case 21:
                triangles.AddRange(new int[] { 11, 2, 3, 0, 3, 2, 0, 2, 1, 1, 4, 0, 1, 5, 4 });
                break;

            case 22:
                triangles.AddRange(new int[] { 8, 5, 4, 0, 11, 2, 0, 10, 11, 4, 10, 0, 8, 4, 0 });
                break;

            case 23:
                triangles.AddRange(new int[] { 1, 5, 4, 2, 1, 4, 4, 10, 2, 10, 11, 2 });
                break;

            case 24:
                triangles.AddRange(new int[] { 4, 8, 5, 8, 9, 5, 8, 1, 9, 1, 2, 9 });
                break;

            case 25:
                triangles.AddRange(new int[] { 9, 5, 4, 2, 9, 4, 4, 0, 2 });
                break;

            case 26:
                triangles.AddRange(new int[] { 10, 3, 0, 0, 3, 2, 2, 1, 0, 1, 2, 9, 1, 9, 8, 9, 5, 8, 8, 5, 4 });
                break;

            case 27:
                triangles.AddRange(new int[] { 9, 5, 4, 2, 9, 4, 10, 2, 4, 10, 3, 2 });
                break;

            case 28:
                triangles.AddRange(new int[] { 5, 4, 8, 8, 9, 5, 8, 1, 9, 3, 11, 1, 1, 11, 9 });
                break;

            case 29:
                triangles.AddRange(new int[] { 9, 5, 4, 2, 9, 4, 0, 2, 4, 11, 2, 3, 0, 3, 2 });
                break;

            case 30:
                triangles.AddRange(new int[] { 8, 5, 4, 8, 9, 5, 1, 9, 8, 11, 9, 0, 11, 0, 10, 0, 9, 1 });
                break;

            case 31:
                triangles.AddRange(new int[] { 9, 5, 4, 11, 9, 4, 10, 11, 4 });
                break;

            case 32:
                triangles.AddRange(new int[] { 10, 4, 7 });
                break;

            case 33:
                triangles.AddRange(new int[] { 4, 7, 10, 0, 4, 10, 0, 8, 4, 0, 1, 8 });
                break;

            case 34:
                triangles.AddRange(new int[] { 4, 7, 0, 0, 7, 3 });
                break;

            case 35:
                triangles.AddRange(new int[] { 8, 3, 1, 8, 4, 3, 4, 7, 3 });
                break;

            case 36:
                triangles.AddRange(new int[] { 4, 7, 10, 10, 7, 11, 10, 11, 3, 3, 11, 2 });
                break;

            case 37:
                triangles.AddRange(new int[] { 0, 1, 8, 0, 2, 1, 0, 3, 2, 3, 11, 2, 10, 11, 3, 10, 7, 11, 4, 7, 10 });
                break;

            case 38:
                triangles.AddRange(new int[] { 0, 11, 2, 0, 4, 11, 4, 7, 11 });
                break;

            case 39:
                triangles.AddRange(new int[] { 4, 7, 11, 4, 11, 2, 4, 2, 8, 8, 2, 1 });
                break;

            case 40:
                triangles.AddRange(new int[] { 4, 7, 10, 1, 2, 9 });
                break;

            case 41:
                triangles.AddRange(new int[] { 4, 7, 10, 0, 4, 10, 0, 8, 4, 0, 2, 8, 2, 9, 8 });
                break;

            case 42:
                triangles.AddRange(new int[] { 1, 2, 9, 0, 2, 1, 0, 3, 2, 0, 4, 7, 3, 0, 7 });
                break;

            case 43:
                triangles.AddRange(new int[] { 3, 2, 9, 8, 3, 9, 4, 3, 8, 7, 3, 4 });
                break;

            case 44:
                triangles.AddRange(new int[] { 4, 7, 10, 10, 7, 11, 3, 10, 11, 3, 11, 1, 1, 11, 9 });
                break;

            case 45:
                triangles.AddRange(new int[] { 4, 7, 10, 0, 3, 11, 0, 11, 8, 8, 11, 9 });
                break;

            case 46:
                triangles.AddRange(new int[] { 0, 9, 1, 0, 11, 9, 4, 11, 0, 4, 7, 11 });
                break;

            case 47:
                triangles.AddRange(new int[] { 8, 11, 9, 4, 11, 8, 4, 7, 11 });
                break;

            case 48:
                triangles.AddRange(new int[] { 10, 8, 5, 10, 5, 7 });
                break;

            case 49:
                triangles.AddRange(new int[] { 0, 1, 5, 0, 5, 10, 10, 5, 7 });
                break;

            case 50:
                triangles.AddRange(new int[] { 3, 5, 7, 0, 5, 3, 0, 8, 5 });
                break;

            case 51:
                triangles.AddRange(new int[] { 5, 7, 1, 7, 3, 1 });
                break;

            case 52:
                triangles.AddRange(new int[] { 8, 5, 7, 8, 7, 10, 10, 7, 11, 10, 11, 3, 3, 11, 2 });
                break;

            case 53:
                triangles.AddRange(new int[] { 3, 11, 2, 0, 1, 5, 0, 5, 10, 10, 5, 7 });
                break;

            case 54:
                triangles.AddRange(new int[] { 5, 7, 8, 8, 7, 0, 7, 11, 0, 0, 11, 2 });
                break;

            case 55:
                triangles.AddRange(new int[] { 5, 2, 1, 5, 11, 2, 5, 7, 11 });
                break;

            case 56:
                triangles.AddRange(new int[] { 8, 7, 10, 8, 5, 7, 8, 1, 9, 8, 9, 5, 1, 2, 9 });
                break;

            case 57:
                triangles.AddRange(new int[] { 0, 2, 9, 0, 9, 5, 0, 5, 10, 10, 5, 7 });
                break;

            case 58:
                triangles.AddRange(new int[] { 0, 7, 3, 0, 8, 7, 8, 5, 7, 8, 9, 5, 8, 1, 9, 1, 2, 9 });
                break;

            case 59:
                triangles.AddRange(new int[] { 7, 9, 5, 7, 2, 9, 7, 3, 2 });
                break;

            case 60:
                triangles.AddRange(new int[] { 10, 7, 11, 10, 11, 3, 11, 1, 3, 1, 11, 9, 10, 8, 7, 7, 8, 5, 8, 9, 5, 8, 1, 9 });
                break;

            case 61:
                triangles.AddRange(new int[] { 0, 8, 1, 8, 5, 9, 8, 9, 1, 9, 5, 7, 11, 9, 7 });
                break;

            case 62:
                triangles.AddRange(new int[] { 7, 11, 5, 5, 11, 9, 10, 11, 7, 10, 3, 11, 10, 0, 3 });
                break;

            case 63:
                triangles.AddRange(new int[] { 7, 9, 5, 7, 11, 9 });
                break;

            case 64:
                triangles.AddRange(new int[] { 7, 6, 11 });
                break;

            case 65:
                triangles.AddRange(new int[] { 7, 6, 11, 0, 1, 8 });
                break;

            case 66:
                triangles.AddRange(new int[] { 7, 6, 11, 10, 7, 11, 10, 11, 3, 10, 3, 0 });
                break;

            case 67:
                triangles.AddRange(new int[] { 7, 6, 11, 10, 7, 11, 10, 11, 3, 10, 3, 1, 10, 1, 8 });
                break;

            case 68:
                triangles.AddRange(new int[] { 7, 6, 2, 7, 2, 3 });
                break;

            case 69:
                triangles.AddRange(new int[] { 7, 6, 2, 7, 2, 3, 0, 3, 2, 0, 2, 1, 1, 8, 0 });
                break;

            case 70:
                triangles.AddRange(new int[] { 7, 6, 10, 0, 10, 6, 0, 6, 2 });
                break;

            case 71:
                triangles.AddRange(new int[] { 8, 2, 1, 8, 6, 2, 8, 7, 6, 8, 10, 7 });
                break;

            case 72:
                triangles.AddRange(new int[] { 7, 6, 11, 6, 2, 11, 6, 9, 2, 1, 2, 9 });
                break;

            case 73:
                triangles.AddRange(new int[] { 7, 6, 11, 6, 2, 11, 6, 9, 2, 8, 2, 9, 8, 0, 2 });
                break;

            case 74:
                triangles.AddRange(new int[] { 7, 6, 11, 6, 2, 11, 6, 9, 2, 1, 2, 9, 0, 2, 1, 0, 3, 2, 10, 3, 0 });
                break;

            case 75:
                triangles.AddRange(new int[] { 7, 6, 11, 6, 2, 11, 6, 9, 2, 8, 2, 9, 8, 3, 2, 8, 10, 3 });
                break;

            case 76:
                triangles.AddRange(new int[] { 7, 6, 9, 3, 7, 9, 1, 3, 9 });
                break;

            case 77:
                triangles.AddRange(new int[] { 8, 6, 9, 8, 0, 6, 0, 3, 6, 3, 7, 6 });
                break;

            case 78:
                triangles.AddRange(new int[] { 0, 9, 1, 0, 6, 9, 0, 10, 6, 10, 7, 6 });
                break;

            case 79:
                triangles.AddRange(new int[] { 8, 6, 9, 8, 10, 6, 10, 7, 6 });
                break;

            case 80:
                triangles.AddRange(new int[] { 8, 5, 4, 4, 5, 6, 4, 6, 7, 7, 6, 11 });
                break;

            case 81:
                triangles.AddRange(new int[] { 4, 5, 6, 4, 6, 7, 7, 6, 11, 0, 1, 4, 4, 1, 5 });
                break;

            case 82:
                triangles.AddRange(new int[] { 4, 5, 6, 4, 6, 7, 7, 6, 11, 10, 7, 11, 10, 11, 3, 0, 10, 3, 4, 8, 5 });
                break;

            case 83:
                triangles.AddRange(new int[] { 4, 10, 7, 4, 7, 6, 4, 6, 5, 1, 5, 6, 1, 6, 11, 1, 11, 3 });
                break;

            case 84:
                triangles.AddRange(new int[] { 8, 5, 4, 4, 5, 6, 4, 6, 7, 6, 2, 7, 3, 7, 2 });
                break;

            case 85:
                triangles.AddRange(new int[] { 4, 5, 6, 4, 6, 7, 0, 3, 2, 0, 2, 1, 2, 7, 6, 2, 3, 7, 0, 1, 5, 0, 5, 4 });
                break;

            case 86:
                triangles.AddRange(new int[] { 4, 5, 6, 4, 6, 7, 4, 8, 5, 10, 7, 6, 0, 10, 6, 2, 0, 6 });
                break;

            case 87:
                triangles.AddRange(new int[] { 4, 10, 7, 4, 7, 6, 4, 6, 5, 5, 6, 2, 1, 5, 2 });
                break;

            case 88:
                triangles.AddRange(new int[] { 7, 6, 11, 6, 9, 2, 6, 2, 11, 1, 2, 9, 8, 1, 9, 8, 9, 5, 4, 8, 5 });
                break;

            case 89:
                triangles.AddRange(new int[] { 7, 6, 11, 6, 9, 2, 6, 2, 11, 0, 2, 4, 4, 2, 9, 4, 9, 5 });
                break;

            case 90:
                triangles.AddRange(new int[] { 7, 6, 11, 6, 2, 11, 6, 9, 2, 4, 5, 6, 4, 6, 7, 10, 7, 11, 10, 11, 3, 4, 8, 5, 0, 10, 3,
                0, 8, 4, 0, 4, 10, 0, 2, 1, 0, 3, 2, 8, 1, 9, 8, 9, 5, 1, 2 ,9 });
                break;

            case 91:
                triangles.AddRange(new int[] { 7, 6, 11, 6, 2, 11, 6, 9, 2, 10, 3, 2, 10, 2, 9, 10, 9, 4, 4, 9, 5 });
                break;

            case 92:
                triangles.AddRange(new int[] { 4, 8, 5, 4, 5, 6, 4, 6, 7, 8, 1, 9, 8, 9, 5, 9, 1, 3, 9, 3, 6, 6, 3, 7 });
                break;

            case 93:
                triangles.AddRange(new int[] { 5, 6, 9, 4, 6, 5, 4, 7, 6, 4, 0, 7, 7, 0, 3 });
                break;

            case 94:
                triangles.AddRange(new int[] { 4, 8, 5, 8, 1, 9, 8, 9, 5, 4, 5, 6, 4, 6, 7, 10, 7, 6, 0, 10, 6, 0, 6, 9, 0, 9, 1 });
                break;

            case 95:
                triangles.AddRange(new int[] { 4, 10, 7, 4, 7, 6, 4, 6, 5, 5, 6, 9 });
                break;

            case 96:
                triangles.AddRange(new int[] { 10, 4, 6, 10, 6, 11 });
                break;

            case 97:
                triangles.AddRange(new int[] { 10, 4, 6, 10, 6, 11, 0, 8, 4, 0, 4, 10, 8, 0, 1 });
                break;

            case 98:
                triangles.AddRange(new int[] { 0, 4, 6, 0, 6, 11, 0, 11, 3 });
                break;

            case 99:
                triangles.AddRange(new int[] { 8, 3, 1, 8, 11, 3, 8, 6, 11, 8, 4, 6 });
                break;

            case 100:
                triangles.AddRange(new int[] { 10, 4, 6, 10, 6, 2, 10, 2, 3 });
                break;

            case 101:
                triangles.AddRange(new int[] { 10, 4, 6, 3, 10, 6, 2, 3, 6, 0, 3, 2, 0, 2, 1, 8, 0, 1 });
                break;

            case 102:
                triangles.AddRange(new int[] { 0, 4, 6, 0, 6, 2 });
                break;

            case 103:
                triangles.AddRange(new int[] { 1, 8, 2, 2, 8, 6, 8, 4, 6 });
                break;

            case 104:
                triangles.AddRange(new int[] { 10, 4, 6, 10, 6, 11, 6, 9, 2, 6, 2, 11, 1, 2, 9 });
                break;

            case 105:
                triangles.AddRange(new int[] { 10, 4, 6, 10, 6, 11, 8, 0, 9, 0, 2, 9, 4, 10, 0, 4, 0, 8, 6, 2, 11, 6, 9, 2 });
                break;

            case 106:
                triangles.AddRange(new int[] { 0, 4, 6, 0, 6, 11, 0, 11, 3, 6, 9, 2, 6, 2, 11, 1, 2, 9 });
                break;

            case 107:
                triangles.AddRange(new int[] { 8, 2, 9, 8, 3, 2, 8, 4, 3, 3, 4, 11, 4, 6, 11 });
                break;

            case 108:
                triangles.AddRange(new int[] { 10, 4, 3, 3, 4, 6, 3, 6, 1, 1, 6, 9 });
                break;

            case 109:
                triangles.AddRange(new int[] { 8, 0, 9, 0, 3, 9, 3, 6, 9, 3, 10, 6, 10, 4, 6 });
                break;

            case 110:
                triangles.AddRange(new int[] { 0, 4, 6, 0, 6, 9, 0, 9, 1 });
                break;

            case 111:
                triangles.AddRange(new int[] { 8, 4, 6, 8, 6, 9 });
                break;

            case 112:
                triangles.AddRange(new int[] { 8, 5, 6, 10, 8, 6, 10, 6, 11 });
                break;

            case 113:
                triangles.AddRange(new int[] { 0, 1, 5, 0, 5, 6, 0, 6, 10, 10, 6, 11 });
                break;

            case 114:
                triangles.AddRange(new int[] { 8, 5, 6, 0, 8, 6, 0, 6, 11, 0, 11, 3 });
                break;

            case 115:
                triangles.AddRange(new int[] { 3, 1, 5, 3, 5, 6, 3, 6, 11 });
                break;

            case 116:
                triangles.AddRange(new int[] { 8, 5, 6, 8, 6, 10, 10, 6, 2, 10, 2, 3 });
                break;

            case 117:
                triangles.AddRange(new int[] { 10, 0, 3, 0, 1, 3, 3, 1, 2, 1, 6, 2, 1, 5, 6 });
                break;

            case 118:
                triangles.AddRange(new int[] { 8, 5, 6, 8, 6, 0, 0, 6, 2 });
                break;

            case 119:
                triangles.AddRange(new int[] { 1, 5, 6, 1, 6, 2 });
                break;

            case 120:
                triangles.AddRange(new int[] { 1, 2, 9, 6, 9, 2, 6, 2, 11, 8, 5, 6, 10, 8, 6, 10, 6, 11 });
                break;

            case 121:
                triangles.AddRange(new int[] { 0, 2, 9, 0, 9, 5, 0, 5, 10, 10, 5, 6, 10, 6, 11 });
                break;

            case 122:
                triangles.AddRange(new int[] { 8, 5, 6, 0, 8, 6, 0, 6, 11, 0, 11, 3, 6, 9, 2, 6, 2, 11, 1, 2, 9 });
                break;

            case 123:
                triangles.AddRange(new int[] { 5, 6, 9, 6, 2, 9, 6, 11, 2, 3, 2, 11 });
                break;

            case 124:
                triangles.AddRange(new int[] { 10, 8, 1, 10, 1, 3, 8, 5, 9, 8, 9, 1, 5, 6, 9 });
                break;

            case 125:
                triangles.AddRange(new int[] { 5, 6, 9, 10, 0, 3 });
                break;

            case 126:
                triangles.AddRange(new int[] { 5, 6, 9, 8, 5, 9, 8, 9, 1, 0, 8, 1 });
                break;

            case 127:
                triangles.AddRange(new int[] { 5, 6, 9 });
                break;

            case 128:
                triangles.AddRange(new int[] { 9, 6, 5 });
                break;

            case 129:
                triangles.AddRange(new int[] { 9, 6, 5, 8, 1, 9, 8, 9, 5, 8, 0, 1 });
                break;

            case 130:
                triangles.AddRange(new int[] { 9, 6, 5, 0, 10, 3 });
                break;

            case 131:
                triangles.AddRange(new int[] { 9, 6, 5, 8, 1, 9, 5, 8, 9, 8, 3, 1, 8, 10, 3 });
                break;

            case 132:
                triangles.AddRange(new int[] { 5, 9, 6, 6, 9, 2, 6, 2, 11, 3, 11, 2 });
                break;

            case 133:
                triangles.AddRange(new int[] { 8, 0, 1, 0, 3, 1, 1, 3, 2, 3, 11, 2, 2, 11, 6, 2, 6, 9, 5, 9, 6, 8, 1, 9, 8, 9, 5 });
                break;

            case 134:
                triangles.AddRange(new int[] { 5, 9, 6, 6, 9, 2, 6, 2, 11, 0, 11, 2, 0, 10, 11 });
                break;

            case 135:
                triangles.AddRange(new int[] { 5, 9, 6, 6, 9, 2, 6, 2, 11, 8, 2, 1, 8, 10, 2, 10, 11, 2 });
                break;

            case 136:
                triangles.AddRange(new int[] { 5, 1, 6, 6, 1, 2 });
                break;

            case 137:
                triangles.AddRange(new int[] { 8, 0, 2, 8, 2, 6, 8, 6, 5 });
                break;

            case 138:
                triangles.AddRange(new int[] { 5, 1, 6, 6, 1, 2, 0, 2, 1, 0, 3, 2, 0, 10, 3 });
                break;

            case 139:
                triangles.AddRange(new int[] { 10, 3, 2, 8, 10, 2, 8, 2, 6, 8, 6, 5 });
                break;

            case 140:
                triangles.AddRange(new int[] { 5, 1, 3, 5, 3, 11, 5, 11, 6 });
                break;

            case 141:
                triangles.AddRange(new int[] { 8, 6, 5, 8, 11, 6, 8, 0, 11, 0, 3, 11 });
                break;

            case 142:
                triangles.AddRange(new int[] { 0, 10, 11, 0, 11, 6, 1, 0, 6, 5, 1, 6 });
                break;

            case 143:
                triangles.AddRange(new int[] { 8, 10, 11, 8, 11, 6, 8, 6, 5 });
                break;

            case 144:
                triangles.AddRange(new int[] { 4, 8, 9, 4, 9, 6 });
                break;

            case 145:
                triangles.AddRange(new int[] { 4, 9, 6, 4, 0, 9, 0, 1, 9 });
                break;

            case 146:
                triangles.AddRange(new int[] { 4, 8, 9, 4, 9, 6, 0, 10, 3, 0, 8, 4, 0, 4, 10 });
                break;

            case 147:
                triangles.AddRange(new int[] { 10, 3, 1, 10, 1, 9, 10, 9, 4, 4, 9, 6 });
                break;

            case 148:
                triangles.AddRange(new int[] { 4, 8, 9, 4, 9, 6, 6, 9, 2, 6, 2, 11, 3, 11, 2 });
                break;

            case 149:
                triangles.AddRange(new int[] { 6, 9, 2, 6, 2, 11, 3, 11, 2, 4, 9, 6, 4, 0, 9, 0, 1, 9 });
                break;

            case 150:
                triangles.AddRange(new int[] { 4, 8, 9, 4, 9, 6, 0, 11, 2, 0, 10, 11, 0, 8, 4, 0, 4, 10, 6, 9, 2, 6, 2, 11 });
                break;

            case 151:
                triangles.AddRange(new int[] { 4, 10, 11, 4, 11, 6, 6, 11, 2, 6, 2, 9, 1, 9, 2 });
                break;

            case 152:
                triangles.AddRange(new int[] { 4, 2, 6, 4, 8, 2, 8, 1, 2 });
                break;

            case 153:
                triangles.AddRange(new int[] { 4, 0, 2, 4, 2, 6 });
                break;

            case 154:
                triangles.AddRange(new int[] { 0, 10, 3, 0, 8, 4, 0, 4, 10, 4, 8, 2, 8, 1, 2, 4, 2, 6 });
                break;

            case 155:
                triangles.AddRange(new int[] { 4, 2, 6, 4, 10, 2, 10, 3, 2 });
                break;

            case 156:
                triangles.AddRange(new int[] { 1, 3, 11, 1, 11, 6, 1, 6, 8, 8, 6, 4 });
                break;

            case 157:
                triangles.AddRange(new int[] { 6, 4, 11, 4, 0, 11, 0, 3, 11 });
                break;

            case 158:
                triangles.AddRange(new int[] { 10, 11, 4, 4, 11, 6, 4, 8, 10, 8, 0, 10, 0, 8, 1 });
                break;

            case 159:
                triangles.AddRange(new int[] { 10, 11, 4, 4, 11, 6 });
                break;

            case 160:
                triangles.AddRange(new int[] { 5, 9, 6, 4, 5, 6, 4, 6, 7, 10, 4, 7 });
                break;

            case 161:
                triangles.AddRange(new int[] { 5, 9, 6, 4, 5, 6, 4, 6, 7, 10, 4, 7, 8, 1, 9, 8, 9, 5, 8, 0, 1 });
                break;

            case 162:
                triangles.AddRange(new int[] { 5, 9, 6, 4, 5, 6, 4, 6, 7, 0, 4, 7, 0, 7, 3 });
                break;

            case 163:
                triangles.AddRange(new int[] { 5, 9, 6, 4, 5, 6, 4, 6, 7, 4, 7, 3, 8, 4, 3, 8, 3, 1, 8, 1, 9, 8, 9, 5 });
                break;

            case 164:
                triangles.AddRange(new int[] { 5, 9, 6, 4, 5, 6, 4, 6, 7, 10, 4, 7, 10, 7, 11, 10, 11, 3, 3, 11, 2, 6, 9, 2, 6, 2, 11 });
                break;

            case 165:
                triangles.AddRange(new int[] { 5, 9, 6, 4, 5, 6, 4, 6, 7, 10, 4, 7, 10, 7, 11, 10, 11, 3, 3, 11, 2, 6, 9, 2, 6, 2, 11,
                0, 8, 4, 0, 4, 10, 8, 1, 9, 8, 9, 5, 0, 3, 2, 0, 2, 1 });
                break;

            case 166:
                triangles.AddRange(new int[] { 6, 9, 2, 6, 2, 11, 5, 9, 6, 4, 5, 6, 4, 6, 7, 4, 7, 11, 0, 4, 11, 0, 11, 2 });
                break;

            case 167:
                triangles.AddRange(new int[] { 6, 9, 2, 6, 2, 11, 5, 9, 6, 8, 2, 1, 8, 11, 2, 8, 4, 11, 4, 7, 11 });
                break;

            case 168:
                triangles.AddRange(new int[] { 4, 7, 10, 4, 5, 6, 4, 6, 7, 5, 1, 2, 5, 2, 6 });
                break;

            case 169:
                triangles.AddRange(new int[] { 4, 7, 10, 4, 5, 6, 4, 6, 7, 8, 6, 5, 8, 2, 6, 8, 0, 2 });
                break;

            case 170:
                triangles.AddRange(new int[] { 4, 5, 6, 4, 6, 7, 4, 7, 0, 0, 7, 3, 0, 2, 1, 0, 3, 2, 5, 1, 2, 5, 2, 6 });
                break;

            case 171:
                triangles.AddRange(new int[] { 4, 5, 8, 4, 6, 5, 4, 7, 6, 7, 2, 6, 7, 3, 2 });
                break;

            case 172:
                triangles.AddRange(new int[] { 10, 4, 7, 4, 5, 6, 4, 6, 7, 5, 1, 6, 6, 1, 11, 3, 11, 1, 10, 7, 11, 10, 11, 3 });
                break;

            case 173:
                triangles.AddRange(new int[] { 10, 7, 11, 10, 11, 3, 10, 4, 7, 0, 3, 11, 0, 11, 8, 8, 11, 6, 8, 6, 5 });
                break;

            case 174:
                triangles.AddRange(new int[] { 4, 7, 11, 0, 4, 11, 1, 0, 11, 6, 1, 11, 6, 5, 1 });
                break;

            case 175:
                triangles.AddRange(new int[] { 8, 4, 5, 4, 6, 5, 4, 7, 6, 7, 11, 6 });
                break;

            case 176:
                triangles.AddRange(new int[] { 10, 8, 9, 10, 9, 6, 10, 6, 7 });
                break;

            case 177:
                triangles.AddRange(new int[] { 0, 1, 9, 0, 9, 10, 10, 9, 6, 10, 6, 7 });
                break;

            case 178:
                triangles.AddRange(new int[] { 0, 7, 3, 0, 6, 7, 0, 9, 6, 0, 8, 9 });
                break;

            case 179:
                triangles.AddRange(new int[] { 3, 1, 9, 3, 9, 6, 3, 6, 7 });
                break;

            case 180:
                triangles.AddRange(new int[] { 10, 8, 9, 10, 9, 6, 10, 6, 7, 6, 9, 2, 6, 2, 11, 3, 11, 2 });
                break;

            case 181:
                triangles.AddRange(new int[] { 10, 9, 6, 10, 6, 7, 6, 9, 2, 6, 2, 11, 3, 11, 2, 10, 0, 9, 0, 1, 9 });
                break;

            case 182:
                triangles.AddRange(new int[] { 7, 11, 6, 6, 11, 2, 6, 2, 9, 0, 8, 9, 0, 9, 2 });
                break;

            case 183:
                triangles.AddRange(new int[] { 7, 11, 6, 6, 11, 2, 6, 2, 9, 1, 9, 2 });
                break;

            case 184:
                triangles.AddRange(new int[] { 8, 1, 2, 10, 8, 2, 6, 10, 2, 7, 10, 6 });
                break;

            case 185:
                triangles.AddRange(new int[] { 6, 10, 2, 7, 10, 6, 10, 0, 2 });
                break;

            case 186:
                triangles.AddRange(new int[] { 0, 8, 1, 0, 1, 2, 0, 2, 3, 7, 3, 2, 7, 2, 6 });
                break;

            case 187:
                triangles.AddRange(new int[] { 7, 3, 2, 7, 2, 6 });
                break;

            case 188:
                triangles.AddRange(new int[] { 7, 11, 6, 10, 3, 11, 10, 11, 7, 10, 8, 1, 10, 1, 3 });
                break;

            case 189:
                triangles.AddRange(new int[] { 7, 11, 6, 10, 3, 11, 10, 11, 7, 10, 0, 3 });
                break;

            case 190:
                triangles.AddRange(new int[] { 7, 11, 6, 0, 8, 1 });
                break;

            case 191:
                triangles.AddRange(new int[] { 7, 11, 6 });
                break;

            case 192:
                triangles.AddRange(new int[] { 7, 5, 9, 7, 9, 11 });
                break;

            case 193:
                triangles.AddRange(new int[] { 7, 5, 9, 7, 9, 11, 8, 1, 9, 8, 9, 5, 8, 0, 1 });
                break;

            case 194:
                triangles.AddRange(new int[] { 7, 5, 9, 7, 9, 11, 10, 7, 11, 10, 11, 3, 0, 10, 3 });
                break;

            case 195:
                triangles.AddRange(new int[] { 7, 5, 9, 7, 9, 11, 10, 7, 11, 10, 11, 3, 8, 1, 9, 8, 9, 5, 8, 10, 3, 8, 3, 1 });
                break;

            case 196:
                triangles.AddRange(new int[] { 5, 9, 2, 7, 5, 2, 3, 7, 2 });
                break;

            case 197:
                triangles.AddRange(new int[] { 8, 0, 1, 1, 0, 2, 0, 3, 2, 3, 7, 2, 7, 9, 2, 7, 5, 9 });
                break;

            case 198:
                triangles.AddRange(new int[] { 7, 5, 9, 10, 7, 9, 0, 10, 9, 2, 0, 9 });
                break;

            case 199:
                triangles.AddRange(new int[] { 2, 1, 9, 1, 8, 9, 9, 8, 5, 8, 7, 5, 8, 10, 7 });
                break;

            case 200:
                triangles.AddRange(new int[] { 7, 5, 1, 7, 1, 2, 7, 2, 11 });
                break;

            case 201:
                triangles.AddRange(new int[] { 8, 0, 2, 8, 2, 11, 8, 11, 7, 8, 7, 5 });
                break;

            case 202:
                triangles.AddRange(new int[] { 0, 10, 3, 10, 7, 11, 10, 11, 3, 7, 5, 11, 11, 5, 2, 2, 5, 1 });
                break;

            case 203:
                triangles.AddRange(new int[] { 11, 3, 2, 7, 10, 3, 7, 3, 11, 7, 5, 8, 7, 8, 10 });
                break;

            case 204:
                triangles.AddRange(new int[] { 7, 5, 3, 5, 1, 3 });
                break;

            case 205:
                triangles.AddRange(new int[] { 0, 3, 7, 8, 0, 7, 5, 8, 7 });
                break;

            case 206:
                triangles.AddRange(new int[] { 10, 7, 5, 0, 10, 5, 1, 0, 5 });
                break;

            case 207:
                triangles.AddRange(new int[] { 8, 7, 5, 8, 10, 7 });
                break;

            case 208:
                triangles.AddRange(new int[] { 4, 8, 9, 4, 9, 11, 4, 11, 7 });
                break;

            case 209:
                triangles.AddRange(new int[] { 4, 9, 11, 4, 11, 7, 4, 0, 9, 0, 1, 9 });
                break;

            case 210:
                triangles.AddRange(new int[] { 4, 8, 9, 4, 9, 11, 4, 11, 7, 0, 8, 4, 0, 4, 10, 0, 10, 3 });
                break;

            case 211:
                triangles.AddRange(new int[] { 3, 1, 9, 3, 9, 11, 10, 3, 11, 10, 11, 7, 4, 10, 7 });
                break;

            case 212:
                triangles.AddRange(new int[] { 4, 2, 7, 7, 2, 3, 4, 9, 2, 4, 8, 9 });
                break;

            case 213:
                triangles.AddRange(new int[] { 1, 9, 2, 0, 1, 2, 0, 2, 3, 4, 0, 3, 4, 3, 7 });
                break;

            case 214:
                triangles.AddRange(new int[] { 0, 8, 9, 0, 9, 2, 4, 8, 0, 4, 0, 10, 4, 10, 7 });
                break;

            case 215:
                triangles.AddRange(new int[] { 4, 10, 7, 1, 9, 2 });
                break;

            case 216:
                triangles.AddRange(new int[] { 8, 1, 2, 8, 2, 4, 4, 2, 11, 4, 11, 7 });
                break;

            case 217:
                triangles.AddRange(new int[] { 4, 11, 7, 11, 4, 0, 11, 0, 2 });
                break;

            case 218:
                triangles.AddRange(new int[] { 4, 10, 7, 7, 10, 11, 10, 3, 11, 11, 3, 2, 3, 0, 2, 0, 1, 2, 0, 8, 1 });
                break;

            case 219:
                triangles.AddRange(new int[] { 4, 10, 7, 7, 10, 11, 10, 3, 11, 11, 3, 2 });
                break;

            case 220:
                triangles.AddRange(new int[] { 4, 3, 7, 4, 8, 3, 8, 1, 3 });
                break;

            case 221:
                triangles.AddRange(new int[] { 4, 0, 3, 4, 3, 7 });
                break;

            case 222:
                triangles.AddRange(new int[] { 4, 10, 7, 0, 4, 8, 0, 10, 4, 0, 8, 1 });
                break;

            case 223:
                triangles.AddRange(new int[] { 4, 10, 7 });
                break;

            case 224:
                triangles.AddRange(new int[] { 4, 5, 9, 10, 4, 9, 11, 10, 9 });
                break;

            case 225:
                triangles.AddRange(new int[] { 4, 5, 9, 10, 4, 9, 11, 10, 9, 0, 1, 8, 8, 1, 9, 8, 9, 5 });
                break;

            case 226:
                triangles.AddRange(new int[] { 4, 5, 9, 0, 4, 9, 0, 9, 11, 0, 11, 3 });
                break;

            case 227:
                triangles.AddRange(new int[] { 8, 4, 5, 8, 5, 9, 8, 9, 1, 3, 1, 9, 3, 9, 11 });
                break;

            case 228:
                triangles.AddRange(new int[] { 4, 5, 9, 10, 4, 9, 2, 10, 9, 2, 3, 10 });
                break;

            case 229:
                triangles.AddRange(new int[] { 4, 5, 9, 10, 4, 9, 2, 10, 9, 2, 3, 10, 1, 0, 2, 2, 0, 3, 8, 0, 1 });
                break;

            case 230:
                triangles.AddRange(new int[] { 4, 5, 9, 0, 4, 9, 2, 0, 9 });
                break;

            case 231:
                triangles.AddRange(new int[] { 8, 4, 5, 8, 5, 9, 8, 9, 1, 2, 1, 9 });
                break;

            case 232:
                triangles.AddRange(new int[] { 5, 1, 2, 4, 5, 2, 10, 4, 2, 11, 10, 2 });
                break;

            case 233:
                triangles.AddRange(new int[] { 10, 0, 2, 10, 2, 11, 4, 8, 0, 4, 0, 10, 8, 4, 5 });
                break;

            case 234:
                triangles.AddRange(new int[] { 3, 2, 11, 0, 1, 2, 0, 2, 3, 0, 4, 1, 1, 4, 5 });
                break;

            case 235:
                triangles.AddRange(new int[] { 3, 2, 11, 8, 4, 5 });
                break;

            case 236:
                triangles.AddRange(new int[] { 4, 5, 1, 10, 4, 1, 3, 10, 1 });
                break;

            case 237:
                triangles.AddRange(new int[] { 8, 4, 5, 4, 8, 0, 4, 0, 10, 10, 0, 3 });
                break;

            case 238:
                triangles.AddRange(new int[] { 0, 4, 5, 0, 5, 1 });
                break;

            case 239:
                triangles.AddRange(new int[] { 8, 4, 5 });
                break;

            case 240:
                triangles.AddRange(new int[] { 10, 8, 9, 10, 9, 11 });
                break;

            case 241:
                triangles.AddRange(new int[] { 10, 9, 11, 10, 0, 9, 0, 1, 9 });
                break;

            case 242:
                triangles.AddRange(new int[] { 8, 9, 11, 8, 11, 0, 3, 0, 11 });
                break;

            case 243:
                triangles.AddRange(new int[] { 3, 1, 9, 3, 9, 11 });
                break;

            case 244:
                triangles.AddRange(new int[] { 10, 8, 9, 3, 10, 9, 2, 3, 9 });
                break;

            case 245:
                triangles.AddRange(new int[] { 10, 0, 3, 0, 1, 2, 0, 2, 3, 1, 9, 2 });
                break;

            case 246:
                triangles.AddRange(new int[] { 0, 8, 2, 8, 9, 2 });
                break;

            case 247:
                triangles.AddRange(new int[] { 1, 9, 2 });
                break;

            case 248:
                triangles.AddRange(new int[] { 10, 8, 11, 8, 1, 11, 1, 2, 11 });
                break;

            case 249:
                triangles.AddRange(new int[] { 10, 0, 2, 10, 2, 11 });
                break;

            case 250:
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 11, 0, 8, 1 });
                break;

            case 251:
                triangles.AddRange(new int[] { 3, 2, 11 });
                break;

            case 252:
                triangles.AddRange(new int[] { 10, 8, 3, 8, 1, 3 });
                break;

            case 253:
                triangles.AddRange(new int[] { 10, 0, 3 });
                break;

            case 254:
                triangles.AddRange(new int[] { 0, 8, 1 });
                break;

            case 255:
                triangles.AddRange(new int[] { });
                break;
        }
    }

    private int GetConfiguration()
    {
        int configuration = 0;

        if (topRightFrontValue > isoValue)
            configuration = configuration | (1 << 0);

        if (topLeftFrontValue > isoValue)
            configuration = configuration | (1 << 1);

        if (bottomLeftFrontValue > isoValue)
            configuration = configuration | (1 << 2);

        if (bottomRightFrontValue > isoValue)
            configuration = configuration | (1 << 3);

        if (topRightBackValue > isoValue)
            configuration = configuration | (1 << 4);

        if (topLeftBackValue > isoValue)
            configuration = configuration | (1 << 5);

        if (bottomLeftBackValue > isoValue)
            configuration = configuration | (1 << 6);

        if (bottomRightBackValue > isoValue)
            configuration = configuration | (1 << 7);

        return configuration;
    }

    //Visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;

        Gizmos.DrawLine(topRightFront, topLeftFront);
        Gizmos.DrawLine(topLeftFront, bottomLeftFront);
        Gizmos.DrawLine(bottomLeftFront, bottomRightFront);
        Gizmos.DrawLine(bottomRightFront, bottomRightBack);
        Gizmos.DrawLine(bottomRightBack, bottomLeftBack);
        Gizmos.DrawLine(bottomLeftBack, topLeftBack);
        Gizmos.DrawLine(topLeftBack, topRightBack);
        Gizmos.DrawLine(topRightBack, bottomRightBack);
        Gizmos.DrawLine(topRightFront, bottomRightFront);
        Gizmos.DrawLine(topRightFront, topRightBack);
        Gizmos.DrawLine(topLeftFront, topLeftBack);
        Gizmos.DrawLine(bottomLeftFront, bottomLeftBack);


        Gizmos.color = Color.black;

        Gizmos.DrawSphere(topRightFront, gridLength / 8);
        Gizmos.DrawSphere(topRightBack, gridLength / 8);
        Gizmos.DrawSphere(topLeftFront, gridLength / 8);
        Gizmos.DrawSphere(topLeftBack, gridLength / 8);
        Gizmos.DrawSphere(bottomLeftBack, gridLength / 8);
        Gizmos.DrawSphere(bottomLeftFront, gridLength / 8);
        Gizmos.DrawSphere(bottomRightBack, gridLength / 8);
        Gizmos.DrawSphere(bottomRightFront, gridLength / 8);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(topCenterFront, gridLength / 16);
        Gizmos.DrawSphere(topCenterBack, gridLength / 16);
        Gizmos.DrawSphere(topCenterRight, gridLength / 16);
        Gizmos.DrawSphere(topCenterLeft, gridLength / 16);
        Gizmos.DrawSphere(rightCenterFront, gridLength / 16);
        Gizmos.DrawSphere(rightCenterBack, gridLength / 16);
        Gizmos.DrawSphere(leftCenterBack, gridLength / 16);
        Gizmos.DrawSphere(leftCenterFront, gridLength / 16);
        Gizmos.DrawSphere(bottomCenterBack, gridLength / 16);
        Gizmos.DrawSphere(bottomCenterFront, gridLength / 16);
        Gizmos.DrawSphere(bottomCenterLeft, gridLength / 16);
        Gizmos.DrawSphere(bottomCenterRight, gridLength / 16);
    }

}
