using UnityEngine;

[CreateAssetMenu(fileName = "New Aerodynamic Surface Config", menuName = "Aerodynamic Surface Config")]
public class AeroSurfaceConfig : ScriptableObject
{
    public float LiftSlope = 6.28f;
    public float SkinFriction = 0.02f;
    public float ZeroLiftAoA = 0;
    public float StallAngleHigh = 15;
    public float StallAngleLow = -15;
    public float Chord = 1;
    public float FlapFraction = 0;
    public float AspectRatio = 2;

    private void OnValidate()
    {
        if (FlapFraction < 0)
            FlapFraction = Mathf.Max(0.0f, FlapFraction);

        if (FlapFraction > 0.4f)
            FlapFraction = Mathf.Max(0.4f, FlapFraction);

        StallAngleHigh = Mathf.Max(0.0f, StallAngleHigh);
        StallAngleLow = Mathf.Min(0.0f, StallAngleLow);
    }
}
