using UnityEngine;

public class AircraftController : MonoBehaviour
{
    AircraftPhysics m_physics;
    Rotator m_propeller;

    public float RollSensitivity = 0.2f, PitchSensitivity = 0.2f, YawSensitivity = 0.2f, ThrustSensitivity = 0.01f, FlapSensitivity = 0.15f;
    float m_currentRoll, m_currentPitch, m_currentYaw, m_currentThrust, m_currentFlap;

    public PhysicMaterial GearsBrakesMaterial;
    public PhysicMaterial GearsDriveMaterial;

    private SphereCollider[] m_elementsSlowDown;

    private bool m_isGrounded = true;

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
            m_isGrounded = true;
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground")
            m_isGrounded = false;
    }

    void Start()
    {
        m_physics = GetComponent<AircraftPhysics>();
        m_propeller = FindObjectOfType<Rotator>();
        m_elementsSlowDown = GetComponentsInChildren<SphereCollider>();
        m_physics.ThrustPercent = 0;

        foreach (var element in m_elementsSlowDown)
        {
            element.material = GearsDriveMaterial;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) // FIXME : Setup customizable keys
        {
            m_currentFlap += FlapSensitivity;
            m_currentFlap = Mathf.Clamp(m_currentFlap, 0f, Mathf.Deg2Rad * 40);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift)) // FIXME : Setup customizable keys
        {
            m_currentFlap -= FlapSensitivity;
            m_currentFlap = Mathf.Clamp(m_currentFlap, 0f, Mathf.Deg2Rad * 40);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (m_isGrounded)
            {
                var rigidbody = m_physics.GetComponent<Rigidbody>();
                rigidbody.velocity = rigidbody.velocity * 0.99f;
                rigidbody.angularVelocity = rigidbody.angularVelocity * 0.99f;

                if (rigidbody.velocity.magnitude <= 1)
                {
                    rigidbody.velocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                }
            }
        }
        m_propeller.Speed = m_currentThrust * 6000f;

        m_currentPitch = PitchSensitivity * Input.GetAxis("Vertical");
        m_currentRoll = RollSensitivity * Input.GetAxis("Horizontal");
        m_currentYaw = YawSensitivity * Input.GetAxis("Yaw");
        m_currentThrust = ThrustSensitivity * Input.GetAxis("Thrust");
    }

    void FixedUpdate()
    {
        m_physics.SetControlSurfacesAngles(m_currentPitch, m_currentRoll, m_currentYaw, m_currentFlap);
        m_physics.ThrustPercent = m_currentThrust;
    }

}