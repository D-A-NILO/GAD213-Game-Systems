using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damage = 10f;
    public float damageCooldown = 2f;
    private bool canDamage = true;
    private Collider damageCollider;
    private Renderer rend;
    private Material attackReady;
    public Material attackCooldown;
    public float attackDuration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.isTrigger = true;

        rend = GetComponent<Renderer>();
        attackReady = rend.material;
        StartCoroutine(DamageLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDamage)
        {
            return;
        }
        if (!other.CompareTag("Player"))
        {
            return;
        }
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }

    private IEnumerator DamageLoop()
    {
        while (true) 
        {
            damageCollider.enabled = true;
            rend.material = attackReady;
            yield return new WaitForSeconds(attackDuration);
            damageCollider.enabled = false;
            rend.material = attackCooldown;
            yield return new WaitForSeconds(damageCooldown);
        }
    }
}
