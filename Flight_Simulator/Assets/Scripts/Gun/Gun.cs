using UnityEngine;
using UnityEngine.Assertions;

public class Gun : MonoBehaviour
{
    public GunConfig Config;

    void Awake()
    {
        Assert.IsTrue(Config);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject spawnedRound = Instantiate(Config.Round, transform.position + transform.forward, transform.rotation);
            spawnedRound.transform.Rotate(new Vector3(Random.Range(-1f, 1f) * Config.RoundDispersion, Random.Range(-1f, 1f) * Config.RoundDispersion, 0));

            Rigidbody rb = spawnedRound.GetComponent<Rigidbody>();
            rb.velocity = spawnedRound.transform.forward * Config.RoundSpeed;
        }
    }
}
