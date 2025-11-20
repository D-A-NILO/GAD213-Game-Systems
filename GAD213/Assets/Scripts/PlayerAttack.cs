using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public TextMeshProUGUI damageText;
    public float slideBaseDamage = 20f;
    public float slamBaseDamage = 40f;


    public float slideSpeedMultiplier = 1.0f;
    public float slamSpeedMultiplier = 2.0f;

    Sliding sliding;
    MovementSystem slam;
    Rigidbody rb;

    private void Start()
    {
        sliding = GetComponent<Sliding>();
        slam = GetComponent<MovementSystem>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Health target = collision.collider.GetComponent<Health>();
        if (target == null) return;

        float speed = slam.moveSpeed;

        //slide damage
        if (sliding != null && sliding.sliding)
        {
            float damage = slideBaseDamage + (speed * slideSpeedMultiplier);
            target.ApplyDamage(damage);
            damageText.text = "Damage: " + damage.ToString("0.0");
            Debug.Log($"slide damage: {damage} (speed = {speed})");
            return;
        }

        //slam damage
        if (slam != null && slam.isSlamming)
        {
            float damage = slamBaseDamage + (speed * slamSpeedMultiplier);
            target.ApplyDamage(damage);

            damageText.text = "Damage: " + damage.ToString("0.0");
            Debug.Log($"slam damage: {damage} (speed = {speed})");

            slam.CancelSlam();
            return;
        }
    }
}
