using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public struct Cube
{

    //Corner Vertices (EdgeEdgeFace)
    private Vector3 topRightFront;
    private Vector3 topLeftFront;
    private Vector3 bottomLeftFront;
    private Vector3 bottomRightFront;
    private Vector3 topRightBack;
    private Vector3 topLeftBack;
    private Vector3 bottomLeftBack;
    private Vector3 bottomRightBack;

    //Midpoint Vertices (EdgeCenterFace)
    private Vector3 topCenterFront;
    private Vector3 topCenterBack;
    private Vector3 rightCenterFront;
    private Vector3 rightCenterBack;
    private Vector3 leftCenterFront;
    private Vector3 leftCenterBack;
    private Vector3 bottomCenterFront;
    private Vector3 bottomCenterBack;
    private Vector3 topCenterRight;
    private Vector3 bottomCenterRight;
    private Vector3 topCenterLeft;
    private Vector3 bottomCenterLeft;

    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uvs;

    public Cube(Vector3 position, float gridScale)
    {

        //Front side positions of the corners
        bottomLeftFront = position - gridScale * Vector3.one / 2;
        bottomRightFront = bottomLeftFront + Vector3.right * gridScale;
        topRightFront = bottomRightFront + Vector3.up * gridScale;
        topLeftFront = topRightFront + Vector3.left * gridScale;

        //Back side positions of the corners
        topLeftBack = topLeftFront + Vector3.forward * gridScale;
        topRightBack = topLeftBack + Vector3.right * gridScale;
        bottomRightBack = topRightBack + Vector3.down * gridScale;
        bottomLeftBack = bottomRightBack + Vector3.left * gridScale;

        //Midpoints positions of the front side
        topCenterFront = topLeftFront + Vector3.right / 2 * gridScale;
        rightCenterFront = topRightFront + Vector3.down / 2 * gridScale;
        bottomCenterFront = bottomRightFront + Vector3.left / 2 * gridScale;
        leftCenterFront = bottomLeftFront + Vector3.up / 2 * gridScale;

        //Midpoints positions of the back side
        topCenterBack = topLeftBack + Vector3.right / 2 * gridScale;
        rightCenterBack = topRightBack + Vector3.down / 2 * gridScale;
        bottomCenterBack = bottomRightBack + Vector3.left / 2 * gridScale;
        leftCenterBack = bottomLeftBack + Vector3.up / 2 * gridScale;

        //Midpoints positions of the middle bridges between the front and back sides
        topCenterRight = topRightFront + Vector3.forward / 2 * gridScale;
        topCenterLeft = topLeftFront + Vector3.forward / 2 * gridScale;
        bottomCenterRight = bottomRightFront + Vector3.forward / 2 * gridScale;
        bottomCenterLeft = bottomLeftFront + Vector3.forward / 2 * gridScale;

        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

    }

    public void TriangulateWithInterpolation(float isoValue, float[] cornerValues)
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

        int configuration = GetConfiguration(isoValue, cornerValues);
        Interpolate(isoValue, cornerValues);
        TrianglesLookUp(configuration);

        // Debug.Log($"Configuration = {configuration}");
    }

    private int GetConfiguration(float isoValue, float[] cornerValues)
    {
        int configuration = 0;

        if (cornerValues[0] > isoValue)
            configuration = configuration | (1 << 0);

        if (cornerValues[1] > isoValue)
            configuration = configuration | (1 << 1);

        if (cornerValues[2] > isoValue)
            configuration = configuration | (1 << 2);

        if (cornerValues[3] > isoValue)
            configuration = configuration | (1 << 3);

        if (cornerValues[4] > isoValue)
            configuration = configuration | (1 << 4);

        if (cornerValues[5] > isoValue)
            configuration = configuration | (1 << 5);

        if (cornerValues[6] > isoValue)
            configuration = configuration | (1 << 6);

        if (cornerValues[7] > isoValue)
            configuration = configuration | (1 << 7);

        return configuration;
    }

    private void Interpolate(float isoValue, float[] cornerValues)
    {
        float topCenterFrontLerp = Mathf.InverseLerp(cornerValues[1], cornerValues[0], isoValue);
        topCenterFront = topLeftFront + (topRightFront - topLeftFront) * topCenterFrontLerp;

        float rightCenterFrontLerp = Mathf.InverseLerp(cornerValues[3], cornerValues[0], isoValue);
        rightCenterFront = bottomRightFront + (topRightFront - bottomRightFront) * rightCenterFrontLerp;

        float bottomCenterFrontLerp = Mathf.InverseLerp(cornerValues[2], cornerValues[3], isoValue);
        bottomCenterFront = bottomLeftFront + (bottomRightFront - bottomLeftFront) * bottomCenterFrontLerp;

        float leftCenterFrontLerp = Mathf.InverseLerp(cornerValues[2], cornerValues[1], isoValue);
        leftCenterFront = bottomLeftFront + (topLeftFront - bottomLeftFront) * leftCenterFrontLerp;

        float topCenterBackLerp = Mathf.InverseLerp(cornerValues[5], cornerValues[4], isoValue);
        topCenterBack = topLeftBack + (topRightBack - topLeftBack) * topCenterBackLerp;

        float rightCenterBackLerp = Mathf.InverseLerp(cornerValues[7], cornerValues[4], isoValue);
        rightCenterBack = bottomRightBack + (topRightBack - bottomRightBack) * rightCenterBackLerp;

        float bottomCenterBackLerp = Mathf.InverseLerp(cornerValues[6], cornerValues[7], isoValue);
        bottomCenterBack = bottomLeftBack + (bottomRightBack - bottomLeftBack) * bottomCenterBackLerp;

        float leftCenterBackLerp = Mathf.InverseLerp(cornerValues[6], cornerValues[5], isoValue);
        leftCenterBack = bottomLeftBack + (topLeftBack - bottomLeftBack) * leftCenterBackLerp;

        float topCenterRightLerp = Mathf.InverseLerp(cornerValues[0], cornerValues[4], isoValue);
        topCenterRight = topRightFront + (topRightBack - topRightFront) * topCenterRightLerp;

        float bottomCenterRightLerp = Mathf.InverseLerp(cornerValues[3], cornerValues[7], isoValue);
        bottomCenterRight = bottomRightFront + (bottomRightBack - bottomRightFront) * bottomCenterRightLerp;

        float topCenterLeftLerp = Mathf.InverseLerp(cornerValues[1], cornerValues[5], isoValue);
        topCenterLeft = topLeftFront + (topLeftBack - topLeftFront) * topCenterLeftLerp;

        float bottomCenterLeftLerp = Mathf.InverseLerp(cornerValues[2], cornerValues[6], isoValue);
        bottomCenterLeft = bottomLeftFront + (bottomLeftBack - bottomLeftFront) * bottomCenterLeftLerp;

    }
    //NEW WAY
   
    private void TrianglesLookUp(int configuration)

    {

        switch (configuration)
        {
            case 0:
                break;

            case 1:
                vertices.AddRange(new Vector3[] { rightCenterFront, topCenterRight, topCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterFront, topCenterRight, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 2:
                vertices.AddRange(new Vector3[] { leftCenterFront, topCenterFront, topCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterFront, topCenterFront, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 3:
                vertices.AddRange(new Vector3[] { leftCenterFront, rightCenterFront, topCenterRight, topCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterFront, rightCenterFront, topCenterRight, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2 });
                break;

            case 4:
                vertices.AddRange(new Vector3[] { leftCenterFront, bottomCenterLeft, bottomCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterFront, bottomCenterLeft, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 5:
                vertices.AddRange(new Vector3[] { topCenterFront, rightCenterFront, topCenterRight, bottomCenterFront, leftCenterFront, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterFront, rightCenterFront, topCenterRight, bottomCenterFront, leftCenterFront, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 1, 0, 4, 3, 0, 5, 3, 4 });
                break;

            case 6:
                vertices.AddRange(new Vector3[] { bottomCenterLeft, bottomCenterFront, topCenterFront, topCenterLeft, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { bottomCenterLeft, bottomCenterFront, topCenterFront, topCenterLeft, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2 });
                break;

            case 7:
                vertices.AddRange(new Vector3[] { bottomCenterFront, rightCenterFront, topCenterRight, bottomCenterLeft, topCenterLeft });
                uvs.AddRange(new Vector2[] { bottomCenterFront, rightCenterFront, topCenterRight, bottomCenterLeft, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 8:
                vertices.AddRange(new Vector3[] { bottomCenterFront, bottomCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterFront, bottomCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 9:
                vertices.AddRange(new Vector3[] { topCenterFront, bottomCenterRight, topCenterRight, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterFront, bottomCenterRight, topCenterRight, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1 });
                break;

            case 10:
                vertices.AddRange(new Vector3[] { leftCenterFront, topCenterFront, topCenterLeft, bottomCenterFront, rightCenterFront, bottomCenterRight });
                uvs.AddRange(new Vector2[] { leftCenterFront, topCenterFront, topCenterLeft, bottomCenterFront, rightCenterFront, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 1, 3, 4, 3, 5, 4 });
                break;

            case 11:
                vertices.AddRange(new Vector3[] { bottomCenterFront, bottomCenterRight, topCenterRight, topCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterFront, bottomCenterRight, topCenterRight, topCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 0, 3 });
                break;

            case 12:
                vertices.AddRange(new Vector3[] { bottomCenterLeft, bottomCenterRight, rightCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterLeft, bottomCenterRight, rightCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2 });
                break;

            case 13:
                vertices.AddRange(new Vector3[] { bottomCenterLeft, bottomCenterRight, topCenterRight, topCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterLeft, bottomCenterRight, topCenterRight, topCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 0, 3 });
                break;

            case 14:
                vertices.AddRange(new Vector3[] { bottomCenterRight, rightCenterFront, topCenterFront, bottomCenterLeft, topCenterLeft });
                uvs.AddRange(new Vector2[] { bottomCenterRight, rightCenterFront, topCenterFront, bottomCenterLeft, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 15:
                vertices.AddRange(new Vector3[] { bottomCenterLeft, bottomCenterRight, topCenterLeft, topCenterRight });
                uvs.AddRange(new Vector2[] { bottomCenterLeft, bottomCenterRight, topCenterLeft, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 2, 1 });
                break;

            case 16:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, topCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, topCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 17:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterFront, rightCenterFront, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterFront, rightCenterFront, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 18:
                vertices.AddRange(new Vector3[] { topCenterLeft, leftCenterFront, topCenterFront, topCenterBack, topCenterRight, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, leftCenterFront, topCenterFront, topCenterBack, topCenterRight, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 3, 0, 2, 4, 3, 4, 5, 3 });
                break;

            case 19:
                vertices.AddRange(new Vector3[] { rightCenterBack, leftCenterFront, rightCenterFront, topCenterBack, topCenterLeft });
                uvs.AddRange(new Vector2[] { rightCenterBack, leftCenterFront, rightCenterFront, topCenterBack, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 1, 0, 4, 1, 3 });
                break;

            case 20:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, topCenterBack, leftCenterFront, bottomCenterLeft, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, topCenterBack, leftCenterFront, bottomCenterLeft, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5 });
                break;

            case 21:
                vertices.AddRange(new Vector3[] { bottomCenterLeft, bottomCenterFront, leftCenterFront, topCenterFront, rightCenterFront, topCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { bottomCenterLeft, bottomCenterFront, leftCenterFront, topCenterFront, rightCenterFront, topCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 2, 1, 3, 1, 4, 4, 5, 3, 4, 6, 5 });
                break;

            case 22:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, topCenterBack, topCenterFront, bottomCenterLeft, bottomCenterFront, topCenterLeft, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, topCenterBack, topCenterFront, bottomCenterLeft, bottomCenterFront, topCenterLeft, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5, 3, 6, 4, 2, 6, 3, 0, 2, 3 });
                break;

            case 23:
                vertices.AddRange(new Vector3[] { rightCenterFront, rightCenterBack, topCenterBack, bottomCenterFront, topCenterLeft, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { rightCenterFront, rightCenterBack, topCenterBack, bottomCenterFront, topCenterLeft, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 2, 4, 3, 4, 5, 3 });
                break;

            case 24:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterRight, rightCenterBack, bottomCenterRight, rightCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterRight, rightCenterBack, bottomCenterRight, rightCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 1, 4, 3, 4, 5, 3 });
                break;

            case 25:
                vertices.AddRange(new Vector3[] { bottomCenterRight, rightCenterBack, topCenterBack, bottomCenterFront, topCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterRight, rightCenterBack, topCenterBack, bottomCenterFront, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 2, 4, 3 });
                break;

            case 26:
                vertices.AddRange(new Vector3[] { topCenterLeft, leftCenterFront, topCenterFront, bottomCenterFront, rightCenterFront, bottomCenterRight, topCenterRight, rightCenterBack, topCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, leftCenterFront, topCenterFront, bottomCenterFront, rightCenterFront, bottomCenterRight, topCenterRight, rightCenterBack, topCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 3, 4, 2, 4, 3, 5, 4, 5, 6, 5, 7, 6, 6, 7, 8 });
                break;

            case 27:
                vertices.AddRange(new Vector3[] { bottomCenterRight, rightCenterBack, topCenterBack, bottomCenterFront, topCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterRight, rightCenterBack, topCenterBack, bottomCenterFront, topCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2, 4, 5, 3 });
                break;

            case 28:
                vertices.AddRange(new Vector3[] { rightCenterBack, topCenterBack, topCenterRight, bottomCenterRight, rightCenterFront, leftCenterFront, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { rightCenterBack, topCenterBack, topCenterRight, bottomCenterRight, rightCenterFront, leftCenterFront, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 3, 0, 2, 4, 3, 5, 6, 4, 4, 6, 3 });
                break;

            case 29://Problem
                vertices.AddRange(new Vector3[] { bottomCenterRight, rightCenterBack, topCenterBack, bottomCenterFront, topCenterFront, bottomCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterRight, rightCenterBack, topCenterBack, bottomCenterFront, topCenterFront, bottomCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2, 5, 3, 6, 4, 6, 3 });
                break;

            case 30:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, topCenterBack, bottomCenterRight, rightCenterFront, bottomCenterLeft, topCenterFront, topCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, topCenterBack, bottomCenterRight, rightCenterFront, bottomCenterLeft, topCenterFront, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 4, 3, 0, 5, 3, 6, 5, 6, 7, 6, 3, 4 });
                break;

            case 31:
                vertices.AddRange(new Vector3[] { bottomCenterRight, rightCenterBack, topCenterBack, bottomCenterLeft, topCenterLeft });
                uvs.AddRange(new Vector2[] { bottomCenterRight, rightCenterBack, topCenterBack, bottomCenterLeft, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 32:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 33:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, topCenterLeft, topCenterFront, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, topCenterLeft, topCenterFront, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 4, 0, 3, 5, 4 });
                break;

            case 34:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, topCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, topCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3 });
                break;

            case 35:
                vertices.AddRange(new Vector3[] { topCenterRight, leftCenterFront, rightCenterFront, topCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, leftCenterFront, rightCenterFront, topCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 3, 4, 1 });
                break;

            case 36:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, topCenterLeft, bottomCenterLeft, leftCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, topCenterLeft, bottomCenterLeft, leftCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5 });
                break;

            case 37:
                vertices.AddRange(new Vector3[] { topCenterFront, rightCenterFront, topCenterRight, bottomCenterFront, leftCenterFront, bottomCenterLeft, topCenterLeft, leftCenterBack, topCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, rightCenterFront, topCenterRight, bottomCenterFront, leftCenterFront, bottomCenterLeft, topCenterLeft, leftCenterBack, topCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 3, 4, 5, 3, 6, 5, 4, 6, 7, 5, 8, 7, 6 });
                break;

            case 38:
                vertices.AddRange(new Vector3[] { topCenterFront, bottomCenterLeft, bottomCenterFront, topCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, bottomCenterLeft, bottomCenterFront, topCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 3, 4, 1 });
                break;

            case 39:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, bottomCenterLeft, bottomCenterFront, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, bottomCenterLeft, bottomCenterFront, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 4, 3, 5 });
                break;

            case 40:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, topCenterLeft, rightCenterFront, bottomCenterFront, bottomCenterRight });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, topCenterLeft, rightCenterFront, bottomCenterFront, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5 });
                break;

            case 41:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, topCenterLeft, topCenterFront, topCenterRight, bottomCenterFront, bottomCenterRight });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, topCenterLeft, topCenterFront, topCenterRight, bottomCenterFront, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 4, 0, 3, 5, 4, 5, 6, 4 });
                break;

            case 42:
                vertices.AddRange(new Vector3[] { rightCenterFront, bottomCenterFront, bottomCenterRight, topCenterFront, leftCenterFront, topCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { rightCenterFront, bottomCenterFront, bottomCenterRight, topCenterFront, leftCenterFront, topCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 1, 0, 3, 4, 1, 4, 5, 6, 4, 3, 5 });
                break;

            case 43:
                vertices.AddRange(new Vector3[] { leftCenterFront, bottomCenterFront, bottomCenterRight, topCenterRight, topCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterFront, bottomCenterFront, bottomCenterRight, topCenterRight, topCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 0, 3, 5, 0, 4 });
                break;

            case 44:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, topCenterLeft, bottomCenterLeft, leftCenterFront, rightCenterFront, bottomCenterRight });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, topCenterLeft, bottomCenterLeft, leftCenterFront, rightCenterFront, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 4, 2, 3, 4, 3, 5, 5, 3, 6 });
                break;

            case 45:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, topCenterLeft, topCenterFront, leftCenterFront, bottomCenterLeft, topCenterRight, bottomCenterRight });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, topCenterLeft, topCenterFront, leftCenterFront, bottomCenterLeft, topCenterRight, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5, 3, 5, 6, 6, 5, 7 });
                break;

            case 46:
                vertices.AddRange(new Vector3[] { topCenterFront, bottomCenterRight, rightCenterFront, bottomCenterLeft, topCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, bottomCenterRight, rightCenterFront, bottomCenterLeft, topCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 4, 3, 0, 4, 5, 3 });
                break;

            case 47:
                vertices.AddRange(new Vector3[] { topCenterRight, bottomCenterLeft, bottomCenterRight, topCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, bottomCenterLeft, bottomCenterRight, topCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 1, 0, 3, 4, 1 });
                break;

            case 48:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterRight, rightCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterRight, rightCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 49:
                vertices.AddRange(new Vector3[] { topCenterFront, rightCenterFront, rightCenterBack, topCenterLeft, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, rightCenterFront, rightCenterBack, topCenterLeft, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4 });
                break;

            case 50:
                vertices.AddRange(new Vector3[] { leftCenterFront, rightCenterBack, leftCenterBack, topCenterFront, topCenterRight });
                uvs.AddRange(new Vector2[] { leftCenterFront, rightCenterBack, leftCenterBack, topCenterFront, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 1, 0, 3, 4, 1 });
                break;

            case 51:
                vertices.AddRange(new Vector3[] { rightCenterBack, leftCenterBack, rightCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, leftCenterBack, rightCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2 });
                break;

            case 52:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, leftCenterBack, topCenterLeft, bottomCenterLeft, leftCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, leftCenterBack, topCenterLeft, bottomCenterLeft, leftCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4, 3, 4, 5, 5, 4, 6 });
                break;

            case 53:
                vertices.AddRange(new Vector3[] { leftCenterFront, bottomCenterLeft, bottomCenterFront, topCenterFront, rightCenterFront, rightCenterBack, topCenterLeft, leftCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterFront, bottomCenterLeft, bottomCenterFront, topCenterFront, rightCenterFront, rightCenterBack, topCenterLeft, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5, 3, 5, 6, 6, 5, 7 });
                break;

            case 54:
                vertices.AddRange(new Vector3[] { rightCenterBack, leftCenterBack, topCenterRight, topCenterFront, bottomCenterLeft, bottomCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, leftCenterBack, topCenterRight, topCenterFront, bottomCenterLeft, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 1, 4, 3, 3, 4, 5 });
                break;

            case 55:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterFront, rightCenterFront, bottomCenterLeft, leftCenterBack });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterFront, rightCenterFront, bottomCenterLeft, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 3 });
                break;

            case 56:
                vertices.AddRange(new Vector3[] { topCenterRight, leftCenterBack, topCenterLeft, rightCenterBack, rightCenterFront, bottomCenterRight, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, leftCenterBack, topCenterLeft, rightCenterBack, rightCenterFront, bottomCenterRight, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 5, 3, 0, 5, 4, 6, 5 });
                break;

            case 57:
                vertices.AddRange(new Vector3[] { topCenterFront, bottomCenterFront, bottomCenterRight, rightCenterBack, topCenterLeft, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, bottomCenterFront, bottomCenterRight, rightCenterBack, topCenterLeft, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 4, 3, 5 });
                break;

            case 58:
                vertices.AddRange(new Vector3[] { topCenterFront, leftCenterBack, leftCenterFront, topCenterRight, rightCenterBack, bottomCenterRight, rightCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterFront, leftCenterBack, leftCenterFront, topCenterRight, rightCenterBack, bottomCenterRight, rightCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 3, 4, 1, 3, 5, 4, 3, 6, 5, 6, 7, 5 });
                break;

            case 59:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterRight, rightCenterBack, bottomCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterRight, rightCenterBack, bottomCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 3 });
                break;

            case 60:
                vertices.AddRange(new Vector3[] { topCenterLeft, leftCenterBack, bottomCenterLeft, leftCenterFront, rightCenterFront, bottomCenterRight, topCenterRight, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, leftCenterBack, bottomCenterLeft, leftCenterFront, rightCenterFront, bottomCenterRight, topCenterRight, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 2, 4, 3, 4, 2, 5, 0, 6, 1, 1, 6, 7, 6, 5, 7, 6, 4, 5 });
                break;

            case 61: //Problem
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterRight, rightCenterFront, rightCenterBack, bottomCenterRight, leftCenterBack, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterRight, rightCenterFront, rightCenterBack, bottomCenterRight, leftCenterBack, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 4, 1, 4, 2, 4, 3, 5, 6, 4, 5 });
                break;

            case 62: //Problem
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterLeft, rightCenterBack, bottomCenterRight, topCenterLeft, leftCenterFront, topCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterLeft, rightCenterBack, bottomCenterRight, topCenterLeft, leftCenterFront, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 4, 1, 0, 4, 5, 1, 4, 6, 5 });
                break;

            case 63:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterRight, rightCenterBack, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterRight, rightCenterBack, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1 });
                break;

            case 64:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 65:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, topCenterFront, rightCenterFront, topCenterRight });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, topCenterFront, rightCenterFront, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5 });
                break;

            case 66:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, topCenterLeft, leftCenterFront, topCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, topCenterLeft, leftCenterFront, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 3, 4, 5 });
                break;

            case 67:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, topCenterLeft, leftCenterFront, rightCenterFront, topCenterRight });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, topCenterLeft, leftCenterFront, rightCenterFront, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 3, 4, 5, 3, 5, 6 });
                break;

            case 68:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 69:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterFront, leftCenterFront, topCenterFront, rightCenterFront, topCenterRight });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterFront, leftCenterFront, topCenterFront, rightCenterFront, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 3, 2, 4, 2, 5, 5, 6, 4 });
                break;

            case 70:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, topCenterLeft, topCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, topCenterLeft, topCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 2, 1, 3, 1, 4 });
                break;

            case 71:
                vertices.AddRange(new Vector3[] { topCenterRight, bottomCenterFront, rightCenterFront, bottomCenterBack, leftCenterBack, topCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterRight, bottomCenterFront, rightCenterFront, bottomCenterBack, leftCenterBack, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 3, 0, 5, 4 });
                break;

            case 72:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 1, 4, 3, 5, 3, 4 });
                break;

            case 73:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, topCenterRight, topCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, topCenterRight, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 1, 4, 3, 5, 3, 4, 5, 6, 3 });
                break;

            case 74:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, rightCenterFront, topCenterFront, leftCenterFront, topCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, rightCenterFront, topCenterFront, leftCenterFront, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 1, 4, 3, 5, 3, 4, 6, 3, 5, 6, 7, 3, 8, 7, 6 });
                break;

            case 75:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, topCenterRight, leftCenterFront, topCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, topCenterRight, leftCenterFront, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 1, 4, 3, 5, 3, 4, 5, 6, 3, 5, 7, 6 });
                break;

            case 76:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterRight, leftCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterRight, leftCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 77:
                vertices.AddRange(new Vector3[] { topCenterRight, bottomCenterBack, topCenterRight, topCenterFront, leftCenterFront, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, bottomCenterBack, topCenterRight, topCenterFront, leftCenterFront, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 3, 4, 1, 4, 5, 1 });
                break;

            case 78:
                vertices.AddRange(new Vector3[] { topCenterFront, bottomCenterRight, rightCenterFront, bottomCenterBack, topCenterLeft, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, bottomCenterRight, rightCenterFront, bottomCenterBack, topCenterLeft, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 3, 4, 5, 3 });
                break;

            case 79:
                vertices.AddRange(new Vector3[] { topCenterRight, bottomCenterBack, bottomCenterRight, topCenterLeft, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, bottomCenterBack, bottomCenterRight, topCenterLeft, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 3, 4, 1 });
                break;

            case 80:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, topCenterBack, bottomCenterBack, leftCenterBack, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, topCenterBack, bottomCenterBack, leftCenterBack, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5 });
                break;

            case 81:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, bottomCenterLeft, topCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, bottomCenterLeft, topCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4, 5, 6, 0, 0, 6, 1 });
                break;

            case 82:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, bottomCenterLeft, topCenterLeft, leftCenterFront, topCenterFront, topCenterRight });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, bottomCenterLeft, topCenterLeft, leftCenterFront, topCenterFront, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4, 5, 3, 4, 5, 4, 6, 7, 5, 6, 0, 8, 1 });
                break;

            case 83:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterLeft, leftCenterBack, bottomCenterBack, rightCenterBack, rightCenterFront, bottomCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterLeft, leftCenterBack, bottomCenterBack, rightCenterBack, rightCenterFront, bottomCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 5, 4, 3, 5, 3, 6, 5, 6, 7 });
                break;

            case 84:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, topCenterBack, bottomCenterBack, leftCenterBack, bottomCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, topCenterBack, bottomCenterBack, leftCenterBack, bottomCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 3, 5, 4, 6, 4, 5 });
                break;

            case 85:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, topCenterFront, leftCenterFront, bottomCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, topCenterFront, leftCenterFront, bottomCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 6, 3, 2, 6, 5, 3, 4, 7, 1, 4, 1, 0 });
                break;

            case 86:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, topCenterRight, topCenterLeft, topCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, topCenterRight, topCenterLeft, topCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 4, 1, 5, 3, 2, 6, 5, 2, 7, 6, 2 });
                break;

            case 87:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterLeft, leftCenterBack, bottomCenterBack, rightCenterBack, bottomCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterLeft, leftCenterBack, bottomCenterBack, rightCenterBack, bottomCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 4, 3, 5, 6, 4, 5 });
                break;

            case 88:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterRight, bottomCenterFront, rightCenterFront, topCenterRight, rightCenterBack, topCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterRight, bottomCenterFront, rightCenterFront, topCenterRight, rightCenterBack, topCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 3, 4, 3, 2, 1, 5, 4, 3, 6, 5, 3, 6, 3, 7, 8, 6, 7 });
                break;

            case 89:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterRight, bottomCenterFront, topCenterFront, topCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterRight, bottomCenterFront, topCenterFront, topCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 4, 2, 1, 4, 5, 4, 6, 6, 4, 3, 6, 3, 7 });
                break;

            case 90:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, topCenterBack, rightCenterBack, topCenterLeft, leftCenterFront, topCenterRight, topCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, topCenterBack, rightCenterBack, topCenterLeft, leftCenterFront, topCenterRight, topCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 1, 4, 3, 5, 6, 1, 5, 1, 0, 7, 0, 2, 7, 2, 8, 5, 9, 6, 10, 7, 8, 10, 9, 5, 10, 5, 7, 10, 3, 11, 10, 8, 3, 9, 11, 4, 9, 4, 6, 11, 3, 4 });
                break;

            case 91:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, topCenterLeft, leftCenterFront, topCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterFront, bottomCenterRight, topCenterLeft, leftCenterFront, topCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 1, 4, 3, 5, 6, 3, 5, 3, 4, 5, 4, 7, 7, 4, 8 });
                break;

            case 92:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterRight, rightCenterBack, bottomCenterBack, leftCenterBack, rightCenterFront, bottomCenterRight, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterRight, rightCenterBack, bottomCenterBack, leftCenterBack, rightCenterFront, bottomCenterRight, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 1, 5, 6, 1, 6, 2, 6, 5, 7, 6, 7, 3, 3, 7, 4 });
                break;

            case 93:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterBack, bottomCenterRight, topCenterBack, leftCenterBack, topCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterBack, bottomCenterRight, topCenterBack, leftCenterBack, topCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 1, 0, 3, 4, 1, 3, 5, 4, 4, 5, 6 });
                break;

            case 94:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterRight, rightCenterBack, topCenterFront, bottomCenterRight, bottomCenterBack, leftCenterBack, topCenterLeft, topCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterRight, rightCenterBack, topCenterFront, bottomCenterRight, bottomCenterBack, leftCenterBack, topCenterLeft, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 4, 1, 4, 2, 0, 2, 5, 0, 5, 6, 7, 6, 5, 8, 7, 5, 8, 5, 4, 8, 4, 3 });
                break;

            case 95:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterLeft, leftCenterBack, bottomCenterBack, rightCenterBack, bottomCenterRight });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterLeft, leftCenterBack, bottomCenterBack, rightCenterBack, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 4, 3, 5 });
                break;

            case 96:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterBack, bottomCenterBack, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterBack, bottomCenterBack, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 97:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterBack, bottomCenterBack, bottomCenterLeft, topCenterFront, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterBack, bottomCenterBack, bottomCenterLeft, topCenterFront, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 1, 4, 1, 0, 5, 4, 6 });
                break;

            case 98:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterBack, bottomCenterBack, bottomCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterBack, bottomCenterBack, bottomCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 99:
                vertices.AddRange(new Vector3[] { topCenterRight, leftCenterFront, rightCenterFront, bottomCenterLeft, bottomCenterBack, topCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, leftCenterFront, rightCenterFront, bottomCenterLeft, bottomCenterBack, topCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 3, 0, 5, 4 });
                break;

            case 100:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterBack, bottomCenterBack, bottomCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterBack, bottomCenterBack, bottomCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 101:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterBack, bottomCenterBack, leftCenterFront, bottomCenterFront, topCenterFront, rightCenterFront, topCenterRight });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterBack, bottomCenterBack, leftCenterFront, bottomCenterFront, topCenterFront, rightCenterFront, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2, 5, 3, 4, 5, 4, 6, 7, 5, 6 });
                break;

            case 102:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterBack, bottomCenterBack, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterBack, bottomCenterBack, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 103:
                vertices.AddRange(new Vector3[] { rightCenterFront, topCenterRight, bottomCenterFront, bottomCenterBack, topCenterBack });
                uvs.AddRange(new Vector2[] { rightCenterFront, topCenterRight, bottomCenterFront, bottomCenterBack, topCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 1, 4, 3 });
                break;

            case 104:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterRight, bottomCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterBack, bottomCenterBack, bottomCenterLeft, bottomCenterRight, bottomCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 2, 4, 5, 2, 5, 3, 6, 5, 4 });
                break;

            case 105:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterBack, bottomCenterBack, bottomCenterLeft, topCenterRight, topCenterFront, bottomCenterRight, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterBack, bottomCenterBack, bottomCenterLeft, topCenterRight, topCenterFront, bottomCenterRight, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 6, 5, 7, 6, 1, 0, 5, 1, 5, 4, 2, 7, 3, 2, 6, 7 });
                break;

            case 106:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterBack, bottomCenterBack, bottomCenterLeft, leftCenterFront, bottomCenterRight, bottomCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterBack, bottomCenterBack, bottomCenterLeft, leftCenterFront, bottomCenterRight, bottomCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 2, 5, 6, 2, 6, 3, 7, 6, 5 });
                break;

            case 107:
                vertices.AddRange(new Vector3[] { topCenterRight, bottomCenterFront, bottomCenterRight, leftCenterFront, topCenterBack, bottomCenterLeft, bottomCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, bottomCenterFront, bottomCenterRight, leftCenterFront, topCenterBack, bottomCenterLeft, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 3, 3, 4, 5, 4, 6, 5 });
                break;

            case 108:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterBack, leftCenterFront, bottomCenterBack, rightCenterFront, bottomCenterRight });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterBack, leftCenterFront, bottomCenterBack, rightCenterFront, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 4, 3, 5 });
                break;

            case 109:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterFront, bottomCenterRight, leftCenterFront, bottomCenterBack, topCenterLeft, topCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterFront, bottomCenterRight, leftCenterFront, bottomCenterBack, topCenterLeft, topCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 3, 4, 2, 3, 5, 4, 5, 6, 4 });
                break;

            case 110:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterBack, bottomCenterBack, bottomCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterBack, bottomCenterBack, bottomCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 111:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterBack, bottomCenterBack, bottomCenterRight });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterBack, bottomCenterBack, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 112:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, bottomCenterBack, topCenterLeft, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, bottomCenterBack, topCenterLeft, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4 });
                break;

            case 113:
                vertices.AddRange(new Vector3[] { topCenterFront, rightCenterFront, rightCenterBack, bottomCenterBack, topCenterLeft, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterFront, rightCenterFront, rightCenterBack, bottomCenterBack, topCenterLeft, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 4, 3, 5 });
                break;

            case 114:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, bottomCenterBack, topCenterFront, bottomCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, bottomCenterBack, topCenterFront, bottomCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 3, 4, 5 });
                break;

            case 115:
                vertices.AddRange(new Vector3[] { leftCenterFront, rightCenterFront, rightCenterBack, bottomCenterBack, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterFront, rightCenterFront, rightCenterBack, bottomCenterBack, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 116:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, bottomCenterBack, topCenterLeft, bottomCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, bottomCenterBack, topCenterLeft, bottomCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4, 3, 4, 5 });
                break;

            case 117:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterFront, leftCenterFront, rightCenterFront, bottomCenterFront, bottomCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterFront, leftCenterFront, rightCenterFront, bottomCenterFront, bottomCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 2, 3, 4, 3, 5, 4, 3, 6, 5 });
                break;

            case 118:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, bottomCenterBack, topCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, bottomCenterBack, topCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4 });
                break;

            case 119:
                vertices.AddRange(new Vector3[] { rightCenterFront, rightCenterBack, bottomCenterBack, bottomCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterFront, rightCenterBack, bottomCenterBack, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 120:
                vertices.AddRange(new Vector3[] { rightCenterFront, bottomCenterFront, bottomCenterRight, bottomCenterBack, bottomCenterLeft, topCenterRight, rightCenterBack, topCenterLeft });
                uvs.AddRange(new Vector2[] { rightCenterFront, bottomCenterFront, bottomCenterRight, bottomCenterBack, bottomCenterLeft, topCenterRight, rightCenterBack, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 2, 1, 3, 1, 4, 5, 6, 3, 7, 5, 3, 7, 3, 4 });
                break;

            case 121:
                vertices.AddRange(new Vector3[] { topCenterFront, bottomCenterFront, bottomCenterRight, rightCenterBack, topCenterLeft, bottomCenterBack, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterFront, bottomCenterFront, bottomCenterRight, rightCenterBack, topCenterLeft, bottomCenterBack, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 4, 3, 5, 4, 5, 6 });
                break;

            case 122:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterBack, bottomCenterBack, topCenterFront, bottomCenterLeft, leftCenterFront, bottomCenterRight, bottomCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterBack, bottomCenterBack, topCenterFront, bottomCenterLeft, leftCenterFront, bottomCenterRight, bottomCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 3, 4, 5, 2, 6, 7, 2, 7, 4, 8, 7, 6 });
                break;

            case 123:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterBack, bottomCenterRight, bottomCenterFront, bottomCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterBack, bottomCenterRight, bottomCenterFront, bottomCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 1, 4, 3, 5, 3, 4 });
                break;

            case 124:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterRight, rightCenterFront, leftCenterFront, rightCenterBack, bottomCenterRight, bottomCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterRight, rightCenterFront, leftCenterFront, rightCenterBack, bottomCenterRight, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 1, 4, 5, 1, 5, 2, 4, 6, 5 });
                break;

            case 125:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterBack, bottomCenterRight, topCenterLeft, topCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterBack, bottomCenterRight, topCenterLeft, topCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5 });
                break;

            case 126:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterBack, bottomCenterRight, topCenterRight, rightCenterFront, topCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterBack, bottomCenterRight, topCenterRight, rightCenterFront, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 5, 3, 4 });
                break;

            case 127:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterBack, bottomCenterRight });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterBack, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 128:
                vertices.AddRange(new Vector3[] { bottomCenterRight, bottomCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { bottomCenterRight, bottomCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 129:
                vertices.AddRange(new Vector3[] { bottomCenterRight, bottomCenterBack, rightCenterBack, topCenterRight, rightCenterFront, topCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterRight, bottomCenterBack, rightCenterBack, topCenterRight, rightCenterFront, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 0, 3, 0, 2, 3, 5, 4 });
                break;

            case 130:
                vertices.AddRange(new Vector3[] { bottomCenterRight, bottomCenterBack, rightCenterBack, topCenterFront, topCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterRight, bottomCenterBack, rightCenterBack, topCenterFront, topCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5 });
                break;

            case 131:
                vertices.AddRange(new Vector3[] { bottomCenterRight, bottomCenterBack, rightCenterBack, topCenterRight, rightCenterFront, leftCenterFront, topCenterLeft });
                uvs.AddRange(new Vector2[] { bottomCenterRight, bottomCenterBack, rightCenterBack, topCenterRight, rightCenterFront, leftCenterFront, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 0, 2, 3, 0, 3, 5, 4, 3, 6, 5 });
                break;

            case 132:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterRight, bottomCenterBack, bottomCenterFront, bottomCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterRight, bottomCenterBack, bottomCenterFront, bottomCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 5, 4, 3 });
                break;

            case 133:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterFront, rightCenterFront, leftCenterFront, bottomCenterFront, bottomCenterLeft, bottomCenterBack, bottomCenterRight, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterFront, rightCenterFront, leftCenterFront, bottomCenterFront, bottomCenterLeft, bottomCenterBack, bottomCenterRight, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 2, 3, 4, 3, 5, 4, 4, 5, 6, 4, 6, 7, 8, 7, 6, 0, 2, 7, 0, 7, 8 });
                break;

            case 134:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterRight, bottomCenterBack, bottomCenterFront, bottomCenterLeft, topCenterFront, topCenterLeft });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterRight, bottomCenterBack, bottomCenterFront, bottomCenterLeft, topCenterFront, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 5, 4, 3, 5, 6, 4 });
                break;

            case 135:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterRight, bottomCenterBack, bottomCenterFront, bottomCenterLeft, topCenterRight, rightCenterFront, topCenterLeft });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterRight, bottomCenterBack, bottomCenterFront, bottomCenterLeft, topCenterRight, rightCenterFront, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 5, 3, 6, 5, 7, 3, 7, 4, 3 });
                break;

            case 136:
                vertices.AddRange(new Vector3[] { rightCenterBack, rightCenterFront, bottomCenterBack, bottomCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, rightCenterFront, bottomCenterBack, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3 });
                break;

            case 137:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterFront, bottomCenterFront, bottomCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterFront, bottomCenterFront, bottomCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 138:
                vertices.AddRange(new Vector3[] { rightCenterBack, rightCenterFront, bottomCenterBack, bottomCenterFront, topCenterFront, leftCenterFront, topCenterLeft });
                uvs.AddRange(new Vector2[] { rightCenterBack, rightCenterFront, bottomCenterBack, bottomCenterFront, topCenterFront, leftCenterFront, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 4, 3, 1, 4, 5, 3, 4, 6, 5 });
                break;

            case 139:
                vertices.AddRange(new Vector3[] { topCenterLeft, leftCenterFront, bottomCenterFront, topCenterRight, bottomCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, leftCenterFront, bottomCenterFront, topCenterRight, bottomCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 3, 4, 5 });
                break;

            case 140:
                vertices.AddRange(new Vector3[] { rightCenterBack, rightCenterFront, leftCenterFront, bottomCenterLeft, bottomCenterBack });
                uvs.AddRange(new Vector2[] { rightCenterBack, rightCenterFront, leftCenterFront, bottomCenterLeft, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 141:
                vertices.AddRange(new Vector3[] { topCenterRight, bottomCenterBack, rightCenterBack, bottomCenterLeft, topCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, bottomCenterBack, rightCenterBack, bottomCenterLeft, topCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 3, 4, 5, 3 });
                break;

            case 142:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterLeft, bottomCenterLeft, bottomCenterBack, rightCenterFront, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterLeft, bottomCenterLeft, bottomCenterBack, rightCenterFront, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 0, 3, 5, 4, 3 });
                break;

            case 143:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterLeft, bottomCenterLeft, bottomCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterLeft, bottomCenterLeft, bottomCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 144:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterBack });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 145:
                vertices.AddRange(new Vector3[] { topCenterBack, bottomCenterRight, bottomCenterBack, topCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, bottomCenterRight, bottomCenterBack, topCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 3, 4, 1 });
                break;

            case 146:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterBack, topCenterFront, topCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterBack, topCenterFront, topCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 1, 0, 4, 0, 5 });
                break;

            case 147:
                vertices.AddRange(new Vector3[] { topCenterLeft, leftCenterFront, rightCenterFront, bottomCenterRight, topCenterBack, bottomCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, leftCenterFront, rightCenterFront, bottomCenterRight, topCenterBack, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 4, 3, 5 });
                break;

            case 148:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterBack, bottomCenterFront, bottomCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterBack, bottomCenterFront, bottomCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4, 3, 4, 5, 6, 5, 4 });
                break;

            case 149:
                vertices.AddRange(new Vector3[] { bottomCenterBack, bottomCenterRight, bottomCenterFront, bottomCenterLeft, leftCenterFront, topCenterBack, topCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterBack, bottomCenterRight, bottomCenterFront, bottomCenterLeft, leftCenterFront, topCenterBack, topCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 3, 2, 5, 1, 0, 5, 6, 1, 6, 7, 1 });
                break;

            case 150:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterBack, topCenterFront, bottomCenterLeft, bottomCenterFront, topCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterBack, topCenterFront, bottomCenterLeft, bottomCenterFront, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 7, 5, 4, 1, 0, 4, 0, 7, 3, 2, 6, 3, 6, 5 });
                break;

            case 151:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterLeft, bottomCenterLeft, bottomCenterBack, bottomCenterFront, bottomCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterLeft, bottomCenterLeft, bottomCenterBack, bottomCenterFront, bottomCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4, 3, 4, 5, 6, 5, 4 });
                break;

            case 152:
                vertices.AddRange(new Vector3[] { topCenterBack, bottomCenterFront, bottomCenterBack, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, bottomCenterFront, bottomCenterBack, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 3, 4, 1 });
                break;

            case 153:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterFront, bottomCenterFront, bottomCenterBack });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterFront, bottomCenterFront, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 154:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterLeft, leftCenterFront, topCenterRight, topCenterBack, bottomCenterFront, rightCenterFront, bottomCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterLeft, leftCenterFront, topCenterRight, topCenterBack, bottomCenterFront, rightCenterFront, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 4, 0, 4, 1, 4, 3, 5, 3, 6, 5, 4, 5, 7 });
                break;

            case 155:
                vertices.AddRange(new Vector3[] { topCenterBack, bottomCenterFront, bottomCenterBack, topCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, bottomCenterFront, bottomCenterBack, topCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 3, 4, 1 });
                break;

            case 156:
                vertices.AddRange(new Vector3[] { rightCenterFront, leftCenterFront, bottomCenterLeft, bottomCenterBack, topCenterRight, topCenterBack });
                uvs.AddRange(new Vector2[] { rightCenterFront, leftCenterFront, bottomCenterLeft, bottomCenterBack, topCenterRight, topCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 4, 3, 5 });
                break;

            case 157:
                vertices.AddRange(new Vector3[] { bottomCenterBack, topCenterBack, bottomCenterLeft, topCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterBack, topCenterBack, bottomCenterLeft, topCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 3, 4, 2 });
                break;

            case 158:
                vertices.AddRange(new Vector3[] { topCenterLeft, bottomCenterLeft, topCenterBack, bottomCenterBack, topCenterRight, topCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, bottomCenterLeft, topCenterBack, bottomCenterBack, topCenterRight, topCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 2, 4, 0, 4, 5, 0, 5, 4, 6 });
                break;

            case 159:
                vertices.AddRange(new Vector3[] { topCenterLeft, bottomCenterLeft, topCenterBack, bottomCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, bottomCenterLeft, topCenterBack, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3 });
                break;

            case 160:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, topCenterLeft });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 5, 3, 4 });
                break;

            case 161:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, topCenterLeft, topCenterRight, rightCenterFront, topCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, topCenterLeft, topCenterRight, rightCenterFront, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 5, 3, 4, 6, 7, 1, 6, 1, 0, 6, 8, 7 });
                break;

            case 162:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, topCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, topCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 5, 3, 4, 5, 4, 6 });
                break;

            case 163:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, leftCenterFront, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, leftCenterFront, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 3, 4, 5, 6, 3, 5, 6, 5, 7, 6, 7, 1, 6, 1, 0 });
                break;

            case 164:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, topCenterLeft, bottomCenterLeft, leftCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, topCenterLeft, bottomCenterLeft, leftCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 5, 3, 4, 5, 4, 6, 5, 6, 7, 7, 6, 8, 2, 1, 8, 2, 8, 6 });
                break;

            case 165:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, topCenterLeft, bottomCenterLeft, leftCenterFront, bottomCenterFront, topCenterFront, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterRight, bottomCenterBack, topCenterBack, leftCenterBack, topCenterLeft, bottomCenterLeft, leftCenterFront, bottomCenterFront, topCenterFront, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 5, 3, 4, 5, 4, 6, 5, 6, 7, 7, 6, 8, 2, 1, 8, 2, 8, 6, 9, 10, 3, 9, 3, 5, 10, 11, 1, 10, 1, 0, 9, 7, 8, 9, 8, 11 });
                break;

            case 166:
                vertices.AddRange(new Vector3[] { bottomCenterBack, bottomCenterRight, bottomCenterFront, bottomCenterLeft, rightCenterBack, topCenterBack, leftCenterBack, topCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterBack, bottomCenterRight, bottomCenterFront, bottomCenterLeft, rightCenterBack, topCenterBack, leftCenterBack, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 1, 0, 5, 4, 0, 5, 0, 6, 5, 6, 3, 7, 5, 3, 7, 3, 2 });
                break;

            case 167:
                vertices.AddRange(new Vector3[] { bottomCenterBack, bottomCenterRight, bottomCenterFront, bottomCenterLeft, rightCenterBack, topCenterRight, rightCenterFront, topCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { bottomCenterBack, bottomCenterRight, bottomCenterFront, bottomCenterLeft, rightCenterBack, topCenterRight, rightCenterFront, topCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 1, 0, 5, 2, 6, 5, 3, 2, 5, 7, 3, 7, 8, 3 });
                break;

            case 168:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, topCenterLeft, rightCenterBack, bottomCenterBack, rightCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, topCenterLeft, rightCenterBack, bottomCenterBack, rightCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 4, 0, 4, 1, 3, 5, 6, 3, 6, 4 });
                break;

            case 169:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, topCenterLeft, rightCenterBack, bottomCenterBack, topCenterRight, bottomCenterFront, topCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, topCenterLeft, rightCenterBack, bottomCenterBack, topCenterRight, bottomCenterFront, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 4, 0, 4, 1, 5, 4, 3, 5, 6, 4, 5, 7, 6 });
                break;

            case 170:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, topCenterFront, leftCenterFront, bottomCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, topCenterFront, leftCenterFront, bottomCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 4, 3, 5, 4, 6, 7, 4, 5, 6, 1, 7, 6, 2, 1, 6 });
                break;

            case 171:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, topCenterRight, bottomCenterBack, leftCenterBack, bottomCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, topCenterRight, bottomCenterBack, leftCenterBack, bottomCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 3, 4, 5, 3, 4, 6, 5 });
                break;

            case 172:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterBack, leftCenterBack, rightCenterBack, bottomCenterBack, rightCenterFront, bottomCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterBack, leftCenterBack, rightCenterBack, bottomCenterBack, rightCenterFront, bottomCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 4, 1, 4, 2, 3, 5, 4, 4, 5, 6, 7, 6, 5, 0, 2, 6, 0, 6, 7 });
                break;

            case 173:
                vertices.AddRange(new Vector3[] { topCenterLeft, leftCenterBack, bottomCenterLeft, leftCenterFront, topCenterBack, topCenterFront, topCenterRight, bottomCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, leftCenterBack, bottomCenterLeft, leftCenterFront, topCenterBack, topCenterFront, topCenterRight, bottomCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 4, 1, 5, 3, 2, 5, 2, 6, 6, 2, 7, 6, 7, 8 });
                break;

            case 174:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterBack, bottomCenterLeft, topCenterFront, rightCenterFront, bottomCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterBack, bottomCenterLeft, topCenterFront, rightCenterFront, bottomCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2, 5, 4, 2, 5, 6, 4 });
                break;

            case 175:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterBack, rightCenterBack, bottomCenterBack, leftCenterBack, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 1, 4, 3, 4, 5, 3 });
                break;

            case 176:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterRight, bottomCenterRight, bottomCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterRight, bottomCenterRight, bottomCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 177:
                vertices.AddRange(new Vector3[] { topCenterFront, rightCenterFront, bottomCenterRight, topCenterLeft, bottomCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, rightCenterFront, bottomCenterRight, topCenterLeft, bottomCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4, 3, 4, 5 });
                break;

            case 178:
                vertices.AddRange(new Vector3[] { topCenterFront, leftCenterBack, leftCenterFront, bottomCenterBack, bottomCenterRight, topCenterRight });
                uvs.AddRange(new Vector2[] { topCenterFront, leftCenterBack, leftCenterFront, bottomCenterBack, bottomCenterRight, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 0, 4, 3, 0, 5, 4 });
                break;

            case 179:
                vertices.AddRange(new Vector3[] { leftCenterFront, rightCenterFront, bottomCenterRight, bottomCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterFront, rightCenterFront, bottomCenterRight, bottomCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 180:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterRight, bottomCenterRight, bottomCenterBack, leftCenterBack, bottomCenterFront, bottomCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterRight, bottomCenterRight, bottomCenterBack, leftCenterBack, bottomCenterFront, bottomCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 3, 2, 5, 3, 5, 6, 7, 6, 5 });
                break;

            case 181:
                vertices.AddRange(new Vector3[] { topCenterLeft, bottomCenterRight, bottomCenterBack, leftCenterBack, bottomCenterFront, bottomCenterLeft, leftCenterFront, topCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, bottomCenterRight, bottomCenterBack, leftCenterBack, bottomCenterFront, bottomCenterLeft, leftCenterFront, topCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 2, 1, 4, 2, 4, 5, 6, 5, 4, 0, 7, 1, 7, 8, 1 });
                break;

            case 182:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterLeft, bottomCenterBack, bottomCenterFront, bottomCenterRight, topCenterFront, topCenterRight });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterLeft, bottomCenterBack, bottomCenterFront, bottomCenterRight, topCenterFront, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 5, 6, 4, 5, 4, 3 });
                break;

            case 183:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterLeft, bottomCenterBack, bottomCenterFront, bottomCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterLeft, bottomCenterBack, bottomCenterFront, bottomCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 2, 3, 4, 5, 4, 3 });
                break;

            case 184:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterFront, bottomCenterFront, topCenterLeft, bottomCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterFront, bottomCenterFront, topCenterLeft, bottomCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2, 5, 3, 4 });
                break;

            case 185:
                vertices.AddRange(new Vector3[] { bottomCenterBack, topCenterLeft, bottomCenterFront, leftCenterBack, topCenterFront });
                uvs.AddRange(new Vector2[] { bottomCenterBack, topCenterLeft, bottomCenterFront, leftCenterBack, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 1, 0, 1, 4, 2 });
                break;

            case 186:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterRight, rightCenterFront, bottomCenterFront, leftCenterFront, leftCenterBack, bottomCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterRight, rightCenterFront, bottomCenterFront, leftCenterFront, leftCenterBack, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 5, 4, 3, 5, 3, 6 });
                break;

            case 187:
                vertices.AddRange(new Vector3[] { leftCenterBack, leftCenterFront, bottomCenterFront, bottomCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterBack, leftCenterFront, bottomCenterFront, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 188:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterLeft, bottomCenterBack, topCenterLeft, leftCenterFront, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterLeft, bottomCenterBack, topCenterLeft, leftCenterFront, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 1, 3, 1, 0, 3, 5, 6, 3, 6, 4 });
                break;

            case 189:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterLeft, bottomCenterBack, topCenterLeft, leftCenterFront, topCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterLeft, bottomCenterBack, topCenterLeft, leftCenterFront, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 1, 3, 1, 0, 3, 5, 4 });
                break;

            case 190:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterLeft, bottomCenterBack, topCenterFront, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterLeft, bottomCenterBack, topCenterFront, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5 });
                break;

            case 191:
                vertices.AddRange(new Vector3[] { leftCenterBack, bottomCenterLeft, bottomCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterBack, bottomCenterLeft, bottomCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 192:
                vertices.AddRange(new Vector3[] { leftCenterBack, rightCenterBack, bottomCenterRight, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterBack, rightCenterBack, bottomCenterRight, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 193:
                vertices.AddRange(new Vector3[] { leftCenterBack, rightCenterBack, bottomCenterRight, bottomCenterLeft, topCenterRight, rightCenterFront, topCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, rightCenterBack, bottomCenterRight, bottomCenterLeft, topCenterRight, rightCenterFront, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 2, 4, 2, 1, 4, 6, 5 });
                break;

            case 194:
                vertices.AddRange(new Vector3[] { leftCenterBack, rightCenterBack, bottomCenterRight, bottomCenterLeft, topCenterLeft, leftCenterFront, topCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, rightCenterBack, bottomCenterRight, bottomCenterLeft, topCenterLeft, leftCenterFront, topCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 0, 3, 4, 3, 5, 6, 4, 5 });
                break;

            case 195:
                vertices.AddRange(new Vector3[] { leftCenterBack, rightCenterBack, bottomCenterRight, bottomCenterLeft, topCenterLeft, leftCenterFront, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, rightCenterBack, bottomCenterRight, bottomCenterLeft, topCenterLeft, leftCenterFront, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 0, 3, 4, 3, 5, 6, 7, 2, 6, 2, 1, 6, 4, 5, 6, 5, 7 });
                break;

            case 196:
                vertices.AddRange(new Vector3[] { rightCenterBack, bottomCenterRight, bottomCenterFront, leftCenterBack, leftCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterBack, bottomCenterRight, bottomCenterFront, leftCenterBack, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 197:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterFront, rightCenterFront, bottomCenterFront, leftCenterFront, leftCenterBack, bottomCenterRight, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterFront, rightCenterFront, bottomCenterFront, leftCenterFront, leftCenterBack, bottomCenterRight, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 1, 4, 3, 4, 5, 3, 5, 6, 3, 5, 7, 6 });
                break;

            case 198:
                vertices.AddRange(new Vector3[] { leftCenterBack, rightCenterBack, bottomCenterRight, topCenterLeft, topCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, rightCenterBack, bottomCenterRight, topCenterLeft, topCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2, 5, 4, 2 });
                break;

            case 199:
                vertices.AddRange(new Vector3[] { bottomCenterFront, rightCenterFront, bottomCenterRight, topCenterRight, rightCenterBack, leftCenterBack, topCenterLeft });
                uvs.AddRange(new Vector2[] { bottomCenterFront, rightCenterFront, bottomCenterRight, topCenterRight, rightCenterBack, leftCenterBack, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 2, 3, 4, 3, 5, 4, 3, 6, 5 });
                break;

            case 200:
                vertices.AddRange(new Vector3[] { leftCenterBack, rightCenterBack, rightCenterFront, bottomCenterFront, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterBack, rightCenterBack, rightCenterFront, bottomCenterFront, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 201:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterFront, bottomCenterFront, bottomCenterLeft, leftCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterFront, bottomCenterFront, bottomCenterLeft, leftCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 });
                break;

            case 202:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterLeft, leftCenterFront, leftCenterBack, bottomCenterLeft, rightCenterBack, bottomCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterLeft, leftCenterFront, leftCenterBack, bottomCenterLeft, rightCenterBack, bottomCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 4, 1, 4, 2, 3, 5, 4, 4, 5, 6, 6, 5, 7 });
                break;

            case 203:
                vertices.AddRange(new Vector3[] { bottomCenterLeft, leftCenterFront, bottomCenterFront, leftCenterBack, topCenterLeft, rightCenterBack, topCenterRight });
                uvs.AddRange(new Vector2[] { bottomCenterLeft, leftCenterFront, bottomCenterFront, leftCenterBack, topCenterLeft, rightCenterBack, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 1, 3, 1, 0, 3, 5, 6, 3, 6, 4 });
                break;

            case 204:
                vertices.AddRange(new Vector3[] { leftCenterBack, rightCenterBack, leftCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { leftCenterBack, rightCenterBack, leftCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2 });
                break;

            case 205:
                vertices.AddRange(new Vector3[] { topCenterFront, leftCenterFront, leftCenterBack, topCenterRight, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, leftCenterFront, leftCenterBack, topCenterRight, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 206:
                vertices.AddRange(new Vector3[] { topCenterLeft, leftCenterBack, rightCenterBack, topCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, leftCenterBack, rightCenterBack, topCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 207:
                vertices.AddRange(new Vector3[] { topCenterRight, leftCenterBack, rightCenterBack, topCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterRight, leftCenterBack, rightCenterBack, topCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1 });
                break;

            case 208:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterLeft, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterLeft, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;

            case 209:
                vertices.AddRange(new Vector3[] { topCenterBack, bottomCenterRight, bottomCenterLeft, leftCenterBack, topCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, bottomCenterRight, bottomCenterLeft, leftCenterBack, topCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 4, 1, 4, 5, 1 });
                break;

            case 210:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterLeft, leftCenterBack, topCenterFront, topCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterRight, bottomCenterRight, bottomCenterLeft, leftCenterBack, topCenterFront, topCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 5, 1, 0, 5, 0, 6, 5, 6, 7 });
                break;

            case 211:
                vertices.AddRange(new Vector3[] { leftCenterFront, rightCenterFront, bottomCenterRight, bottomCenterLeft, topCenterLeft, leftCenterBack, topCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterFront, rightCenterFront, bottomCenterRight, bottomCenterLeft, topCenterLeft, leftCenterBack, topCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 0, 3, 4, 3, 5, 6, 4, 5 });
                break;

            case 212:
                vertices.AddRange(new Vector3[] { topCenterBack, bottomCenterFront, leftCenterBack, leftCenterFront, bottomCenterRight, topCenterRight });
                uvs.AddRange(new Vector2[] { topCenterBack, bottomCenterFront, leftCenterBack, leftCenterFront, bottomCenterRight, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 0, 4, 1, 0, 5, 4 });
                break;

            case 213:
                vertices.AddRange(new Vector3[] { rightCenterFront, bottomCenterRight, bottomCenterFront, topCenterFront, leftCenterFront, topCenterBack, leftCenterBack });
                uvs.AddRange(new Vector2[] { rightCenterFront, bottomCenterRight, bottomCenterFront, topCenterFront, leftCenterFront, topCenterBack, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 5, 3, 4, 5, 4, 6 });
                break;

            case 214:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterRight, bottomCenterRight, bottomCenterFront, topCenterBack, topCenterLeft, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterRight, bottomCenterRight, bottomCenterFront, topCenterBack, topCenterLeft, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 1, 0, 4, 0, 5, 4, 5, 6 });
                break;

            case 215:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterLeft, leftCenterBack, rightCenterFront, bottomCenterRight, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterLeft, leftCenterBack, rightCenterFront, bottomCenterRight, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5 });
                break;

            case 216:
                vertices.AddRange(new Vector3[] { topCenterRight, rightCenterFront, bottomCenterFront, topCenterBack, bottomCenterLeft, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, rightCenterFront, bottomCenterFront, topCenterBack, bottomCenterLeft, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4, 3, 4, 5 });
                break;

            case 217:
                vertices.AddRange(new Vector3[] { topCenterBack, bottomCenterLeft, leftCenterBack, topCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, bottomCenterLeft, leftCenterBack, topCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 0, 3, 1, 3, 4 });
                break;

            case 218:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterLeft, leftCenterBack, bottomCenterLeft, leftCenterFront, bottomCenterFront, topCenterFront, rightCenterFront, topCenterRight });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterLeft, leftCenterBack, bottomCenterLeft, leftCenterFront, bottomCenterFront, topCenterFront, rightCenterFront, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 1, 4, 3, 3, 4, 5, 4, 6, 5, 6, 7, 5, 6, 8, 7 });
                break;

            case 219:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterLeft, leftCenterBack, bottomCenterLeft, leftCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterLeft, leftCenterBack, bottomCenterLeft, leftCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 2, 1, 3, 1, 4, 3, 3, 4, 5 });
                break;

            case 220:
                vertices.AddRange(new Vector3[] { topCenterBack, leftCenterFront, leftCenterBack, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, leftCenterFront, leftCenterBack, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 3, 4, 1 });
                break;

            case 221:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterFront, leftCenterFront, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterFront, leftCenterFront, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 222:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterLeft, leftCenterBack, topCenterFront, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterLeft, leftCenterBack, topCenterFront, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 4, 3, 1, 0, 3, 4, 5 });
                break;

            case 223:
                vertices.AddRange(new Vector3[] { topCenterBack, topCenterLeft, leftCenterBack });
                uvs.AddRange(new Vector2[] { topCenterBack, topCenterLeft, leftCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 224:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterLeft, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterLeft, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 225:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterLeft, bottomCenterLeft, topCenterFront, rightCenterFront, topCenterRight });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterLeft, bottomCenterLeft, topCenterFront, rightCenterFront, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2, 5, 6, 7, 7, 6, 2, 7, 2, 1 });
                break;

            case 226:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterFront, bottomCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterFront, bottomCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 3, 2, 4, 3, 4, 5 });
                break;

            case 227:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterBack, rightCenterBack, bottomCenterRight, rightCenterFront, leftCenterFront, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterBack, rightCenterBack, bottomCenterRight, rightCenterFront, leftCenterFront, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 5, 4, 3, 5, 3, 6 });
                break;

            case 228:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterLeft, bottomCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterLeft, bottomCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2, 4, 5, 3 });
                break;

            case 229:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterLeft, bottomCenterFront, leftCenterFront, rightCenterFront, topCenterFront, topCenterRight });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterLeft, bottomCenterFront, leftCenterFront, rightCenterFront, topCenterFront, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2, 4, 5, 3, 6, 7, 4, 4, 7, 5, 8, 7, 6 });
                break;

            case 230:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, bottomCenterRight, topCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 231:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterBack, rightCenterBack, bottomCenterRight, rightCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterBack, rightCenterBack, bottomCenterRight, rightCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 5, 4, 3 });
                break;

            case 232:
                vertices.AddRange(new Vector3[] { rightCenterBack, rightCenterFront, bottomCenterFront, topCenterBack, topCenterLeft, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { rightCenterBack, rightCenterFront, bottomCenterFront, topCenterBack, topCenterLeft, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2, 5, 4, 2 });
                break;

            case 233:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterFront, bottomCenterFront, bottomCenterLeft, topCenterBack, topCenterRight, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterFront, bottomCenterFront, bottomCenterLeft, topCenterBack, topCenterRight, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 1, 4, 1, 0, 5, 4, 6 });
                break;

            case 234:
                vertices.AddRange(new Vector3[] { leftCenterFront, bottomCenterFront, bottomCenterLeft, topCenterFront, rightCenterFront, topCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterFront, bottomCenterFront, bottomCenterLeft, topCenterFront, rightCenterFront, topCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 1, 3, 1, 0, 3, 5, 4, 4, 5, 6 });
                break;

            case 235:
                vertices.AddRange(new Vector3[] { leftCenterFront, bottomCenterFront, bottomCenterLeft, topCenterRight, topCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { leftCenterFront, bottomCenterFront, bottomCenterLeft, topCenterRight, topCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 4, 5 });
                break;

            case 236:
                vertices.AddRange(new Vector3[] { topCenterBack, rightCenterBack, rightCenterFront, topCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterBack, rightCenterBack, rightCenterFront, topCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 237:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterBack, rightCenterBack, topCenterFront, topCenterLeft, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterBack, rightCenterBack, topCenterFront, topCenterLeft, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 0, 3, 1, 3, 4, 4, 3, 5 });
                break;

            case 238:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterBack, rightCenterBack, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterBack, rightCenterBack, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 239:
                vertices.AddRange(new Vector3[] { topCenterRight, topCenterBack, rightCenterBack });
                uvs.AddRange(new Vector2[] { topCenterRight, topCenterBack, rightCenterBack });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 240:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterRight, bottomCenterRight, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterRight, bottomCenterRight, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 241:
                vertices.AddRange(new Vector3[] { topCenterLeft, bottomCenterRight, bottomCenterLeft, topCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, bottomCenterRight, bottomCenterLeft, topCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 3, 1, 3, 4, 1 });
                break;

            case 242:
                vertices.AddRange(new Vector3[] { topCenterRight, bottomCenterRight, bottomCenterLeft, topCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterRight, bottomCenterRight, bottomCenterLeft, topCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 4, 3, 2 });
                break;

            case 243:
                vertices.AddRange(new Vector3[] { leftCenterFront, rightCenterFront, bottomCenterRight, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterFront, rightCenterFront, bottomCenterRight, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 244:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterRight, bottomCenterRight, leftCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterRight, bottomCenterRight, leftCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 3, 0, 2, 4, 3, 2 });
                break;

            case 245:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterFront, leftCenterFront, rightCenterFront, bottomCenterFront, bottomCenterRight });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterFront, leftCenterFront, rightCenterFront, bottomCenterFront, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 4, 1, 4, 2, 3, 5, 4 });
                break;

            case 246:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterRight, bottomCenterFront, bottomCenterRight });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterRight, bottomCenterFront, bottomCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2 });
                break;

            case 247:
                vertices.AddRange(new Vector3[] { rightCenterFront, bottomCenterRight, bottomCenterFront });
                uvs.AddRange(new Vector2[] { rightCenterFront, bottomCenterRight, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 248:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterRight, bottomCenterLeft, rightCenterFront, bottomCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterRight, bottomCenterLeft, rightCenterFront, bottomCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2, 3, 4, 2 });
                break;

            case 249:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterFront, bottomCenterFront, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterFront, bottomCenterFront, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;

            case 250:
                vertices.AddRange(new Vector3[] { topCenterFront, rightCenterFront, bottomCenterFront, leftCenterFront, bottomCenterLeft, topCenterRight });
                uvs.AddRange(new Vector2[] { topCenterFront, rightCenterFront, bottomCenterFront, leftCenterFront, bottomCenterLeft, topCenterRight });
                triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4, 0, 5, 1 });
                break;

            case 251:
                vertices.AddRange(new Vector3[] { leftCenterFront, bottomCenterFront, bottomCenterLeft });
                uvs.AddRange(new Vector2[] { leftCenterFront, bottomCenterFront, bottomCenterLeft });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 252:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterRight, leftCenterFront, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterRight, leftCenterFront, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2, 1, 3, 2 });
                break;

            case 253:
                vertices.AddRange(new Vector3[] { topCenterLeft, topCenterFront, leftCenterFront });
                uvs.AddRange(new Vector2[] { topCenterLeft, topCenterFront, leftCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 254:
                vertices.AddRange(new Vector3[] { topCenterFront, topCenterRight, rightCenterFront });
                uvs.AddRange(new Vector2[] { topCenterFront, topCenterRight, rightCenterFront });
                triangles.AddRange(new int[] { 0, 1, 2 });
                break;

            case 255:
                break;
        }
    }

    public Vector3[] GetVertices()
    {
        return vertices.ToArray();
    }

    public int[] GetTriangles()

    {
        return triangles.ToArray();
    }

    public Vector2[] GetUvs()
    {
        return uvs.ToArray();
    }
}
