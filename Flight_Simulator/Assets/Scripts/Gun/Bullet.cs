using UnityEngine;

public class Bullet : MonoBehaviour
{
    float m_remainingTime = 1f;

    private void Update()
    {
        if (m_remainingTime > 0)
            m_remainingTime -= Time.deltaTime;
        else
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision : " + collision.gameObject.name);
        if (collision.gameObject.name != "Spitfire")
        {
            Debug.Log("COLLISION WITH " + collision.gameObject.name);
            Destroy(gameObject);
        }
    }
}
