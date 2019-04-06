using System.Collections.Generic;
using UnityEngine;

public static class BezierCurve
{
    public static Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float tt = t * t;
        float uu = (1-t)*(1-t);

        return p1 + uu*(p0-p1) + tt*(p2-p1);
    }

    public static Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        t = Mathf.Clamp01(t);
        return (1-t)*QuadraticBezier(p0,p1,p2,t) + t*QuadraticBezier(p1, p2, p3, t);
    }
}
