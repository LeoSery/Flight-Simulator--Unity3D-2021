using UnityEngine;

public static class Utilities
{
    public static float TransformAngle(float angle, float fov, float pixelHeight)
    {
        return (Mathf.Tan(angle * Mathf.Deg2Rad) / Mathf.Tan(fov / 2 * Mathf.Deg2Rad)) * pixelHeight / 2;
    }

    public static float ConvertAngle(float angle)
    {
        if (angle > 180)
            angle -= 360f;
        return angle;
    }

    public static float GetPosition(float angle, Camera CurrentCamera)
    {
        float fov = CurrentCamera.fieldOfView;
        return Utilities.TransformAngle(angle, fov, CurrentCamera.pixelHeight);
    }
}
