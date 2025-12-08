using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamaegable
{
    public float health = 100f;
    private Transform target;
    public float moveSpeed = 5;
    private Rigidbody rb;
    public float slideKnockbackForce = 12f;
    public float slamKnockUpForce = 10f;
    public bool isKnockedBack = false;
    public float knockbackDuration = 0.25f;
    public Material whiteMaterial;
    private Material originalMaterial;
    private Renderer rend;
    private bool isFlashing = false;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        { 
            target = player.transform;
        }
    }

    public void Update()
    {
        if (target != null)
        {
            if (isKnockedBack)
            {
                return;
            }
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (!isFlashing)
        {
            StartCoroutine(DamageFlash());
        }
        if (health <= 0)
        {
            Die();
            StartCoroutine(DamageFlash());
        }
            

    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody playerRb = collision.rigidbody;
        if (playerRb == null) return;

        Sliding slide = collision.gameObject.GetComponent<Sliding>();
        MovementSystem movement = collision.gameObject.GetComponent<MovementSystem>();

        if (slide != null && slide.sliding)
        {
            float minDamage = 10f;
            float maxDamage = 50f;

            float damage = playerRb.velocity.magnitude * slide.slideDamageMultiplier * slide.slideDamageBoost; ;
            damage = Mathf.Clamp(damage, minDamage, maxDamage);
            TakeDamage(damage);
            Debug.Log($"enemy took slide {damage}");
            SlideKnockback(playerRb);
        }
    }

    private void SlideKnockback(Rigidbody playerRb)
    {
        Vector3 knockDir = (transform.position - playerRb.position).normalized;

        rb.AddForce(knockDir * slideKnockbackForce, ForceMode.Impulse);

        StartCoroutine(KnockbackPause());
    }

    public void SlamKnockUp()
    {
        rb.AddForce(Vector3.up * slamKnockUpForce, ForceMode.Impulse);

        StartCoroutine(KnockbackPause());
    }

    public IEnumerator KnockbackPause()
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    public IEnumerator DamageFlash()
    { 
        isFlashing = true;

        rend.material = whiteMaterial;
        yield return new WaitForSeconds(0.1f);

        rend.material = originalMaterial;
        isFlashing = false;
    }
}
