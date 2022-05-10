using UnityEngine;

public struct BiVector3
{
    public Vector3 Force;
    public Vector3 Torque;

    public BiVector3(Vector3 force, Vector3 torque)
    {
        Force = force;
        Torque = torque;
    }

    public static BiVector3 operator +(BiVector3 a, BiVector3 b)
    {
        return new BiVector3(a.Force + b.Force, a.Torque + b.Torque);
    }

    public static BiVector3 operator *(float f, BiVector3 a)
    {
        return new BiVector3(f * a.Force, f * a.Torque);
    }

    public static BiVector3 operator *(BiVector3 a, float f)
    {
        return f * a;
    }
}