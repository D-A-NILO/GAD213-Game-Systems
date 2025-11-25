using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamaegable
{
    public float health = 100f;
    public float slamDamageMultiplier = 3f; 

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float minDamage = 10f;
        float maxDamage = 100f;

        Rigidbody playerRb = collision.rigidbody;
        if (playerRb == null) return;

        Sliding slide = collision.gameObject.GetComponent<Sliding>();
        MovementSystem movement = collision.gameObject.GetComponent<MovementSystem>();

        if (slide != null && slide.sliding)
        {
            float damage = playerRb.velocity.magnitude * slide.slideDamageMultiplier * slide.slideSpeed * slide.slideDamageBoost; ;
            damage = Mathf.Clamp(damage, minDamage, maxDamage);
            TakeDamage(damage);
            Debug.Log($"enemy took slide {damage}");
        }

        if (movement != null && movement.isSlamming)
        {
            float verticalSpeed = Mathf.Abs(playerRb.velocity.y);
            float damage = verticalSpeed * slamDamageMultiplier;
            damage = Mathf.Clamp(damage, minDamage, maxDamage);
            TakeDamage(damage);
            Debug.Log($"enemy took slam {damage}");
        }
    }
}
