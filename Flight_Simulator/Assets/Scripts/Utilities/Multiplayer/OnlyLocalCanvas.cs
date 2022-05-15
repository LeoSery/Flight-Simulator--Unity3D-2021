using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class OnlyLocalCanvas : NetworkBehaviour
{
    void Start()
    {
        if (isLocalPlayer)
        {
            GetComponentInChildren<Canvas>().enabled = true;
        }
        else
        {
            GetComponentInChildren<Canvas>().enabled = false;
        }
    }
}
