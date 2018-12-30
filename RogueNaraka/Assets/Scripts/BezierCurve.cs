using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve {

    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return (1f - t) * a + t * b;
    }

    public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 a = Lerp(p0, p1, t);
        Vector3 b = Lerp(p1, p2, t);
        Vector3 c = Lerp(p2, p3, t);
        Vector3 d = Lerp(a, b, t);
        Vector3 e = Lerp(b, c, t);
        Vector3 pointOnCurve = Lerp(d, e, t);

        return pointOnCurve;
    }
}
