using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f;
    public TextMeshProUGUI healthText;

    public void ApplyDamage(float amount)
    {
        health -= amount;
        healthText.text = "health: " + health.ToString("0.0");
        Debug.Log($"{gameObject.name} took {amount} damage!");

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }
}
