using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Compass Compass;
    public PitchLadder Ladder;
    public Transform VelocityMarker;
    public Text SpeedText;
    public Text AltitudeText;
    public Transform PlaneTransform;
    public Rigidbody PlaneRigidBody;
    public Camera Camera;

    private void Awake()
    {
        Assert.IsTrue(Compass);
        Assert.IsTrue(Ladder);
        Assert.IsTrue(VelocityMarker);
        Assert.IsTrue(SpeedText);
        Assert.IsTrue(AltitudeText);
        //Assert.IsTrue(Camera);
    }

    private Vector3 TransformToHUDSpace(Vector3 worldSpace)
    {
        var screenSpace = Camera.WorldToScreenPoint(worldSpace);
        return screenSpace - new Vector3(Camera.pixelWidth / 2, Camera.pixelHeight / 2);
    }

    private void LateUpdate()
    {
        if (PlaneTransform == null || PlaneRigidBody == null)
            return;

        Vector3 velocity = (PlaneRigidBody.velocity.sqrMagnitude > 1) ? PlaneRigidBody.velocity : PlaneTransform.forward;

        // FPM
        var hudPos = TransformToHUDSpace(Camera.transform.position + velocity);
        if (hudPos.z > 0)
        {
            VelocityMarker.gameObject.SetActive(true);
            VelocityMarker.transform.localPosition = new Vector3(hudPos.x, hudPos.y, 0);
        }
        else
        {
            VelocityMarker.gameObject.SetActive(false);
        }

        // SPEED
        var speed = PlaneRigidBody.velocity.magnitude;
        SpeedText.text = speed.ToString("0");

        // ALTITUDE
        var altitude = PlaneTransform.position.y * 3.28;
        AltitudeText.text = altitude.ToString("0");
    }
}
