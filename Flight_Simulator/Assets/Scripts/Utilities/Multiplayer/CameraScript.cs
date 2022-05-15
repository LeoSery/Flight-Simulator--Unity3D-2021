using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class CameraScript : NetworkBehaviour
{
    public Camera AircraftCamera;

    void Start()
    {
        if (isLocalPlayer) return;

        AircraftCamera.enabled = false;
    }
}
