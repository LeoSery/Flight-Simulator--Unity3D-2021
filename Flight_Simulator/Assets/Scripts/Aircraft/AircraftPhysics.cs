using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AircraftPhysics : MonoBehaviour
{
    public List<AeroSurface> AeroSurfaces = null;
    public List<ControlSurface> ControlSurfaces = null;
    public float ThrustPercent = 0;
    public float ThrustMultiplier = 0;

    const float PREDICTION_TIMESTEP_FRACTION = 0.5f;
    Rigidbody m_model;
    BiVector3 m_currentForceAndTorque;

    private void Awake()
    {
        m_model = GetComponent<Rigidbody>();
    }

    public void SetControlSurfacesAngles(float pitch, float roll, float yaw, float flap)
    {
        foreach (var currentSurface in ControlSurfaces)
        {
            if (currentSurface.Surface == null) return;
            switch (currentSurface.Type)
            {
                case ControlSurfaceType.Pitch:
                    currentSurface.Surface.SetFlapAngle(pitch * currentSurface.FlapAngle);
                    break;
                case ControlSurfaceType.Roll:
                    currentSurface.Surface.SetFlapAngle(roll * currentSurface.FlapAngle);
                    break;
                case ControlSurfaceType.Yaw:
                    currentSurface.Surface.SetFlapAngle(yaw * currentSurface.FlapAngle);
                    break;
                case ControlSurfaceType.Flap:
                    currentSurface.Surface.SetFlapAngle(flap * currentSurface.FlapAngle);
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        BiVector3 forceAndTorqueThisFrame = CalculateAerodynamicForces(m_model.velocity, m_model.angularVelocity, Vector3.zero, 1.2f, m_model.worldCenterOfMass);

        Vector3 velocityPrediction = PredictVelocity(forceAndTorqueThisFrame.Force);
        Vector3 angularVelocityPrediction = PredictAngularVelocity(forceAndTorqueThisFrame.Torque);

        BiVector3 forceAndTorquePrediction = CalculateAerodynamicForces(velocityPrediction, angularVelocityPrediction, Vector3.zero, 1.2f, m_model.worldCenterOfMass);

        m_currentForceAndTorque = (forceAndTorqueThisFrame + forceAndTorquePrediction) * 0.5f;
        m_model.AddForce(m_currentForceAndTorque.Force);
        m_model.AddTorque(m_currentForceAndTorque.Torque);
        m_model.AddForce(transform.forward * ThrustMultiplier * ThrustPercent);
    }

    private BiVector3 CalculateAerodynamicForces(Vector3 velocity, Vector3 angularVelocity, Vector3 wind, float airDensity, Vector3 centerOfMass)
    {
        BiVector3 forceAndTorque = new BiVector3();
        foreach (var surface in AeroSurfaces)
        {
            Vector3 relativePosition = surface.transform.position - centerOfMass;
            forceAndTorque += surface.CalculateForces(-velocity + wind - Vector3.Cross(angularVelocity, relativePosition), airDensity, relativePosition);
        }
        return forceAndTorque;
    }

    private Vector3 PredictVelocity(Vector3 force)
    {
        return m_model.velocity + Time.fixedDeltaTime * PREDICTION_TIMESTEP_FRACTION * (force / m_model.mass + Physics.gravity);
    }

    private Vector3 PredictAngularVelocity(Vector3 torque)
    {
        Quaternion inertiaTensorWorldRotation = m_model.rotation * m_model.inertiaTensorRotation;
        Vector3 torqueInDiagonalSpace = Quaternion.Inverse(inertiaTensorWorldRotation) * torque;
        Vector3 angularVelocityChangeInDiagonalSpace;
        angularVelocityChangeInDiagonalSpace.x = torqueInDiagonalSpace.x / m_model.inertiaTensor.x;
        angularVelocityChangeInDiagonalSpace.y = torqueInDiagonalSpace.y / m_model.inertiaTensor.y;
        angularVelocityChangeInDiagonalSpace.z = torqueInDiagonalSpace.z / m_model.inertiaTensor.z;

        return m_model.angularVelocity + Time.fixedDeltaTime * PREDICTION_TIMESTEP_FRACTION * (inertiaTensorWorldRotation * angularVelocityChangeInDiagonalSpace);
    }

#if UNITY_EDITOR
    public void CalculateCenterOfLift(out Vector3 center, out Vector3 force, Vector3 displayAirVelocity, float displayAirDensity, float pitch, float yaw, float roll, float flap)
    {
        if (AeroSurfaces == null)
        {
            center = Vector3.zero;
            force = Vector3.zero;
            return;
        }

        Vector3 centerOfmass;
        BiVector3 forceAndTorque;
        if (m_model == null)
        {
            centerOfmass = GetComponent<Rigidbody>().worldCenterOfMass;
            foreach (var surface in AeroSurfaces)
                if (surface.Config != null)
                    surface.Awake();

            SetControlSurfacesAngles(pitch, roll, yaw, flap);
            forceAndTorque = CalculateAerodynamicForces(-displayAirVelocity, Vector3.zero, Vector3.zero, displayAirDensity, centerOfmass);
        }
        else
        {
            centerOfmass = m_model.worldCenterOfMass;
            forceAndTorque = m_currentForceAndTorque;
        }

        force = forceAndTorque.Force;
        center = centerOfmass + Vector3.Cross(forceAndTorque.Force, forceAndTorque.Torque) / forceAndTorque.Force.sqrMagnitude;
    }
#endif
}

[System.Serializable]
public class ControlSurface
{
    public AeroSurface Surface;
    public float FlapAngle;
    public ControlSurfaceType Type;
}

public enum ControlSurfaceType { Pitch, Yaw, Roll, Flap }
