using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelpers
{
    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }
    public static Vector2 RadianToVector2(float radian, float length)
    {
        return RadianToVector2(radian) * length;
    }
    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
    public static Vector2 DegreeToVector2(float degree, float length)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad) * length;
    }
    public static float IncreaseAngle(float angle, Vector2 standard, Vector2 vec)
    {
        float result = angle;
        bool isAdd = true;
        if (standard == Vector2.up && vec.x > 0)
            isAdd = false;
        else if (standard == Vector2.right && vec.y < 0)
            isAdd = false;
        else if (standard == Vector2.down && vec.x < 0)
            isAdd = false;
        else if (standard == Vector2.left && vec.y > 0)
            isAdd = false;
        else return float.NaN;
        if (isAdd)
            result += 180;
        return CutAngle(result);
    }
    public static float CutAngle(float angle)
    {
        if (angle >= 360)
            return angle % 360;
        else if (angle <= -360)
            return -((-angle) % 360);
        else
            return angle;
    }
}
