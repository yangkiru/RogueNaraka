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
        float standard = 90;
        bool isInverse = true;
        if (degree > 180)
        {
            degree = 360 - degree;
            isInverse = false;
        }
        Vector2 vec = RadianToVector2((degree + standard) * Mathf.Deg2Rad);
        if (isInverse)
            vec = new Vector2(-vec.x, vec.y);
        return vec;
    }
    public static Vector2 DegreeToVector2(float degree, float length)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad) * length;
    }
    public static float Vector2ToDegree(Vector2 vec)
    {
        float angle = Vector2.Angle(Vector2.up, vec);
        return IncreaseAngle(angle, vec);
    }
    public static float IncreaseAngle(float angle, Vector2 vec)
    {
        float result = angle;
        if (vec.x < 0)
            result = 360 - angle;
        return CutAngle(result);
    }
    public static float CutAngle(float angle)
    {
        if (angle >= 360)
            return CutAngle(angle - 360);
        else if (angle <= -360)
            return CutAngle(angle + 360);
        else
            return angle;
    }
    public static float DecelerateDistance(float _decelerationRate, float _speed) {
        return Mathf.Max(Mathf.Pow(_speed, 2.0f) * (1 - _decelerationRate), 0.1f);
    }
    public static Vector2 Vector2Clamp(Vector2 input, Vector2 boundMin, Vector2 boundMax)
    {
        return new Vector2(Mathf.Clamp(input.x, boundMin.x, boundMax.x),
        Mathf.Clamp(input.y, boundMin.y, boundMax.y));
    }
    public static Quaternion GetRandomAngle(float min = 0, float max = 360)
    {
        return Quaternion.Euler(0, 0, Random.Range(min, max));
    }
}
