using UnityEngine;
using UnityEngine.Assertions;

public class AeroSurface : MonoBehaviour
{
    public AeroSurfaceConfig Config;
    public float FlapAngle { get; private set; }
    public Vector3 CurrentLift { get; private set; }
    public Vector3 CurrentDrag { get; private set; }
    public Vector3 CurrentTorque { get; private set; }
    public bool IsAtStall { get; private set; }

    float m_area;
    float m_correctedLiftSlope;
    float m_zeroLiftAoA;
    float m_stallAngleHigh;
    float m_stallAngleLow;
    float m_fullStallAngleHigh;
    float m_fullStallAngleLow;

    public void Awake()
    {
        Assert.IsTrue(Config);

        m_area = Mathf.Pow(Config.Chord, 2) * Config.AspectRatio;
        m_correctedLiftSlope = Config.LiftSlope * Config.AspectRatio / (Config.AspectRatio + 2 * (Config.AspectRatio + 4) / (Config.AspectRatio + 2));
        SetFlapAngle(0);
    }

    #region Surfaces

    public void SetFlapAngle(float newFlapAngle)
    {
        if (!gameObject.activeInHierarchy) return;

        FlapAngle = Mathf.Clamp(newFlapAngle, -Mathf.Deg2Rad * 50, Mathf.Deg2Rad * 50);

        float theta = Mathf.Acos(2 * Config.FlapFraction - 1);
        float flapEffectivness = 1 - (theta - Mathf.Sin(theta)) / Mathf.PI;

        float deltaLift = m_correctedLiftSlope * flapEffectivness * FlapEffectivnessCorrection(FlapAngle) * FlapAngle;
        float zeroLiftAoaBase = Config.ZeroLiftAoA * Mathf.Deg2Rad;

        float stallAngleHighBase = Config.StallAngleHigh * Mathf.Deg2Rad;
        float stallAngleLowBase = Config.StallAngleLow * Mathf.Deg2Rad;

        float clMaxHigh = m_correctedLiftSlope * (stallAngleHighBase - zeroLiftAoaBase) + deltaLift * LiftCoefficientMaxFraction(Config.FlapFraction);
        float clMaxLow = m_correctedLiftSlope * (stallAngleLowBase - zeroLiftAoaBase) + deltaLift * LiftCoefficientMaxFraction(Config.FlapFraction);

        float blendAngle = Mathf.Deg2Rad * Mathf.Lerp(8, 14, Mathf.Abs(Mathf.Abs(Mathf.Rad2Deg * FlapAngle) - 50) / 50);

        m_zeroLiftAoA = zeroLiftAoaBase - deltaLift / m_correctedLiftSlope;
        m_stallAngleHigh = m_zeroLiftAoA + clMaxHigh / m_correctedLiftSlope;
        m_stallAngleLow = m_zeroLiftAoA + clMaxLow / m_correctedLiftSlope;
        m_fullStallAngleHigh = m_stallAngleHigh + blendAngle;
        m_fullStallAngleLow = m_stallAngleLow - blendAngle;
    }

    #endregion

    #region Calculations

    private Vector3 CalculateCoefficientsAtLowAoA(float angleOfAttack)
    {
        float liftCoefficient = m_correctedLiftSlope * (angleOfAttack - m_zeroLiftAoA);
        float inducedAngle = liftCoefficient / (Mathf.PI * Config.AspectRatio);
        float effectiveAngle = angleOfAttack - m_zeroLiftAoA - inducedAngle;

        float tangentialCoefficient = Config.SkinFriction * Mathf.Cos(effectiveAngle);

        float normalCoefficient = (liftCoefficient + Mathf.Sin(effectiveAngle) * tangentialCoefficient) / Mathf.Cos(effectiveAngle);
        float dragCoefficient = normalCoefficient * Mathf.Sin(effectiveAngle) + tangentialCoefficient * Mathf.Cos(effectiveAngle);
        float torqueCoefficient = -normalCoefficient * TorqueCoefficientProportion(effectiveAngle);

        return new Vector3(liftCoefficient, dragCoefficient, torqueCoefficient);
    }

    private Vector3 CalculateCoefficientsAtStall(float angleOfAttack)
    {
        float liftLowAoACoef, lerp;
        if (angleOfAttack > m_stallAngleHigh)
        {
            lerp = (Mathf.PI / 2 - Mathf.Clamp(angleOfAttack, -Mathf.PI / 2, Mathf.PI / 2)) / (Mathf.PI / 2 - m_stallAngleHigh);
            liftLowAoACoef = m_correctedLiftSlope * (m_stallAngleHigh - m_zeroLiftAoA);
        }
        else
        {
            liftLowAoACoef = m_correctedLiftSlope * (m_stallAngleLow - m_zeroLiftAoA);
            lerp = (-Mathf.PI / 2 - Mathf.Clamp(angleOfAttack, -Mathf.PI / 2, Mathf.PI / 2)) / (-Mathf.PI / 2 - m_stallAngleLow);
        }

        float inducedAngle = Mathf.Lerp(0, liftLowAoACoef / (Mathf.PI * Config.AspectRatio), lerp);
        float effectiveAngle = angleOfAttack - m_zeroLiftAoA - inducedAngle;

        float normalCoef = FrictionAt90Degrees() * Mathf.Sin(effectiveAngle) * (1 / (0.56f + 0.44f * Mathf.Abs(Mathf.Sin(effectiveAngle))) - 0.41f * (1 - Mathf.Exp(-17 / Config.AspectRatio)));
        float tangentialCoef = 0.5f * Config.SkinFriction * Mathf.Cos(effectiveAngle);

        float liftCoef = normalCoef * Mathf.Cos(effectiveAngle) - tangentialCoef * Mathf.Sin(effectiveAngle);
        float dragCoef = normalCoef * Mathf.Sin(effectiveAngle) + tangentialCoef * Mathf.Cos(effectiveAngle);
        float torqueCoef = -normalCoef * TorqueCoefficientProportion(effectiveAngle);
        return new Vector3(liftCoef, dragCoef, torqueCoef);
    }

    private Vector3 CalculateCoefficients(float angleOfAttack)
    {
        if (angleOfAttack < m_stallAngleHigh && angleOfAttack > m_stallAngleLow)
            return CalculateCoefficientsAtLowAoA(angleOfAttack);
        else if (angleOfAttack > m_fullStallAngleHigh || angleOfAttack < m_fullStallAngleLow)
            return CalculateCoefficientsAtStall(angleOfAttack);

        float lerp;
        if (angleOfAttack > m_stallAngleHigh)
        {
            lerp = (angleOfAttack - m_stallAngleHigh) / (m_fullStallAngleHigh - m_stallAngleHigh);
            return Vector3.Lerp(CalculateCoefficientsAtLowAoA(m_stallAngleHigh), CalculateCoefficientsAtStall(m_fullStallAngleHigh), lerp);
        }

        lerp = (angleOfAttack - m_stallAngleLow) / (m_fullStallAngleLow - m_stallAngleLow);
        return Vector3.Lerp(CalculateCoefficientsAtLowAoA(m_stallAngleLow), CalculateCoefficientsAtStall(m_fullStallAngleLow), lerp);
    }

    public BiVector3 CalculateForces(Vector3 worldAirVelocity, float airDensity, Vector3 relativePosition)
    {
        if (!gameObject.activeInHierarchy) return new BiVector3();

        Vector3 invAirVelocity = transform.InverseTransformDirection(worldAirVelocity);
        Vector3 airVelocity = new Vector3(invAirVelocity.x, invAirVelocity.y);

        float angleOfAttack = Mathf.Atan2(airVelocity.y, -airVelocity.x);
        float dynamicPressure = 0.5f * airDensity * airVelocity.sqrMagnitude;

        Vector3 dragDirection = transform.TransformDirection(airVelocity.normalized);
        Vector3 liftDirection = Vector3.Cross(dragDirection, transform.forward);
        Vector3 aerodynamicCoefficients = CalculateCoefficients(angleOfAttack);

        CurrentLift = liftDirection * aerodynamicCoefficients.x * dynamicPressure * m_area;
        CurrentDrag = dragDirection * aerodynamicCoefficients.y * dynamicPressure * m_area;
        CurrentTorque = -transform.forward * aerodynamicCoefficients.z * dynamicPressure * m_area * Config.Chord;
        IsAtStall = !(angleOfAttack < m_stallAngleHigh && angleOfAttack > m_stallAngleLow);

        BiVector3 forceAndTorque = new BiVector3();
        forceAndTorque.Force += CurrentDrag + CurrentLift;
        forceAndTorque.Torque += Vector3.Cross(relativePosition, forceAndTorque.Force);
        forceAndTorque.Torque += CurrentTorque;

        return forceAndTorque;
    }

    #endregion

    #region Utilities
    private float FlapEffectivnessCorrection(float flapAngle)
    {
        return Mathf.Lerp(0.8f, 0.4f, (flapAngle * Mathf.Rad2Deg - 10) / 50);
    }

    private float LiftCoefficientMaxFraction(float flapFraction)
    {
        return Mathf.Clamp01(1 - 0.5f * (flapFraction - 0.1f) / 0.3f);
    }

    private float TorqueCoefficientProportion(float effectiveAngle)
    {
        return 0.25f - 0.175f * (1 - 2 * Mathf.Abs(effectiveAngle) / Mathf.PI);
    }

    private float FrictionAt90Degrees()
    {
        return 1.98f - 4.26e-2f * FlapAngle * FlapAngle + 2.1e-1f * FlapAngle;
    }
    #endregion
}
