using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomSpline
{
    public static Vector3[] GetSplineBetweenPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = -p0 + 3f * p1 - 3f * p2 + p3; // x^3
        Vector3 b = 2f * p0 - 5f * p1 + 4f * p2 - p3; // x^2
        Vector3 c = p2 - p0; // x^1
        Vector3 d = 2f * p1; // x^0

        return new Vector3[] { a, b, c, d };
    }
}
