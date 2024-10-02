using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeTester : MonoBehaviour
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

    private float[] cornerValues;


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
        cornerValues = new float[] {topRightFrontValue,topLeftFrontValue,bottomLeftFrontValue,bottomRightFrontValue,
        topRightBackValue,topLeftBackValue,bottomLeftBackValue,bottomRightBackValue};


        Cube cubeData = new Cube(Vector3.zero, gridLength);
        cubeData.TriangulateWithInterpolation(isoValue,cornerValues);

        mesh.vertices = cubeData.GetVertices();
        mesh.triangles = cubeData.GetTriangles();

        //Assigning the two lists to the mesh filter
        filter.mesh = mesh;
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
