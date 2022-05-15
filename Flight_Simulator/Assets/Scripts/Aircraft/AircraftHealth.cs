using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftHealth : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;
    public HealthManager healthBar;
    public Collision collision;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (collision.gameObject.tag == "Balle")
        {
            Debug.Log("COLLISION WITH " + collision.gameObject.name);
            TakeDamage(20);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.setHealth(currentHealth);
    }
}
