using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
    [SerializeField] bool visualize = false;

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
        cubeData.TriangulateWithInterpolation(isoValue, cornerValues);
        if (visualize)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = topRightFront;
            sphere.transform.localScale = Vector3.one * gridLength / 8;
            sphere.GetComponent<Renderer>().material.color = Color.red;


            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = topRightBack;
            sphere.transform.localScale = Vector3.one * gridLength / 8;
            sphere.GetComponent<Renderer>().material.color = Color.red;


            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = topLeftBack;
            sphere.transform.localScale = Vector3.one * gridLength / 8;
            sphere.GetComponent<Renderer>().material.color = Color.red;


            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = topLeftFront;
            sphere.transform.localScale = Vector3.one * gridLength / 8;
            sphere.GetComponent<Renderer>().material.color = Color.red;


            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = bottomRightFront;
            sphere.transform.localScale = Vector3.one * gridLength / 8;
            sphere.GetComponent<Renderer>().material.color = Color.red;


            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = bottomRightBack;
            sphere.transform.localScale = Vector3.one * gridLength / 8;
            sphere.GetComponent<Renderer>().material.color = Color.red;


            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = bottomLeftBack;
            sphere.transform.localScale = Vector3.one * gridLength / 8;
            sphere.GetComponent<Renderer>().material.color = Color.red;


            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = bottomLeftFront;
            sphere.transform.localScale = Vector3.one * gridLength / 8;
            sphere.GetComponent<Renderer>().material.color = Color.red;

            // midpoints

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = topCenterFront;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = topCenterBack;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = topCenterRight;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = topCenterLeft;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = rightCenterFront;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = rightCenterBack;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = leftCenterFront;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = leftCenterBack;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = bottomCenterFront;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = bottomCenterBack;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = bottomCenterLeft;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = bottomCenterRight;
            sphere.transform.localScale = Vector3.one * gridLength / 16;
            sphere.GetComponent<Renderer>().material.color = Color.blue;

        }

        mesh.vertices = cubeData.GetVertices();
        mesh.triangles = cubeData.GetTriangles();

        //Assigning the two lists to the mesh filter
        filter.mesh = mesh;
    }

    public static void DrawLine(Vector3 p1, Vector3 p2, float width)
    {
        int count = 1 + Mathf.CeilToInt(width); // how many lines are needed.
        if (count == 1)
        {
            Gizmos.DrawLine(p1, p2);
        }
        else
        {
            Camera c = Camera.current;
            if (c == null)
            {
                Debug.LogError("Camera.current is null");
                return;
            }
            var scp1 = c.WorldToScreenPoint(p1);
            var scp2 = c.WorldToScreenPoint(p2);

            Vector3 v1 = (scp2 - scp1).normalized; // line direction
            Vector3 n = Vector3.Cross(v1, Vector3.forward); // normal vector

            for (int i = 0; i < count; i++)
            {
                Vector3 o = 0.99f * n * width * ((float)i / (count - 1) - 0.5f);
                Vector3 origin = c.ScreenToWorldPoint(scp1 + o);
                Vector3 destiny = c.ScreenToWorldPoint(scp2 + o);
                Gizmos.DrawLine(origin, destiny);
            }
        }
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
        Gizmos.color = Color.black;

        DrawLine(topRightFront, topLeftFront, 3);
        DrawLine(topLeftFront, bottomLeftFront, 3);
        DrawLine(bottomLeftFront, bottomRightFront, 3);
        DrawLine(bottomRightFront, bottomRightBack, 3);
        DrawLine(bottomRightBack, bottomLeftBack, 3);
        DrawLine(bottomLeftBack, topLeftBack, 3);
        DrawLine(topLeftBack, topRightBack, 3);
        DrawLine(topRightBack, bottomRightBack, 3);
        DrawLine(topRightFront, bottomRightFront, 3);
        DrawLine(topRightFront, topRightBack, 3);
        DrawLine(topLeftFront, topLeftBack, 3);
        DrawLine(bottomLeftFront, bottomLeftBack, 3);


        // Gizmos.color = Color.red;

        // Gizmos.DrawSphere(topRightFront, gridLength / 8);
        // Gizmos.DrawSphere(topRightBack, gridLength / 8);
        // Gizmos.DrawSphere(topLeftFront, gridLength / 8);
        // Gizmos.DrawSphere(topLeftBack, gridLength / 8);
        // Gizmos.DrawSphere(bottomLeftBack, gridLength / 8);
        // Gizmos.DrawSphere(bottomLeftFront, gridLength / 8);
        // Gizmos.DrawSphere(bottomRightBack, gridLength / 8);
        // Gizmos.DrawSphere(bottomRightFront, gridLength / 8);

        // Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(topCenterFront, gridLength / 16);
        // Gizmos.DrawSphere(topCenterBack, gridLength / 16);
        // Gizmos.DrawSphere(topCenterRight, gridLength / 16);
        // Gizmos.DrawSphere(topCenterLeft, gridLength / 16);
        // Gizmos.DrawSphere(rightCenterFront, gridLength / 16);
        // Gizmos.DrawSphere(rightCenterBack, gridLength / 16);
        // Gizmos.DrawSphere(leftCenterBack, gridLength / 16);
        // Gizmos.DrawSphere(leftCenterFront, gridLength / 16);
        // Gizmos.DrawSphere(bottomCenterBack, gridLength / 16);
        // Gizmos.DrawSphere(bottomCenterFront, gridLength / 16);
        // Gizmos.DrawSphere(bottomCenterLeft, gridLength / 16);
        // Gizmos.DrawSphere(bottomCenterRight, gridLength / 16);
    }

}
