using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamaegable
{
    public float currentHealth;
    public float maxHealth = 100f;
    // Start is called before the first frame update
    void Start()
    {
       currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Player took " + amount + " damage. Current HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    public void Die()
    {
        Debug.Log("player died");
        Destroy(gameObject);
    }
}
